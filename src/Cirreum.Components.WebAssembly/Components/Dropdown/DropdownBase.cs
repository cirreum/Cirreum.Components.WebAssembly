namespace Cirreum.Components;

using Cirreum.Components.Interop;
using Cirreum.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

/// <summary>
/// A reusable base dropdown button or link for use with a Dropdown Button or a Nav Item Dropdown Link.
/// </summary>
/// <typeparam name="TItemType">The 'Type' of item the dropdown list will represents.</typeparam>
/// <remarks>
/// Follows the disclosure pattern.
/// https://www.w3.org/WAI/ARIA/apg/patterns/disclosure/
/// </remarks>
[CascadingTypeParameter(nameof(TItemType))]
public abstract class DropdownBase<TItemType> : BaseAfterRenderComponent, IDropdown<TItemType> {

	private const string NAVBAR_SELECTOR = ".navbar";
	private const string DROPDOWN_CENTERED_CSSCLASS = "dropdown-centered";
	private const string DROPUP_CENTERED_CSSCLASS = "dropup-centered";
	private const string BTN_GROUP_CSSCLASS = "btn-group";
	private const string BTN_CSSCLASS = "btn";
	private const string NAV_ITEM_CSSCLASS = "nav-item";
	private const string NAV_LINK_CSSCLASS = "nav-link";

	private string? currentPopperId;
	private string dropdownCss = "";
	protected string buttonCss = "";

	private readonly List<IDropdownItemInternal<TItemType>> InternalItems = [];

	[Inject]
	protected IJSAppModule JSApp { get; set; } = default!;

	[Inject]
	IClickDetectorService ClickDetectorService { get; set; } = default!;

	/// <inheritdoc/>
	[Parameter]
	public string Text { get; set; } = "";

	/// <inheritdoc/>
	[Parameter]
	public RenderFragment? TextTemplate { get; set; } = default!;

	/// <inheritdoc/>
	[Parameter, EditorRequired]
	public RenderFragment ChildContent { get; set; } = default!;

	/// <summary>
	/// The minimum width of the dropdown content, in 'rem'. Default: 10.0
	/// </summary>
	[Parameter]
	public double DropdownMinWidth { get; set; } = DefaultDropdownMinWidth;

	/// <summary>
	/// Default value for the <see cref="DropdownMinWidth"/>.
	/// </summary>
	public const double DefaultDropdownMinWidth = 10.0;

	/// <inheritdoc/>
	[Parameter]
	public bool IsDisabled { get; set; }
	protected string AriaIsDisabled => this.IsDisabled.ToAttributeValue();

	/// <inheritdoc/>
	[Parameter]
	public bool TextWrap { get; set; }

	/// <summary>
	/// Should the button be displayed as a split button.
	/// </summary>
	[Parameter]
	public bool IsSplitButton { get; set; }

	/// <inheritdoc/>
	[Parameter]
	public EventCallback<DropdownBase<TItemType>> OnSplitButtonClicked { get; set; }
	protected void HandleSplitButtonClicked() {
		if (this.OnSplitButtonClicked.HasDelegate) {
			this.OnSplitButtonClicked.InvokeAsync(this);
		}
	}

	/// <summary>
	/// The optional tooltip.
	/// </summary>
	[Parameter]
	public string Tooltip { get; set; } = string.Empty;

	public bool HasTooltip { get; private set; }


	/// <summary>
	/// Gets or sets the dropdown button color. Default: ContextualColor.Primary
	/// </summary>
	[Parameter]
	public ButtonColor ButtonColor { get; set; } = ButtonColor.Primary;

	/// <summary>
	/// Gets or sets the dropdown button size. Default: ResponsiveSize.Default
	/// </summary>
	[Parameter]
	public ButtonSize ButtonSize { get; set; } = ButtonSize.Default;

	/// <inheritdoc/>
	[Parameter]
	public bool ButtonOutlined { get; set; }

	/// <inheritdoc/>
	[Parameter]
	public string ButtonCss { get; set; } = "";

	/// <inheritdoc/>
	[Parameter]
	public string? ButtonStyle { get; set; }

	/// <inheritdoc/>
	[Parameter]
	public string ContainerCss { get; set; } = "";

	/// <inheritdoc/>
	[Parameter]
	public string? ContainerStyle { get; set; } = "";

	/// <inheritdoc/>
	[Parameter]
	public DropdownDirection DropdownDirection { get; set; } = DropdownDirection.Down;

	/// <inheritdoc/>
	[Parameter]
	public DropdownDisplay DropdownDisplayType { get; set; }

	/// <inheritdoc/>
	[Parameter]
	public DropdownAlignment ResponsiveAlignment { get; set; } = DropdownAlignment.Start;

	/// <summary>
	/// Default: ResponsiveSize.Default (xs): &lt; 576;
	/// </summary>
	[Parameter]
	public ResponsiveSize DropdownStaticSize { get; set; } = ResponsiveSize.Default;

	/// <inheritdoc/>
	[Parameter]
	public string AriaLabel { get; set; } = string.Empty;

	/// <inheritdoc/>
	[Parameter]
	public EventCallback OnDropdownShow { get; set; }
	/// <inheritdoc/>
	[Parameter]
	public EventCallback OnDropdownShown { get; set; }

	/// <inheritdoc/>
	[Parameter]
	public EventCallback OnDropdownHide { get; set; }
	/// <inheritdoc/>
	[Parameter]
	public EventCallback OnDropdownHidden { get; set; }

	/// <inheritdoc/>
	[Parameter]
	public TItemType SelectedValue { get; set; } = default!;

	/// <inheritdoc/>
	[Parameter]
	public EventCallback<TItemType> SelectedValueChanged { get; set; }

	/// <inheritdoc/>
	[Parameter]
	public bool IsDropMenu { get; set; } = true;

	private IDropdownItemInternal<TItemType>? activeItem;

	/// <inheritdoc/>
	public async Task ToggleAsync() {
		if (this.IsShowing) {
			await this.HideAsync();
			return;
		}
		await this.ShowAsync();
	}

	/// <inheritdoc/>
	public async Task ShowAsync() {

		if (this.IsShowing) {
			return;
		}

		if (this.OnDropdownShow.HasDelegate) {
			await this.OnDropdownShow.InvokeAsync();
		}

		this.module?.InvokeVoid("CaptureFocusedElement", this.ElementId);

		this.IsShowing = true;

		if (this.ChildOfNavbar is false) {
			this.CreatePopper();
		}

		this.RunAfterRender(async () => {
			this.ClickDetectorService.Register(this.ElementId, async (id) => { await this.HideAsync(); });
			if (this.OnDropdownShown.HasDelegate) {
				await this.OnDropdownShown.InvokeAsync();
			}
		});

		this.Update();

	}

	/// <inheritdoc/>
	public async Task HideAsync() {

		this.ClickDetectorService.Unregister(this.ElementId);

		if (this.OnDropdownHide.HasDelegate) {
			await this.OnDropdownHide.InvokeAsync();
		}

		this.IsShowing = false;

		if (this.ChildOfNavbar is false) {
			this.DestroyPopper();
		}

		this.RunAfterRender(async () => {
			this.module?.InvokeVoid("RestoreFocusedElement", this.ElementId);
			if (this.OnDropdownHidden.HasDelegate) {
				await this.OnDropdownHidden.InvokeAsync();
			}
		});

		this.Update();

	}

	bool HasDropdownAlignment() {
		return this.IsDisplayStatic() is false &&
			this.ResponsiveAlignment != DropdownAlignment.Start;
	}

	bool IsDisplayStatic() {
		return this.DropdownDisplayType == DropdownDisplay.Static;
	}

	private readonly int[] popperOffset = [0, 2];
	Placement DropdownPlacement {
		get {

			var isEnd = this.ResponsiveAlignment == DropdownAlignment.End;
			var isCentered = this.ResponsiveAlignment == DropdownAlignment.Centered;

			return this.DropdownDirection switch {
				DropdownDirection.Right => Placement.RightStart,
				DropdownDirection.Left => Placement.LeftStart,
				DropdownDirection.Up => isEnd ? Placement.TopEnd : isCentered ? Placement.Top : Placement.TopStart,
				_ => isEnd ? Placement.BottomEnd : isCentered ? Placement.Bottom : Placement.BottomStart
			};
		}
	}
	void CreatePopper() {

		if (this.JSApp is null) {
			throw new NullReferenceException("JSApp instance is null.");
		}

		var options = new PopperJSOptions {
			TriggerElement = this.TargetElement,
			PopperElement = this.PopperRef,
			DisplayStatic = this.DropdownDisplayType == DropdownDisplay.Static,
			Placement = this.DropdownPlacement,
			Modifiers = [
				new PopperModifer (
						name: "offset",
						options: new {
							offset = popperOffset
						}
					),
				new PopperModifer(
					name: "preventOverflow",
					options: new {
						altBoundary = true,
						rootBoundary = Boundary.ClippingParents.ToName()
					}
				)
			]
		};

		if (this.ChildOfNavbar || this.IsDisplayStatic()) {
			options.Modifiers.Add(
				new PopperModifer(
					name: "applyStyles",
					enabled: false
				)
			);
		}

		this.currentPopperId = this.JSApp.ShowPopperJS(options);

	}
	void DestroyPopper() {
		var id = this.currentPopperId;
		this.currentPopperId = null;
		if (id is not null) {
			this.JSApp.ClosePopperJS(id);
		}
	}

	protected virtual string ButtonCssClass =>
		CssBuilder.Empty()
			.AddClass(() => CssBuilder.Default(this.buttonCss)
				.AddClass(this.ButtonSize.ToName(), when: this.ButtonSize != ButtonSize.Default)
				.AddClass($"{this.buttonCss}-outline{this.ButtonColor.ToShortName()}", when: this.ButtonColor != ButtonColor.None && this.ButtonOutlined is true)
				.AddClass(this.ButtonColor.ToName(), when: this.ButtonColor != ButtonColor.None && this.ButtonOutlined is false)
				.AddClassIfNotEmpty(this.ButtonCss)
				.AddClass("disabled", when: this.IsDisabled)
				.AddClass("text-wrap", when: this.TextWrap)
				.AddClass("show", when: this.IsShowing)
				.Build()
			, when: this.IsNav is false)
			.AddClass(() => CssBuilder.Default(this.buttonCss)
				.AddClassIfNotEmpty(this.ButtonCss)
				.AddClass("disabled", when: this.IsDisabled)
				.AddClass("text-wrap", when: this.TextWrap)
				.AddClass("show", when: this.IsShowing)
				.Build()
			, when: this.IsNav is true)
		.Build();

	/// <summary>
	/// Gets or sets if the Dropdown Button is for a Nav (.navbar-nav or .nav) or a
	/// Standalone Button (.btn-group). Should be set during initialization.
	/// </summary>
	protected bool IsNav { get; set; }

	protected bool ChildOfNavbar { get; private set; }

	protected bool IsShowing { get; private set; }
	protected string AriaIsExpanded => this.IsShowing.ToAttributeValue();

	protected readonly string DropdownId = IdGenerator.Next;

	protected readonly string DropdownMenuId = IdGenerator.Next;

	protected virtual string DropdownContainerCss => CssBuilder
		.Default(this.dropdownCss)
			.AddClass(this.DropdownDirection.ToName())
			.AddClassIfNotEmpty(this.ContainerCss)
		.Build();

	protected virtual string DropdownContainerStyle => StyleBuilder
		.Empty()
			.AddStyleIfNotEmpty(this.ContainerStyle)
		.Build();

	protected string MenuCssModifier => CssBuilder
		.Empty()
			.AddClass(() => $"dropdown-menu{this.DropdownStaticSize.ToShortName()}{this.ResponsiveAlignment.ToShortName()}", when: this.IsDisplayStatic())
			.AddClass(() => $"dropdown-menu{this.ResponsiveAlignment.ToShortName()}", when: this.HasDropdownAlignment())
		.Build();

	protected virtual string DropdownMenuClass => CssBuilder
		.Default("dropdown-menu")
			.AddClass(this.MenuCssModifier)
			.AddClass("show", when: this.IsShowing)
		.Build();

	protected virtual string? DropdownMenuStyle => StyleBuilder
		.Empty()
			.AddStyle("box-shadow", "var(--bs-box-shadow)")
			.AddStyle("--bs-dropdown-min-width", $"{this.DropdownMinWidth}rem", when: DefaultDropdownMinWidth != this.DropdownMinWidth)
		.NullIfEmpty();

	private bool isConnected;
	protected string SplitButtonCssClass => CssBuilder
		.Default(this.buttonCss)
			.AddClass(this.ButtonSize.ToName(), when: this.ButtonSize != ButtonSize.Default)
			.AddClass($"{this.buttonCss}-outline{this.ButtonColor.ToShortName()}", when: this.ButtonColor != ButtonColor.None && this.ButtonOutlined is true)
			.AddClass(this.ButtonColor.ToName(), when: this.ButtonColor != ButtonColor.None && this.ButtonOutlined is false)
			.AddClass("text-wrap", when: this.TextWrap)
		.Build();
	private DotNetObjectReference<DropdownBase<TItemType>>? dotnetRef;
	private IJSInProcessObjectReference? module;

	protected ElementReference PopperRef { get; set; }
	protected ElementReference TargetElement { get; set; }

	private async ValueTask FocusItemAsync(int index, bool preventScroll = false) {

		var item = this.InternalItems[index];

		await item.FocusAsync(preventScroll);

	}

	/// <summary>
	/// Sets the <see cref="SelectedValue"/> value.
	/// </summary>
	/// <param name="value">The value to set.</param>
	/// <param name="notifyValueChanged">Specify <see langword="true"/>, to call the <see cref="SelectedValueChanged"/> event callback. Default: <see langword="false"/></param>
	/// <returns>An awaitable task.</returns>
	/// <remarks>
	/// If <see cref="IsDropMenu"/> is <see langword="false"/>, will also synchronize the active item.
	/// </remarks>
	public virtual Task SetSelectedValue(TItemType value, bool notifyValueChanged = false) {

		if (ItemComparer.Compare(this.SelectedValue, value) != 0) {

			this.SelectedValue = value;

			if (this.IsDropMenu is false) {
				var item = this.GetItemFromValue(value);
				if (item is not null) {
					this.SetActiveItem(item);
				}
			}

			if (notifyValueChanged && this.SelectedValueChanged.HasDelegate) {
				return this.SelectedValueChanged.InvokeAsync(this.SelectedValue);
			}

		}

		return Task.CompletedTask;

	}
	private readonly Comparer<TItemType> ItemComparer = Comparer<TItemType>.Default;
	private IDropdownItemInternal<TItemType>? GetItemFromValue(TItemType value) {
		return this.InternalItems
			.FirstOrDefault(item => ItemComparer.Compare(item.Value, value) == 0);
	}
	private void SetActiveItem(IDropdownItemInternal<TItemType> item) {
		this.activeItem?.SetIsActive(false);
		this.activeItem = item;
		item.SetIsActive(true);
	}

	internal async Task HandleItemSelectionAsync(IDropdownItemInternal<TItemType> item) {

		if (this.IsDropMenu is false) {
			this.SetActiveItem(item);
		}

		this.SelectedValue = item.Value;

		await this.HideAsync();

		if (this.SelectedValueChanged.HasDelegate) {
			await this.SelectedValueChanged.InvokeAsync(this.SelectedValue);
		}

	}

	internal void RegisterItem(IDropdownItemInternal<TItemType> item) {
		this.InternalItems.Add(item);
		if (this.IsDropMenu is false) {
			if (ItemComparer.Compare(this.SelectedValue, item.Value) == 0) {
				this.SetActiveItem(item);
			}
		}
	}

	protected override void OnInitialized() {

		if (this.IsNav) {
			this.dropdownCss = NAV_ITEM_CSSCLASS;
			this.buttonCss = NAV_LINK_CSSCLASS;
		} else if (this.ResponsiveAlignment == DropdownAlignment.Centered && this.DropdownDirection == DropdownDirection.Up) {
			this.dropdownCss = DROPUP_CENTERED_CSSCLASS;
			this.buttonCss = BTN_CSSCLASS;
		} else if (this.ResponsiveAlignment == DropdownAlignment.Centered && this.DropdownDirection == DropdownDirection.Down) {
			this.dropdownCss = DROPDOWN_CENTERED_CSSCLASS;
			this.buttonCss = BTN_CSSCLASS;
		} else {
			this.dropdownCss = BTN_GROUP_CSSCLASS;
			this.buttonCss = BTN_CSSCLASS;
		}

		this.HasTooltip = this.Tooltip.HasValue();
		this.dotnetRef = DotNetObjectReference.Create(this);

		base.OnInitialized();

	}
	protected async override Task OnInitializedAsync() {
		const string jsPath = "./_content/Cirreum.Components.WebAssembly/Components/Dropdown/DropdownButton.razor.js";
		this.module = await this.JSApp.InvokeAsync<IJSInProcessObjectReference>("import", jsPath);
	}
	protected override Task OnAfterFirstRenderAsync() {
		this.ChildOfNavbar = this.JSApp.IsChildOf(this.TargetElement, NAVBAR_SELECTOR);
		return Task.CompletedTask;
	}
	protected async override Task OnAfterRenderAsync(bool firstRender) {

		await base.OnAfterRenderAsync(firstRender);

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
				this.dotnetRef,
				this.TargetElement,
				this.PopperRef);
			this.isConnected = true;
		}

	}

	protected override void Dispose(bool disposing) {
		this.DestroyPopper();
		this.module?.InvokeVoid("Disconnect", this.ElementId);
		this.dotnetRef?.Dispose();
		this.module?.Dispose();
		base.Dispose(disposing);
	}

	private async Task<bool> FocusNextItem() => await this.FocusAdjacentItem(+1);
	private async Task<bool> FocusPreviousItem() => await this.FocusAdjacentItem(-1);
	private async Task<bool> FocusAdjacentItem(int direction) {

		if (this.InternalItems == null || this.InternalItems.Count == 0) {
			return false;
		}

		// Find the index of the currently focused item
		var focusIndex = this.InternalItems.FindIndex(item => item.IsFocused);

		// If no focused item is found, focus the first non-disabled item
		if (focusIndex == -1) {
			focusIndex = this.InternalItems.FindIndex(item => !item.IsDisabled);

			// If no non-disabled item is found, return
			if (focusIndex == -1) {
				return false;
			}

			await this.FocusItemAsync(focusIndex);
			return true;
		}

		// Calculate the new index
		var newIndex = focusIndex;
		var wrappedAround = false;
		do {

			newIndex = (newIndex + direction + this.InternalItems.Count) % this.InternalItems.Count;

			// Check if we've wrapped around
			if ((direction == 1 && newIndex == 0) || (direction == -1 && newIndex == this.InternalItems.Count - 1)) {
				if (wrappedAround) {
					// If we've wrapped around again, it means no suitable item was found
					break;
				}

				wrappedAround = true;
			}

		} while (this.InternalItems[newIndex].IsDisabled && newIndex != focusIndex);


		// If we have circled back to the original active item...
		if (newIndex == focusIndex) {
			return false;
		}

		// select the next item...
		await this.FocusItemAsync(newIndex);
		return true;

	}

	[JSInvokable]
	public async void HandleArrowUpKey() {

		if (this.IsShowing is false) {
			await this.ShowAsync();
			return;
		}

		if (this.IsShowing) {

			if (this.InternalItems.Any(i => i.IsFocused) is false) {
				// start with the last item
				await this.FocusItemAsync(this.InternalItems.Count - 1);
				return;
			}

			await this.FocusPreviousItem();

		}
	}
	[JSInvokable]
	public async void HandleArrowDownKey() {

		if (this.IsShowing is false) {
			await this.ShowAsync();
			return;
		}

		if (this.IsShowing) {

			if (this.InternalItems.Any(i => i.IsFocused) is false) {
				// start with the first item
				await this.FocusItemAsync(0);
				return;
			}

			await this.FocusNextItem();

		}

	}
	[JSInvokable]
	public async void HandleEscapeKey() {
		if (this.IsShowing) {
			await this.HideAsync();
		}
	}
	[JSInvokable]
	public async void HandleEnterKey() {
		if (this.IsShowing) {
			var focusedItem = this.InternalItems.FirstOrDefault(i => i.IsFocused);
			if (focusedItem is not null) {
				await this.HandleItemSelectionAsync(focusedItem);
			}
		}
	}
	[JSInvokable]
	public async void HandleTabKey(string id, bool direction) {
		if (this.IsShowing) {
			if (this.DropdownId == id) {
				if (direction is false) {
					await this.HideAsync();
					return;
				}
				return;
			}
			var focusedItem = this.InternalItems.FirstOrDefault(i => i.Id == id);
			var lastItem = this.InternalItems.LastOrDefault();
			if (direction && focusedItem is not null && lastItem is not null && focusedItem.Id == lastItem.Id) {
				await this.HideAsync();
			}
		}
	}

}