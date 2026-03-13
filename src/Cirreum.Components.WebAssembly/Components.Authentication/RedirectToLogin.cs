namespace Cirreum.Components.Authentication;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

/// <summary>
/// A component that redirects the user to the login page with optional OIDC parameters.
/// </summary>
/// <remarks>
/// Typically used in a route guard or <c>AppRouteView</c> to force authentication before
/// rendering a protected page component.
/// </remarks>
public class RedirectToLogin : ComponentBase {

	[Inject]
	private NavigationManager Navigation { get; set; } = default!;

	/// <summary>
	/// The path to the login page. Default: <c>authentication/login</c>
	/// </summary>
	[Parameter]
	public string LoginPath { get; set; } = "authentication/login";

	/// <summary>
	/// Optional login hint passed to the identity provider to pre-populate the username field.
	/// Maps to the OIDC <c>loginHint</c> parameter.
	/// </summary>
	[Parameter]
	public string? LoginHint { get; set; }

	/// <summary>
	/// Optional OIDC <c>prompt</c> parameter to control IdP interaction behavior.
	/// When <c>null</c>, the parameter is omitted from the request entirely.
	/// </summary>
	/// <remarks>
	/// Use <see cref="OidcPrompt.Login"/> to force re-authentication even when an
	/// active session exists. Use <see cref="OidcPrompt.None"/> for silent checks.
	/// </remarks>
	[Parameter]
	public OidcPrompt? LoginPrompt { get; set; }

	/// <summary>
	/// The URL to return to after successful login. When <see langword="null"/>,
	/// defaults to <see cref="NavigationManager.Uri"/> at the time of initialization.
	/// </summary>
	/// <remarks>
	/// Callers that intercept navigation before the URI changes — such as route guard
	/// components that evaluate auth state in <c>SetParametersAsync</c> — should supply
	/// the intended return URL explicitly to ensure the correct page is restored
	/// after the authentication callback completes.
	/// </remarks>
	[Parameter]
	public string? ReturnUrl { get; set; }

	protected override void OnInitialized() {

		InteractiveRequestOptions requestOptions = new() {
			Interaction = InteractionType.SignIn,
			ReturnUrl = this.ReturnUrl ?? this.Navigation.Uri
		};

		if (!string.IsNullOrWhiteSpace(this.LoginHint)) {
			requestOptions.TryAddAdditionalParameter("loginHint", this.LoginHint);
		}

		if (this.LoginPrompt.HasValue) {
			var promptValue = this.LoginPrompt.Value switch {
				OidcPrompt.None => "none",
				OidcPrompt.Login => "login",
				OidcPrompt.Consent => "consent",
				OidcPrompt.SelectAccount => "select_account",
				_ => null
			};

			if (promptValue is not null) {
				requestOptions.TryAddAdditionalParameter("prompt", promptValue);
			}
		}

		this.Navigation.NavigateToLogin(this.LoginPath, requestOptions);

	}

}