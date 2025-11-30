namespace Cirreum.Components.Authentication;

/// <summary>
/// Provides an abstraction for managing authenticated user context in telemetry systems.
/// Enables correlation of telemetry events with specific users and accounts across sessions,
/// supporting user-scoped analysis and diagnostics in application monitoring solutions.
/// </summary>
public interface ITelemetryUserContext {

	/// <summary>
	/// Indicates whether telemetry user context tracking is enabled.
	/// Returns false for null/no-op implementations, allowing callers to skip
	/// expensive setup work when telemetry is not configured.
	/// </summary>
	bool IsEnabled { get; }

	/// <summary>
	/// Clears the authenticated user context from telemetry tracking.
	/// </summary>
	ValueTask ClearAuthenticatedUser();

	/// <summary>
	/// Establishes an authenticated user context for telemetry tracking.
	/// </summary>
	ValueTask SetAuthenticatedUser(
		string authenticatedUserId,
		string? accountId = null,
		bool? storeInCookie = null);
}