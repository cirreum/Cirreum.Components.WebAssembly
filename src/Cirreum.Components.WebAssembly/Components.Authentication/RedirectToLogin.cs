namespace Cirreum.Components.Authentication;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

public class RedirectToLogin : ComponentBase {

	[Inject]
	private NavigationManager Navigation { get; set; } = default!;

	/// <summary>
	/// The path to the login page. Default: authentication/login
	/// </summary>
	[Parameter]
	public string LoginPath { get; set; } = "authentication/login";

	protected override void OnInitialized() {

		InteractiveRequestOptions requestOptions = new() {
			Interaction = InteractionType.SignIn,
			ReturnUrl = this.Navigation.Uri,
		};

		this.Navigation.NavigateToLogin(this.LoginPath, requestOptions);

	}

}