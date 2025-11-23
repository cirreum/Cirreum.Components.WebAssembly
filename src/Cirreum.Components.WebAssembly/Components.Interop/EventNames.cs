namespace Cirreum.Components.Interop;

/// <summary>
/// Defines standardized event names for JavaScript interop callbacks used by the theme system.
/// </summary>
/// <remarks>
/// These constants ensure consistent event naming between C# and JavaScript when
/// registering listeners for theme-related changes via JSRuntime.
/// </remarks>
public static class EventNames {
	/// <summary>
	/// Event name for theme mode change notifications from the JavaScript theme monitor.
	/// Raised when the system's preferred color scheme changes (e.g., user switches OS from light to dark mode).
	/// </summary>
	public const string ModeChangedEvent = "OnModeChanged";
}