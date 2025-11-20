namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies the size options for form control elements.
/// </summary>
/// <remarks>
/// This enum is used to define the size of form control elements, allowing for consistent sizing across different form components such as input fields, select dropdowns, and buttons.
/// </remarks>
public enum FormControlSize {
	/// <summary>
	/// Represents the default size for form control elements.
	/// </summary>
	/// <remarks>
	/// Use this when you want the form control to have its standard, unmodified size.
	/// This is typically suitable for most form layouts and designs.
	/// </remarks>
	[Display(Name = "", ShortName = "")]
	Default,

	/// <summary>
	/// Represents an even smaller size for form control elements.
	/// </summary>
	/// <remarks>
	/// Use this when you want the form control to have a dense, space-saving appearance.
	/// This is useful for forms with limited space, dense layouts, or when creating a more condensed user interface.
	/// </remarks>
	[Display(Name = "form-control-xs", ShortName = "-xs")]
	ExtraSmall,

	/// <summary>
	/// Represents a small size for form control elements.
	/// </summary>
	/// <remarks>
	/// Use this when you want the form control to have a compact, space-saving appearance.
	/// This is useful for forms with limited space, dense layouts, or when creating a more condensed user interface.
	/// </remarks>
	[Display(Name = "form-control-sm", ShortName = "-sm")]
	Small,

	/// <summary>
	/// Represents a large size for form control elements.
	/// </summary>
	/// <remarks>
	/// Use this when you want the form control to have a more prominent, easily interactable appearance.
	/// This is useful for improving accessibility, touch-friendly interfaces, or for emphasizing certain form controls.
	/// </remarks>
	[Display(Name = "form-control-lg", ShortName = "-lg")]
	Large
}