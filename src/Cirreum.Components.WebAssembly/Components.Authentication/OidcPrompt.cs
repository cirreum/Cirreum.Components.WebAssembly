namespace Cirreum.Components.Authentication;

/// <summary>
/// Specifies the OIDC <c>prompt</c> parameter behavior sent to the identity provider
/// during an authorization request.
/// </summary>
/// <remarks>
/// Controls whether the IdP shows interactive UI regardless of an existing session.
/// Not all values are supported by all identity providers.
/// </remarks>
public enum OidcPrompt {
	/// <summary>
	/// The IdP must not display any authentication or consent UI. If the user does
	/// not have an active session or consent has not been granted, the request will
	/// fail silently. Typically used for background token refresh checks.
	/// </summary>
	None,
	/// <summary>
	/// Forces the IdP to present a login screen even if the user has an active session.
	/// Useful for step-up authentication or sensitive operations requiring fresh credentials.
	/// </summary>
	Login,
	/// <summary>
	/// Forces the IdP to present a consent screen even if the user has previously granted consent.
	/// </summary>
	Consent,
	/// <summary>
	/// Prompts the user to select an account, even if only one session exists.
	/// Useful in multi-account or shared-device scenarios.
	/// </summary>
	SelectAccount
}