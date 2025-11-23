namespace Cirreum.Components;

/// <summary>
/// Provides standardized resource paths for theme CSS files.
/// </summary>
public static class ThemeResourcePaths {

	/// <summary>
	/// The base path for theme CSS files.
	/// </summary>
	public const string BasePath = "_content/Cirreum.Components.WebAssembly";

	/// <summary>
	/// Gets the CSS file path for the specified theme.
	/// </summary>
	/// <param name="scheme">The color scheme to get the path for.</param>
	public static string GetSchemePath(ColorScheme scheme) =>
		$"{BasePath}/css/{scheme.CssFileName}";

	/// <summary>
	/// Gets the CSS file path for the specified theme by short name.
	/// </summary>
	/// <param name="id">The scheme ID (e.g., "aspire", "default").</param>
	/// <returns>The resource path to the theme's CSS file.</returns>
	public static string GetSchemePath(string id) =>
		$"{BasePath}/css/cirreum-bootstrap-{id}.css";

}