namespace Cirreum.Components;

/// <summary>
/// Extends the <see cref="IDropdownItemBase"/> with support for being interactive.
/// </summary>
/// <remarks>
/// Supports IsActive/SetIsActive, and Value
/// </remarks>
/// <typeparam name="TItemType">The 'Type' of item this dropdown item represents.</typeparam>
public interface IDropdownInteractiveItem<TItemType> : IDropdownItemBase {

	/// <summary>
	/// The unique id of this item.
	/// </summary>
	string Id { get; }

	/// <summary>
	/// Gets if this item is the active item.
	/// </summary>
	bool IsActive { get; }

	/// <summary>
	/// Is this item disabled.
	/// </summary>
	bool IsDisabled { get; set; }

	/// <summary>
	/// The 'value' associated with this item.
	/// </summary>
	TItemType Value { get; set; }

}