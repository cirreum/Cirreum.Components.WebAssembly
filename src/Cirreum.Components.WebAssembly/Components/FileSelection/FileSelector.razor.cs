namespace Cirreum.Components;

using Cirreum.Components.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Threading.Tasks;

public partial class FileSelector {

	[Inject]
	protected IJSAppModule JSApp { get; set; } = default!;
	private IJSInProcessObjectReference? module;

	/// <summary>
	/// Gets or sets the elements unique identifier.
	/// </summary>
	[Parameter]
	public string? Id { get; set; } = IdGenerator.Next;

	private bool DropOver { get; set; } = false;

	/// <summary>
	/// Clear the input selection box.
	/// </summary>
	public void Reset() {
		this.module?.InvokeVoid("clearInput", this.Id);
	}

	/// <summary>
	/// Gets or sets a value indicating whether the Drag/Drop zone is visible.
	/// Default is true.
	/// </summary>
	[Parameter]
	public bool DragDropZoneVisible { get; set; } = true;

	/// <summary>
	/// Optional CSS class names. If given, these will be included in the class attribute of the component.
	/// </summary>
	[Parameter]
	public virtual string? Class { get; set; } = null;

	protected string? ClassValue => CssBuilder
		.Default("inputfile-container")
			.AddClassIfNotEmpty(this.Class)
		.Build();

	/// <summary>
	/// Optional in-line styles. If given, these will be included in the style attribute of the component.
	/// </summary>
	[Parameter]
	public virtual string? Style { get; set; } = null;

	protected string? StyleValue => StyleBuilder.Empty()
			.AddStyleIfNotEmpty(this.Style)
			.AddStyle("display", "none", () => !this.DragDropZoneVisible)
		.Build();

	/// <summary>
	/// Gets or sets the identifier of the source component clickable by the end user.
	/// </summary>
	[Parameter]
	public string AnchorId { get; set; } = string.Empty;

	/// <summary>
	/// To enable multiple file selection and upload, set the Multiple property to true.
	/// Set <see cref="MaximumFileCount"/> to change the number of allowed files.
	/// </summary>
	[Parameter]
	public bool Multiple { get; set; } = false;

	/// <summary>
	/// To select multiple files, set the maximum number of files allowed to be uploaded.
	/// Default value is 10.
	/// </summary>
	[Parameter]
	public int MaximumFileCount { get; set; } = 10;

	/// <summary>
	/// Gets or sets the maximum size of a file to be uploaded (in bytes).
	/// Default value is 10 MB.
	/// </summary>
	[Parameter]
	public long MaximumFileSize { get; set; } = 10 * 1024 * 1024;

	/// <summary>
	/// Gets or sets the filter for what file types the user can pick from the file input dialog box.
	/// Example: ".gif, .jpg, .png, .doc", "audio/*", "video/*", "image/*"
	/// See <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Attributes/accept">https://developer.mozilla.org/en-US/docs/Web/HTML/Attributes/accept</see>
	/// for more information.
	/// </summary>
	[Parameter]
	public string Accept { get; set; } = string.Empty;

	/// <summary>
	/// Disables the form control, ensuring it doesn't participate in form submission.
	/// </summary>
	[Parameter]
	public bool Disabled { get; set; }

	/// <summary>
	/// Gets or sets the child content of the component.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public Action<List<IBrowserFile>>? OnSelected { get; set; }

	[Parameter]
	public Action? OnCanceled { get; set; }

	[Parameter]
	public Action<string>? OnSelectionFailed { get; set; }

	private EventCallback<InputFileChangeEventArgs> OnInputFileChange { get; set; }

	private void OnFileSelectionCanceled() {
		this.DropOver = false;
		this.OnCanceled?.Invoke();
	}

	private void OnFileSelectionFailed(string message) {
		this.DropOver = false;
		this.OnCanceled?.Invoke();
	}

	private void OnFileSelectedChanged(InputFileChangeEventArgs e) {

		this.DropOver = false;

		if (e.FileCount > this.MaximumFileCount) {
			this.Reset();
			this.OnSelectionFailed?.Invoke($"File(s) selected, exceeded the maximum allowed.");
			return;
		}

		if (e.FileCount == 0) {
			this.OnCanceled?.Invoke();
			return;
		}

		var allFiles = e.GetMultipleFiles(this.MaximumFileCount);
		var acceptedFiles = new List<IBrowserFile>();

		foreach (var file in allFiles) {

			if (file.Size > this.MaximumFileSize) {
				continue;
			}

			acceptedFiles.Add(file);

		}

		this.OnSelected?.Invoke(acceptedFiles);

	}

	protected override async Task OnAfterFirstRenderAsync() {
		if (this.AnchorId.HasValue() && this.Id.HasValue()) {
			const string jsPath = "./_content/Cirreum.Blazor.Components/Components/FileSelection/FileSelector.razor.js";
			this.module = await this.JSApp.InvokeAsync<IJSInProcessObjectReference>("import", jsPath);
			await this.module.InvokeVoidAsync("attachClickHandler", this.AnchorId, this.Id);
		}
	}

}