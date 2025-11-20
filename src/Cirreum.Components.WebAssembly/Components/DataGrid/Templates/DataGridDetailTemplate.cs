namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Detail Template
/// </summary>
/// <typeparam name="TData"></typeparam>
public class DataGridDetailTemplate<TData> : ComponentBase {

	[CascadingParameter]
	internal InternalGridContext<TData> GridContext { get; set; } = default!;

	/// <summary>
	/// The Detail Row Content.
	/// </summary>
	[Parameter]
	public RenderFragment<TData>? ChildContent { get; set; }

	/// <summary>
	/// When initialized, tell table of this item
	/// </summary>
	protected override void OnInitialized() {
		this.GridContext.Grid.SetDetailTemplate(this);
	}

}