namespace Cirreum.Components.Authentication;

using Microsoft.AspNetCore.Components;
using System.Web;

/// <summary>
/// A component used to redirect to the default unauthorized page.
/// </summary>
public class RedirectToUnauthorized : ComponentBase {

	[Inject]
	private NavigationManager Navigation { get; set; } = default!;

	/// <summary>
	/// The relative path to the page when a user is authenticated, but not authorized.
	/// </summary>
	[Parameter]
	public string UnauthorizedPage { get; set; } = "/";

	/// <summary>
	/// Optional, set to <see langword="true"/> to force a reload of the application.
	/// </summary>
	[Parameter]
	public bool ForceReload { get; set; }

	/// <summary>
	/// Optional, set to <see langword="true"/> to include the source page as the ReturnUrl on the redirect path. Default: <see langword="false"/>
	/// </summary>
	[Parameter]
	public bool IncludeReturnUrl { get; set; } = false;

	protected override void OnInitialized() {

		var currentUri = this.Navigation.Uri;
		var redirectUri = this.UnauthorizedPage;
		var queryString = HttpUtility.ParseQueryString(string.Empty);

		// Add return URL if requested and we're not already on the unauthorized page
		if (this.IncludeReturnUrl && !currentUri.Contains(this.UnauthorizedPage)) {
			var returnUrl = HttpUtility.UrlEncode(currentUri);
			queryString.Add("returnUrl", returnUrl);
		}

		// set query string
		if (queryString.Count > 0) {
			redirectUri += $"?{queryString}";
		}

		// Perform redirect...
		this.Navigation.NavigateTo(redirectUri, new NavigationOptions {
			ReplaceHistoryEntry = true,
			ForceLoad = this.ForceReload
		});

	}

}