namespace Cirreum.Extensions;

using Cirreum.Components;
using System;

/// <summary>
/// Provides extension methods for the BackgroundColor enum.
/// </summary>
public static class BackgroundColorExtensions {
	/// <summary>
	/// Converts a BackgroundColor to its corresponding ButtonColor.
	/// Subtle variants and special cases are mapped to their base color equivalent.
	/// </summary>
	/// <param name="backgroundColor">The BackgroundColor to convert.</param>
	/// <returns>The corresponding ButtonColor, or the closest base color equivalent.</returns>
	public static ButtonColor ToButtonColor(this BackgroundColor backgroundColor) {
		return backgroundColor switch {
			BackgroundColor.None => ButtonColor.None,
			BackgroundColor.Primary => ButtonColor.Primary,
			BackgroundColor.PrimarySubtle => ButtonColor.Primary,
			BackgroundColor.Secondary => ButtonColor.Secondary,
			BackgroundColor.SecondarySubtle => ButtonColor.Secondary,
			BackgroundColor.Success => ButtonColor.Success,
			BackgroundColor.SuccessSubtle => ButtonColor.Success,
			BackgroundColor.Danger => ButtonColor.Danger,
			BackgroundColor.DangerSubtle => ButtonColor.Danger,
			BackgroundColor.Warning => ButtonColor.Warning,
			BackgroundColor.WarningSubtle => ButtonColor.Warning,
			BackgroundColor.Info => ButtonColor.Info,
			BackgroundColor.InfoSubtle => ButtonColor.Info,
			BackgroundColor.Light => ButtonColor.Light,
			BackgroundColor.LightSubtle => ButtonColor.Light,
			BackgroundColor.Dark => ButtonColor.Dark,
			BackgroundColor.DarkSubtle => ButtonColor.Dark,
			BackgroundColor.Body => ButtonColor.Light,
			BackgroundColor.BodySecondary => ButtonColor.Light,
			BackgroundColor.BodyTertiary => ButtonColor.Light,
			BackgroundColor.White => ButtonColor.Light,
			BackgroundColor.Black => ButtonColor.Dark,
			BackgroundColor.Transparent => ButtonColor.None,
			_ => throw new ArgumentOutOfRangeException(nameof(backgroundColor), backgroundColor, null)
		};
	}

	/// <summary>
	/// Converts a ButtonColor to its corresponding BackgroundColor.
	/// </summary>
	/// <param name="buttonColor">The ButtonColor to convert.</param>
	/// <returns>The corresponding BackgroundColor.</returns>
	public static BackgroundColor ToBackgroundColor(this ButtonColor buttonColor) {
		return buttonColor switch {
			ButtonColor.None => BackgroundColor.None,
			ButtonColor.Primary => BackgroundColor.Primary,
			ButtonColor.Secondary => BackgroundColor.Secondary,
			ButtonColor.Success => BackgroundColor.Success,
			ButtonColor.Danger => BackgroundColor.Danger,
			ButtonColor.Warning => BackgroundColor.Warning,
			ButtonColor.Info => BackgroundColor.Info,
			ButtonColor.Light => BackgroundColor.Light,
			ButtonColor.Dark => BackgroundColor.Dark,
			ButtonColor.Link => BackgroundColor.None, // Mapping Link to None as there's no direct equivalent
			_ => throw new ArgumentOutOfRangeException(nameof(buttonColor), buttonColor, null)
		};
	}

}