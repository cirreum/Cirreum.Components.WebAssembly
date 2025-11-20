namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Responsive Break Size Class Extension:
/// Default (""): &lt; 576;
/// Small (-sm): ≥ 576px;
/// Medium (-md): ≥ 768px;
/// Large (-lg): ≥ 992px;
/// ExtraLarge (-xl): ≥ 1200px;
/// ExtraExtraLarge (-xxl): ≥ 1400px;
/// </summary>
public enum ResponsiveSize {
	/// <summary>
	/// none; (equates to -xs) &lt; 576;
	/// </summary>
	[Display(Name = "", ShortName = "")]
	Default,
	/// <summary>
	/// -sm: ≥ 576px;
	/// </summary>
	[Display(Name = "-sm", ShortName = "-sm")]
	Small,
	/// <summary>
	/// -md: ≥ 768px;
	/// </summary>
	[Display(Name = "-md", ShortName = "-md")]
	Medium,
	/// <summary>
	/// -lg: ≥ 992px;
	/// </summary>
	[Display(Name = "-lg", ShortName = "-lg")]
	Large,
	/// <summary>
	/// -xl: ≥ 1200px;
	/// </summary>
	[Display(Name = "-xl", ShortName = "-xl")]
	ExtraLarge,
	/// <summary>
	/// -xl: ≥ 1400px;
	/// </summary>
	[Display(Name = "-xxl", ShortName = "-xxl")]
	ExtraExtraLarge
}