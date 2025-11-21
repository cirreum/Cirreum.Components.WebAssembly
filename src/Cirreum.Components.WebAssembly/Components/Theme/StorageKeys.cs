namespace Cirreum.Components;

/// <summary>
/// Defines standardized keys for browser local storage used by theme and mode management.
/// </summary>
public static class StorageKeys {
	/// <summary>
	/// Storage key for the user's selected color scheme mode (e.g., "light", "dark", or "auto").
	/// </summary>
	public static readonly string ModeKey = "user-theme-mode";

	/// <summary>
	/// Storage key for the user's selected theme name (e.g., "default", "aqua", "aspire").
	/// </summary>
	public static readonly string ThemeKey = "user-theme-name";
}