namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// The theme modes available to the application.
/// </summary>
public enum ThemeMode {
	/// <summary>
	/// Represents the automatic theme mode that adapts to system preferences.
	/// </summary>
	[Display(Name = "Auto", ShortName = "auto", Description = "Represents the automatic theme mode that adapts to system preferences")]
	Auto,
	/// <summary>
	/// Represents the theme light mode.
	/// </summary>
	[Display(Name = "Light", ShortName = "light", Description = "Represents the theme light mode")]
	Light,
	/// <summary>
	/// Represents the theme dark mode.
	/// </summary>
	[Display(Name = "Dark", ShortName = "dark", Description = "Represents the theme dark mode")]
	Dark
}

/// <summary>
/// Provides constants for theme mode values used throughout the application.
/// </summary>
public static class ThemeModeNames {
	/// <summary>
	/// Represents the theme light mode.
	/// </summary>
	public const string Light = "light";

	/// <summary>
	/// Represents the theme dark mode.
	/// </summary>
	public const string Dark = "dark";

	/// <summary>
	/// Represents the automatic theme mode that adapts to system preferences.
	/// </summary>
	public const string Auto = "auto";

	/// <summary>
	/// Gets a read-only collection containing all available theme modes.
	/// </summary>
	public static readonly IReadOnlyList<string> All = [Light, Dark, Auto];
}