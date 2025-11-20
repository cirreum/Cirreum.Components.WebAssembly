namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Breakpoint Sizes:
/// Small: ≥ 576px;
/// Medium: ≥ 768px;
/// Large: ≥ 992px;
/// ExtraLarge: ≥ 1200px;
/// ExtraExtraLarge: ≥ 1400px;
/// </summary>
public enum Breakpoint {
	/// <summary>
	/// ≥ 576px;
	/// </summary>
	[Display(Name = "576px")]
	Small,
	/// <summary>
	/// ≥ 768px;
	/// </summary>
	[Display(Name = "768px")]
	Medium,
	/// <summary>
	/// ≥ 992px;
	/// </summary>
	[Display(Name = "992px")]
	Large,
	/// <summary>
	/// ≥ 1200px;
	/// </summary>
	[Display(Name = "1200px")]
	ExtraLarge,
	/// <summary>
	/// ≥ 1400px;
	/// </summary>
	[Display(Name = "1400px")]
	ExtraExtraLarge
}