namespace Cirreum.Components;

/// <summary>
/// Defines standardized keys for browser local storage used by theme and mode management.
/// </summary>
public static class StorageKeys {
	/// <summary>
	/// Storage key (user-theme-mode) for the user's selected theme mode (e.g., "light", "dark", or "auto").
	/// </summary>
	public static readonly string ModeKey = "user-theme-mode";

	/// <summary>
	/// Storage key (user-color-scheme) for the user's selected color scheme stylesheet (e.g., "default", "aqua", "aspire").
	/// </summary>
	public static readonly string SchemeKey = "user-color-scheme";
}