namespace Cirreum.Components.Authentication;

using Microsoft.AspNetCore.Components;

/// <summary>
/// A component that redirects an authenticated but unauthorized user to a designated page.
/// </summary>
/// <remarks>
/// Replaces the current history entry to prevent the user navigating back into the
/// unauthorized route via the browser back button.
/// </remarks>
public class RedirectToUnauthorized : ComponentBase {

	[Inject]
	private NavigationManager Navigation { get; set; } = default!;

	/// <summary>
	/// The relative path to redirect to when a user is authenticated but not authorized.
	/// Default: <c>/unauthorized</c>
	/// </summary>
	[Parameter]
	public string UnauthorizedPage { get; set; } = "/unauthorized";

	/// <summary>
	/// When <see langword="true"/>, forces a full page reload on redirect.
	/// Default: <see langword="false"/>
	/// </summary>
	[Parameter]
	public bool ForceReload { get; set; }

	/// <summary>
	/// When <see langword="true"/>, appends the current page as a <c>returnUrl</c> query
	/// parameter on the redirect, provided the current page is not already the unauthorized
	/// page (to avoid redirect loops). Default: <see langword="false"/>
	/// </summary>
	[Parameter]
	public bool IncludeReturnUrl { get; set; }

	protected override void OnInitialized() {
		var currentUri = this.Navigation.Uri;
		var redirectBase = this.Navigation.ToAbsoluteUri(this.UnauthorizedPage);
		var redirectUri = this.UnauthorizedPage;

		if (this.IncludeReturnUrl && !IsCurrentPageUnauthorizedPage(currentUri, redirectBase)) {
			var returnUrl = Uri.EscapeDataString(currentUri);
			redirectUri += $"?returnUrl={returnUrl}";
		}

		this.Navigation.NavigateTo(redirectUri, new NavigationOptions {
			ReplaceHistoryEntry = true,
			ForceLoad = this.ForceReload
		});
	}

	private static bool IsCurrentPageUnauthorizedPage(string currentUri, Uri redirectBase) {
		if (!Uri.TryCreate(currentUri, UriKind.Absolute, out var current)) {
			return false;
		}
		return string.Equals(current.AbsolutePath, redirectBase.AbsolutePath, StringComparison.OrdinalIgnoreCase);
	}

}