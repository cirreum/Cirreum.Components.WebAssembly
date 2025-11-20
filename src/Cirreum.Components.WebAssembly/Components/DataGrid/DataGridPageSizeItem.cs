namespace Cirreum.Components;

/// <summary>
/// A record to hold the value and display of a page size drop down item.
/// </summary>
/// <param name="Value">The value of the page size option.</param>
/// <param name="Display">The display string of the page size option.</param>
public record DataGridPageSizeItem(int Value, string Display) {
	/// <summary>
	/// Creates an <see cref="DataGridPageSizeItem"/> instance with the values
	/// to indicate a page size of -1 (show all items - 'All').
	/// </summary>
	public static DataGridPageSizeItem All => new(-1, "All");
	/// <summary>
	/// Creates a default <see cref="DataGridPageSizeItem"/> instance using the
	/// <see cref="DefaultPageSize"/> value.
	/// </summary>
	public static DataGridPageSizeItem Default => new(DefaultPageSize, $"{DefaultPageSize}");
	/// <summary>
	/// The default PageSize value (10)
	/// </summary>
	public static readonly int DefaultPageSize = 10;
}