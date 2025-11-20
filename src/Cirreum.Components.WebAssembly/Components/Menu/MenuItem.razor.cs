namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

public partial class MenuItem {

	private readonly string LIElementId = IdGenerator.Next;
	private readonly string submenuElementId = IdGenerator.Next;
	private readonly List<MenuItem> subMenuItems = [];
	private int currentFocusIndex = -1;
	private ElementReference menuItemElementRef;
	private bool expanded;
	internal string menuText = "";
	internal string tabIndex = "-1";

	private bool HasSubMenu => this.MenuItems != null;
	private string AriaDisabled => this.Disabled.ToAttributeValue();
	private string AriaHasPopup => this.HasSubMenu.ToAttributeValue();
	private string AriaExpanded => (this.HasSubMenu && this.expanded).ToAttributeValue();
	private string ElementClassList => CssBuilder
		.Default("dropdown-item")
			.AddClass("dropdown-toggle", when: this.HasSubMenu)
			.AddClass("disabled", when: this.Disabled)
			.AddClass("active", when: this.expanded)
		.Build();
	private string MenuClassList => CssBuilder
		.Default("dropdown-menu")
			.AddClass("show", when: this.expanded)
		.Build();


	[CascadingParameter]
	private IMenuInternal MenuInternal { get; set; } = default!;
	public IMenu Menu => this.MenuInternal;

	[CascadingParameter]
	private MenuItem? ParentMenuItem { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public string? Label { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the element is disabled.
	/// </summary>
	[Parameter]
	public bool Disabled { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="RenderFragment"/> containing a sub-menu components.
	/// </summary>
	[Parameter]
	public RenderFragment? MenuItems { get; set; }

	/// <summary>
	/// Event raised when the user click on this item.
	/// </summary>
	[Parameter]
	public EventCallback<MouseEventArgs> OnClick { get; set; }

	[Parameter]
	public string Value { get; set; } = "";

	public Task FocusAsync() {
		this.tabIndex = "0";
		this.RunAfterRender(async () => {
			await this.menuItemElementRef.FocusAsync(true);
		});
		this.Update();
		return Task.CompletedTask;
	}


	private void HandleMouseEnter() {
		this.tabIndex = "0";
		if (this.HasSubMenu && this.expanded is false) {
			this.expanded = true;
		}
	}

	private void HandleMouseLeave() {
		this.tabIndex = "-1";
		if (this.expanded) {
			this.expanded = false;
		}
	}

	private async Task HandleClick(MouseEventArgs args) {
		if (this.Disabled) {
			return;
		}
		if (this.OnClick.HasDelegate) {
			await this.OnClick.InvokeAsync(args);
		}
		await this.MenuInternal.HandleItemSelectionAsync(this);
	}

	private async Task HandleFocusKeyDown(KeyboardEventArgs e) {
		var key = e.Code ?? e.Key;
		switch (key) {
			case "ArrowDown":
				this.JSApp.FocusElement(this.ElementId, true);
				break;
			case "Escape":
				await this.HandleEscapeKey();
				break;
			case "Tab":
				if (e.ShiftKey) {
					await this.HandleShiftTabKey();
				} else {
					await this.FocusAsync();
				}
				break;
		}
	}
	private async Task HandleKeyDown(KeyboardEventArgs e) {
		var key = e.Code ?? e.Key;
		switch (key) {
			case null:
				break;
			case "Enter":
			case " ":
				await this.ActivateAsync();
				break;
			case "ArrowRight":
				this.HandleRightArrow();
				break;
			case "ArrowLeft":
				await this.HandleLeftArrow();
				break;
			case "ArrowDown":
				this.HandleDownArrow();
				break;
			case "ArrowUp":
				this.HandleUpArrow();
				break;
			case "Home":
				await this.HandleHomeKey();
				break;
			case "End":
				await this.HandleEndKey();
				break;
			case "Escape":
				await this.HandleEscapeKey();
				break;
			case "Tab":
				if (e.ShiftKey) {
					await this.HandleShiftTabKey();
				} else {
					await this.HandleTabKey();
				}
				break;
			default:
				if (key.Length == 1 && char.IsLetterOrDigit(key[0])) {
					await this.HandleCharacterSearch(key.ToLowerInvariant());
				}
				break;
		}
	}

	private void HandleRightArrow() {
		if (this.HasSubMenu && this.expanded is false) {
			this.OpenSubmenu();
		}
	}

	private void HandleDownArrow() {
		this.tabIndex = "-1";
		this.RunAfterRender(async () => {
			if (this.ParentMenuItem is null) {
				await this.MenuInternal.MoveFocusAsync(1);
				return;
			}
			await this.ParentMenuItem.MoveFocusAsync(1);
		});
		this.Update();
	}

	private void HandleUpArrow() {
		this.tabIndex = "-1";
		this.RunAfterRender(async () => {
			if (this.ParentMenuItem is null) {
				await this.MenuInternal.MoveFocusAsync(-1);
				return;
			}
			await this.ParentMenuItem.MoveFocusAsync(-1);
		});
		this.Update();
	}

	private async Task HandleLeftArrow() {
		if (this.ParentMenuItem != null) {
			await this.CloseParentMenu();
			return;
		}
		await this.MenuInternal.HideAsync(true);
	}

	private async Task HandleHomeKey() {
		if (this.ParentMenuItem is not null) {
			this.ParentMenuItem.currentFocusIndex = 0;
			await this.ParentMenuItem.subMenuItems[0].FocusAsync();
			return;
		}
		await this.MenuInternal.FocusFirstAsync();
	}

	private async Task HandleEndKey() {
		if (this.ParentMenuItem is not null) {
			this.ParentMenuItem.currentFocusIndex = this.ParentMenuItem.subMenuItems.Count - 1;
			await this.ParentMenuItem.subMenuItems[this.ParentMenuItem.currentFocusIndex].FocusAsync();
			return;
		}
		await this.MenuInternal.FocusLastAsync();
	}

	private async Task HandleEscapeKey() {
		await this.MenuInternal.HideAsync(true);
		if (this.Menu.Trigger.HasValue()) {
			this.JSApp.FocusElement(this.Menu.Trigger, true);
		}
	}

	private async Task HandleTabKey() {
		await this.MenuInternal.HideAsync(true);
		this.JSApp.FocusNextElement(false, true, this.Menu.Trigger);
	}

	private async Task HandleShiftTabKey() {
		await this.MenuInternal.HideAsync(true);
		this.JSApp.FocusElement(this.Menu.Trigger, true);
	}

	private async Task HandleCharacterSearch(string key) {

		if (this.ParentMenuItem is null) {
			await this.MenuInternal.FocusByFirstCharacter(key);
			return;
		}

		await this.ParentMenuItem.FocusByFirstCharacter(key);

	}


	private void RegisterSubMenuItem(MenuItem menuItem) {
		this.subMenuItems.Add(menuItem);
	}

	private void UnregisterSubMenuItem(MenuItem menuItem) {
		this.subMenuItems.Remove(menuItem);
	}

	private async Task FocusByFirstCharacter(string key) {

		var currentIndex = this.currentFocusIndex;
		var startIndex = (currentIndex + 1) % this.subMenuItems.Count; // Start from the next item

		for (var i = 0; i < this.subMenuItems.Count; i++) {
			var index = (startIndex + i) % this.subMenuItems.Count;
			if (this.subMenuItems[index].menuText.StartsWith(key)) {
				this.subMenuItems[currentIndex].tabIndex = "-1";
				this.currentFocusIndex = index;
				await this.subMenuItems[index].FocusAsync();
				return;
			}
		}

		// If no match found, optionally start from the beginning
		for (var i = 0; i <= currentIndex; i++) {
			if (this.subMenuItems[i].menuText.StartsWith(key)) {
				this.subMenuItems[currentIndex].tabIndex = "-1";
				this.currentFocusIndex = i;
				await this.subMenuItems[i].FocusAsync();
				return;
			}
		}

	}

	private async Task MoveFocusAsync(int direction) {
		if (this.subMenuItems.Count == 0) {
			return;
		}

		var startIndex = this.currentFocusIndex;
		do {
			this.currentFocusIndex = (this.currentFocusIndex + direction + this.subMenuItems.Count) % this.subMenuItems.Count;
			if (!this.subMenuItems[this.currentFocusIndex].Disabled) {
				await this.subMenuItems[this.currentFocusIndex].FocusAsync();
				return;
			}
		} while (this.currentFocusIndex != startIndex);

		// If we've looped through all items and they're all disabled, don't change focus
		this.currentFocusIndex = startIndex;
	}

	private async Task ActivateAsync() {
		if (this.HasSubMenu) {
			if (this.expanded is false) {
				this.OpenSubmenu();
			}
			return;
		}
		await this.HandleClick(new MouseEventArgs());
	}

	private void OpenSubmenu() {
		this.tabIndex = "-1";
		this.expanded = true;
		this.RunAfterRender(async () => {
			this.currentFocusIndex = 0;
			await this.subMenuItems[0].FocusAsync();
		});
		this.Update();
	}

	private async Task CloseParentMenu(bool focusParentItem = true) {
		this.tabIndex = "-1";
		if (this.ParentMenuItem is not null) {
			await this.ParentMenuItem.CloseMenu(focusParentItem);
		}
	}

	internal async Task CloseMenu(bool focusItem = true) {
		if (focusItem) {
			await this.FocusAsync();
		}
		if (this.expanded) {
			this.expanded = false;
			this.Update();
		}
	}

	internal void EnableFocus() {
		this.tabIndex = "0";
		this.RunAfterRender(() => {
			this.JSApp.FocusElement(this.LIElementId, true);
		});
		this.Update();
	}


	protected override void OnInitialized() {
		base.OnInitialized();
		if (this.ParentMenuItem is null) {
			this.MenuInternal.RegisterMenuItem(this);
		} else {
			this.ParentMenuItem.RegisterSubMenuItem(this);
		}
	}

	protected override Task OnAfterFirstRenderAsync() {
		if (this.Label is not null) {
			this.menuText = this.Label.ToLowerInvariant();
			return Task.CompletedTask;
		}
		this.menuText = this.JSApp.GetElementText(this.menuItemElementRef).ToLowerInvariant();
		return Task.CompletedTask;
	}

	protected override void Dispose(bool disposing) {
		if (this.ParentMenuItem is null) {
			this.MenuInternal.UnregisterMenuItem(this);
		} else {
			this.ParentMenuItem.UnregisterSubMenuItem(this);
		}
		base.Dispose(disposing);
	}

}