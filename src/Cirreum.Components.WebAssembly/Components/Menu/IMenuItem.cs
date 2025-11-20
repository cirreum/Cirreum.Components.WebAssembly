namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

public interface IMenuItem {

	/// <summary>
	/// Gets the owning <see cref="IMenu"/> instance that
	/// contains this menu item.
	/// </summary>
	IMenu Menu { get; }

	/// <summary>
	/// Gets or sets a value to be associated with this item.
	/// </summary>
	/// <remarks>
	/// This allows for an additional arbitrary non-displayed
	/// value that can be evaluated upon selection.
	/// </remarks>
	string Value { get; set; }

	/// <summary>
	/// Gets or sets the menu item text that will be displayed as the menu item's label.
	/// </summary>
	string? Label { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="RenderFragment"/> containing rthe contents for this menu item.
	/// </summary>
	/// <remarks>
	/// If a value is provided, <see cref="Label"/> is not necessary as this value will
	/// be rendered in place of the the <see cref="Label"/>.
	/// </remarks>
	RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the menu item is disabled.
	/// </summary>
	bool Disabled { get; set; }

	/// <summary>
	/// Move focus to this menu item.
	/// </summary>
	/// <returns>An awaitable task</returns>
	Task FocusAsync();

	/// <summary>
	/// Event raised when the user click on this item.
	/// </summary>
	EventCallback<MouseEventArgs> OnClick { get; set; }

}