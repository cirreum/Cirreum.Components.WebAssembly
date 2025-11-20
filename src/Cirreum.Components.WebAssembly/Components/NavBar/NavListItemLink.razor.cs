namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

public partial class NavListItemLink {

	[Parameter, EditorRequired]
	public RenderFragment ChildContent { get; set; } = default!;

	/// <summary>
	/// The optional tooltip
	/// </summary>
	[Parameter]
	public string Tooltip { get; set; } = "";

	/// <summary>
	/// Controls whether the tooltip is enabled
	/// </summary>
	[Parameter]
	public bool TooltipEnabled { get; set; } = true;

	/// <summary>
	/// The placement of the tooltip
	/// </summary>
	/// <remarks>
	/// See <see cref="Tooltip.Placement"/> for details.
	/// </remarks>
	[Parameter]
	public Placement TooltipPlacement { get; set; } = Placement.TopEnd;

	/// <summary>
	/// The delay before showing the tooltip (in milliseconds). Default: 100ms
	/// </summary>
	/// <remarks>
	/// See <see cref="Tooltip.Delay"/> for details.
	/// </remarks>
	[Parameter]
	public int TooltipDelay { get; set; } = 100;

	/// <summary>
	/// Whether to show the tooltip on focus as well as hover
	/// </summary>
	/// <remarks>
	/// See <see cref="Tooltip.ShowOnFocus"/> for details.
	/// </remarks>
	[Parameter]
	public bool TooltipShowOnFocus { get; set; } = false;

	/// <summary>
	/// Whether to show the tooltip arrow
	/// </summary>
	/// <remarks>
	/// See <see cref="Tooltip.ShowArrow"/> for details.
	/// </remarks>
	[Parameter]
	public bool TooltipShowArrow { get; set; } = true;

	/// <summary>
	/// Custom CSS for the tooltip
	/// </summary>
	/// <remarks>
	/// See <see cref="Tooltip.Css"/> for details.
	/// </remarks>
	[Parameter]
	public string? TooltipCss { get; set; }

	/// <summary>
	/// Custom template for the tooltip content
	/// </summary>
	/// <remarks>
	/// See <see cref="Tooltip.TitleTemplate"/> for details.
	/// </remarks>
	[Parameter]
	public RenderFragment? TooltipTemplate { get; set; }

	private bool ShouldRenderTooltip => (this.Tooltip.HasValue() || this.TooltipTemplate is not null) && this.TooltipEnabled;

	/// <summary>
	/// The Css Class List for the li. Default: nav-item
	/// </summary>
	[Parameter]
	public string LiCss { get; set; } = "nav-item";

	/// <summary>
	/// The Css Class List for the link. Default: nav-link
	/// </summary>
	[Parameter]
	public string LinkCss { get; set; } = "nav-link";

	[Parameter]
	public string Href { get; set; } = "";

	[Parameter]
	public bool IsDisabled { get; set; }

	/// <summary>
	/// Gets or sets a value representing the URL matching behavior.
	/// </summary>
	[Parameter]
	public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;

	/// <summary>
	/// Gets the element's ID
	/// </summary>
	public string Id { get; init; } = IdGenerator.Next;

	private string AriaCurrent => this.isActive ? "page" : "";
	private string? _hrefAbsolute = default!;
	private bool isActive;

	string LiCssClass => CssBuilder
		.Default(this.LiCss)
		.AddClass("active", when: this.isActive)
		.Build();

	string LinkCssClass => CssBuilder
		.Default(this.LinkCss)
		.AddClass("active", when: this.isActive)
		.AddClass("disabled", when: this.IsDisabled)
		.Build();

	private void LocationChanged(object? sender, LocationChangedEventArgs e) {
		var shouldBeActive = this.ShouldMatch(e.Location);
		if (shouldBeActive != this.isActive) {
			this.isActive = shouldBeActive;
			this.StateHasChanged();
		}
	}

	protected override void OnParametersSet() {
		this._hrefAbsolute = this.Href is null ? null : this.NavManager.ToAbsoluteUri(this.Href).AbsoluteUri;
		this.isActive = this.ShouldMatch(this.NavManager.Uri);
	}

	protected override void OnInitialized() {
		this.NavManager.LocationChanged += this.LocationChanged;
	}

	public void Dispose() {
		this.NavManager.LocationChanged -= this.LocationChanged;
	}

	#region Matching Logic

	//
	// Credit to Microsoft from https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Routing/NavLink.cs
	//

	private const string EnableMatchAllForQueryStringAndFragmentSwitchKey = "Microsoft.AspNetCore.Components.Routing.NavLink.EnableMatchAllForQueryStringAndFragment";
	private static readonly bool _enableMatchAllForQueryStringAndFragment = AppContext.TryGetSwitch(EnableMatchAllForQueryStringAndFragmentSwitchKey, out var switchValue) && switchValue;

	private static readonly CaseInsensitiveCharComparer CaseInsensitiveComparer = new CaseInsensitiveCharComparer();

	/// <summary>
	/// Determines whether the current URI should match the link.
	/// </summary>
	/// <param name="uriAbsolute">The absolute URI of the current location.</param>
	/// <returns>True if the link should be highlighted as active; otherwise, false.</returns>
	protected virtual bool ShouldMatch(string uriAbsolute) {

		if (this._hrefAbsolute == null) {
			return false;
		}

		var uriAbsoluteSpan = uriAbsolute.AsSpan();
		var hrefAbsoluteSpan = this._hrefAbsolute.AsSpan();
		if (EqualsHrefExactlyOrIfTrailingSlashAdded(uriAbsoluteSpan, hrefAbsoluteSpan)) {
			return true;
		}

		if (this.Match == NavLinkMatch.Prefix
			&& IsStrictlyPrefixWithSeparator(uriAbsolute, this._hrefAbsolute)) {
			return true;
		}

		if (_enableMatchAllForQueryStringAndFragment || this.Match != NavLinkMatch.All) {
			return false;
		}

		var uriWithoutQueryAndFragment = GetUriIgnoreQueryAndFragment(uriAbsoluteSpan);
		if (EqualsHrefExactlyOrIfTrailingSlashAdded(uriWithoutQueryAndFragment, hrefAbsoluteSpan)) {
			return true;
		}

		hrefAbsoluteSpan = GetUriIgnoreQueryAndFragment(hrefAbsoluteSpan);
		return EqualsHrefExactlyOrIfTrailingSlashAdded(uriWithoutQueryAndFragment, hrefAbsoluteSpan);

	}

	private static ReadOnlySpan<char> GetUriIgnoreQueryAndFragment(ReadOnlySpan<char> uri) {

		if (uri.IsEmpty) {
			return [];
		}

		var queryStartPos = uri.IndexOf('?');
		var fragmentStartPos = uri.IndexOf('#');

		if (queryStartPos < 0 && fragmentStartPos < 0) {
			return uri;
		}

		var minPos =
			queryStartPos < 0 ? fragmentStartPos
			: fragmentStartPos < 0 ? queryStartPos
			: Math.Min(queryStartPos, fragmentStartPos);
		return uri[..minPos];
	}

	private static bool EqualsHrefExactlyOrIfTrailingSlashAdded(ReadOnlySpan<char> currentUriAbsolute, ReadOnlySpan<char> hrefAbsolute) {

		if (currentUriAbsolute.SequenceEqual(hrefAbsolute, CaseInsensitiveComparer)) {
			return true;
		}

		if (currentUriAbsolute.Length == hrefAbsolute.Length - 1) {
			// Special case: highlight links to http://host/path/ even if you're
			// at http://host/path (with no trailing slash)
			//
			// This is because the router accepts an absolute URI value of "same
			// as base URI but without trailing slash" as equivalent to "base URI",
			// which in turn is because it's common for servers to return the same page
			// for http://host/vdir as they do for host://host/vdir/ as it's no
			// good to display a blank page in that case.
			if (hrefAbsolute[^1] == '/' &&
				currentUriAbsolute.SequenceEqual(hrefAbsolute[..^1], CaseInsensitiveComparer)) {
				return true;
			}
		}

		return false;
	}

	private static bool IsStrictlyPrefixWithSeparator(string value, string prefix) {
		var prefixLength = prefix.Length;
		if (value.Length > prefixLength) {
			return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
				&& (
					// Only match when there's a separator character either at the end of the
					// prefix or right after it.
					// Example: "/abc" is treated as a prefix of "/abc/def" but not "/abcdef"
					// Example: "/abc/" is treated as a prefix of "/abc/def" but not "/abcdef"
					prefixLength == 0
					|| !IsUnreservedCharacter(prefix[prefixLength - 1])
					|| !IsUnreservedCharacter(value[prefixLength])
				);
		} else {
			return false;
		}
	}

	private static bool IsUnreservedCharacter(char c) {
		// Checks whether it is an unreserved character according to
		// https://datatracker.ietf.org/doc/html/rfc3986#section-2.3
		// Those are characters that are allowed in a URI but do not have a reserved
		// purpose (e.g. they do not separate the components of the URI)
		return char.IsLetterOrDigit(c) ||
				c == '-' ||
				c == '.' ||
				c == '_' ||
				c == '~';
	}

	private class CaseInsensitiveCharComparer : IEqualityComparer<char> {

		public bool Equals(char x, char y) {
			return char.ToLowerInvariant(x) == char.ToLowerInvariant(y);
		}

		public int GetHashCode(char obj) {
			return char.ToLowerInvariant(obj).GetHashCode();
		}

	}

	#endregion

}