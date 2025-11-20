namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

/// <summary>
/// A clickable item for <see cref="Tabs"/> component.
/// </summary>
public partial class Tab {

	private readonly Dictionary<string, object> Attributes = [];
	internal ElementReference tabElement;

	internal RenderFragment RenderTab;

	public Tab() {
		this.RenderTab = this.RenderTabContent;
	}


	/// <summary>
	/// Cascaded parent <see cref="Tabs"/> component.
	/// </summary>
	[CascadingParameter]
	protected Tabs? Parent { get; set; }

	/// <summary>
	/// Defines the tab's Id.
	/// </summary>
	/// <remarks>
	/// Auto-generated if not provided.
	/// </remarks>
	[Parameter]
	public string Id { get; set; } = "";

	/// <summary>
	/// The name of the tab and used as the text for the label, unless a 
	/// <see cref="LabelTemplate"/> is provided. A value is Required.
	/// </summary>
	[Parameter, EditorRequired]
	public required string Name { get; set; } = "";

	/// <summary>
	/// Gets or sets the customized content to use for Label content.
	/// </summary>
	[Parameter]
	public RenderFragment? LabelTemplate { get; set; }

	/// <summary>
	/// Gets or sets the customized content of this tab panel.
	/// </summary>
	[Parameter]
	public RenderFragment? PanelContent { get; set; }

	/// <summary>
	/// Gets or sets the content to be rendered inside the component.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Flag to indicate that the tab is not responsive for user interaction.
	/// </summary>
	[Parameter]
	public bool Disabled { get; set; }

	/// <summary>
	/// Flag to indicate that this tab is initially the Active tab.
	/// </summary>
	[Parameter]
	public bool InitiallyActive { get; set; }

	internal int Index { get; set; }

	internal bool Active { get; set; }

	internal bool Visible { get; set; }

	internal string TabPanelId = IdGenerator.Next;

	internal async Task FocusAsync(bool preventScroll) {
		await this.tabElement.FocusAsync(preventScroll);
	}

	private string ClassList =>
		CssBuilder.Default("nav-link")
			.AddClass("text-nowrap", when: this.Parent?.Justified != true)
			.AddClass("active", when: this.Active)
			.AddClass("disabled", when: this.Disabled)
		.Build();


	/// <summary>
	/// Handles the item onclick event.
	/// </summary>
	/// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	protected async Task ClickHandler(MouseEventArgs eventArgs) {

		if (this.Disabled) {
			return;
		}

		if (this.Parent is not null) {
			await this.Parent.ActivateTabAsync(this);
		}

	}

	protected override void OnInitialized() {
		if (this.Id.HasValue()) {
			this.ElementId = this.Id;
		} else {
			this.Id = this.ElementId;
		}
		this.Parent?.RegisterTab(this);
		this.Attributes["aria-controls"] = this.TabPanelId;
	}


}