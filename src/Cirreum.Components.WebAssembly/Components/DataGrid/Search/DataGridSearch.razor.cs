namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

public partial class DataGridSearch<TData> {

	internal string GridSearchValue = "";

	private void HandleTextChanged(string newText) {
		this.GridSearchValue = newText;
		this.GridContext.Grid.ApplyCurrentData();
		this.GridContext.Grid.Update();
	}

	[CascadingParameter]
	internal InternalGridContext<TData> GridContext { get; set; } = default!;

	[Parameter]
	public int DebounceDelay { get; set; } = 500;

	[Parameter]
	public bool HasData { get; set; }

}