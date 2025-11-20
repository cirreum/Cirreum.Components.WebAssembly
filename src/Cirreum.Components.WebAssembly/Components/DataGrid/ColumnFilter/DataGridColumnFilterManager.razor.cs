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

	private void ApplyFilter() {
		if (this.OnFilterApply.HasDelegate) {
			this.OnFilterApply.InvokeAsync();
		}
	}

	private void ClearFilter() {
		if (this.OnFilterClear.HasDelegate) {
			this.OnFilterClear.InvokeAsync();
		}
	}

	private void Close() {
		if (this.OnFilterCancel.HasDelegate) {
			this.OnFilterCancel.InvokeAsync();
		}
	}

}