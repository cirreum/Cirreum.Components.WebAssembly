namespace Cirreum.Components.Authentication;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

/// <summary>
/// Redirects the user to the login page with <c>prompt=login</c> forced.
/// </summary>
[Obsolete("Use RedirectToLogin with Prompt=\"Login\" instead. This component will be removed in a future version.")]
public class RedirectToLoginPrompt : ComponentBase {

	[Inject]
	private NavigationManager Navigation { get; set; } = default!;

	/// <summary>
	/// The path to the login page. Default: <c>authentication/login</c>
	/// </summary>
	[Parameter]
	public string LoginPath { get; set; } = "authentication/login";

	protected override void OnInitialized() {
		InteractiveRequestOptions requestOptions = new() {
			Interaction = InteractionType.SignIn,
			ReturnUrl = this.Navigation.Uri,
		};
		requestOptions.TryAddAdditionalParameter("prompt", "login");
		this.Navigation.NavigateToLogin(this.LoginPath, requestOptions);
	}
}