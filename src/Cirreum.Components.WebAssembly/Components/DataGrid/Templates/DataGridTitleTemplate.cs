namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

public class DataGridTitleTemplate<TData> : ComponentBase {

	[CascadingParameter]
	internal InternalGridContext<TData> GridContext { get; set; } = default!;

	/// <summary>
	/// Content to show
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// When initialized, tell table of this item
	/// </summary>
	protected override void OnInitialized() {
		this.GridContext.Grid.SetTitleTemplate(this);
	}

}