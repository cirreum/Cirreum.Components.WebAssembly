namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents CSS text alignment options.
/// </summary>
public enum TextAlign {
	/// <summary>
	/// No specific text alignment applied.
	/// </summary>
	[Display(Name = "")]
	None,

	/// <summary>
	/// Aligns the text to the start of the container.
	/// In left-to-right languages, this is typically the left side.
	/// </summary>
	[Display(Name = "start")]
	Start,

	/// <summary>
	/// Aligns the text to the center of the container.
	/// </summary>
	[Display(Name = "center")]
	Center,

	/// <summary>
	/// Aligns the text to the end of the container.
	/// In left-to-right languages, this is typically the right side.
	/// </summary>
	[Display(Name = "end")]
	End
}