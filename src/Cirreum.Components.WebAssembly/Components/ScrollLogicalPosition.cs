namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the logical position for scrolling an element into view.
/// This enum corresponds to the 'scrollIntoViewOptions' parameter of the 'scrollIntoView()' method in HTML.
/// </summary>
public enum ScrollLogicalPosition {
	/// <summary>
	/// Centers the element within the scrollable area.
	/// </summary>
	[Display(Name = "center")]
	Center,

	/// <summary>
	/// Scrolls the element to the end of the scrollable area.
	/// </summary>
	[Display(Name = "end")]
	End,

	/// <summary>
	/// Scrolls to the nearest edge of the element.
	/// If the element is fully visible, no scrolling occurs.
	/// </summary>
	[Display(Name = "nearest")]
	Nearest,

	/// <summary>
	/// Scrolls the element to the start of the scrollable area.
	/// </summary>
	[Display(Name = "start")]
	Start
}