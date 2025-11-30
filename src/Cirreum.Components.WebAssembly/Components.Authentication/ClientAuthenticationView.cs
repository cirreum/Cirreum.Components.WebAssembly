namespace Cirreum.Components.Authentication;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

/// <summary>
/// A Blazor component that extends <see cref="RemoteAuthenticatorViewCore{RemoteAuthenticationState}"/>
/// to automatically integrate authenticated user context with telemetry systems.
/// Captures user identity on successful login and clears context on logout.
/// </summary>
/// <remarks>
/// <para>
/// This component works with any telemetry provider that implements <see cref="ITelemetryUserContext"/>.
/// Register your telemetry integration (e.g., Application Insights, OpenTelemetry) to have the
/// authenticated user context automatically established upon successful login and cleared on logout.
/// </para>
/// <para>
/// The component extracts the user's name claim and optional tenant identifier from the authentication
/// state and propagates them to the telemetry system for correlation with application events.
/// </para>
/// </remarks>
public class ClientAuthenticationView : RemoteAuthenticatorViewCore<RemoteAuthenticationState> {
	public ClientAuthenticationView() => this.AuthenticationState = new RemoteAuthenticationState();

	[Inject]
	private ITelemetryUserContext? UserTelemetryContext { get; set; }

	[CascadingParameter]
	private Task<AuthenticationState>? AuthState { get; set; }

	protected override void OnInitialized() {
		this.OnLogInSucceeded = EventCallback.Factory.Create<RemoteAuthenticationState>(this, this.LoginSucceeded);
		this.OnLogOutSucceeded = EventCallback.Factory.Create<RemoteAuthenticationState>(this, this.LogoutSucceeded);
	}

	private async Task LoginSucceeded(RemoteAuthenticationState clientAuthState) {

		if (this.AuthState is not null &&
			this.UserTelemetryContext?.IsEnabled == true) {
			// get the newly authenticated user
			var authState = await this.AuthState;

			// User Name
			var authUserId = AuthUtil.CleanString(
				authState.User.FindFirst("name")?.Value,
				"guest");

			// optional Account Id
			var accountId = AuthUtil.CleanString(
				AuthUtil.TryGetTenantId(authState.User),
				string.Empty);

			// set authenticated user context for telemetry
			await this.UserTelemetryContext.SetAuthenticatedUser(
				authUserId,
				accountId,
				storeInCookie: true);
		}
	}

	private async Task LogoutSucceeded(RemoteAuthenticationState clientAuthState) {
		if (this.UserTelemetryContext?.IsEnabled == true) {
			await this.UserTelemetryContext.ClearAuthenticatedUser();
		}
	}
}