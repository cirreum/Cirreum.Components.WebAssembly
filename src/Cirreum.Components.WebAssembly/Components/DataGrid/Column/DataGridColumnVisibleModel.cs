namespace Cirreum.Components;
internal record DataGridColumnVisibleModel(string ColumnId, string Title) {

	public string ElementId = IdGenerator.Next;

	public bool IsFiltered { get; set; }

	public bool IsVisible { get; set; }

	public string IsVisibleAttrValue => this.IsVisible.ToString().ToLowerInvariant();

	internal static DataGridColumnVisibleModel FromDataGridColumn<TData>(DataGridColumn<TData> column) {

		return new DataGridColumnVisibleModel(column.ColumnId, column.HeaderText) {
			IsFiltered = column.IsFiltered,
			IsVisible = column.IsVisible
		};

	}

}