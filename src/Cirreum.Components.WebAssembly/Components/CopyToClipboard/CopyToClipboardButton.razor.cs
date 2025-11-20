namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public partial class CopyToClipboardButton(
	IJSRuntime jSRuntime
) : ComponentBase {

	private const string _copyToClipboardText = "Copy";
	private const string _copiedToClipboardText = "Copied!";
	private const string _errorText = "Not Copied!";

	private const string _copyToClipboardTitle = "copy to clipboard...";
	private const string _copiedToClipboardTitle = "Copied to clipboard...";
	private const string _errorTitle = "Failed to copy to the clipboard!";

	private const string _biCopyClass = "bi bi-clipboard";
	private const string _biCopiedClass = "bi bi-check-circle";
	private const string _biErrorClass = "bi bi-x-circle";

	/// <summary>
	/// The text to copy to the clipboard when the button is clicked.
	/// </summary>
	[Parameter, EditorRequired]
	public string Text { get; set; } = string.Empty;

	/// <summary>
	/// True to use an outline button.
	/// </summary>
	[Parameter]
	public bool OutlineButton { get; set; }

	/// <summary>
	/// Should the button show a text label with the icon, or just the icon.
	/// </summary>
	[Parameter]
	public bool ShowLabel { get; set; } = true;

	/// <summary>
	/// The button's Color. Default: <see cref="ButtonColor.Primary"/>
	/// </summary>
	[Parameter]
	public ButtonColor ButtonColor { get; set; } = ButtonColor.Primary;

	/// <summary>
	/// Optional additional Css class(es) to add to the button.
	/// </summary>
	[Parameter]
	public string? ButtonCss { get; set; }

	record ButtonState(
			bool IsDisabled,
			string ButtonText,
			string ButtonClass,
			string IconClass,
			string Title);

	ButtonState buttonState = new(false, _copyToClipboardText, "btn btn-xs btn-primary", _biCopyClass, _copyToClipboardTitle);

	protected override void OnParametersSet() {
		this.buttonState = new(
			false,
			this.ShowLabel ? _copyToClipboardText : "",
			this.ButtonClass,
			_biCopyClass,
			_copyToClipboardTitle);
	}

	private string ButtonClass => CssBuilder.Default("btn btn-xs")
			.AddClass(this.ButtonColor.ToName())
			.AddClassIfNotEmpty(this.ButtonCss)
		.Build();

	public async Task CopyToClipboard() {

		var previousState = this.buttonState;

		try {
			await jSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", this.Text);
			this.buttonState = new ButtonState(true, this.ShowLabel ? _copiedToClipboardText : "", this.ButtonClass, _biCopiedClass, _copiedToClipboardTitle);
			await this.TriggerStateChange();
			this.buttonState = previousState;
		} catch {
			this.buttonState = new ButtonState(true, this.ShowLabel ? _errorText : "", this.ButtonClass, _biErrorClass, _errorTitle);
			await this.TriggerStateChange();
			this.buttonState = previousState;
		}

	}

	private async Task TriggerStateChange() {
		this.StateHasChanged();
		await Task.Delay(1500);
	}


}