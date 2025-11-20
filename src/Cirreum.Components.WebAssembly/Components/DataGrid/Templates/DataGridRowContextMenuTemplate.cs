namespace Cirreum.Components;
using Microsoft.AspNetCore.Components;

[CascadingTypeParameter(nameof(TData))]
public sealed class DataGridRowContextMenuTemplate<TData> : ComponentBase {

	[CascadingParameter]
	internal InternalGridContext<TData> GridContext { get; set; } = default!;

	[Parameter, EditorRequired]
	public RenderFragment ChildContent { get; set; } = default!;

	/// <summary>
	/// When initialized, tell table of this item
	/// </summary>
	protected override void OnInitialized() {
		this.GridContext.Grid.SetContextMenuTemplate(this);
	}

}