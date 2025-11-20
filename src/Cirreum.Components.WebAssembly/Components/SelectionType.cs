namespace Cirreum.Components;

/// <summary>
/// Defines the selection behavior for data related components.
/// </summary>
public enum SelectionType {
	/// <summary>
	/// No selection is allowed. Users cannot select any items in the component.
	/// </summary>
	None,
	/// <summary>
	/// Only one item can be selected at a time. Selecting a new item will deselect the previously selected item.
	/// </summary>
	Single,
	/// <summary>
	/// Multiple items can be selected simultaneously. Users can select any number of items in the component.
	/// </summary>
	Multiple
}