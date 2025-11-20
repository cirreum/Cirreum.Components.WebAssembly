namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

/// <summary>
/// A container for a <see cref="Tab"/>'s content.
/// </summary>
public partial class TabPanel {

	private readonly Dictionary<string, object> Attributes = [];
	private bool IsLazyLoaded;

	/// <summary>
	/// Cascaded parent <see cref="Tabs"/> component.
	/// </summary>
	[CascadingParameter]
	private Tabs Parent { get; set; } = default!;

	/// <summary>
	/// Specifies the content to be rendered inside this <see cref="TabPanel"/>.
	/// </summary>
	[Parameter]
	public RenderFragment ChildContent { get; set; } = default!;

	private TabsRenderMode RenderMode => this.Parent.RenderMode;

	[Parameter, EditorRequired]
	public string TabId { get; set; } = "";

	[Parameter, EditorRequired]
	public string TabPanelId { get; set; } = "";

	[Parameter, EditorRequired]
	public bool IsActive { get; set; }


	private bool ShouldLoad =>
		this.RenderMode == TabsRenderMode.Default ||
		(this.RenderMode == TabsRenderMode.LazyReload && this.IsActive) ||
		(this.RenderMode == TabsRenderMode.LazyLoad && this.IsLazyLoaded);

	private string ClassList =>
		CssBuilder.Default("tab-pane fade")
			.AddClass("show active", when: this.IsActive)
		.Build();

	protected override Task OnParametersSetAsync() {
		if (this.IsActive) {
			this.IsLazyLoaded = (this.RenderMode == TabsRenderMode.LazyLoad);
		}
		return base.OnParametersSetAsync();
	}

	protected override void OnInitialized() {
		base.OnInitialized();
		this.ElementId = this.TabPanelId;
		this.Attributes.Add("role", "tabpanel");
		this.Attributes.Add("aria-labelledby", this.TabId);
		this.Attributes.Add("tabindex", "0");
	}

}