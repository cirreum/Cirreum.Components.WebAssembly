namespace Cirreum.Components;

/// <summary>
/// Represents a Cirreum color scheme used for Bootstrap-based UI theming.
/// Schemes are metadata-only and contain everything a UI component needs
/// to display theme options and resolve the appropriate stylesheet.
/// </summary>
/// <param name="Id">
/// A unique identifier for the color scheme (e.g., "default", "aspire").
/// </param>
/// <param name="DisplayName">
/// The human-readable name shown in UI theme selectors.
/// </param>
/// <param name="CssKey">
/// The key used to generate or resolve stylesheet filenames, e.g.:
/// <c>cirreum-bootstrap-{CssKey}.css</c>.
/// </param>
/// <param name="IconCssClass">
/// The Bootstrap Icons class used to represent this theme visually.
/// </param>
/// <param name="Description">
/// Optional descriptive text, shown in help panels or tooltips.
/// </param>
public sealed record ColorScheme(
	string Id,
	string DisplayName,
	string CssKey,
	string IconCssClass,
	string? Description = null
) {

	/// <summary>
	/// Gets the standard CSS filename for the theme based on the <see cref="CssKey"/>.
	/// For example, if <c>CssKey</c> is "aspire", the result is:
	/// <c>cirreum-bootstrap-aspire.css</c>.
	/// </summary>
	public string CssFileName => $"cirreum-bootstrap-{this.CssKey}.css";

	/// <summary>
	/// Returns the identifier of the color scheme for logging, debugging,
	/// and display purposes.
	/// </summary>
	/// <returns>The <see cref="Id"/> of the color scheme.</returns>
	public override string ToString() => this.Id;

}