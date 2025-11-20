namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the behavior of a scroll action in HTML.
/// This enum corresponds to the 'behavior' property of the 'ScrollIntoViewOptions' interface used with the 'scrollIntoView()' method.
/// </summary>
public enum ScrollBehavior {
	/// <summary>
	/// Scrolls in a single jump if the scroll distance is large, or smoothly for shorter distances.
	/// This is the default scroll behavior.
	/// </summary>
	[Display(Name = "auto")]
	Auto,

	/// <summary>
	/// Scrolls in a single jump to the new position without smooth scrolling.
	/// </summary>
	[Display(Name = "instant")]
	Instant,

	/// <summary>
	/// Scrolls smoothly to the new position using an animation.
	/// </summary>
	[Display(Name = "smooth")]
	Smooth
}