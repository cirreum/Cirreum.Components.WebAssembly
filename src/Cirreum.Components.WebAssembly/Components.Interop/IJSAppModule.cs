namespace Cirreum.Components.Interop;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading;
using System.Threading.Tasks;

public interface IJSAppModule {

	AppAuthInfo GetAuthInfo();

	void FocusFirstElement(ElementReference container, bool preventScroll);
	void FocusFirstElement(string container, bool preventScroll);
	void FocusLastElement(ElementReference container, bool preventScroll);
	void FocusLastElement(string container, bool preventScroll);
	void FocusNextElement(bool reverse, ElementReference element, bool preventScroll);
	void FocusNextElement(bool reverse, bool preventScroll, string? elementId = null);
	void FocusElement(ElementReference element, bool preventScroll);
	void FocusElement(string element, bool preventScroll);
	void AddElementClass(ElementReference element, string token);
	void AddElementClass(string selector, string token);
	void AddElementClasses(ElementReference element, params string[] tokens);
	void AddElementClasses(string selector, params string[] tokens);
	void ClosePopperJS(string id);
	bool ElementContainsClass(ElementReference element, string token);
	bool ElementContainsClass(string selector, string token);
	void ForceUpdatePopperJS(int id);

	DomRect GetBoundingClientRect(ElementReference element);
	DomRect GetBoundingClientRect(string selector);
	DomRect GetAccurateBoundingClientRect(ElementReference element);
	DomRect GetAccurateBoundingClientRect(string selector);
	ElementDimensions GetViewportDimensions();
	Coordinates GetScrollPosition();
	ElementDimensions GetElementDimensions(ElementReference element);
	ElementDimensions GetElementDimensions(string selector);
	ElementCoordinates GetElementCoordinates(ElementReference element);
	ElementCoordinates GetElementCoordinates(string selector);
	string GetElementAttribute(ElementReference element, string property);
	string GetElementAttribute(string selector, string property);

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

	string GetElementId(ElementReference element);
	int GetElementScrollHeight(ElementReference element);
	int GetElementScrollHeight(string selector);
	int GetElementScrollLeft(ElementReference element);
	int GetElementScrollLeft(string selector);
	int GetElementScrollTop(ElementReference element);
	int GetElementScrollTop(string selector);
	int GetElementScrollWidth(ElementReference element);
	int GetElementScrollWidth(string selector);
	bool GetCurrentBreakPoint(string minBreakpointSize);
	/// <summary>
	/// Looks for a footer element (footer/.footer), and if found returns its height.
	/// </summary>
	/// <returns>The footer height, if found. Otherwise zero (0).</returns>
	double GetFooterHeight();
	/// <summary>
	/// Adds an HTML Link Element for a StyleSheet to the HEAD section, if it doesn't already exist.
	/// </summary>
	/// <param name="href">The uri of the link.</param>
	/// <param name="integrity">The optional integrity string to include with the link.</param>
	/// <param name="title">The optional title of the LINKElement</param>
	/// <param name="disabled">Optionally set the LINKElement to disabled</param>
	/// <returns>A <see cref="void"/></returns>
	void IncludeStyleSheet(string href, string? integrity = null, string? title = null, bool? disabled = null);
	TResult Invoke<TResult>(string identifier, params object?[]? args);
	ValueTask<TResult> InvokeAsync<TResult>(string identifier, CancellationToken token, params object?[]? args);
	ValueTask<TResult> InvokeAsync<TResult>(string identifier, params object?[]? args);
	void InvokeVoid(string identifier, params object?[]? args);
	ValueTask InvokeVoidAsync(string identifier, CancellationToken token, params object?[]? args);
	ValueTask InvokeVoidAsync(string identifier, params object?[]? args);
	bool IsChildOf(ElementReference element, string selector);
	bool IsStandAlone();
	bool IsVerticalScrollbarVisible(string selector);
	bool IsHorizontalScrollbarVisible(string selector);
	void AddMouseButtonListener<T>(
		string elementIdOrSelector,
		DotNetObjectReference<T> callback,
		int delay,
		MouseButton button) where T : class;
	void AddMouseButtonListener<T>(
		ElementReference element,
		DotNetObjectReference<T> callback,
		int delay,
		MouseButton button) where T : class;
	void RemoveMouseButtonListener(string elementIdOrSelector);
	void RemoveMouseButtonListener(ElementReference element);
	/// <summary>
	/// If scrollbars are detected on the selector, then
	/// adds the supplied <paramref name="classes"/> to that elements
	/// class list.
	/// </summary>
	/// <param name="selector"></param>
	/// <param name="vertical"></param>
	/// <param name="classes"></param>
	void SetElementClassIfScrollbar(string selector, bool vertical, params string[] classes);

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

	void MonitorBreakpoint<T>(DotNetObjectReference<T> dotnetObjRef, string minBreakpointSize) where T : class;

	bool ParentContainsClass(ElementReference child, string token);
	bool ParentContainsClass(string selector, string token);
	void RemoveElementClass(ElementReference element, string token);
	void RemoveElementClass(string selector, string token);
	void RemoveElementClasses(ElementReference element, string[] tokens);
	void RemoveElementClasses(string selector, string[] tokens);
	/// <summary>
	/// Replaces an HTML Link Element's <c>href</c> value.
	/// </summary>
	/// <param name="oldHref">The existing uri of the link to be replaced.</param>
	/// <param name="newHref">The new uri of the link to set, if a link with the <paramref name="oldHref"/> value exists.</param>
	/// <returns>A <see cref="void"/></returns>
	void ReplaceHeadLink(string oldHref, string newHref);
	void ScrollElementIntoView(string selector, ScrollBehavior behavior = ScrollBehavior.Smooth, ScrollLogicalPosition block = ScrollLogicalPosition.Nearest, ScrollLogicalPosition inline = ScrollLogicalPosition.End);
	void SetCheckBoxIndeterminate(ElementReference element, bool value);
	void SetCheckBoxIndeterminate(string selector, bool value);
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
	void SetElementClass(ElementReference element, string value);
	void SetElementClass(string selector, string value);
	void SetElementHeight(ElementReference element, string height);
	void SetElementHeight(string selector, string height);
	void SetElementMaxHeight(ElementReference element, string maxHeight);
	void SetElementMaxHeight(string selector, string maxHeight);
	void SetElementMaxHeightFromScrollHeight(ElementReference element);
	void SetElementMaxHeightFromScrollHeight(string selector);
	/// <summary>
	/// Sets an attribute value, on one or more elements.
	/// </summary>
	/// <param name="selector"></param>
	/// <param name="property"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	void SetElementsAttribute(string selector, string property, object value);
	void SetElementScrollLeft(ElementReference element, int value);
	void SetElementScrollLeft(string selector, int value);
	void SetElementScrollTop(ElementReference element, int value);
	void SetElementScrollTop(string selector, int value);
	string ShowPopperJS(PopperJSOptions options);
	bool SwapElementClass(ElementReference element, string token, string newToken);
	bool SwapElementClass(string selector, string token, string newToken);
	bool ToggleElementClass(ElementReference element, string token, bool? force = null);
	bool ToggleElementClass(string selector, string token, bool? force = null);
	void SetElementStyleProperty(string menuElementId, string property, string value);
}