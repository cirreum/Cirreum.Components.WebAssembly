namespace Cirreum.Components;

using Cirreum.Components.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Threading.Tasks;

/// <summary>
/// An abstract base class for use with a Button or
/// a Link that includes support for a context menu.
/// </summary>
/// <remarks>
/// Follows the menu button pattern yet is constrained to a Context (right-click) menu only.
/// https://www.w3.org/WAI/ARIA/apg/patterns/menu-button/
/// </remarks>
public abstract class ContextMenuBase : BaseAfterRenderComponent {

	private DotNetObjectReference<ContextMenuBase>? dotnetRef;
	private IJSInProcessObjectReference? module;
	private bool isConnected;
	protected Menu? MenuRef;
	private IMenuInternal? MenuInternal => MenuRef;

	protected string AriaIsDisabled => this.IsDisabled.ToAttributeValue();
	protected string AriaIsExpanded => this.IsMenuShowing.ToAttributeValue();
	private bool IsMenuShowing => this.MenuRef is not null && this.MenuRef.IsShowing;


	[Inject]
	IJSAppModule JSApp { get; set; } = default!;

	/// <summary>
	/// The text to display for the button.
	/// </summary>
	[Parameter]
	public string? Content { get; set; }

	/// <summary>
	/// The custom content to display for the button.
	/// </summary>
	[Parameter]
	public RenderFragment? ContentTemplate { get; set; }

	/// <summary>
	/// The collection of menu items that make context menu.
	/// </summary>
	[Parameter, EditorRequired]
	public RenderFragment ChildContent { get; set; } = default!;

	[Parameter]
	public bool IsDisabled { get; set; }

	[Parameter]
	public EventCallback OnMenuShow { get; set; }

	[Parameter]
	public EventCallback OnMenuShown { get; set; }

	[Parameter]
	public EventCallback OnMenuHide { get; set; }

	[Parameter]
	public EventCallback OnMenuHidden { get; set; }

	[Parameter]
	public EventCallback<IMenuItem> OnItemSelected { get; set; }

	[Parameter]
	public EventCallback<MouseEventArgs> OnClicked { get; set; } = default!;

	[Parameter]
	public EventCallback<MouseEventArgs> OnDoubleClicked { get; set; } = default!;

	protected async Task ClickHandler(MouseEventArgs args) {

		if (args.Detail == 2) {
			try {
				if (this.OnDoubleClicked.HasDelegate) {
					await this.OnDoubleClicked.InvokeAsync(args);
				}
			} catch {
				// swallow user errors
			}
			return; // don't select on dbl click
		}

		if (args.Detail == 1) {
			try {
				if (this.OnClicked.HasDelegate) {
					await this.OnClicked.InvokeAsync(args);
				}
			} catch {
				// swallow user errors
			}
		}

		if (this.IsMenuShowing) {
			await this.MenuRef!.CloseAsync();
		}

	}
	protected async Task ToggleMenuAsync(MouseEventArgs e) {

		if (this.MenuRef is null) {
			return;
		}

		if (this.MenuRef.IsShowing) {
			await this.MenuRef.CloseAsync();
			return;
		}

		await this.MenuRef.ShowAsync(e.PageY, e.PageX);

	}

	protected override void OnInitialized() {
		base.OnInitialized();
		this.dotnetRef = DotNetObjectReference.Create(this);
	}
	protected async override Task OnInitializedAsync() {
		const string jsPath = $"./_content/Cirreum.Blazor.Components/Components/Menu/{nameof(ContextMenuButton)}.razor.js";
		this.module = await this.JSApp.InvokeAsync<IJSInProcessObjectReference>("import", jsPath);
		await base.OnInitializedAsync();
	}
	protected async override Task OnAfterRenderAsync(bool firstRender) {

		if (this.IsDisabled) {
			if (this.isConnected && this.module is not null) {
				this.module.InvokeVoid(
					"Disconnect",
					this.ElementId);
				this.isConnected = false;
			}
			await base.OnAfterRenderAsync(firstRender);
			return;
		}

		if (this.isConnected is false && this.module is not null) {
			this.module.InvokeVoid(
				"Connect",
				this.ElementId,
				this.dotnetRef);
			this.isConnected = true;
		}

		await base.OnAfterRenderAsync(firstRender);

	}

	protected override void Dispose(bool disposing) {
		this.module?.InvokeVoid("Disconnect", this.ElementId);
		this.dotnetRef?.Dispose();
		this.module?.Dispose();
		base.Dispose(disposing);
	}

	[JSInvokable]
	public async void HandleArrowUpKey() {
		if (this.IsMenuShowing) {
			await this.MenuInternal!.FocusLastAsync();
			return;
		}
		await this.MenuRef!.ShowAsync(-1, -1);
	}
	[JSInvokable]
	public async void HandleArrowDownKey() {
		if (this.IsMenuShowing) {
			await this.MenuInternal!.FocusFirstAsync();
			return;
		}
		await this.MenuRef!.ShowAsync(-1, -1);
	}
	[JSInvokable]
	public async void HandleEscapeKey() {
		if (this.IsMenuShowing) {
			await this.MenuRef!.CloseAsync();
		}
	}
	[JSInvokable]
	public async void HandleEnterOrSpace() {
		await this.ClickHandler(new MouseEventArgs() { Detail = 1 });
	}

}