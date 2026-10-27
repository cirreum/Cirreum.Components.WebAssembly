namespace Cirreum.Components.Interop;

/// <summary>
/// Defines the contract for the reference objects
/// used with the system theme mode monitor.
/// </summary>
public interface IThemeModeChangedRef {
	/// <summary>
	/// Called by JS Runtime when the system theme mode
	/// has changed.
	/// </summary>
	/// <param name="isDarkMode"><see langword="true"/> when the system prefers a dark color scheme; otherwise <see langword="false"/>.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	Task ModeChanged(bool isDarkMode);
}