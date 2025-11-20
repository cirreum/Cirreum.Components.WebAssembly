namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

public class DataGridEmptyDataTemplate<TData> : ComponentBase {

	[CascadingParameter]
	internal InternalGridContext<TData> GridContext { get; set; } = default!;

	/// <summary>
	/// Content to show
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Attached to DataGrid
	/// </summary>
	protected override void OnInitialized() {
		this.GridContext.Grid.SetEmptyDataTemplate(this);
	}

}