namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

public class TabsFooter : ComponentBase {

	/// <summary>
	/// Cascaded parent <see cref="Tabs"/> component.
	/// </summary>
	[CascadingParameter]
	protected Tabs Parent { get; set; } = default!;

	[Parameter, EditorRequired]
	public RenderFragment ChildContent { get; set; } = default!;

	protected override void OnInitialized() {
		this.Parent.RegisterFooter(this);
	}

}