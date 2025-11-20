namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Optional custom max-width setting.
/// </summary>
public enum DialogSize {
	/// <summary>
	/// Default dialog max-width: 95vw at or below 480px breakpoint, 350px at or below 575px breakpoint, otherwise unset.
	/// </summary>
	[Display(Description = "Default dialog max-width: 95vw at or below 480px breakpoint, 350px at or below 575px breakpoint, otherwise unset.")]
	Default,
	/// <summary>
	/// Small dialog max-width: 95vw at or below 480px breakpoint, 350px at or below 575px breakpoint, otherwise 350px.
	/// </summary>
	[Display(Description = "Small dialog max-width: 95vw at or below 480px breakpoint, 350px at or below 575px breakpoint, otherwise 350px.")]
	Small,
	/// <summary>
	/// Small dialog max-width: 95vw at or below 480px breakpoint, 350px at or below 575px breakpoint, otherwise 500px.
	/// </summary>
	[Display(Description = "Small dialog max-width: 95vw at or below 480px breakpoint, 350px at or below 575px breakpoint, otherwise 500px.")]
	Medium,
	/// <summary>
	/// Large dialog max-width: 95vw at or below 480px breakpoint, 350px at or below 575px breakpoint, otherwise 500px above 576px breakpoint and 800px above 992px breakpoint.
	/// </summary>
	[Display(Description = "Large dialog max-width: 95vw at or below 480px breakpoint, 350px at or below 575px breakpoint, otherwise 500px above 576px breakpoint and 800px above 992px breakpoint.")]
	Large,
	/// <summary>
	/// Extra Large dialog max-width: 95vw at or below 480px breakpoint, 350px at or below 575px breakpoint, otherwise 500px above 576px breakpoint and 800px above 992px breakpoint and 1140px above 1200px breakpoint.
	/// </summary>
	[Display(Description = "Extra Large dialog max-width: 95vw at or below 480px breakpoint, 350px at or below 575px breakpoint, otherwise 500px above 576px breakpoint and 800px above 992px breakpoint and 1140px above 1200px breakpoint.")]
	ExtraLarge
}