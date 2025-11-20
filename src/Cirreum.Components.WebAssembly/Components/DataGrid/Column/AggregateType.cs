namespace Cirreum.Components;

/// <summary>
/// Defines the supported aggregation types for a <see cref="DataGridColumn{TData}"/>
/// </summary>
public enum AggregateType {
	/// <summary>
	/// Sum the column values.
	/// </summary>
	Sum,
	/// <summary>
	/// Calculate the average of the column values.
	/// </summary>
	Average,
	/// <summary>
	/// Count the column values.
	/// </summary>
	Count,
	/// <summary>
	/// Calculate the minimum column value.
	/// </summary>
	Min,
	/// <summary>
	/// Calculate the maximum column value.
	/// </summary>
	Max
}