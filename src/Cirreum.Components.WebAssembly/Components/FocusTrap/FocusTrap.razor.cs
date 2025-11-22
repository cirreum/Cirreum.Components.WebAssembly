namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading;

public partial class FocusTrap : BaseAfterRenderComponent {
	private IJSInProcessObjectReference? module;
	private ElementReference firstItemRef;

	[Parameter]
	public RenderFragment ChildContent { get; set; } = default!;

	/// <summary>
	/// Is the focus trap active.
	/// </summary>
	[Parameter]
	public bool IsActive { get; set; }

	/// <summary>
	/// Which element should receive focus when <see cref="IsActive"/>
	/// is <see langword="true"/>.
	/// </summary>
	[Parameter]
	public DefaultFocusType FocusType { get; set; }

	private bool hasFocusTrap;
	private bool previousIsActive;

	protected override async Task OnParametersSetAsync() {
		if (this.IsDisposed) {
			return;
		}

		// Check if IsActive changed
		if (this.IsActive != this.previousIsActive) {
			if (this.IsActive) {
				this.RunAfterRender(this.ActivateTrap);
			} else {
				this.DeactivateTrap();
			}
			this.previousIsActive = this.IsActive;
		}

		await base.OnParametersSetAsync();
	}

	private async Task ActivateTrap(CancellationToken cancellationToken = default) {

		if (this.IsDisposed || cancellationToken.IsCancellationRequested) {
			return;
		}

		// CRITICAL: Don't try to activate if IsActive is now false
		if (!this.IsActive) {
			return;
		}

		if (!this.hasFocusTrap && this.module is not null) {
			try {
				var added = this.module.Invoke<bool>("activate", this.ElementId, (int)this.FocusType);
				if (added) {
					this.hasFocusTrap = true;
					if (this.FocusType == DefaultFocusType.Default && !cancellationToken.IsCancellationRequested) {
						await this.firstItemRef.FocusAsync();
					}
				}
			} catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
				// Expected during disposal
			} catch (JSDisconnectedException) {
				// Browser disconnected, ignore
			} catch (JSException) {
				// JS module may not be ready or other JS error, ignore for now
			}
		}
	}

	private void DeactivateTrap() {
		if (this.hasFocusTrap) {
			this.hasFocusTrap = false;
			try {
				this.module?.InvokeVoid("deactivate", this.ElementId);
			} catch (JSDisconnectedException) {
				// Browser disconnected, ignore
			} catch (JSException) {
				// JS module issue, ignore during deactivation
			}
		}
	}

	protected override async Task OnAfterFirstRenderAsync() {
		if (this.IsDisposed) {
			return;
		}

		try {

			const string jsPath = "./_content/Cirreum.Components.WebAssembly/Components/FocusTrap/FocusTrap.razor.js";
			this.module = await this.JSApp.InvokeAsync<IJSInProcessObjectReference>("import", this.DisposalToken, jsPath);

		} catch (OperationCanceledException) when (this.DisposalToken.IsCancellationRequested) {
			// Expected during disposal
		} catch (JSDisconnectedException) {
			// Browser disconnected during module load
		}
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {

			// Deactivate the trap before disposing the module
			this.DeactivateTrap();

			// Dispose the JS module
			this.module?.Dispose();
			this.module = null;

		}

		base.Dispose(disposing);
	}

}