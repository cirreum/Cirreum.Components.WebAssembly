namespace Cirreum.Components;

internal interface IDropdownItemInternal<TItemType> : IDropdownInteractiveItem<TItemType> {

	/// <summary>
	/// Set the <see cref="IDropdownInteractiveItem{T}.IsActive"/> value.
	/// </summary>
	/// <param name="value">The value to assign.</param>
	void SetIsActive(bool value);

	/// <summary>
	/// Focuses the element.
	/// </summary>
	/// <param name="preventScroll">Optional parameter to prevent scrolling into view.</param>
	ValueTask FocusAsync(bool preventScroll);

	/// <summary>
	/// Gets if this item is currently focused.
	/// </summary>
	bool IsFocused { get; }

}