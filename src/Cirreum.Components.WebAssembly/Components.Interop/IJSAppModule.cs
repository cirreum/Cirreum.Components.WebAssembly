namespace Cirreum.Components.Interop;

using Cirreum.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading;
using System.Threading.Tasks;

public interface IJSAppModule {

	/// <summary>
	/// Initializes the JavaScript interop functionality.
	/// </summary>
	/// <returns>A task that represents the asynchronous operation.</returns>
	ValueTask InitializeAsync();

	#region Arbitrary Invocation

	/// <summary>
	/// Invokes the specified JavaScript function synchronously.
	/// </summary>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>someScope.someFunction</c> on the target instance.</param>
	/// <param name="args">JSON-serializable arguments.</param>
	void InvokeVoid(string identifier, params object?[]? args);

	/// <summary>
	/// Invokes the specified JavaScript function synchronously.
	/// </summary>
	/// <typeparam name="TResult">The JSON-serializable return type.</typeparam>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>someScope.someFunction</c> on the target instance.</param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>An instance of <typeparamref name="TResult"/> obtained by JSON-deserializing the return value.</returns>
	TResult Invoke<TResult>(string identifier, params object?[]? args);

	/// <summary>
	/// Invokes the specified JavaScript function asynchronously.
	/// </summary>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>A <see cref="ValueTask"/> that represents the asynchronous invocation operation.</returns>
	ValueTask InvokeVoidAsync(string identifier, params object?[]? args);

	/// <summary>
	/// Invokes the specified JavaScript function asynchronously.
	/// </summary>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
	/// <param name="cancellationToken">
	/// A cancellation token to signal the cancellation of the operation. Specifying this parameter will override any default cancellations such as due to timeouts
	/// (<see cref="JSRuntime.DefaultAsyncTimeout"/>) from being applied.
	/// </param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>A <see cref="ValueTask"/> that represents the asynchronous invocation operation.</returns>
	ValueTask InvokeVoidAsync(string identifier, CancellationToken cancellationToken, params object?[]? args);

	/// <summary>
	/// Invokes the specified JavaScript function asynchronously.
	/// </summary>
	/// <typeparam name="TResult">The JSON-serializable return type.</typeparam>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>someScope.someFunction</c> on the target instance.</param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>An instance of <typeparamref name="TResult"/> obtained by JSON-deserializing the return value.</returns>
	ValueTask<TResult> InvokeAsync<TResult>(string identifier, params object?[]? args);

	/// <summary>
	/// Invokes the specified JavaScript function asynchronously.
	/// </summary>
	/// <typeparam name="TResult">The JSON-serializable return type.</typeparam>
	/// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>someScope.someFunction</c> on the target instance.</param>
	/// <param name="cancellationToken">
	/// A cancellation token to signal the cancellation of the operation. Specifying this parameter will override any default cancellations such as due to timeouts
	/// (<see cref="JSRuntime.DefaultAsyncTimeout"/>) from being applied.
	/// </param>
	/// <param name="args">JSON-serializable arguments.</param>
	/// <returns>An instance of <typeparamref name="TResult"/> obtained by JSON-deserializing the return value.</returns>
	ValueTask<TResult> InvokeAsync<TResult>(string identifier, CancellationToken cancellationToken, params object?[]? args);

	#endregion

	#region Cirreum Namespace

	/// <summary>
	/// Gets the application name from the cirreum namespace.
	/// </summary>
	string GetAppName();

	/// <summary>
	/// Gets the assembly name from the cirreum namespace.
	/// </summary>
	string? GetAssemblyName();

	/// <summary>
	/// Gets the current theme from the cirreum namespace.
	/// </summary>
	string GetCurrentTheme();

	/// <summary>
	/// Sets the theme via the cirreum namespace.
	/// </summary>
	/// <param name="scheme">The theme name to set.</param>
	/// <returns>True if the theme was set successfully.</returns>
	bool SetTheme(string scheme);

	/// <summary>
	/// Gets valid theme names from the cirreum namespace.
	/// </summary>
	string[] GetValidThemes();

	/// <summary>
	/// Gets authentication configuration from the cirreum namespace.
	/// </summary>
	AppAuthInfo GetAuthInfo();

	/// <summary>
	/// Gets tenant configuration from the cirreum namespace (for dynamic auth).
	/// </summary>
	TenantAuthConfig? GetTenantConfig();

	/// <summary>
	/// Gets the tenant slug from the cirreum namespace.
	/// </summary>
	string GetTenantSlug();

	/// <summary>
	/// Gets the tenant display name from the cirreum namespace.
	/// </summary>
	string? GetTenantDisplayName();

	#endregion

	#region Media Queries

	bool IsStandAlone();
	/// <summary>
	/// Gets the current system theme mode.
	/// </summary>
	/// <returns></returns>
	string GetSystemThemeMode();
	/// <summary>
	/// Supply an object that implements <see cref="IThemeModeChangedRef"/> to receive
	/// updates when the system theme mode changes.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IThemeModeChangedRef"/></typeparam>
	/// <param name="dotnetObjRef">The <see cref="DotNetObjectReference{T}"/> to receive callbacks.</param>
	void MonitorSystemThemeMode<T>(DotNetObjectReference<T> dotnetObjRef) where T : class, IThemeModeChangedRef;
	void RemoveSystemThemeModeMonitor();
	bool GetCurrentBreakPoint(string minBreakpointSize);
	void MonitorBreakpoint<T>(DotNetObjectReference<T> dotnetObjRef, string minBreakpointSize) where T : class;
	void RemoveBreakpointMonitor(string minBreakpointSize);

	#endregion

	#region Global

	/// <summary>
	/// Gets the browser's user agent string.
	/// </summary>
	/// <returns>The user agent string.</returns>
	string GetUserAgent();

	#endregion

	#region Head

	/// <summary>
	/// Adds an HTML Link Element for a StyleSheet to the HEAD section, if it doesn't already exist.
	/// </summary>
	/// <param name="href">The uri of the link.</param>
	/// <param name="integrity">The optional integrity string to include with the link.</param>
	/// <param name="title">The optional title of the LINKElement</param>
	/// <param name="disabled">Optionally set the LINKElement to disabled</param>
	/// <returns>A <see cref="void"/></returns>
	void IncludeStyleSheet(string href, string? integrity = null, string? title = null, bool? disabled = null);
	/// <summary>
	/// Replaces an HTML Link Element's <c>href</c> value.
	/// </summary>
	/// <param name="oldHref">The existing uri of the link to be replaced.</param>
	/// <param name="newHref">The new uri of the link to set, if a link with the <paramref name="oldHref"/> value exists.</param>
	/// <returns>A <see cref="void"/></returns>
	void ReplaceHeadLink(string oldHref, string newHref);
	#endregion

	#region Elements

	// Focus
	void FocusElement(ElementReference element, bool preventScroll = false);
	void FocusElement(string element, bool preventScroll = false);
	void FocusFirstElement(ElementReference container, bool preventScroll = false);
	void FocusFirstElement(string container, bool preventScroll = false);
	void FocusLastElement(ElementReference container, bool preventScroll = false);
	void FocusLastElement(string container, bool preventScroll = false);
	void FocusNextElement(bool reverse, ElementReference element, bool preventScroll = false);
	void FocusNextElement(bool reverse, bool preventScroll, string? elementId = null);

	// Scrollbars
	bool IsVerticalScrollbarVisible(string selector);
	bool IsHorizontalScrollbarVisible(string selector);
	/// <summary>
	/// If scrollbars are detected on the selector, then
	/// adds the supplied <paramref name="classes"/> to that elements
	/// class list.
	/// </summary>
	/// <param name="selector"></param>
	/// <param name="vertical"></param>
	/// <param name="classes"></param>
	void SetElementClassIfScrollbar(string selector, bool vertical, params string[] classes);

	// Queries
	bool IsChildOf(ElementReference element, string selector);
	void SetCheckBoxIndeterminate(ElementReference element, bool value);
	void SetCheckBoxIndeterminate(string selector, bool value);

	// Attributes

	/// <summary>
	/// Sets an element's attribute value.
	/// </summary>
	/// <param name="element"></param>
	/// <param name="property"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	void SetElementAttribute(ElementReference element, string property, object value);
	/// <summary>
	/// Sets an element's attribute value.
	/// </summary>
	/// <param name="selector"></param>
	/// <param name="property"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	void SetElementAttribute(string selector, string property, object value);
	/// <summary>
	/// Sets an attribute value, on one or more elements.
	/// </summary>
	/// <param name="selector"></param>
	/// <param name="property"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	void SetElementsAttribute(string selector, string property, object value);
	string GetElementAttribute(ElementReference element, string property);
	string GetElementAttribute(string selector, string property);
	string GetElementId(ElementReference element);

	// Text
	/// <summary>
	/// Extracts innerText from an elements first child node if its an HTMLElement or the element itself.
	/// </summary>
	/// <param name="element">The target element (<see cref="ElementReference"/>).</param>
	/// <param name="tryFirstChild">If true, tries to extract the textContent from the first child node that is an HTMLElement.</param>
	/// <returns>The extracted innerText, or an empty string if no text is found.</returns>
	string GetElementText(ElementReference element, bool tryFirstChild = false);
	/// <summary>
	/// Extracts innerText from an elements first child node if its an HTMLElement or the element itself.
	/// </summary>
	/// <param name="selector">The target element selector (can be an any valid selector.</param>
	/// <param name="tryFirstChild">If true, tries to extract the textContent from the first child node that is an HTMLElement.</param>
	/// <returns>The extracted innerText, or an empty string if no text is found.</returns>
	string GetElementText(string selector, bool tryFirstChild = false);
	/// <summary>
	/// Extracts textContent from an elements first child node if its an HTMLElement or the element itself.
	/// </summary>
	/// <param name="element">The target element (<see cref="ElementReference"/>).</param>
	/// <param name="tryFirstChild">If true, tries to extract the textContent from the first child node that is an HTMLElement.</param>
	/// <returns>The extracted textContent, or an empty string if no text is found.</returns>
	string GetElementTextContent(ElementReference element, bool tryFirstChild = false);
	/// <summary>
	/// Extracts textContent from an elements first child node if its an HTMLElement or the element itself.
	/// </summary>
	/// <param name="selector">The target element selector (can be an any valid selector.</param>
	/// <param name="tryFirstChild">If true, tries to extract the textContent from the first child node that is an HTMLElement.</param>
	/// <returns>The extracted textContent, or an empty string if no text is found.</returns>
	string GetElementTextContent(string selector, bool tryFirstChild = false);

	// Classes
	bool ParentContainsClass(ElementReference child, string token);
	bool ParentContainsClass(string selector, string token);
	bool ElementContainsClass(ElementReference element, string token);
	bool ElementContainsClass(string selector, string token);
	void AddElementClass(ElementReference element, string token);
	void AddElementClass(string selector, string token);
	void AddElementClasses(ElementReference element, params string[] tokens);
	void AddElementClasses(string selector, params string[] tokens);
	void RemoveElementClass(ElementReference element, string token);
	void RemoveElementClass(string selector, string token);
	void RemoveElementClasses(ElementReference element, string[] tokens);
	void RemoveElementClasses(string selector, string[] tokens);
	bool ToggleElementClass(ElementReference element, string token, bool? force = null);
	bool ToggleElementClass(string selector, string token, bool? force = null);
	void SetElementClass(ElementReference element, string value);
	void SetElementClass(string selector, string value);
	bool SwapElementClass(ElementReference element, string token, string newToken);
	bool SwapElementClass(string selector, string token, string newToken);

	// Styles
	void SetElementStyleProperty(string elementId, string property, string value);
	void ScrollElementIntoView(string selector, ScrollBehavior behavior = ScrollBehavior.Smooth, ScrollLogicalPosition block = ScrollLogicalPosition.Nearest, ScrollLogicalPosition inline = ScrollLogicalPosition.End);

	// Dimensions
	void SetElementHeight(string selector, string height);
	void SetElementHeight(ElementReference element, string height);
	void SetElementMaxHeight(string selector, string maxHeight);
	void SetElementMaxHeight(ElementReference element, string maxHeight);
	void SetElementMaxHeightFromScrollHeight(string selector);
	void SetElementMaxHeightFromScrollHeight(ElementReference element);
	int GetElementScrollHeight(string selector);
	int GetElementScrollHeight(ElementReference element);
	int GetElementScrollWidth(string selector);
	int GetElementScrollWidth(ElementReference element);
	int GetElementScrollLeft(string selector);
	int GetElementScrollLeft(ElementReference element);
	void SetElementScrollLeft(string selector, int value);
	void SetElementScrollLeft(ElementReference element, int value);
	int GetElementScrollTop(string selector);
	int GetElementScrollTop(ElementReference element);
	void SetElementScrollTop(string selector, int value);
	void SetElementScrollTop(ElementReference element, int value);

	// Rectangles & Coordinates
	DomRect GetAccurateBoundingClientRect(ElementReference element);
	DomRect GetAccurateBoundingClientRect(string selector);
	DomRect GetBoundingClientRect(string selector);
	DomRect GetBoundingClientRect(ElementReference element);
	ElementCoordinates GetElementCoordinates(string selector);
	ElementCoordinates GetElementCoordinates(ElementReference element);
	ElementDimensions GetElementDimensions(ElementReference element);
	ElementDimensions GetElementDimensions(string selector);
	ElementDimensions GetViewportDimensions();
	Coordinates GetScrollPosition();
	/// <summary>
	/// Looks for a footer element (footer/.footer), and if found returns its height.
	/// </summary>
	/// <returns>The footer height, if found. Otherwise zero (0).</returns>
	double GetFooterHeight();

	#endregion

	#region PopperJS

	void ForceUpdatePopperJS(int id);
	void ClosePopperJS(string id);
	string ShowPopperJS(PopperJSOptions options);

	#endregion

	#region MouseButtonListener

	void AddMouseButtonListener<T>(string elementIdOrSelector, DotNetObjectReference<T> callback, int delay, MouseButton button) where T : class;
	void AddMouseButtonListener<T>(ElementReference element, DotNetObjectReference<T> callback, int delay, MouseButton button) where T : class;
	void RemoveMouseButtonListener(string elementIdOrSelector);
	void RemoveMouseButtonListener(ElementReference element);

	#endregion

}