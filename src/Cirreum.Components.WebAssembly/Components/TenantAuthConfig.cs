namespace Cirreum.Components;

/// <summary>
/// Configuration for dynamic tenant authentication resolved at runtime.
/// Populated by the loader from the <c>auth-type-url</c> endpoint.
/// </summary>
/// <remarks>
/// <para>
/// This configuration is fetched by the JavaScript loader when <c>auth-type="dynamic"</c>.
/// </para>
/// <para>
/// The configuration supports three authentication scenarios:
/// <list type="bullet">
///   <item><b>Entra Workforce</b>: Set <see cref="TenantId"/> and <see cref="ClientId"/></item>
///   <item><b>Entra External (CIAM)</b>: Set <see cref="Domain"/>, <see cref="ClientId"/>, and <see cref="IsEntraExternal"/> = true</item>
///   <item><b>OIDC</b>: Set <see cref="Authority"/> and <see cref="ClientId"/></item>
/// </list>
/// </para>
/// </remarks>
public sealed class TenantAuthConfig {

	/// <summary>
	/// The tenant's unique slug/identifier extracted from the URL path.
	/// </summary>
	/// <example>acme</example>
	public string? Slug { get; init; }

	/// <summary>
	/// Display name for the tenant (for UI purposes).
	/// </summary>
	/// <example>Acme Corporation</example>
	public string? DisplayName { get; init; }

	/// <summary>
	/// The OIDC authority URL.
	/// </summary>
	/// <remarks>
	/// <para>Used by: Generic OIDC providers (Okta, Auth0, Ping, etc.)</para>
	/// <para>For Entra, the authority is computed from <see cref="TenantId"/> or <see cref="Domain"/>.</para>
	/// </remarks>
	/// <example>https://acme.okta.com</example>
	public string? Authority { get; init; }

	/// <summary>
	/// The OAuth client ID for the SPA registered in the tenant's IdP.
	/// </summary>
	/// <remarks>This is the only required field.</remarks>
	public required string ClientId { get; init; }

	/// <summary>
	/// The OAuth response type. Default: "code" (PKCE flow).
	/// </summary>
	public string? ResponseType { get; init; }

	/// <summary>
	/// OAuth scopes to request during authentication.
	/// </summary>
	/// <remarks>
	/// Common scopes include "openid", "profile", "email", and API-specific scopes.
	/// </remarks>
	public List<string>? Scopes { get; init; }

	/// <summary>
	/// For Entra Workforce: The Azure AD tenant ID (GUID).
	/// </summary>
	/// <remarks>
	/// Mutually exclusive with <see cref="Domain"/> and <see cref="Authority"/>.
	/// When set, the loader uses MSAL and computes the authority as
	/// <c>https://login.microsoftonline.com/{TenantId}</c>.
	/// </remarks>
	/// <example>12345678-1234-1234-1234-123456789012</example>
	public string? TenantId { get; init; }

	/// <summary>
	/// For Entra External (CIAM): The CIAM domain (without .ciamlogin.com suffix).
	/// </summary>
	/// <remarks>
	/// <para>Mutually exclusive with <see cref="TenantId"/> and <see cref="Authority"/>.</para>
	/// <para>When set with <see cref="IsEntraExternal"/> = true, the loader uses MSAL
	/// and computes the authority as <c>https://{Domain}.ciamlogin.com</c>.</para>
	/// </remarks>
	/// <example>contoso</example>
	public string? Domain { get; init; }

	/// <summary>
	/// Indicates if this tenant uses Entra External (CIAM) authentication.
	/// </summary>
	/// <remarks>
	/// When true, <see cref="Domain"/> is used to construct the CIAM authority.
	/// When false, <see cref="TenantId"/> is used for standard Entra Workforce authentication.
	/// </remarks>
	public bool IsEntraExternal { get; init; }

	/// <summary>
	/// Optional hint for which authentication library to use.
	/// </summary>
	/// <remarks>
	/// <para>Values: "msal" or "oidc"</para>
	/// <para>If not specified, the loader infers the library from the configuration shape:
	/// presence of <see cref="TenantId"/>, <see cref="Domain"/>, or <see cref="IsEntraExternal"/>
	/// indicates MSAL; otherwise OIDC is used.</para>
	/// </remarks>
	public string? AuthLibrary { get; init; }

}