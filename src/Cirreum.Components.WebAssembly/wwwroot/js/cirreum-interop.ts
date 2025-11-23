/**
 * Interface representing detailed information about scrollbars.
 */
interface ScrollbarInfo {
	/** Indicates if any type of scrollbar is present */
	hasScrollbars: boolean;
	/** Indicates if standard scrollbars are present */
	hasStandard: boolean;
	/** Indicates if custom scrollbars are present */
	hasCustom: boolean;
	/** Detailed information about scrollbar types and dimensions */
	details: {
		/** Information about standard scrollbars */
		standard: {
			/** Indicates if a standard vertical scrollbar is present */
			hasVertical: boolean;
			/** Indicates if a standard horizontal scrollbar is present */
			hasHorizontal: boolean;
		};
		/** Dimensions of detected scrollbars */
		dimensions: {
			/** Width of the vertical scrollbar in pixels */
			vertical: number;
			/** Height of the horizontal scrollbar in pixels */
			horizontal: number;
		};
	};
}

interface DotNetHelper {
	invokeMethodAsync(methodName: string, ...args: any[]): Promise<void>;
}

interface IPopper {
	createPopper(host: HTMLElement, popper: HTMLElement, config: any): any;
	forceUpdate(): void;
	destroy(): void;
}
declare const Popper: IPopper;

interface Window2 extends Window {
	loadCss(href: string, integrity?: string | undefined, title?: string | undefined, disabled?: boolean | undefined): void;
	authConfig: IAuthInfo;
}
declare const window: Window2;

interface IAuthInfo {
	include: boolean,
	authType: string
}

interface IInternationalFormat {
	locale: string;
	calendar: string;
	numberingSystem: string;
	timeZone: string;
	hour12?: boolean;
	weekday?: string;
	era?: string;
	year?: string;
	month?: string;
	day?: string;
	hour?: string;
	minute?: string;
	second?: string;
	timeZoneName?: string;
	timeZoneOffset: number
}

enum MouseButton {
	None = -1,
	Left = 0,
	Middle = 1,
	Right = 2,
	Back = 3,
	Forward = 4
}

/**
 * Resolves a DOM element based on the provided input.
 * 
 * @param selectorOrElement - Can be one of:
 *   - A string representing any valid CSS selector
 *   - An HTMLElement object
 * 
 * @returns The resolved HTMLElement, or null if no element is found.
 * 
 * @description
 * This function attempts to resolve an element using the following strategy:
 * 1. For ID selectors starting with '#', it uses querySelector.
 * 2. For strings without '#' that look like valid ID names, it tries getElementById first.
 * 3. For all other string inputs, it uses querySelector.
 * 4. If an HTMLElement is provided, it is returned as-is.
 * This approach ensures maximum compatibility with various selector types.
 * 
 * @example
 * resolveElement('myElementId');        // Tries getElementById, then fallback to querySelector
 * resolveElement('#myElementId');       // Uses querySelector
 * resolveElement('.myClass');           // Uses querySelector
 * resolveElement('body');               // Uses querySelector
 * resolveElement('[data-attr]');        // Uses querySelector
 * resolveElement('div > p');            // Uses querySelector
 * resolveElement(document.body);        // Returns the provided HTMLElement
 */
function resolveElement<T extends HTMLElement>(selectorOrElement: string | T): T | null {
	if (typeof selectorOrElement === 'string') {
		let element: Element | null;

		if (selectorOrElement.charAt(0) === '#') {
			// For ID selectors, use querySelector directly
			element = document.querySelector(selectorOrElement);
		} else if (/^[a-zA-Z][\w-]*$/.test(selectorOrElement)) {
			// For strings that look like valid ID names, try getElementById first
			element = document.getElementById(selectorOrElement);
			// If not found by ID, fall back to querySelector
			if (!element) {
				element = document.querySelector(selectorOrElement);
			}
		} else {
			// For all other selectors, use querySelector
			element = document.querySelector(selectorOrElement);
		}

		// Ensure the element is an HTMLElement
		return (element instanceof HTMLElement) ? element as T : null;
	}
	// selectorOrElement is an HTMLElement
	return selectorOrElement;
}

/**
 * Detects the presence and type of scrollbars in the current document.
 * 
 * This function checks for the presence of standard and custom scrollbars,
 * and provides detailed information about their dimensions and presence in both
 * vertical and horizontal directions.
 * 
 * @returns {ScrollbarInfo} An object containing detailed information about scrollbars.
 * @property {boolean} hasScrollbars - True if any type of scrollbar is detected.
 * @property {boolean} hasStandard - True if standard scrollbars are detected.
 * @property {boolean} hasCustom - True if custom scrollbars are detected.
 * @property {Object} details - Detailed information about scrollbars.
 * @property {Object} details.standard - Information about standard scrollbars.
 * @property {boolean} details.standard.hasVertical - True if a standard vertical scrollbar is present.
 * @property {boolean} details.standard.hasHorizontal - True if a standard horizontal scrollbar is present.
 * @property {Object} details.dimensions - Dimensions of detected scrollbars.
 * @property {number} details.dimensions.vertical - Width of the vertical scrollbar in pixels.
 * @property {number} details.dimensions.horizontal - Height of the horizontal scrollbar in pixels.
 * 
 * @example
 * const scrollInfo = detectScrollbars();
 * console.log(scrollInfo.hasScrollbars);
 * console.log(scrollInfo.details.standard.hasVertical);
 * console.log(scrollInfo.details.dimensions.horizontal);
 */
function detectScrollbars(): ScrollbarInfo {
	// Function to check for custom scrollbars
	function hasCustomScrollbars(): boolean {
		const style = document.createElement('style');
		style.textContent = '::-webkit-scrollbar { width: 0; height: 0; }';
		document.head.appendChild(style);
		const hasCustom = window.getComputedStyle(document.body).scrollbarWidth === 'none';
		document.head.removeChild(style);
		return hasCustom;
	}

	// Detect scrollbar dimensions
	const outer = document.createElement('div');
	const inner = document.createElement('div');
	outer.style.visibility = 'hidden';
	outer.style.overflow = 'scroll';
	outer.style.position = 'absolute';
	outer.style.top = '-9999px';
	document.body.appendChild(outer);
	outer.appendChild(inner);

	// Vertical scrollbar
	const verticalWidth1 = window.innerWidth - document.documentElement.clientWidth;
	const verticalWidth2 = outer.offsetWidth - inner.offsetWidth;

	// Horizontal scrollbar
	outer.style.overflowY = 'hidden';
	const horizontalHeight1 = window.innerHeight - document.documentElement.clientHeight;
	const horizontalHeight2 = outer.offsetHeight - inner.offsetHeight;

	document.body.removeChild(outer);

	// Get the smaller of the two measurements for each direction
	const verticalWidth = Math.min(verticalWidth1, verticalWidth2);
	const horizontalHeight = Math.min(horizontalHeight1, horizontalHeight2);

	// Determine if standard scrollbars are present (using a small threshold)
	const hasStandardVertical = verticalWidth > 1;
	const hasStandardHorizontal = horizontalHeight > 1;

	// Determine if any custom scrollbards
	const hasCustombars = hasCustomScrollbars();

	return {
		hasScrollbars: hasStandardVertical || hasStandardHorizontal || hasCustombars,
		hasStandard: hasStandardVertical || hasStandardHorizontal,
		hasCustom: hasCustombars,
		details: {
			standard: {
				hasVertical: hasStandardVertical,
				hasHorizontal: hasStandardHorizontal
			},
			dimensions: {
				vertical: verticalWidth,
				horizontal: horizontalHeight
			}
		}
	};

}


/**
 * Determines if a scrollbar is visible for a given element in the specified direction.
 * 
 * This function checks whether a scrollbar is visible for the provided element,
 * taking into account various factors such as overflow settings, element dimensions,
 * and parent element configurations.
 * 
 * @param {Element} element - The DOM element to check for scrollbar visibility.
 * @param {'horizontal' | 'vertical'} [direction='horizontal'] - The direction to check for scrollbar visibility.
 *        Can be either 'horizontal' or 'vertical'. Defaults to 'horizontal'.
 * 
 * @returns {boolean} True if a scrollbar is visible in the specified direction, false otherwise.
 * 
 * @example
 * const myElement = document.getElementById('my-element');
 * const isHorizontalScrollbarVisible = isElementScrollbarVisible(myElement);
 * const isVerticalScrollbarVisible = isElementScrollbarVisible(myElement, 'vertical');
 * 
 * @remarks
 * - For root elements (html or body), the function compares against the viewport size.
 * - For elements with overflow set to 'visible', the function checks parent elements
 *   until it finds a non-'visible' overflow or reaches the viewport.
 * - The function takes into account various overflow settings: 'hidden', 'visible',
 *   'auto', and 'scroll'.
 */
function isElementScrollbarVisible(element: Element, direction: 'horizontal' | 'vertical' = 'horizontal'): boolean {
	// Get the computed style of the element
	const style = window.getComputedStyle(element);

	// Determine which properties to use based on the direction
	const overflow = direction === 'horizontal' ? style.overflowX : style.overflowY;
	const scrollSize = direction === 'horizontal' ? element.scrollWidth : element.scrollHeight;
	const clientSize = direction === 'horizontal' ? element.clientWidth : element.clientHeight;

	// Check if the element is the root element (html or body)
	const isRootElement = element === document.documentElement || element === document.body;

	if (isRootElement) {
		const viewportSize = direction === 'horizontal' ?
			window.innerWidth || document.documentElement.clientWidth :
			window.innerHeight || document.documentElement.clientHeight;

		// For root elements, compare against viewport size
		return scrollSize > viewportSize && overflow !== 'hidden';
	}

	// For regular elements
	if (overflow === 'hidden') {
		return false;
	}

	if (overflow === 'visible') {
		// Check if any parent has overflow other than 'visible'
		let parent = element.parentElement;
		while (parent) {
			const parentStyle = window.getComputedStyle(parent);
			const parentOverflow = direction === 'horizontal' ? parentStyle.overflowX : parentStyle.overflowY;
			if (parentOverflow !== 'visible') {
				return scrollSize > clientSize;
			}
			parent = parent.parentElement;
		}
		// If all parents are 'visible', check against viewport
		const viewportSize = direction === 'horizontal' ?
			window.innerWidth || document.documentElement.clientWidth :
			window.innerHeight || document.documentElement.clientHeight;
		return scrollSize > viewportSize;
	}

	// For 'auto', 'scroll', or any other value
	return scrollSize > clientSize;
}

/*
 * 
 * Cirreum Interop
 * 
 */

//#region Browser Environment

/**
 * Gets browser's internationalization format information.
 * Returns the resolved options from Intl.DateTimeFormat with additional timezone offset.
 * @returns An object containing the browser's locale, timezone, and formatting preferences.
 */
export function getInternationalFormats(): IInternationalFormat {
	// Get basic info from default constructor
	const basicOptions = Intl.DateTimeFormat().resolvedOptions();

	// Create a formatter with more explicit options to get additional fields
	const fullFormatter = new Intl.DateTimeFormat(undefined, {
		weekday: 'long',
		era: 'long',
		year: 'numeric',
		month: 'numeric',
		day: 'numeric',
		hour: 'numeric',
		minute: 'numeric',
		second: 'numeric',
		timeZoneName: 'long'
	});

	// Get the resolved options from the full formatter
	const fullOptions = fullFormatter.resolvedOptions();

	// Combine everything
	return {
		locale: basicOptions.locale || "",
		calendar: basicOptions.calendar || "",
		numberingSystem: basicOptions.numberingSystem || "",
		timeZone: basicOptions.timeZone || "",
		timeZoneOffset: new Date().getTimezoneOffset(),
		hour12: fullOptions.hour12,
		weekday: fullOptions.weekday || undefined,
		era: fullOptions.era || undefined,
		year: fullOptions.year || undefined,
		month: fullOptions.month || undefined,
		day: fullOptions.day || undefined,
		hour: fullOptions.hour || undefined,
		minute: fullOptions.minute || undefined,
		second: fullOptions.second || undefined,
		timeZoneName: fullOptions.timeZoneName || undefined
	};
}

/**
 * Gets the current local time as a string.
 * @returns A string representation of the current local time.
 */
export function getCurrentLocalTime(): string {
	return new Date().toString();
}

/**
 * Gets the current UTC time as a string.
 * @returns A string representation of the current UTC time.
 */
export function getCurrentUtcTime(): string {
	return new Date().toUTCString();
}

/**
 * Determines if daylight saving time is currently in effect.
 * @returns 'Yes' if DST is in effect, 'No' otherwise.
 */
export function isDaylightSavingTime(): string {
	const jan: Date = new Date(new Date().getFullYear(), 0, 1);
	const jul: Date = new Date(new Date().getFullYear(), 6, 1);
	const stdTimezoneOffset: number = Math.max(jan.getTimezoneOffset(), jul.getTimezoneOffset());
	return new Date().getTimezoneOffset() < stdTimezoneOffset ? 'Yes' : 'No';
}

/**
 * Interface for formatted date samples
 */
interface FormattedSamples {
	defaultFormat: string;
	dateOnly: string;
	timeOnly: string;
	full: string;
	withTimeZone: string;
}

/**
 * Gets samples of different date/time formats.
 * @returns An object containing various formatted date/time strings.
 */
export function getFormattedSamples(): FormattedSamples {
	const now: Date = new Date();

	// Try to test if dateStyle/timeStyle are supported
	let supportsNewOptions = false;
	try {
		// This will throw an error if dateStyle isn't supported
		new Intl.DateTimeFormat('en', { dateStyle: 'full' }).format(now);
		supportsNewOptions = true;
	} catch (e) {
		supportsNewOptions = false;
	}

	let fullFormatted: string;
	if (supportsNewOptions) {
		try {
			fullFormatted = new Intl.DateTimeFormat(undefined, {
				dateStyle: 'full',
				timeStyle: 'long'
			} as Intl.DateTimeFormatOptions).format(now);
		} catch (e) {
			fullFormatted = now.toLocaleString();
		}
	} else {
		// Fallback to traditional options for older browsers
		try {
			fullFormatted = new Intl.DateTimeFormat(undefined, {
				year: 'numeric',
				month: 'long',
				day: 'numeric',
				hour: '2-digit',
				minute: '2-digit',
				second: '2-digit'
			}).format(now);
		} catch (e) {
			fullFormatted = now.toLocaleString();
		}
	}

	let withTimeZoneFormatted: string;
	let timeZoneName: string;
	try {
		const formatter = new Intl.DateTimeFormat(undefined, { timeZoneName: 'long' });
		const parts = formatter.formatToParts(now);
		const timeZonePart = parts.find(part => part.type === 'timeZoneName');
		timeZoneName = timeZonePart ? timeZonePart.value :
			Intl.DateTimeFormat().resolvedOptions().timeZone;
	} catch (e) {
		timeZoneName = Intl.DateTimeFormat().resolvedOptions().timeZone;
	}

	// Format date and time separately from the time zone name
	let dateTimeFormat: string;
	try {
		dateTimeFormat = new Intl.DateTimeFormat(undefined, {
			year: 'numeric',
			month: 'numeric',
			day: 'numeric',
			hour: 'numeric',
			minute: '2-digit',
			second: '2-digit'
		}).format(now);
	} catch (e) {
		dateTimeFormat = now.toLocaleString();
	}

	// Combine them manually
	const withTimeZone = `${dateTimeFormat} (${timeZoneName})`;

	return {
		defaultFormat: now.toLocaleString(),
		dateOnly: now.toLocaleDateString(),
		timeOnly: now.toLocaleTimeString(),
		full: fullFormatted,
		withTimeZone: withTimeZone
	};
}

/**
 * Checks if the browser supports the Intl.DateTimeFormat timeZone feature.
 * @returns True if timeZone is supported, false otherwise.
 */
export function hasTimeZoneSupport(): boolean {
	return typeof Intl !== 'undefined' &&
		'DateTimeFormat' in Intl &&
		typeof new Intl.DateTimeFormat().resolvedOptions().timeZone === 'string';
}

/**
 * Checks if the browser supports the Date.getTimezoneOffset method.
 * @returns True if getTimezoneOffset is supported, false otherwise.
 */
export function hasOffsetSupport(): boolean {
	return typeof new Date().getTimezoneOffset === 'function';
}

/**
 * Gets the browser's user agent string.
 * @returns The user agent string.
 */
export function getUserAgent(): string {
	return navigator.userAgent;
}

//#endregion

//#region Window

export function getAuthInfo(): IAuthInfo {
	return window.authConfig;
}

if (typeof window.loadCss !== "function") {
	window.loadCss = (href: string, integrity?: string | undefined, title?: string | undefined, disabled?: boolean | undefined) => {
		const linkCss = document.createElement("link");
		linkCss.crossOrigin = "anonymous";
		linkCss.referrerPolicy = "no-referrer";
		linkCss.rel = "stylesheet";
		linkCss.type = "text/css";
		if (integrity) {
			linkCss.integrity = integrity;
		}
		if (title) {
			linkCss.title = title;
		}
		if (disabled) {
			linkCss.disabled = disabled;
		}
		linkCss.onerror = () => {
			// Error occurred while loading stylesheet
			console.error('Error occurred while loading stylesheet', href);
		};
		linkCss.href = href;
		document.head.appendChild(linkCss);
	}
}

//#endregion

//#region Media Queries

export function isStandAlone(): boolean {
	return window.matchMedia('(display-mode: standalone)').matches;
}

export function getSystemThemeMode(): string {
	return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}

export function monitorSystemThemeMode(dotnetRef: DotNetHelper): void {
	const darkThemeMq = window.matchMedia("(prefers-color-scheme: dark)");
	darkThemeMq.addEventListener("change", async e => {
		const storedTheme = localStorage.getItem("user-theme-mode");
		await dotnetRef.invokeMethodAsync("OnModeChanged", e.matches, storedTheme)
	});
}

export function getCurrentBreakPoint(minBreakpoint: string): boolean {
	return window.matchMedia(`(min-width: ${minBreakpoint})`).matches;
}
export function monitorBreakpointChanges(dotnetRef: DotNetHelper, minBreakpointSize: string): void {
	const breakpointMq = window.matchMedia(`(min-width: ${minBreakpointSize})`);
	breakpointMq.addEventListener("change", async e => {
		await dotnetRef.invokeMethodAsync("OnBreakpointChange", e.matches)
	});
}


//#endregion

//#region Headers

export function includeStyleSheet(href: string, integrity?: string | undefined, title?: string | undefined, disabled?: boolean | undefined): void {
	var elink = document.documentElement.querySelector(`link[href="${href}"]`);
	if (elink === null) {
		window.loadCss(href, integrity, title, disabled);
	}
}

export function replaceHeadLink(oldHref: string, newHref: string): void {
	var elink: HTMLLinkElement | null = document.documentElement.querySelector(`link[href="${oldHref}"]`);
	if (elink !== null) {
		elink.href = newHref;
		return;
	}
}

//#endregion

//#region Elements

const focusableElementsQuery: string = [
	"a:not([disabled]):not([tabindex='-1'])",
	"area:not([tabindex='-1'])",
	"button:not([disabled]):not([tabindex='-1'])",
	"input:not([disabled]):not([tabindex='-1'])",
	"select:not([disabled]):not([tabindex='-1'])",
	"textarea:not([disabled]):not([tabindex='-1'])",
	"iframe:not([tabindex='-1'])",
	"details:not([tabindex='-1'])",
	"[tabindex]:not([disabled]):not([tabindex='-1'])",
	"[contentEditable=true]:not([tabindex='-1'])"
].join(',');
export function focusFirstElement(parentElement: string | HTMLElement, preventScroll: boolean): void {
	if (parentElement) {
		const container = resolveElement(parentElement);
		if (container) {
			const focusableElements = container.querySelectorAll<HTMLElement>(focusableElementsQuery);
			if (focusableElements.length === 0) {
				return;
			}
			const firstElement = focusableElements[0];
			firstElement.focus({ preventScroll });
		}
	}
}
export function focusLastElement(parentElement: string | HTMLInputElement, preventScroll: boolean): void {
	if (parentElement) {
		const container = resolveElement(parentElement);
		if (container) {
			const focusableElements = container.querySelectorAll<HTMLElement>(focusableElementsQuery);
			if (focusableElements.length === 0) {
				return;
			}
			if (focusableElements.length === 1) {
				focusableElements[0].focus({ preventScroll } as FocusOptions);
				return;
			}
			focusableElements[focusableElements.length - 1].focus({ preventScroll } as FocusOptions);
		}
	}
}
export function focusElement(element: string | HTMLInputElement, preventScroll: boolean): void {
	const ele = resolveElement(element);
	if (ele) {
		ele.focus({ preventScroll } as FocusOptions);
	}
}
export function focusNextElement(reverse: boolean = false, element: string | null = null, preventScroll: boolean): boolean {
	let activeElem = element
		? resolveElement(element) ?? document.activeElement
		: document.activeElement;
	function innerFocusNextElement(reverse: boolean, activeElem: Element | null): boolean {
		let queryResult = Array.from(document.querySelectorAll(focusableElementsQuery))
			.filter(elem => {
				if (elem instanceof HTMLElement) {
					return elem.offsetWidth > 0 || elem.offsetHeight > 0 || elem === activeElem;
				}
				return false;
			});

		const indexedList = queryResult
			.filter(elem => elem instanceof HTMLElement && elem.tabIndex > 0)
			.sort((a, b) => (a as HTMLElement).tabIndex - (b as HTMLElement).tabIndex);

		const focusable = indexedList.concat(
			queryResult.filter(elem => elem instanceof HTMLElement && elem.tabIndex === 0)
		);

		const currentIndex = focusable.indexOf(activeElem as HTMLElement);

		const nextElement = reverse
			? (focusable[currentIndex - 1] || focusable[focusable.length - 1])
			: (focusable[currentIndex + 1] || focusable[0]);

		if (nextElement instanceof HTMLElement) {
			nextElement.focus({ preventScroll } as FocusOptions);
			return true;
		}
		return false;
	}

	if (!innerFocusNextElement(reverse, activeElem)) {
		return false;
	}
	return true;
}

export function isVerticalScrollbarVisible(selector: string): boolean {
	const ele = resolveElement(selector);
	if (ele) {
		return isElementScrollbarVisible(ele, "vertical");
	}
	return false;
}
export function isHorizontalScrollbarVisible(selector: string): boolean {
	const ele = resolveElement(selector);
	if (ele) {
		return isElementScrollbarVisible(ele, "horizontal");
	}
	return false;
}
export function addElementClassIfScrollbar(selector: string, vertical: boolean, ...tokens: string[]): void {
	const ele = resolveElement(selector);
	if (ele) {

		const scrollbars = detectScrollbars();
		if (scrollbars.hasScrollbars) {
			document.documentElement.style.removeProperty('--scrollbar-width');
		} else {
			document.documentElement.style.setProperty('--scrollbar-width', "0");
		}

		if (vertical) {
			if (isElementScrollbarVisible(ele, "vertical")) {
				ele.classList.add(...tokens);
			}
			return;
		}

		if (isElementScrollbarVisible(ele, "horizontal")) {
			ele.classList.add(...tokens);
		}

	}
}

export function isChildOf(element: Element, selector: any): boolean {
	return element.closest(selector) !== null;
}

//
// Set indeterminate
//
export function setCheckBoxIndeterminate(element: string | HTMLInputElement, value: boolean): void {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			localEl.indeterminate = value;
		}
	}
}

//
// Sets an element's attribute value.
//
export function setElementAttribute(element: string | HTMLElement, property: string, value: string): void {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			localEl.setAttribute(property, value);
		}
	}
}

//
// Sets an attribute value, on one or more elements.
//
export function setElementsAttribute(selector: string, property: string, value: string): void {
	if (selector) {
		document.documentElement.querySelectorAll(selector).forEach((ele) => {
			ele.setAttribute(property, value);
		});
	}
}
//
// Gets an element's attribute.
//
export function getElementAttribute(element: string | HTMLElement, property: string): string {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			return localEl.getAttribute(property) ?? "";
		}
	}
	return "";
}

/**
 * Extracts innerText from an elements first child node if its an HTMLElement or the element itself.
 * @param element - The target element (can be an element Id, string selector or an HTMLElement).
 * @param tryFirstChild - If true, tries to extract the innerText from the first child node that is an HTMLElement.
 * @returns The extracted innerText, or an empty string if no text is found.
 */
export function getElementText(
	element: string | HTMLElement,
	tryFirstChild: boolean
): string {
	try {
		const localEl = resolveElement(element);
		if (!localEl) {
			console.warn(`getElementText: Element not found for selector "${element}"`);
			return "";
		}

		if (tryFirstChild && localEl.childNodes.length > 0) {
			// find first HTMLElement Child Node, if any...
			for (const child of localEl.childNodes) {
				if (child instanceof HTMLElement) {
					const text = child.innerText.trim();
					if (text) return text;
					break; // Stop after first HTMLElement, even if its text is empty
				}
			}
		}


		// If no suitable child found or tryFirstChild is false
		return localEl.innerText.trim();

	} catch (error) {
		console.error(`Error in getElementText: ${error instanceof Error ? error.message : String(error)}`);
		return "";
	}
}

/**
 * Extracts textContent from an elements first child node if its an HTMLElement or the element itself.
 * @param element - The target element (can be an element Id, string selector or an HTMLElement).
 * @param tryFirstChild - If true, tries to extract the textContent from the first child node that is an HTMLElement.
 * @returns The extracted textContent, or an empty string if no text is found.
 */
export function getElementTextContent(
	element: string | HTMLElement,
	tryFirstChild: boolean
): string {
	try {
		const localEl = resolveElement(element);
		if (!localEl) {
			console.warn(`getElementTextContent: Element not found for selector "${element}"`);
			return "";
		}

		if (tryFirstChild && localEl.childNodes.length > 0) {
			// find first HTMLElement Child Node, if any...
			for (const child of localEl.childNodes) {
				if (child instanceof HTMLElement) {
					const text = child.textContent?.trim() ?? "";
					if (text) return text;
					break; // Stop after first HTMLElement, even if its text is empty
				}
			}
		}


		// If no suitable child found or tryFirstChild is false
		return localEl.textContent?.trim() ?? "";

	} catch (error) {
		console.error(`Error in getElementTextContent: ${error instanceof Error ? error.message : String(error)}`);
		return "";
	}
}

//
// Gets the elements ID value.
//
export function getElementId(element: HTMLElement): string {
	if (element) {
		return element.id;
	}
	return "";
}

//
// Determines if the child's parent element
// class list contains the specified css class.
//
export function parentContainsClass(child: string | HTMLElement, token: string): boolean {
	if (child) {
		const localEl = resolveElement(child);
		if (localEl) {
			return localEl.parentElement?.classList.contains(token) ?? false;
		}
	}
	return false;
}

//
// Determines if the element's class list contains the
// specified css class.
//
export function elementContainsClass(element: string | HTMLElement, token: string): boolean {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			return localEl.classList.contains(token);
		}
	}
	return false;
}

//
// Add one or more Css class to an element's classList.
//
export function addElementClasses(element: string | HTMLElement, classString: string): void {
	const el = resolveElement(element);
	if (el && classString) {
		const classes = classString.split(' ');
		el.classList.add(...classes);
	}
}

//
// Remove one or more CssClasses from an element's class list.
//
export function removeElementClasses(element: string | HTMLElement, ...tokens: string[]): void {
	if (tokens) {
		const localEl = resolveElement(element);
		if (localEl) {
			localEl.classList.remove(...tokens);
		}
	}
}

//
// Toggle a CssClass from an element's class list.
//
export function toggleElementClass(element: string | HTMLElement, token: string, force?: boolean | undefined): boolean {
	if (element && token) {
		const localEl = resolveElement(element);
		if (localEl) {
			return localEl.classList.toggle(token, force);
		}
	}
	return false;
}

//
// Swap one class for another class in an element's class list.
//
export function swapElementClass(element: string | HTMLElement, token: string, newToken: string): boolean {
	if (element && token && newToken) {
		const localEl = resolveElement(element);
		if (localEl) {
			return localEl.classList.replace(token, newToken);
		}
	}
	return false;
}

//
// Sets an element's class attribute value.
//
export function setElementClass(element: string | HTMLElement, value: string): void {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			localEl.setAttribute("class", value);
		}
	}
}


//
// Sets an element's style property value.
//
export function setElementStyleProperty(element: string | HTMLElement, property: string, value: string): void {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			localEl.style.setProperty(property, value);
		}
	}
}


//
// Scroll Element into View
//
export function scrollIntoView(element: string | HTMLElement, options: ScrollIntoViewOptions): void {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			localEl.scrollIntoView(options);
		}
	}
}

//
// Set Element Height
//
export function setElementHeight(element: string | HTMLElement, height: string): void {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			localEl.style["height"] = height;
		}
	}
}

//
// Set Element Max-Height
//
export function setElementMaxHeight(element: string | HTMLElement, height: string): void {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			localEl.style["maxHeight"] = height;
		}
	}
}

//
// Set Element Max-Height from ScrollHeight
//
export function setElementMaxHeightFromScrollHeight(element: string | HTMLElement): void {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			localEl.style["maxHeight"] = `${localEl.scrollHeight}px`;
		}
	}
}

//
// Get Element ScrollHeight
//
export function getElementScrollHeight(element: string | HTMLElement): number {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			return localEl.scrollHeight | 0;
		}
	}
	return -9999;
}

//
// Get Element ScrollWidth
//
export function getElementScrollWidth(element: string | HTMLElement): number {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			return localEl.scrollWidth | 0;
		}
	}
	return -9999;
}


//
// Set Element ScrollLeft
//
export function setElementScrollLeft(element: string | HTMLElement, value: number): void {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			localEl.scrollLeft = value;
		}
	}
}

//
// Get Element ScrollLeft
//
export function getElementScrollLeft(element: string | HTMLElement): number {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			return localEl.scrollLeft | 0;
		}
	}
	return -9999;
}

//
// Set Element ScrollTop
//
export function setElementScrollTop(element: string | HTMLElement, value: number): void {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			localEl.scrollTop = value;
		}
	}
}

//
// Get Element ScrollTop
//
export function getElementScrollTop(element: string | HTMLElement): number {
	let result: number = -9999;
	if (element) {
		const localEl = resolveElement(element);
		if (localEl) {
			result = localEl.scrollTop | 0;
		}
	}
	return result;
}

//
// Get Element bounding rectangle
//
export function getBoundingClientRect(element: string | HTMLElement): any {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl && typeof localEl.getBoundingClientRect === "function") {
			var rect = localEl.getBoundingClientRect();

			let result = {
				Bottom: rect.bottom,
				Height: rect.height,
				Left: rect.left,
				Right: rect.right,
				Top: rect.top,
				Width: rect.width,
				X: rect.x,
				Y: rect.y
			}

			return result;
		}
	}
	return null;
}

//
// Get accurate Element bounding rectangle accounting for transforms
//
export function getAccurateBoundingClientRect(element: string | HTMLElement): any {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl && typeof localEl.getBoundingClientRect === "function") {
			const rect = localEl.getBoundingClientRect();
			const computedStyle = window.getComputedStyle(localEl);

			// Get the actual rendered width and height
			const width = localEl.offsetWidth;
			const height = localEl.offsetHeight;

			// Account for transforms
			const transform = computedStyle.transform;
			let offsetX = 0;
			let offsetY = 0;
			if (transform && transform !== 'none') {
				const matrix = new DOMMatrixReadOnly(transform);
				offsetX = matrix.m41;
				offsetY = matrix.m42;
			}

			return {
				Bottom: rect.top + offsetY + height,
				Height: height,
				Left: rect.left + offsetX,
				Right: rect.left + offsetX + width,
				Top: rect.top + offsetY,
				Width: width,
				X: rect.x,
				Y: rect.y
			}

		}
	}
	return null;
}


//
// Get Element location coordinates, accounting for the window's scroll location.
//
export function getElementCoordinates(element: string | HTMLElement): any {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl && typeof localEl.getBoundingClientRect === "function") {
			const rect = localEl.getBoundingClientRect();
			return {
				left: rect.left + window.scrollX,
				top: rect.top + window.scrollY
			};
		}
	}
	return {
		left: window.scrollX,
		top: window.scrollY
	};
}

//
// Get Element size (height/width), using scroll values to include overflow.
//
export function getElementDimensions(element: string | HTMLElement): any {
	if (element) {
		const localEl = resolveElement(element);
		if (localEl && typeof localEl.getBoundingClientRect === "function") {
			return {
				width: localEl.scrollWidth,
				height: localEl.scrollHeight
			};
		}
	}
	return {
		width: 0,
		height: 0
	};
}

//
// Gets the view port size (height/width), using window or documentElement.
//
export function getViewPortDimensions(): any {
	return {
		width: window.innerWidth || document.documentElement.clientWidth,
		height: window.innerHeight || document.documentElement.clientHeight
	};
}

//
// Gets the view port scroll position using window.scrollX and window.scrollY with fallbacks.
//
export function getScrollPosition(): any {

	const x = window.scrollX; // !== undefined ? window.scrollX : window.pageXOffset;
	const y = window.scrollY; // !== undefined ? window.scrollY : window.pageYOffset;

	let totalX = x;
	let totalY = y;

	// Check for scrollable parents
	let currentElement: HTMLElement | null = document.activeElement as HTMLElement;
	while (currentElement && currentElement !== document.body) {
		if (currentElement.scrollHeight > currentElement.clientHeight) {
			totalX += currentElement.scrollLeft;
			totalY += currentElement.scrollTop;
			//console.log(`Scrollable parent found: ${currentElement.tagName}, scroll position: ${currentElement.scrollLeft}, ${currentElement.scrollTop}`);
		}
		currentElement = currentElement.parentElement;
	}

	//console.log(`Window scroll position: ${x}, ${y}`);
	//console.log(`Final total scroll position: ${totalX}, ${totalY}`);
	//console.log(`Viewport dimensions: ${window.innerWidth}x${window.innerHeight}`);
	//console.log(`Document dimensions: ${document.documentElement.scrollWidth}x${document.documentElement.scrollHeight}`);

	return { X: totalX, Y: totalY };
}

//
// Gets the footer's height if it exists; otherwise returns 0.
//
export function getFooterHeight() {
	let footer = document.querySelector('footer');
	if (!footer) {
		footer = document.querySelector('.footer');
	}
	if (footer) {
		const footerStyles = window.getComputedStyle(footer);
		const footerHeight = footer.offsetHeight +
			parseFloat(footerStyles.marginTop) +
			parseFloat(footerStyles.marginBottom);
		return footerHeight;
	}
	return 0;
}

//#endregion

//#region PopperJS

const registeredPoppers: Map<string, IPopper> = new Map<string, IPopper>();
export function destroyPopper(id: string): void {
	const instance = registeredPoppers.get(id);
	if (instance) {
		try {
			instance.destroy();
			registeredPoppers.delete(id);
		} catch (e: any) {
			console.error(e.message);
		}
	} else {
		console.warn("popper id " + id + " not found");
	}
}
export function updatePopper(id: string): void {
	const instance = registeredPoppers.get(id);
	if (instance) {
		try {
			instance.forceUpdate();
		} catch (e: any) {
			console.error(e.message);
		}
	}
}
export function showPopper(id: string, host: HTMLElement, popper: HTMLElement, config: any): void {
	if (registeredPoppers.has(id)) {
		console.warn("showPopper: cannot register a duplicate popper id.");
		return;
	}
	try {
		const instance = Popper.createPopper(host, popper, config);
		registeredPoppers.set(id, instance);
	} catch (e: any) {
		console.error(e.message);
	}
}

//#endregion

//#region MouseButtonListener

enum MouseButtonEventType {
	ContextMenu = 'contextmenu',
	Click = 'click',
}
interface MouseButtonEventInfo {
	pageX: number;
	pageY: number;
	type: MouseButtonEventType;
	button: MouseButton;
}
class MouseButtonListener {
	private timer: number | null = null;

	constructor(
		private element: HTMLElement,
		private dotnetHelper: DotNetHelper,
		private delay: number,
		private triggerButton: MouseButton
	) {
		this.attachListeners();
	}

	private getEventInfo(button: number, e: MouseEvent | Touch, type: MouseButtonEventType): MouseButtonEventInfo {
		return {
			pageX: e.pageX,
			pageY: e.pageY,
			type,
			button: button
		};
	}

	private handleClick = (e: MouseEvent): void => {
		e.preventDefault();
		if (e.button === this.triggerButton) {
			e.stopPropagation();
			const eventInfo = this.getEventInfo(e.button, e, MouseButtonEventType.Click);
			this.dotnetHelper.invokeMethodAsync('OnEventCallback', eventInfo);
		}
	};

	private handleContextMenu = (e: MouseEvent): void => {
		e.preventDefault();
		if (this.triggerButton === MouseButton.Right) {
			e.stopPropagation();
			const eventInfo = this.getEventInfo(e.button, e, MouseButtonEventType.ContextMenu);
			this.dotnetHelper.invokeMethodAsync('OnEventCallback', eventInfo);
		}
	};

	private handleAuxclick = (e: MouseEvent): void => {
		e.preventDefault();
	};

	private handleTouchStart = (e: TouchEvent): void => {
		if (this.triggerButton === MouseButton.Right) {
			this.startLongPress(e.touches[0]);
		} else {
			// For non-right button configurations, trigger immediately on touch
			const eventInfo = this.getEventInfo(this.triggerButton, e.touches[0], MouseButtonEventType.Click);
			this.dotnetHelper.invokeMethodAsync('OnEventCallback', eventInfo);
		}
	};

	private handleTouchEnd = (e: TouchEvent): void => {
		e.preventDefault();
		this.cancelLongPress();
	};

	private startLongPress = (e: Touch): void => {
		this.timer = window.setTimeout(() => {
			const eventInfo = this.getEventInfo(MouseButton.None, e, MouseButtonEventType.ContextMenu);
			this.dotnetHelper.invokeMethodAsync('OnEventCallback', eventInfo);
		}, this.delay);
	};

	private cancelLongPress = (): void => {
		if (this.timer !== null) {
			clearTimeout(this.timer);
			this.timer = null;
		}
	};

	private attachListeners(): void {
		this.element.addEventListener('auxclick', this.handleAuxclick);
		this.element.addEventListener('contextmenu', this.handleContextMenu);
		this.element.addEventListener('click', this.handleClick);
		this.element.addEventListener('touchstart', this.handleTouchStart, { passive: true });
		this.element.addEventListener('touchend', this.handleTouchEnd);
		this.element.addEventListener('touchcancel', this.handleTouchEnd);
	}

	public detachListeners(): void {
		this.cancelLongPress();
		this.element.removeEventListener('auxclick', this.handleAuxclick);
		this.element.removeEventListener('contextmenu', this.handleContextMenu);
		this.element.removeEventListener('click', this.handleClick);
		this.element.removeEventListener('touchstart', this.handleTouchStart);
		this.element.removeEventListener('touchend', this.handleTouchEnd);
		this.element.removeEventListener('touchcancel', this.handleTouchEnd);
	}

}
const registeredMouseButtonListeners: Map<string, MouseButtonListener> = new Map<string, MouseButtonListener>();
export function addMouseButtonListener(
	selectorOrElement: string | HTMLElement,
	dotnetHelper: DotNetHelper,
	delay: number,
	triggerButton: MouseButton): void {

	const element = resolveElement(selectorOrElement);
	if (!element) {
		console.warn("addMouseButtonListener: Element not found");
		return;
	}

	if (!element.id) {
		console.warn("addMouseButtonListener: Element does not contain an ID.");
		return;
	}

	if (registeredMouseButtonListeners.has(element.id)) {
		console.warn(`addMouseButtonListener: An element with ID ${element.id} already registered`);
		return;
	}

	const listener = new MouseButtonListener(element, dotnetHelper, delay, triggerButton);
	registeredMouseButtonListeners.set(element.id, listener);

}
export function removeMouseButtonListener(selectorOrElement: string | HTMLElement): void {
	const element = resolveElement(selectorOrElement);
	if (!element) {
		console.warn("removeMouseButtonListener: Element not found");
		return;
	}

	if (!registeredMouseButtonListeners.has(element.id)) {
		//console.info(`removeMouseButtonListener: An element with ID ${element.id} not registered`);
		return;
	}

	const listener = registeredMouseButtonListeners.get(element.id);
	if (listener) {
		listener.detachListeners();
		registeredMouseButtonListeners.delete(element.id);
	}
}

//#endregion
