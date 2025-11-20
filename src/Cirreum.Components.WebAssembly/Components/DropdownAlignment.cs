namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Dropdown Content Alignment: Left (-start), Right (-end) or Centered (-center).
/// </summary>
public enum DropdownAlignment {
	/// <summary>
	/// -start
	/// </summary>
	[Display(Name = "-start", ShortName = "-start")]
	Start,
	/// <summary>
	/// -end
	/// </summary>
	[Display(Name = "-end", ShortName = "-end")]
	End,
	/// <summary>
	/// -center
	/// </summary>
	[Display(Name = "-center", ShortName = "-center")]
	Centered
}