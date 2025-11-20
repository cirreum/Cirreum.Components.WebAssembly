namespace Cirreum.Components;

/// <summary>
/// The Auth Info provided by the parent application via html/js.
/// </summary>
/// <param name="Include">Is authentication included for the application.</param>
/// <param name="AuthType">What is the authentication type ('msal' or 'oidc').</param>
public sealed record AppAuthInfo(bool Include, string AuthType);