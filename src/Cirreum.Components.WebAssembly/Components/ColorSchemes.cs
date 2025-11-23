namespace Cirreum.Components;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Provides the built-in Cirreum color schemes used by the component library.
/// </summary>
public static class ColorSchemes {

	public const string DefaultId = "default";
	public const string AquaId = "aqua";
	public const string AspireId = "aspire";
	public const string ExcelId = "excel";
	public const string OfficeId = "office";
	public const string OutlookId = "outlook";
	public const string WindowsId = "windows";

	/// <summary>
	/// Neutral Cirreum style (Flatly-based).
	/// </summary>
	public static readonly ColorScheme Default = new(
		Id: DefaultId,
		DisplayName: "Default",
		IconCssClass: "bi-palette",
		Description: "Neutral Cirreum style");

	/// <summary>
	/// macOS/Big Sur inspired theme.
	/// </summary>
	public static readonly ColorScheme Aqua = new(
		Id: AquaId,
		DisplayName: "Aqua",
		IconCssClass: "bi-apple",
		Description: "macOS-inspired theme");

	/// <summary>
	/// Purple .NET-inspired theme.
	/// </summary>
	public static readonly ColorScheme Aspire = new(
		Id: AspireId,
		DisplayName: "Aspire",
		IconCssClass: "bi-stars",
		Description: "Purple .NET-inspired theme");

	/// <summary>
	/// Data-focused theme with Excel-like palette.
	/// </summary>
	public static readonly ColorScheme Excel = new(
		Id: ExcelId,
		DisplayName: "Excel",
		IconCssClass: "bi-table",
		Description: "Data-focused theme");

	/// <summary>
	/// Office 365-style theme.
	/// </summary>
	public static readonly ColorScheme Office = new(
		Id: OfficeId,
		DisplayName: "Office",
		IconCssClass: "bi-briefcase",
		Description: "Office 365-style theme");

	/// <summary>
	/// Outlook-inspired blue theme.
	/// </summary>
	public static readonly ColorScheme Outlook = new(
		Id: OutlookId,
		DisplayName: "Outlook",
		IconCssClass: "bi-envelope",
		Description: "Mail & calendar inspired theme");

	/// <summary>
	/// Classic Windows-style blue theme.
	/// </summary>
	public static readonly ColorScheme Windows = new(
		Id: WindowsId,
		DisplayName: "Windows",
		IconCssClass: "bi-windows",
		Description: "Classic Microsoft feel");

	/// <summary>
	/// All built-in Cirreum color schemes, in a stable display order.
	/// </summary>
	public static readonly IReadOnlyList<ColorScheme> All =
	[
		Default,
		Aqua,
		Aspire,
		Excel,
		Office,
		Outlook,
		Windows
	];

	/// <summary>
	/// Attempts to resolve a scheme by identifier.
	/// </summary>
	/// <param name="id">The scheme identifier to look up.</param>
	/// <param name="scheme">
	/// When this method returns, contains the resolved <see cref="ColorScheme"/> if found;
	/// otherwise, the <see cref="Default"/> scheme.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if a matching scheme was found; otherwise <see langword="false"/>
	/// and <paramref name="scheme"/> is set to <see cref="Default"/>.
	/// </returns>
	public static bool TryGet(string? id, out ColorScheme scheme) {
		if (string.IsNullOrWhiteSpace(id)) {
			scheme = Default;
			return false;
		}

		var match = All.FirstOrDefault(
			s => string.Equals(s.Id, id, StringComparison.OrdinalIgnoreCase));

		if (match is null) {
			scheme = Default;
			return false;
		}

		scheme = match;
		return true;
	}

	/// <summary>
	/// Resolves a scheme by identifier or returns <see cref="Default"/> when not found.
	/// </summary>
	/// <param name="id">The scheme identifier to look up.</param>
	/// <returns>
	/// The matching <see cref="ColorScheme"/> if found; otherwise the <see cref="Default"/> scheme.
	/// </returns>
	public static ColorScheme GetOrDefault(string? id) =>
		TryGet(id, out var scheme) ? scheme : Default;

}