namespace Cirreum.Components;

/// <summary>
/// Provides icon mappings for theme modes and color schemes.
/// </summary>
/// <remarks>
/// Implementations of this interface supply the appropriate icon identifiers
/// for different theme modes (e.g., Light, Dark, Auto) and visual themes (e.g., Default, Aspire, Excel).
/// This allows for consistent icon representation across theme-aware components.
/// </remarks>
public interface IThemeIconProvider {
	/// <summary>
	/// Gets the icon identifier for the specified theme mode.
	/// </summary>
	/// <param name="mode">The theme mode (Light, Dark, or Auto).</param>
	/// <returns>The icon identifier corresponding to the specified mode (e.g., "bi-sun", "bi-moon", "bi-circle-half").</returns>
	string ResolveModeIcon(ThemeMode mode);

	/// <summary>
	/// Gets the icon identifier for the specified visual theme.
	/// </summary>
	/// <param name="theme">The visual theme (Default, Aspire, Excel, etc.).</param>
	/// <returns>The icon identifier corresponding to the specified theme (e.g., "bi-palette", "bi-microsoft").</returns>
	string ResolveThemeIcon(ThemeName theme);
}