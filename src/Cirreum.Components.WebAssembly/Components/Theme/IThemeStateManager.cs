namespace Cirreum.Components;

/// <summary>
/// Manages theme state changes and synchronizes with the JavaScript/HTML environment.
/// </summary>
/// <remarks>
/// This service is responsible for:
/// <list type="bullet">
/// <item><description>Setting theme mode (Light, Dark, Auto) and persisting preferences</description></item>
/// <item><description>Setting visual theme (Default, Aspire, Excel, etc.)</description></item>
/// <item><description>Synchronizing changes with the DOM and localStorage</description></item>
/// </list>
/// </remarks>
public interface IThemeStateManager {
	/// <summary>
	/// Sets the application's current theme mode and synchronizes with the DOM.
	/// </summary>
	/// <param name="mode">The theme mode to apply (Light, Dark, or Auto).</param>
	void SetMode(ThemeMode mode);

	/// <summary>
	/// Sets the application's visual theme (color scheme) and synchronizes with the DOM.
	/// </summary>
	/// <param name="theme">The name of the theme to apply (Default, Aspire, Excel, etc.).</param>
	void SetTheme(ThemeName theme);
}