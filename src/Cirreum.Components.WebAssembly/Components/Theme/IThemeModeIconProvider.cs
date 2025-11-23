namespace Cirreum.Components;

/// <summary>
/// Provides icon mappings for theme modes.
/// </summary>
/// <remarks>
/// Implementations of this interface supply the appropriate icon identifiers
/// for different theme modes (e.g., Light, Dark, Auto).
/// This allows for consistent icon representation across theme-aware components.
/// Color scheme icons are provided directly by the <see cref="ColorScheme.IconCssClass"/> property.
/// </remarks>
public interface IThemeModeIconProvider {
	/// <summary>
	/// Gets the icon identifier for the specified theme mode.
	/// </summary>
	/// <param name="mode">The theme mode (Light, Dark, or Auto).</param>
	/// <returns>The icon identifier corresponding to the specified mode (e.g., "bi-sun-fill", "bi-moon-stars-fill", "bi-circle-half").</returns>
	string ResolveModeIcon(ThemeMode mode);
}