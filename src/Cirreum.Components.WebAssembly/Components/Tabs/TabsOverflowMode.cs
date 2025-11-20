namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies the behavior for handling overflow in a tabs component.
/// </summary>
public enum TabsOverflowMode {

	/// <summary>
	/// Enables scrolling for overflowing tabs.
	/// Tabs that don't fit within the container will be accessible via scrolling.
	/// </summary>
	[Display(Name = "overflow-mode-scrollable")]
	Scrollable,

	/// <summary>
	/// Creates a popup or dropdown menu for overflowing tabs.
	/// Tabs that don't fit within the container will be placed in a separate selectable menu.
	/// </summary>
	[Display(Name = "overflow-mode-popup")]
	Popup,

	/// <summary>
	/// Allows tabs to wrap onto multiple lines.
	/// Tabs will form new rows as needed to fit within the container width.
	/// </summary>
	[Display(Name = "overflow-mode-multiline")]
	Multiline

}