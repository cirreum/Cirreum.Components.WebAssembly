namespace Cirreum.Components;

using Cirreum.Startup;

/// <summary>
/// Monitors system theme preference changes and initializes theme state on application startup.
/// </summary>
/// <remarks>
/// <para>
/// This service is responsible for:
/// <list type="bullet">
/// <item><description>Restoring persisted theme preferences from localStorage on startup</description></item>
/// <item><description>Monitoring system/browser <c>prefers-color-scheme</c> changes</description></item>
/// <item><description>Automatically updating applied theme when system preference changes (in Auto mode)</description></item>
/// </list>
/// </para>
/// <para>
/// As an <see cref="IAutoInitialize"/> implementation, it will be automatically initialized
/// during application startup. Components should inject <see cref="IThemeStateManager"/>
/// to change themes, not this monitor.
/// </para>
/// </remarks>
public interface IThemeMonitor : IAutoInitialize {
	/// <summary>
	/// Forces a refresh of the applied theme mode based on current system/browser preferences.
	/// </summary>
	/// <remarks>
	/// Called automatically when system theme preference changes are detected.
	/// Can also be called manually to force a refresh.
	/// </remarks>
	void RefreshAppliedMode();
}