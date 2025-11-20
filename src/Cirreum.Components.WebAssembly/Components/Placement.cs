namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the placement options for Bootstrap components.
/// </summary>
public enum Placement {
	/// <summary>
	/// Automatic placement.
	/// </summary>
	[Display(Name = "auto")]
	Auto,

	/// <summary>
	/// Automatic placement with a start preference.
	/// </summary>
	[Display(Name = "auto-start")]
	AutoStart,

	/// <summary>
	/// Automatic placement with an end preference.
	/// </summary>
	[Display(Name = "auto-end")]
	AutoEnd,

	/// <summary>
	/// Top placement.
	/// </summary>
	[Display(Name = "top")]
	Top,

	/// <summary>
	/// Top-start placement.
	/// </summary>
	[Display(Name = "top-start")]
	TopStart,

	/// <summary>
	/// Top-end placement.
	/// </summary>
	[Display(Name = "top-end")]
	TopEnd,

	/// <summary>
	/// Right placement.
	/// </summary>
	[Display(Name = "right")]
	Right,

	/// <summary>
	/// Right-start placement.
	/// </summary>
	[Display(Name = "right-start")]
	RightStart,

	/// <summary>
	/// Right-end placement.
	/// </summary>
	[Display(Name = "right-end")]
	RightEnd,

	/// <summary>
	/// Bottom placement.
	/// </summary>
	[Display(Name = "bottom")]
	Bottom,

	/// <summary>
	/// Bottom-start placement.
	/// </summary>
	[Display(Name = "bottom-start")]
	BottomStart,

	/// <summary>
	/// Bottom-end placement.
	/// </summary>
	[Display(Name = "bottom-end")]
	BottomEnd,

	/// <summary>
	/// Left placement.
	/// </summary>
	[Display(Name = "left")]
	Left,

	/// <summary>
	/// Left-start placement.
	/// </summary>
	[Display(Name = "left-start")]
	LeftStart,

	/// <summary>
	/// Left-end placement.
	/// </summary>
	[Display(Name = "left-end")]
	LeftEnd
}