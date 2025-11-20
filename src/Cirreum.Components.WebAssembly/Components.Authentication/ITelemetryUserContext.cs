namespace Cirreum.Components.Authentication;

/// <summary>
/// Provides an abstraction for managing authenticated user context in telemetry systems.
/// Enables correlation of telemetry events with specific users and accounts across sessions,
/// supporting user-scoped analysis and diagnostics in application monitoring solutions.
/// </summary>
public interface ITelemetryUserContext
{
    /// <summary>
    /// Clears the authenticated user context from telemetry tracking.
    /// Removes user and account identifiers from active telemetry sessions,
    /// allowing subsequent events to be tracked as anonymous until a new
    /// authenticated context is established.
    /// </summary>
    Task ClearAuthenticatedUser();

    /// <summary>
    /// Establishes an authenticated user context for telemetry tracking.
    /// Associates subsequent telemetry events with the specified user and
    /// optional account identifiers, enabling user-scoped analysis and correlation.
    /// </summary>
    /// <param name="authenticatedUserId">
    /// The authenticated user identifier. Should be a stable, consistent value
    /// across sessions (e.g., subject claim, user ID).
    /// </param>
    /// <param name="accountId">
    /// Optional account or tenant identifier for multi-tenant scenarios.
    /// </param>
    /// <param name="storeInCookie">
    /// Optional flag controlling persistence behavior. When true, the telemetry
    /// context may persist across browser sessions. Implementation-specific.
    /// </param>
    Task SetAuthenticatedUser(
        string authenticatedUserId,
        string? accountId = null,
        bool? storeInCookie = null);
}