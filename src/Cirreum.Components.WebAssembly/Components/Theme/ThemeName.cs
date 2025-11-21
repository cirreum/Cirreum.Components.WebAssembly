namespace Cirreum.Components;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the available design themes for the application.
/// </summary>
public enum ThemeName {
	/// <summary>
	/// The default Cirreum Bootstrap theme.
	/// </summary>
	[Display(Name = "Default", ShortName = "default",
		Description = "The default Cirreum Bootstrap theme.")]
	Default,

	/// <summary>
	/// The Aspire-inspired theme.
	/// </summary>
	[Display(Name = "Aspire", ShortName = "aspire",
		Description = "A theme inspired by Microsoft Aspire.")]
	Aspire,

	/// <summary>
	/// The Excel-inspired green theme.
	/// </summary>
	[Display(Name = "Excel", ShortName = "excel",
		Description = "A theme inspired by Microsoft Excel.")]
	Excel,

	/// <summary>
	/// The classic Office theme.
	/// </summary>
	[Display(Name = "Office", ShortName = "office",
		Description = "A theme inspired by Microsoft Office.")]
	Office,

	/// <summary>
	/// The Outlook-inspired blue theme.
	/// </summary>
	[Display(Name = "Outlook", ShortName = "outlook",
		Description = "A theme inspired by Microsoft Outlook.")]
	Outlook,

	/// <summary>
	/// The Windows-inspired modern blue theme.
	/// </summary>
	[Display(Name = "Windows", ShortName = "windows",
		Description = "A theme inspired by the Windows design system.")]
	Windows,

	/// <summary>
	/// The macOS-inspired aqua theme.
	/// </summary>
	[Display(Name = "Aqua", ShortName = "aqua",
		Description = "A theme inspired by the macOS design system.")]
	Aqua,
}

public static class ThemeNames {
	public const string Default = "default";
	public const string Aspire = "aspire";
	public const string Excel = "excel";
	public const string Office = "office";
	public const string Outlook = "outlook";
	public const string Windows = "windows";
	public const string Aqua = "aqua";

	/// <summary>
	/// Gets a read-only collection containing all available themes.
	/// </summary>
	public static readonly IReadOnlyList<string> All = [
		Default,
		Aspire,
		Excel,
		Office,
		Outlook,
		Windows,
		Aqua
	];
}
