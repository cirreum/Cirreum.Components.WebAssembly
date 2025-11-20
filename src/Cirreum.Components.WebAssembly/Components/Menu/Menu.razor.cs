namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

public partial class Menu {

	private readonly string MenuElementId = IdGenerator.Next;
	private readonly List<MenuItem> MenuItems = [];
	private int currentFocusIndex = -1;
	private string AriaIsShowing => this.IsShowing.ToAttributeValue();
	private string CustomStyle { get; set; } = "left:0;top:0;";
	private string trigger = "";
	private MouseButton triggerButton = MouseButton.None;
	private ElementReference menuElementReference;
	private IDisposable? mouseButtonHandler;

	string IMenuInternal.MenuId => this.MenuElementId;
	RenderFragment? IMenuInternal.MenuTemplate => this._MenuInstance;

	/// <inheritdoc/>
	[Parameter]
	public string Trigger { get; set; } = string.Empty;

	/// <inheritdoc/>
	[Parameter]
	public MouseButton TriggerButton { get; set; } = MouseButton.None;

	/// <inheritdoc/>
	[Parameter]
	public bool Anchored { get; set; } = false;

	/// <inheritdoc/>
	[Parameter, EditorRequired]
	public RenderFragment ChildContent { get; set; } = default!;

	/// <inheritdoc/>
	[Parameter]
	public EventCallback OnMenuShow { get; set; }

	/// <inheritdoc/>
	[Parameter]
	public EventCallback OnMenuShown { get; set; }

	/// <inheritdoc/>
	[Parameter]
	public EventCallback OnMenuHide { get; set; }

	/// <inheritdoc/>
	[Parameter]
	public EventCallback OnMenuHidden { get; set; }

	/// <inheritdoc/>
	[Parameter]
	public EventCallback<IMenuItem> OnMenuItemSelected { get; set; }

	/// <inheritdoc/>
	public async Task ShowAsync(double top, double left) {

		if (this.OnMenuShow.HasDelegate) {
			await this.OnMenuShow.InvokeAsync();
		}

		var viewport = this.JSApp.GetViewportDimensions();
		var scrollPosition = this.JSApp.GetScrollPosition();
		var footerHeight = this.JSApp.GetFooterHeight();

		var triggerRect = (this.Anchored || top < 1 || left < 1)
			? this.JSApp.GetAccurateBoundingClientRect(this.Trigger)
			: null;
		var offsetDistanceFromTrigger = this.Anchored ? 2 : 0;

		// best effort to move the menu around the
		// trigger element to ensure it doesn't
		// overflow the view port.

		// Use supplied top/left unless Anchored is true and we have a trigger rect
		if (this.Anchored && triggerRect is not null) {
			left = triggerRect.Right + scrollPosition.X;
			top = triggerRect.Bottom + scrollPosition.Y + offsetDistanceFromTrigger;
		}

		// Convert document coordinates to viewport coordinates
		var viewportRelativeLeft = left - scrollPosition.X;
		var viewportRelativeTop = top - scrollPosition.Y;

		// Default to offscreen for initial rendering
		this.CustomStyle = $"left:{-9999}px;top:{-9999}px;";
		this.IsShowing = true;
		this.MenuService.ShowMenu(this, this.Trigger.NullIfWhiteSpace());

		// Wait for rendering to occur
		await Task.Delay(50);

		// Get menu dimensions and adjust position
		var menuRect = this.JSApp.GetBoundingClientRect(this.MenuElementId);
		if (menuRect is null) {
			await Task.Delay(50);
			menuRect = this.JSApp.GetBoundingClientRect(this.MenuElementId);
			if (menuRect is null) {
				this.MenuService.CloseMenu();
				return;
			}
		}

		this.isMeasuring = false;
		this.JSApp.RemoveElementClass(this.MenuElementId, "measuring");

		// Adjust horizontal position if menu overflows viewport
		if (viewportRelativeLeft + menuRect.Width > viewport.Width) {
			if (this.Anchored && triggerRect is not null) {
				// Right-align with the trigger
				left = triggerRect.Left + scrollPosition.X - menuRect.Width;
			} else {
				// Position to the left of the click/supplied position
				left = left - menuRect.Width - 16; // 16px/1rem offset
			}
		}

		// Ensure menu doesn't overflow left edge
		left = Math.Max(scrollPosition.X, left);

		// Adjust vertical position
		var viewportBottom = scrollPosition.Y + viewport.Height;
		var spaceBelow = viewportBottom - top;
		var spaceAbove = viewportRelativeTop;

		// Calculate the minimum space needed below the menu
		var minSpaceBelow = Math.Max(16 /*1rem*/, footerHeight);

		if ((spaceBelow - minSpaceBelow) < menuRect.Height) {
			if (spaceAbove > menuRect.Height || spaceAbove > spaceBelow) {
				// Position above if there's more space above or if it fits entirely above
				top = Math.Max(scrollPosition.Y, top - menuRect.Height);
			} else if (spaceBelow < minSpaceBelow) {
				// If there's very little space below, position at the bottom of the viewport
				top = Math.Max(scrollPosition.Y, viewportBottom - menuRect.Height);
			}
			// Else: leave it below if there's not enough space above and more than minSpaceBelow below
		}

		// Ensure menu doesn't go above the top of the viewport
		top = Math.Max(scrollPosition.Y, top);

		// Set final position
		this.JSApp.SetElementStyleProperty(this.MenuElementId, "left", $"{left}px");
		this.JSApp.SetElementStyleProperty(this.MenuElementId, "top", $"{top}px");

		// Set this in case Blazor re-renders
		this.CustomStyle = $"left:{left}px;top:{top}px;";

		// Add 'root-shown' class after a brief delay to trigger the transition
		await Task.Delay(20);
		this.JSApp.AddElementClasses(this.MenuElementId, "root-shown");

	}

	/// <inheritdoc/>
	public Task CloseAsync() {
		return this.CloseMenuAsync();
	}

	/// <inheritdoc/>
	public bool IsShowing { get; private set; }
	private bool isMeasuring = true;

	protected string RootMenuCss => CssBuilder
		.Empty()
			.AddClass("menu-root")
			.AddClass("dropdown-menu")
			.AddClass("measuring", this.isMeasuring && this.IsShowing && this.Anchored)
			.AddClass("root-shown", this.IsShowing && this.Anchored is false)
		.Build();

	void IMenuInternal.RegisterMenuItem(MenuItem menuItem) {
		this.MenuItems.Add(menuItem);
	}
	void IMenuInternal.UnregisterMenuItem(MenuItem menuItem) {
		this.MenuItems.Remove(menuItem);
	}
	async Task IMenuInternal.FocusFirstAsync() {
		if (this.MenuItems.Count > 0) {
			this.currentFocusIndex = 0;
			await this.MenuItems[0].FocusAsync();
		}
	}
	async Task IMenuInternal.FocusLastAsync() {
		if (this.MenuItems.Count > 0) {
			this.currentFocusIndex = 0;
			await this.AdjustFocus(-1);
		}
	}
	async Task IMenuInternal.FocusByFirstCharacter(string key) {
		if (this.MenuItems.Count == 0) {
			return;
		}

		var currentIndex = this.currentFocusIndex;
		var startIndex = (currentIndex + 1) % this.MenuItems.Count; // Start from the next item

		for (var i = 0; i < this.MenuItems.Count; i++) {
			var index = (startIndex + i) % this.MenuItems.Count;
			if (this.MenuItems[index].menuText.StartsWith(key)) {
				this.MenuItems[currentIndex].tabIndex = "-1";
				this.currentFocusIndex = index;
				await this.MenuItems[index].FocusAsync();
				return;
			}
		}

		// If no match found, optionally start from the beginning
		for (var i = 0; i <= currentIndex; i++) {
			if (this.MenuItems[i].menuText.StartsWith(key)) {
				this.MenuItems[currentIndex].tabIndex = "-1";
				this.currentFocusIndex = i;
				await this.MenuItems[i].FocusAsync();
				return;
			}
		}

	}
	async Task IMenuInternal.MoveFocusAsync(int direction) {
		await this.AdjustFocus(direction);
	}
	async Task IMenuInternal.HandleItemSelectionAsync(IMenuItem item) {
		await this.CloseMenuAsync();
		if (this.OnMenuItemSelected.HasDelegate) {
			await this.OnMenuItemSelected.InvokeAsync(item);
		}
	}
	async Task IMenuInternal.HideAsync(bool notifyContainer) {
		if (this.IsShowing) {
			await this.CloseMenuAsync(notifyContainer);
		}
	}
	async Task IMenuInternal.OnOpenedAsync() {
		if (this.MenuItems.Count > 0) {
			this.currentFocusIndex = 0;
			this.MenuItems[0].EnableFocus();
		}
		if (this.OnMenuShown.HasDelegate) {
			await this.OnMenuShown.InvokeAsync();
		}
	}
	async Task IMenuInternal.OnClosedAsync() {
		if (this.OnMenuHidden.HasDelegate) {
			await this.OnMenuHidden.InvokeAsync();
		}
		if (this.Trigger.HasValue()) {
			this.JSApp.FocusElement(this.Trigger, true);
		}
	}

	private static void ContextMenuNoOp() {
		// Do nothing :)
	}
	private async Task ToggleMenu(MouseButtonEventInfo e) {
		if (this.IsShowing) {
			await this.CloseAsync();
			return;
		}
		await this.ShowAsync(e.PageY, e.PageX);
	}
	private async Task CloseMenuAsync(bool notifyContainer = true) {

		if (this.IsShowing is false) {
			return;
		}

		if (this.OnMenuHide.HasDelegate) {
			await this.OnMenuHide.InvokeAsync();
		}

		foreach (var item in this.MenuItems) {
			await item.CloseMenu(false);
		}

		this.IsShowing = false;

		if (notifyContainer) {
			this.MenuService.CloseMenu();
			return;
		}

		await ((IMenuInternal)this).OnClosedAsync();

	}
	private async Task AdjustFocus(int direction) {
		if (this.MenuItems.Count == 0) {
			return;
		}

		if (this.currentFocusIndex > -1) {
			await this.MenuItems[this.currentFocusIndex].CloseMenu(false);
		}

		var startIndex = this.currentFocusIndex;
		do {
			this.currentFocusIndex = (this.currentFocusIndex + direction + this.MenuItems.Count) % this.MenuItems.Count;
			if (!this.MenuItems[this.currentFocusIndex].Disabled) {
				await this.MenuItems[this.currentFocusIndex].FocusAsync();
				return;
			}
		} while (this.currentFocusIndex != startIndex);

		// If we've looped through all items and they're all disabled, don't change focus
		this.currentFocusIndex = startIndex;
	}

	public override async Task SetParametersAsync(ParameterView parameters) {

		this.trigger = this.Trigger;
		this.triggerButton = this.TriggerButton;

		await base.SetParametersAsync(parameters);

	}

	protected override void OnParametersSet() {

		var hasChanged = false;
		if (this.trigger != this.Trigger) {
			this.trigger = this.Trigger;
			hasChanged = true;
		}
		if (this.triggerButton != this.TriggerButton) {
			this.triggerButton = this.TriggerButton;
			hasChanged = true;
		}

		if (hasChanged && this.mouseButtonHandler is not null) {
#if DEBUG
			Console.WriteLine($"Menu::OnParametersSet[hasChanged= {hasChanged}]");
#endif
			this.mouseButtonHandler.Dispose();
			this.mouseButtonHandler = null;
		}

		if (hasChanged && this.TriggerButton != MouseButton.None && this.Trigger.HasValue()) {
#if DEBUG
			Console.WriteLine($"Menu::OnParametersSet::RunAfterRender()");
#endif

			this.RunAfterRender(() => {
				this.mouseButtonHandler = this.Trigger.AddMouseButtonListener(
					this.JSApp,
					async (e) => await this.ToggleMenu(e),
					800,
					this.TriggerButton);
#if DEBUG
				Console.WriteLine($"Menu::OnParametersSet::Trigger.AddMouseButtonListener()");
#endif

			});
		}

	}

	protected override void Dispose(bool disposing) {
#if DEBUG
		Console.WriteLine($"Menu::Dispose => Trigger[{this.trigger}]");
#endif
		this.mouseButtonHandler?.Dispose();
		base.Dispose(disposing);
	}

}