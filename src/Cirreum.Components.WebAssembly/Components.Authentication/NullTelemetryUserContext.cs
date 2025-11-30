namespace Cirreum.Components.Authentication;

using System.Threading.Tasks;

public class NullTelemetryUserContext : ITelemetryUserContext {

	public bool IsEnabled => false;

	public ValueTask SetAuthenticatedUser(string authenticatedUserId, string? accountId = null, bool? storeInCookie = null)
		=> ValueTask.CompletedTask;

	public ValueTask ClearAuthenticatedUser()
		=> ValueTask.CompletedTask;
}
