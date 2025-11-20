namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// The available drop down direction (dropdown, dropup, dropstart and dropend).
/// </summary>
public enum DropdownDirection {
	/// <summary>
	/// "dropdown"
	/// </summary>
	[Display(Name = "dropdown")]
	Down,
	/// <summary>
	/// "dropup"
	/// </summary>
	[Display(Name = "dropup")]
	Up,
	/// <summary>
	/// "dropstart"
	/// </summary>
	[Display(Name = "dropstart")]
	Left,
	/// <summary>
	/// "dropend"
	/// </summary>
	[Display(Name = "dropend")]
	Right
}