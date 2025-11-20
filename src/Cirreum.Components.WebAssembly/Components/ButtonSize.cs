namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

public enum ButtonSize {
	/// <summary>
	/// none/normal/default;
	/// </summary>
	[Display(Name = "", ShortName = "")]
	Default,
	/// <summary>
	/// Large (-lg);
	/// </summary>
	[Display(Name = "btn-lg", ShortName = "-lg")]
	Large,
	/// <summary>
	/// Small (-sm);
	/// </summary>
	[Display(Name = "btn-sm", ShortName = "-sm")]
	Small,
	/// <summary>
	/// Extra Small (-xs);
	/// </summary>
	[Display(Name = "btn-xs", ShortName = "-xs")]
	ExtraSmall,
}