namespace Cirreum.Components;

internal class InternalGridContext<TData>(DataGrid<TData> grid) {
	public DataGrid<TData> Grid { get; } = grid;
}