namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;
using static Cirreum.Components.ToastStyles;

/// <summary>
/// Represents the position of a toast notification.
/// </summary>
public enum ToastPosition {

	/// <summary>
	/// Positions the toast in the top-right corner of the container.
	/// </summary>
	[Display(Name = Classes.Positions.TopRight)]
	TopRight,

	/// <summary>
	/// Positions the toast in the top-left corner of the container.
	/// </summary>
	[Display(Name = Classes.Positions.TopLeft)]
	TopLeft,

	/// <summary>
	/// Positions the toast at the top-center of the container.
	/// </summary>
	[Display(Name = Classes.Positions.TopCenter)]
	TopCenter,

	/// <summary>
	/// Positions the toast at the top, spanning the full width of the container.
	/// </summary>
	[Display(Name = Classes.Positions.TopFullWidth)]
	TopFullWidth,

	/// <summary>
	/// Positions the toast in the bottom-left corner of the container.
	/// </summary>
	[Display(Name = Classes.Positions.BottomLeft)]
	BottomLeft,

	/// <summary>
	/// Positions the toast in the bottom-right corner of the container.
	/// </summary>
	[Display(Name = Classes.Positions.BottomRight)]
	BottomRight,

	/// <summary>
	/// Positions the toast at the bottom-center of the container.
	/// </summary>
	[Display(Name = Classes.Positions.BottomCenter)]
	BottomCenter,

	/// <summary>
	/// Positions the toast at the bottom, spanning the full width of the container.
	/// </summary>
	[Display(Name = Classes.Positions.BottomFullWidth)]
	BottomFullWidth

}