namespace Cirreum.Extensions;

using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Web;

public static class NavigationManagerExtensions {

	/// <summary>
	/// Gets the entire querystring name/value collection
	/// </summary>
	/// <param name="navigationManager"></param>
	/// <returns>The <see cref="NameValueCollection"/> containing the query string parameters</returns>
	public static NameValueCollection GetQueryString(this NavigationManager navigationManager) {
		return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
	}

	/// <summary>
	/// Gets a single querystring value with specified key 
	/// </summary>
	/// <param name="navigationManager"></param>
	/// <param name="key"></param>
	/// <returns>nullable string</returns>
	public static string? GetQueryStringItem(this NavigationManager navigationManager, string key) {
		return navigationManager.GetQueryString()[key];
	}

}