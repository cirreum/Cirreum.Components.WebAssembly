namespace Cirreum.Components;

using Cirreum.Components.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

public partial class DialogManager : IDisposable {

	private readonly RenderFragment _renderDialogs;
	private readonly CancellationTokenSource _cancellationTokenSource = new();
	private bool _disposed;

	public DialogManager() {
		this._renderDialogs = this.RenderDialogs;
	}

	[Inject]
	NavigationManager NavigationManager { get; set; } = default!;

	[Inject]
	private IDialogServiceInternal DialogService { get; set; } = default!;

	[Parameter]
	public DialogOptions DefaultOptions { get; set; } = default!;

	internal readonly Dictionary<string, DialogReference> DialogReferences = [];
	internal int DialogsCentered = 0;
	internal int DialogsCenteredLeft = 0;
	internal int DialogsCenteredRight = 0;
	internal int DialogsTop = 0;
	internal int DialogsTopLeft = 0;
	internal int DialogsTopRight = 0;
	internal int DialogsBottom = 0;
	internal int DialogsBottomLeft = 0;
	internal int DialogsBottomRight = 0;

	internal void DismissInstance(string id, DialogResult result) {
		var reference = this.GetDialogReference(id);
		this.DismissInstance(reference, result);
	}

	private async void HandleDialogClosed(DialogReference? dialogReference, DialogResult result) {

		// Gracefully close the dialog
		if (dialogReference?.Instance != null) {
			await dialogReference.Instance.CloseAsync(result);
			return;
		}

		this.DismissInstance(dialogReference, result);

	}

	private void DismissInstance(DialogReference? dialogReference, DialogResult result) {
		if (dialogReference != null) {

			dialogReference.Instance?.RemoveDraggability();

			this.DialogReferences.Remove(dialogReference.Id);

			this.StateHasChanged();

			this.OnDialogDismissed();

			dialogReference.SetResultsAndCompleteTask(result);

		}
	}

	private void OnLocationChanged(object? sender, LocationChangedEventArgs e) {

		var dialogIdsToClose = new List<string>(this.DialogReferences.Keys);

		foreach (var dialogId in dialogIdsToClose) {
			if (this.DialogReferences.TryGetValue(dialogId, out var dialogReference)) {

				dialogReference.Instance?.CancelImmediately();
				dialogReference.Instance?.RemoveDraggability();

				this.DialogReferences.Remove(dialogId);
				dialogReference.SetResultsAndCompleteTask(DialogResult.Cancel());

			}
		}

		this.StateHasChanged();
		this.OnDialogDismissed();
	}

	private void ShowDialogInstance(DialogReference dialogReference) {
		if (_disposed) {
			return;
		}
		this.DialogReferences.Add(dialogReference.Id, dialogReference);
		this.StateHasChanged();
	}

	bool addedNoscroll;
	internal void OnModalShown() {
		if (this.addedNoscroll is false && !this._disposed) {
			this.JS.SetElementClassIfScrollbar("body", true, "noscroll");
			this.addedNoscroll = true;
		}
	}
	private void OnDialogDismissed() {
		if (this.DialogReferences.Any(d => d.Value.Instance?.IsModal is true) is false
			&& this.addedNoscroll) {
			this.JS.RemoveElementClass("body", "noscroll");
			this.addedNoscroll = false;
		}
	}

	private DialogReference? GetDialogReference(string id) {
		this.DialogReferences.TryGetValue(id, out var dialogReference);
		return dialogReference;
	}

	protected override void OnInitialized() {

		this.DialogService.OnDialogInstanceAdded += this.ShowDialogInstance;
		this.DialogService.OnDialogCloseRequested += this.HandleDialogClosed;

		this.NavigationManager.LocationChanged += this.OnLocationChanged;

	}

	[Inject]
	private IJSAppModule JS { get; set; } = default!;
	private IJSInProcessObjectReference? module;
	protected override async Task OnAfterRenderAsync(bool firstRender) {
		if (firstRender && !this._disposed) {
			const string jsPath = "./_content/Cirreum.Blazor.Components/Components/Dialog/DialogManager.razor.js";
			this.module = await this.JS.InvokeAsync<IJSInProcessObjectReference>("import", jsPath);
		}
	}

	internal void AdjustNestedPosition(int assignedPosition, int assignedCount, ElementReference instance) {
		if (this._disposed) {
			return;
		}
		this.module?.InvokeVoid("adjustPosition", assignedPosition, assignedCount, instance);
	}
	internal bool AddDraggable(string id) {
		if (this._disposed) {
			return false;
		}
		var options = new {
			containment = ".dialog-container"
		};
		return this.module?.Invoke<bool>("addDraggable", id, options) ?? false;
	}
	internal bool AddDraggable(string id, string headerClass) {
		if (this._disposed) {
			return false;
		}
		var options = new {
			containment = ".dialog-container",
			handle = headerClass
		};
		return this.module?.Invoke<bool>("addDraggable", id, options) ?? false;
	}
	internal void RemoveDraggable(string id) {
		this.module?.InvokeVoid("removeDraggable", id);
	}

	public void Dispose() {
		if (this._disposed) {
			return;
		}

		this._disposed = true;

		this.NavigationManager.LocationChanged -= this.OnLocationChanged;

		this.DialogService.OnDialogInstanceAdded -= this.ShowDialogInstance;
		this.DialogService.OnDialogCloseRequested -= this.HandleDialogClosed;

		// just a sanity check to ensure they're all removed
		// they should all be gone already
		var dialogReferences = this.DialogReferences.Values.ToList();
		foreach (var dialogRef in dialogReferences) {
			try {
				dialogRef.Instance?.RemoveDraggability();
				dialogRef.SetResultsAndCompleteTask(DialogResult.Cancel());
			} catch (Exception ex) {
				Console.Error.WriteLine($"warn: Error during dialog cleanup: {ex.Message}");
			}
		}
		this.DialogReferences.Clear();

		// Dispose JS module
		this.module?.Dispose();
		this.module = null;

		GC.SuppressFinalize(this);
	}

}