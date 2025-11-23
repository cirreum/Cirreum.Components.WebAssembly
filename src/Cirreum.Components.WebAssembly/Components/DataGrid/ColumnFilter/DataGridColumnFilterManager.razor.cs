namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

[CascadingTypeParameter(nameof(TData))]
public partial class DataGridColumnFilterManager<TData> {

	[CascadingParameter]
	public DataGridColumn<TData> Column { get; set; } = default!;

	[Parameter, EditorRequired]
	public RenderFragment ChildContent { get; set; } = default!;

	[Parameter]
	public EventCallback OnFilterApply { get; set; } = default!;

	[Parameter]
	public EventCallback OnFilterClear { get; set; } = default!;

	[Parameter]
	public EventCallback OnFilterCancel { get; set; } = default!;

	private async Task ApplyFilter() {
		await this.OnFilterApply.InvokeAsync();
	}

	private async Task ClearFilter() {
		await this.OnFilterClear.InvokeAsync();
	}

	private async Task Close() {
		await this.OnFilterCancel.InvokeAsync();
	}

}