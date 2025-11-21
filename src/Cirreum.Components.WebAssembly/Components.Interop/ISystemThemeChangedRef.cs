namespace Cirreum.Components.Interop;

/// <summary>
/// Defines the contract for the reference objects
/// used with the system theme mode monitor.
/// </summary>
public interface ISystemThemeChangedRef {
	/// <summary>
	/// Called by JS Runtime when the system theme mode
	/// has changed.
	/// </summary>
	/// <param name="isDarkMode"><see langword="true"/> when in Dark mode; otherwise false.</param>
	/// <param name="storedMode">The value stored in the media query (applied mode: light/dark).</param>
	/// <returns></returns>
	Task ThemeChanged(bool isDarkMode, string storedMode);
}