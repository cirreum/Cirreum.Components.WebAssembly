namespace Cirreum.Components.Authentication;
using System.Security.Claims;
using System.Text.RegularExpressions;

internal static partial class AuthUtil {

	// Use the Regex Source Generator in .NET 9 for better performance
	[GeneratedRegex(@"[,;=| ]+")]
	private static partial Regex SpecialCharactersRegex();

	/// <summary>
	/// Replace invalid characters with underscores and single backslash with double backslash.
	/// </summary>
	/// <param name="input"></param>
	/// <param name="defaultValue"></param>
	/// <returns></returns>
	public static string CleanString(string? input, string defaultValue) {

		if (string.IsNullOrWhiteSpace(input)) {
			return defaultValue;
		}

		// Replacing single backslash with double backslash
		var result = input.Replace("\\", "\\\\", StringComparison.Ordinal);

		// Using the precompiled regex for replacing special characters with an underscore
		result = SpecialCharactersRegex().Replace(result, "_");

		return result;

	}

	public static string? TryGetTenantId(ClaimsPrincipal principal) {

		// Try to get tenant ID from standard claims
		var tenantId = principal.FindFirst("tid")?.Value
			?? principal.FindFirst("dct")?.Value // Descope
			?? principal.FindFirst("tenant")?.Value
			?? principal.FindFirst("tenant_id")?.Value
			?? principal.FindFirst("org")?.Value // Alternative
			?? principal.FindFirst("org_id")?.Value // Alternative
			?? principal.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value; // MS schema format

		return tenantId;

	}

}