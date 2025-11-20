namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// The available drop down display options (dynamic or static).
/// </summary>
public enum DropdownDisplay {
	/// <summary>
	/// Default display type. Uses PopperJs for automatic aligment, except when contained in a Navbar.
	/// </summary>
	[Display(Name = "dynamic")]
	Dynamic = 0,
	/// <summary>
	/// Static, for when you want to use the Responsive Sizing options. (see: <see cref="ResponsiveSize"/>)
	/// </summary>
	[Display(Name = "static")]
	Static
}