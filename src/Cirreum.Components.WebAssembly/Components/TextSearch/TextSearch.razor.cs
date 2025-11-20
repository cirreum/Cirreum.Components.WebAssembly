namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

/// <summary>
/// Represents a text search component with debounce functionality.
/// </summary>
/// <remarks>
/// This component provides a debounced search functionality, which means it waits for a specified delay
/// after the last input before triggering the search. This helps to reduce the number of unnecessary
/// search operations, especially when the user is still typing.
/// </remarks>
public partial class TextSearch : IDisposable {

	private readonly Debouncer _debouncer = new();
	private string _searchValue = "";
	private ElementReference inputElement;

	/// <summary>
	/// The accessible label for the search box. Default: search
	/// </summary>
	[Parameter]
	public string AriaLabel { get; set; } = "search";

	/// <summary>
	/// The visible label for the search box. Default: search
	/// </summary>
	[Parameter]
	public string Label { get; set; } = "search";

	/// <summary>
	/// Gets or sets the delay (in milliseconds) before the search is triggered after the last input.
	/// </summary>
	/// <value>The debounce delay in milliseconds. Default is 500ms.</value>
	[Parameter]
	public int DebounceDelay { get; set; } = 500;

	/// <summary>
	/// Gets or sets a value indicating whether the search box is disabled.
	/// </summary>
	/// <value><c>true</c> if the search box is disabled; otherwise, <c>false</c>.</value>
	[Parameter]
	public bool Disabled { get; set; }

	/// <summary>
	/// Event callback that is triggered when the search value changes (after the debounce delay).
	/// </summary>
	[Parameter]
	public EventCallback<string> OnSearchChanged { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="FormControlSize"/> of the text box. Default: <see cref="FormControlSize.Small"/>
	/// </summary>
	[Parameter]
	public FormControlSize InputSize { get; set; } = FormControlSize.Small;

	/// <summary>
	/// When <see langword="true"/>, the enter key will trigger the <see cref="OnSearchChanged"/>
	/// event. Default: <see langword="true"/>
	/// </summary>
	[Parameter]
	public bool IncludeEnterKey { get; set; } = true;

	/// <summary>
	/// Gets or sets the current search value.
	/// </summary>
	/// <value>
	/// The current search text. Setting this value triggers the debounced search process.
	/// </value>
	/// <remarks>
	/// When this property is set to a new value, it automatically triggers the <see cref="ApplySearch"/> method,
	/// which applies the debounce logic before invoking the <see cref="OnSearchChanged"/> event.
	/// </remarks>
	public string SearchValue {
		get => this._searchValue;
		set {
			if (this._searchValue != value) {
				this._searchValue = value;
				this.ApplySearch();
			}
		}
	}

	private async Task HandleKeyPress(KeyboardEventArgs e) {
		if (e.Key == "Enter" && this.IncludeEnterKey) {
			await this.OnSearchChanged.InvokeAsync(this.SearchValue);
		}
	}

	/// <summary>
	/// Attempts to focus the search box.
	/// </summary>
	/// <param name="preventScroll">
	/// <para>
	///     A <see cref="bool" /> value indicating whether or not the browser should scroll the document to bring the
	///     newly-focused element into view. A value of false for preventScroll (the default) means that the browser
	///     will scroll the element into view after focusing it. If preventScroll is set to true, no scrolling will
	///     occur.
	/// </para>
	/// </param>
	/// <returns></returns>
	public async Task FocusAsync(bool preventScroll) {
		if (this.inputElement.Context != null) {
			await this.inputElement.FocusAsync(preventScroll);
		}
	}

	private readonly string InputId = IdGenerator.Next;
	private readonly string LabelId = IdGenerator.Next;
	private ElementReference LabelRef;

	private string InputClassList => CssBuilder
		.Default("form-control")
			.AddClass(this.InputSize.ToName(), when: this.InputSize != FormControlSize.Default)
			.AddClass("active", this._searchValue.HasValue())
		.Build();
	private int NotchWidth = 38;
	private string NotchStyle => StyleBuilder
		.Empty()
			.AddStyle("width", () => $"{this.NotchWidth}px")
		.Build();

	/// <summary>
	/// Applies the search after the specified debounce delay.
	/// </summary>
	/// <remarks>
	/// This method uses the timer-based debounce service to delay the search operation.
	/// Each call to this method cancels any pending search and starts a new debounce timer.
	/// </remarks>
	private void ApplySearch() {
		this._debouncer.Invoke();
	}

	protected override void OnInitialized() {
		base.OnInitialized();
		this._debouncer.Initialize(this.DebounceDelay, async () => {
			await this.OnSearchChanged.InvokeAsync(this.SearchValue);
		});
	}

	protected override async Task OnAfterRenderAsync(bool firstRender) {
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender) {
			this.NotchWidth = this.JSApp.GetElementScrollWidth(this.LabelRef);
			// we manually reduce the value, as the label font is smaller
			// when the input is focused...
			this.NotchWidth = (int)(this.NotchWidth * .84);
			if (this.NotchWidth < 3 && this.Label.HasValue()) {
				this.NotchWidth = this.Label.Length > 8 ? 40 : 36;
			}
			this.Update();
		}
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			this._debouncer.Dispose();
		}
		base.Dispose(disposing);
	}

}