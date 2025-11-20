namespace Cirreum.Components;

using Cirreum.Components.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Tabs organize content across different screens, data sets, and other interactions.
/// </summary>
public partial class Tabs {

	private readonly string tabsId = IdGenerator.Next;
	private readonly List<Tab> tabItems = [];
	private readonly List<Tab> overflowTabItems = [];
	private readonly List<string> tabPanels = [];
	private readonly string tabsPanelsId = IdGenerator.Next;
	private readonly Dictionary<string, object> Attributes = [];
	private readonly string tabsInstanceId = IdGenerator.Next;
	private ElementReference tabsInstance;
	private DotNetObjectReference<Tabs>? dotnetRef;
	private IJSInProcessObjectReference? module;
	private int _focusedTabIndex;
	private int _activeTabIndex;
	private bool HasOverflow;
	private bool IsVertical;

	/// <summary>
	/// Gets the currently selected tab; if any.
	/// </summary>
	private Tab? _selectedTab;

	[Inject]
	protected IJSAppModule JSApp { get; set; } = default!;

	[Parameter, EditorRequired]
	public RenderFragment ChildContent { get; set; } = default!;

	/// <summary>
	/// The type of the tabs - Tab, Pill or Underline.
	/// </summary>
	[Parameter]
	public TabType TabType { get; set; }

	/// <summary>
	/// Position of tab items.
	/// </summary>
	[Parameter]
	public TabPosition TabPosition { get; set; } = TabPosition.Top;

	/// <summary>
	/// Gets or sets the overflow behavior for tabs when they exceed the available space.
	/// </summary>
	/// <value>
	/// The overflow mode to use. Defaults to <see cref="TabsOverflowMode.Scrollable"/>.
	/// </value>
	/// <remarks>
	/// Available modes:
	/// <list type="bullet">
	///     <item>
	///         <term><see cref="TabsOverflowMode.Scrollable"/></term>
	///         <description>Enables horizontal scrolling for overflowing tabs.</description>
	///     </item>
	///     <item>
	///         <term><see cref="TabsOverflowMode.Popup"/></term>
	///         <description>Creates a popup menu for overflowing tabs.</description>
	///     </item>
	///     <item>
	///         <term><see cref="TabsOverflowMode.Multiline"/></term>
	///         <description>Wraps tabs onto multiple lines.</description>
	///     </item>
	/// </list>
	/// </remarks>
	[Parameter]
	public TabsOverflowMode OverflowMode { get; set; } = TabsOverflowMode.Scrollable;

	/// <summary>
	/// Defines how the tabs content will be rendered.
	/// </summary>
	[Parameter]
	public TabsRenderMode RenderMode { get; set; } = TabsRenderMode.Default;

	/// <summary>
	/// Makes each tab wide enough to fill the available width.
	/// </summary>
	[Parameter]
	public bool Filled { get; set; }

	/// <summary>
	/// Makes each tab wide enough to fill the available width, yet each Tab will be the same width.
	/// </summary>
	[Parameter]
	public bool Justified { get; set; }

	/// <summary>
	/// Optionally indicate if the tabs will function as a navigation menu.
	/// </summary>
	/// <remarks>
	/// When <see langword="true"/>, wraps the tabs list in a nav element.
	/// </remarks>
	[Parameter]
	public bool IsNavigationMenu { get; set; }

	/// <summary>
	/// The optional bootstrap border css class (e.g., border-primary)
	/// </summary>
	[Parameter]
	public string? BorderClass { get; set; }

	/// <summary>
	/// The optional fixed height of the tabs component.
	/// </summary>
	[Parameter]
	public int? Height { get; set; }

	/// <summary>
	/// The optional fixed width of the tabs component.
	/// </summary>
	[Parameter]
	public int? Width { get; set; }

	/// <summary>
	/// EventCallback to be notified when the active tab has changed.
	/// </summary>
	[Parameter]
	public EventCallback<Tab> OnTabChanged { get; set; }


	/// <summary>
	/// Gets the id of the Active Tab
	/// </summary>
	public string ActiveTabId => this._selectedTab?.Id ?? "";

	/// <summary>
	/// Gets the index of the Active Tab
	/// </summary>
	public int ActiveTabIndex => this._activeTabIndex;

	/// <summary>
	/// Gets the name of the Active Tab
	/// </summary>
	public string? ActiveTabName => this._selectedTab?.Name;


	private CssBuilder PositionCssBuilder =>
		CssBuilder.Empty()
			.AddClass("position-top", when: this.TabPosition == TabPosition.Top)
			.AddClass("position-right", when: this.TabPosition == TabPosition.Right)
			.AddClass("position-bottom", when: this.TabPosition == TabPosition.Bottom)
			.AddClass("position-left", when: this.TabPosition == TabPosition.Left);

	private string TabsInstanceCss =>
		CssBuilder.Default("card")
			.AddClass("flex-row", when: this.IsVertical)
			.AddClassIfNotEmpty(this.BorderClass)
		.Build();
	private string TabsInstanceStyle =>
		StyleBuilder.Empty()
			.AddStyle("height", $"{this.Height}px", when: this.Height.HasValue)
			.AddStyle("width", $"{this.Width}px", when: this.Width.HasValue)
		.Build();

	private string TabItemsContainerCss =>
		CssBuilder.Default("tab-items")
			.AddBuilder(this.PositionCssBuilder)
			.AddClass("overflowed", when: this.HasOverflow && this.OverflowMode != TabsOverflowMode.Multiline)
			.AddClass(this.OverflowMode.ToName())
			.AddClass("flex-column", when: this.IsVertical)
		.Build();

	private string ScrollContainerCss =>
		CssBuilder.Default("nav-scroll-container")
			.AddClass("flex-column", when: this.IsVertical)
		.Build();

	private string TabItemsCss =>
		CssBuilder.Default("nav")
			.AddClass("nav-tabs", when: this.TabType == TabType.Tabs)
			.AddClass("nav-pills", when: this.TabType == TabType.Pills)
			.AddClass("nav-underline", when: this.TabType == TabType.Underlines)
			.AddClass("nav-fill", when: this.Filled)
			.AddClass("nav-justified", when: this.Justified)
			.AddClass("flex-column", when: this.IsVertical)
		.Build();

	private List<Tab> PreviousTabs => this.overflowTabItems.Where(t => t.Index < this._activeTabIndex).ToList();
	private readonly string ScrollBackwardBtnId = IdGenerator.Next;
	private bool ScrollBackwardsDisabled =>
		this.tabItems.Count == 0 ||
		this.HasOverflow is false ||
		this.tabItems[0].Visible;
	private CssBuilder ScrollBackwardsBuilder => CssBuilder
		.Default("btn")
			.AddClass("btn-outline-primary")
			.AddClass("btn-sm")
			.AddClass("border-0")
			.AddClass("me-2", when: this.IsVertical is false)
			.AddClass("w-100", when: this.IsVertical)
			.AddClass("p-0", when: this.IsVertical)
			.AddClass("mb-2", when: this.IsVertical)
			.AddClass("ms-2", when: this.TabPosition == TabPosition.Right)
			.AddClass("me-0", when: this.TabPosition == TabPosition.Right)
			.AddClass("ms-0", when: this.TabPosition == TabPosition.Left)
			.AddClass("me-2", when: this.TabPosition == TabPosition.Left);
	private string ScrollBackwardsCss => CssBuilder
		.Empty()
			.AddClass("d-none", when: this.tabItems.Count == 0 || this.HasOverflow is false || this.OverflowMode != TabsOverflowMode.Scrollable)
			.AddBuilder(this.ScrollBackwardsBuilder, when: this.tabItems.Count > 0 && this.HasOverflow && this.OverflowMode == TabsOverflowMode.Scrollable)
		.Build();

	private List<Tab> NextTabs => this.overflowTabItems.Where(t => t.Index > this._activeTabIndex).ToList();
	private readonly string ScrollForwardBtnId = IdGenerator.Next;
	private bool ScrollForwardDisabled =>
		this.tabItems.Count == 0 ||
		this.HasOverflow is false ||
		this.tabItems[^1].Visible;
	private CssBuilder ScrollForwardCssBuilder => CssBuilder
		.Default("btn")
			.AddClass("btn-outline-primary")
			.AddClass("btn-sm")
			.AddClass("border-0")
			.AddClass("ms-2", when: this.IsVertical is false)
			.AddClass("w-100", when: this.IsVertical)
			.AddClass("p-0", when: this.IsVertical)
			.AddClass("mt-2", when: this.IsVertical)
			.AddClass("ms-2", when: this.TabPosition == TabPosition.Right)
			.AddClass("me-0", when: this.TabPosition == TabPosition.Right)
			.AddClass("ms-0", when: this.TabPosition == TabPosition.Left)
			.AddClass("me-2", when: this.TabPosition == TabPosition.Left);
	private string ScrollForwardCss => CssBuilder
		.Empty()
			.AddClass("d-none", when: this.tabItems.Count == 0 || this.HasOverflow is false || this.OverflowMode != TabsOverflowMode.Scrollable)
			.AddBuilder(this.ScrollForwardCssBuilder, when: this.tabItems.Count > 0 && this.HasOverflow && this.OverflowMode == TabsOverflowMode.Scrollable)
		.Build();
	private async Task OnContextMenuClicked(IMenuItem item) {
		await this.TryActivateTabByIdAsync(item.Value);
	}

	private string PopupSelectorButtonCss => CssBuilder
		.Empty()
			.AddClass("ms-2", when: this.IsVertical is false)
			.AddClass("w-100", when: this.IsVertical)
			.AddClass("p-0", when: this.IsVertical)
			.AddClass("mt-2", when: this.IsVertical)
			.AddClass("ms-2", when: this.TabPosition == TabPosition.Right)
			.AddClass("me-0", when: this.TabPosition == TabPosition.Right)
			.AddClass("ms-0", when: this.TabPosition == TabPosition.Left)
			.AddClass("me-2", when: this.TabPosition == TabPosition.Left)
		.Build();

	private string PopupSelectorCss => CssBuilder
		.Empty()
			.AddClass("d-none", when: this.tabItems.Count == 0 || this.HasOverflow is false || this.OverflowMode != TabsOverflowMode.Popup)
		.Build();
	private string PopupSelectorStyle => StyleBuilder
		.Empty()
			.AddStyle("width", "100%", when: this.IsVertical)
		.Build();

	private string NavIconCss(int direction) =>
		direction > 0
			? IsVertical
				? "bi bi-chevron-down" : "bi bi-chevron-right"
			: IsVertical
				? "bi bi-chevron-up" : "bi bi-chevron-left";


	/// <summary>
	/// Attempts to find a tab by its <see cref="Tab.Id"/>, and if found activates it.
	/// </summary>
	/// <param name="id">The tab's <see cref="Tab.Id"/>.</param>
	/// <returns><see langword="true"/> if found and activated; otherwise false.</returns>
	public async Task<bool> TryActivateTabByIdAsync(string id) {
		if (this.tabItems.Count == 0) {
			return false;
		}
		var tab = this.tabItems.FirstOrDefault(t => t.Id == id);
		if (tab is null) {
			return false;
		}
		if (tab.Disabled) {
			return false;
		}
		return await this.ActivateTabAsync(tab);
	}

	/// <summary>
	/// Attempts to find a tab by <see cref="Tab.Name"/>, and if found activates it.
	/// </summary>
	/// <param name="name">The tab's <see cref="Tab.Name"/>.</param>
	/// <returns><see langword="true"/> if found and activated; otherwise false.</returns>
	public async Task<bool> TryActivateTabByNameAsync(string name) {
		if (this.tabItems.Count == 0) {
			return false;
		}
		var tab = this.tabItems.FirstOrDefault(t => t.Name == name);
		if (tab is null) {
			return false;
		}
		if (tab.Disabled) {
			return false;
		}
		return await this.ActivateTabAsync(tab);
	}

	/// <summary>
	/// Set the Active tab by Index.
	/// </summary>
	/// <param name="index">The tab's zero based index.</param>
	/// <returns><see langword="true"/> if found and activated; otherwise false.</returns>
	public async Task<bool> ActivateTabByIndexAsync(int index) {
		if (this.tabItems.Count == 0) {
			return false;
		}
		var tab = this.tabItems.FirstOrDefault(t => t.Index == index);
		if (tab is null) {
			return false;
		}
		if (tab.Disabled) {
			return false;
		}
		return await this.ActivateTabAsync(tab);
	}

	/// <summary>
	/// Activates the first tab, if any.
	/// </summary>
	/// <returns><see langword="true"/> if found and activated; otherwise false.</returns>
	public async Task<bool> ActivateFirstTabAsync() {
		if (this.tabItems.Count == 0) {
			return false;
		}
		var tab = this.tabItems.FirstOrDefault();
		if (tab is null) {
			return false;
		}
		if (tab.Disabled) {
			return false;
		}
		return await this.ActivateTabAsync(tab);
	}

	/// <summary>
	/// Activates the last tab, if any.
	/// </summary>
	/// <returns><see langword="true"/> if found and activated; otherwise false.</returns>
	public async Task<bool> ActivateLastTabAsync() {
		if (this.tabItems.Count == 0) {
			return false;
		}
		var tab = this.tabItems.LastOrDefault();
		if (tab is null) {
			return false;
		}
		if (tab.Disabled) {
			return false;
		}
		return await this.ActivateTabAsync(tab);
	}

	internal async Task<bool> ActivateTabAsync(Tab tab) {

		if (tab.Disabled) {
			return false;
		}

		if (this._selectedTab is not null && this._selectedTab == tab) {
			// already active...
			return true;
		}

		// set the new tab as the Active Tab
		this._focusedTabIndex
			= this._activeTabIndex
			= tab.Index;

		// perform activation
		this.ActivateTab(tab);

		// notify listeners
		if (this.OnTabChanged.HasDelegate) {
			await this.OnTabChanged.InvokeAsync(this._selectedTab);
		}

		return true;

	}
	private void ActivateTab(bool scrollIntoView) {

		if (this.tabItems.Count == 0) {
			return;
		}

		var tab = this.tabItems.FirstOrDefault(t => t.Index == this._activeTabIndex);
		if (tab is null) {
			return;
		}

		this.ActivateTab(tab, scrollIntoView);

	}
	private void ActivateTab(Tab tab, bool scrollIntoView = true) {

		if (tab is null) {
			return;
		}

		// De-activate current tab
		if (this._selectedTab is not null) {
			this._selectedTab.Active = false;
			this.Update();
		}

		// Activate specified tab...
		this._selectedTab = tab;
		tab.Active = true;
		this.Attributes["aria-labelledby"] = tab.Id;

		// update the UI
		this.RunAfterRender(() => {
			if (scrollIntoView) {
				this.JSApp.ScrollElementIntoView(
					tab.ElementId,
					ScrollBehavior.Smooth,
					ScrollLogicalPosition.Nearest,
					ScrollLogicalPosition.Nearest);
			}
			this.module?.InvokeVoid("onTabSelected", this.tabsInstance, tab.tabElement);
		});
		this.Update();

	}

	private bool TryFindFocusableTab(int direction, [NotNullWhen(true)] out Tab? focusableTab) {
		focusableTab = null;
		if (this.tabItems.Count == 0) {
			return false;
		}

		var startIndex = this._focusedTabIndex;
		var currentIndex = startIndex;
		var currentCount = this.tabItems.Count;
		var indexedTabs = this.tabItems;

		do {
			// Move to the next/previous index, wrapping around at the ends
			currentIndex = (currentIndex + direction + currentCount) % currentCount;

			var tab = indexedTabs[currentIndex];
			if (tab.Disabled is false) {
				focusableTab = tab;
				return true;
			}
		} while (currentIndex != startIndex);

		// If we've looped through all items and haven't found a focusable tab
		return false;

	}

	private void ScrollBackwards() {
		this.module?.InvokeVoid("scroll", this.tabsInstance, -1);
	}
	private void ScrollForward() {
		this.module?.InvokeVoid("scroll", this.tabsInstance, 1);
	}


	[JSInvokable]
	public async Task HandleArrowUpKey() {
		if (this.TryFindFocusableTab(-1, out var previousTab)) {
			this._focusedTabIndex = previousTab.Index;
			await previousTab.FocusAsync(false);
		}
	}
	[JSInvokable]
	public async Task HandleArrowDownKey() {
		if (this.TryFindFocusableTab(1, out var nextTab)) {
			this._focusedTabIndex = nextTab.Index;
			await nextTab.FocusAsync(false);
		}
	}
	[JSInvokable]
	public async Task HandleArrowLeftKey() {
		if (this.TryFindFocusableTab(-1, out var previousTab)) {
			this._focusedTabIndex = previousTab.Index;
			await previousTab.FocusAsync(false);
		}
	}
	[JSInvokable]
	public async Task HandleArrowRightKey() {
		if (this.TryFindFocusableTab(1, out var nextTab)) {
			this._focusedTabIndex = nextTab.Index;
			await nextTab.FocusAsync(false);
		}
	}
	[JSInvokable]
	public async Task HandleEnterKey() {
		await this.ActivateTabByIndexAsync(this._focusedTabIndex);
	}
	[JSInvokable]
	public async Task HandleHomeKey() {
		await this.ActivateFirstTabAsync();
	}
	[JSInvokable]
	public async Task HandleEndKey() {
		await this.ActivateLastTabAsync();
	}

	[JSInvokable]
	public void HandleTabListScrolled(int[] visibleIndices) {
		this.overflowTabItems.Clear();
		foreach (var tab in this.tabItems) {
			var isVisible = visibleIndices.Contains(tab.Index);
			tab.Visible = isVisible;
			if (isVisible is false) {
				this.overflowTabItems.Add(tab);
			}
		}
		this.HasOverflow = this.overflowTabItems.Count > 0;
		this.Update();
	}


	internal void RegisterTab(Tab tab) {
		this.tabItems.Add(tab);
		tab.Index = this.tabItems.Count - 1;
		if (tab.InitiallyActive) {
			this._focusedTabIndex = this._activeTabIndex = tab.Index;
		}
	}

	private RenderFragment? footer;
	internal void RegisterFooter(TabsFooter tabsFooter) {
		this.footer = tabsFooter.ChildContent;
	}

	private void SetOrientation() {
		if (this.TabPosition == TabPosition.Left || this.TabPosition == TabPosition.Right) {
			this.IsVertical = true;
			this.Attributes["aria-orientation"] = "vertical";
		} else {
			this.IsVertical = false;
			this.Attributes.Remove("aria-orientation");
		}
	}
	private void ConfigureTabs() {
		this.RunAfterRender(() => {
			var visibleTabIndexes = this.module?.Invoke<int[]>("getVisibleTabs", this.tabsInstance);
			this.HandleTabListScrolled(visibleTabIndexes ?? []);
		});
		this.ActivateTab(false);
	}

	private void CheckTabTypeChanged(ParameterView parameters) {
		if (parameters.TryGetValue<TabType>(nameof(this.TabType), out var value) && this.TabType != value) {
			this.RunAfterRender(this.OnTabTypeChanged);
		}
	}
	private void OnTabTypeChanged() {
		this.module?.InvokeVoid("changeTabType", this.tabsInstance, this.TabType.ToString());
		this.ConfigureTabs();
	}

	private void CheckTabPositionChanged(ParameterView parameters) {
		if (parameters.TryGetValue<TabPosition>(nameof(this.TabPosition), out var value) && this.TabPosition != value) {
			this.RunAfterRender(this.OnTabPositionChanged);
		}
	}
	private void OnTabPositionChanged() {
		this.module?.InvokeVoid("changeTabPosition", this.tabsInstance, this.TabPosition.ToString());
		this.ConfigureTabs();
	}

	private void CheckOverflowModeChanged(ParameterView parameters) {
		if (parameters.TryGetValue<TabsOverflowMode>(nameof(this.OverflowMode), out var value) && this.OverflowMode != value) {
			this.RunAfterRender(this.OnOverflowModeChanged);
		}
	}
	private void OnOverflowModeChanged() {
		var visibleTabIndexes = this.module?.Invoke<int[]>("getVisibleTabs", this.tabsInstance);
		this.HandleTabListScrolled(visibleTabIndexes ?? []);
		Task.Run(async () => {
			await Task.Delay(150);
			this.module?.InvokeVoid("update", this.tabsInstance);
		});
	}

	private void CheckFilledChanged(ParameterView parameters) {
		if (parameters.TryGetValue<bool>(nameof(this.Filled), out var value) && this.Filled != value) {
			this.RunAfterRender(this.OnFilledChanged);
		}
	}
	private void OnFilledChanged() {
		this.module?.InvokeVoid("update", this.tabsInstance);
	}

	private void CheckJustifiedChanged(ParameterView parameters) {
		if (parameters.TryGetValue<bool>(nameof(this.Justified), out var value) && this.Justified != value) {
			this.RunAfterRender(this.OnJustifiedChanged);
		}
	}
	private void OnJustifiedChanged() {
		this.module?.InvokeVoid("update", this.tabsInstance);
	}

	public override Task SetParametersAsync(ParameterView parameters) {

		if (this.Rendered) {

			this.CheckTabTypeChanged(parameters);

			this.CheckTabPositionChanged(parameters);

			this.CheckOverflowModeChanged(parameters);

			this.CheckFilledChanged(parameters);

			this.CheckJustifiedChanged(parameters);

		}

		return base.SetParametersAsync(parameters);

	}

	protected override void OnParametersSet() {
		this.SetOrientation();
	}

	protected override void OnInitialized() {
		this.dotnetRef = DotNetObjectReference.Create(this);
		this.Attributes.Add("role", "tablist");
	}

	protected override async Task OnAfterFirstRenderAsync() {
		const string jsPath = "./_content/Cirreum.Blazor.Components/Components/Tabs/Tabs.razor.js";
		this.module = await this.JSApp.InvokeAsync<IJSInProcessObjectReference>("import", jsPath);
		this.module.InvokeVoid(
			"connect",
			this.tabsInstance,
			this.dotnetRef,
			this.TabType.ToString(),
			this.TabPosition.ToString());
		this.RunAfterRender(this.ConfigureTabs);
		this.Update();
	}

	protected override void Dispose(bool disposing) {
		this.module?.InvokeVoid("disconnect", this.tabsInstance);
		this.dotnetRef?.Dispose();
		this.module?.Dispose();
		base.Dispose(disposing);
	}

}