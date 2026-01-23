/*!
 * cirreum-interop.ts
 * Cirreum WebAssembly Interop Module
 * =====================================================================
 * Runtime JavaScript interop for Cirreum-based Blazor WebAssembly apps.
 * This module is loaded on-demand after the application starts.
 *
 * Dependencies:
 *  • window.cirreum namespace (provided by cirreum-wasm-loader.js)
 *  • Popper.js (loaded by cirreum-wasm-loader.js)
 *
 * Version:     @VERSION@
 * License:     MIT
 * Repository:  https://github.com/cirreum/Cirreum.Components.WebAssembly
 * Copyright:   2025 Cirreum Contributors
 * =====================================================================
 */

// =====================================================================
// Type Definitions
// =====================================================================

//#region Cirreum Namespace Types
interface CirreumTenantConfig {
	slug?: string;
	displayName?: string;
	authority?: string;
	clientId: string;
	responseType?: string;
	scopes?: string[];
	tenantId?: string;
	domain?: string;
	isEntraExternal?: boolean;
	authLibrary?: string;
}
interface CirreumNamespace {
	app: {
		getName(): string;
		getAssemblyName(): string | null;
	};
	auth: {
		getMode(): string;
		getLibrary(): string;
		isEnabled(): boolean;
	};
	tenant: {
		getConfig(): CirreumTenantConfig | null;
		getSlug(): string;
		getDisplayName(): string | null;
	};
	theme: {
		getCurrent(): string;
		getValidThemes(): string[];
	};
	assets: {
		loadCss(href: string, integrity?: string, title?: string, disabled?: boolean): void;
		loadJs(src: string, integrity?: string): void;
	};
}
declare global {
	interface Window {
		cirreum: CirreumNamespace;
	}
}
//#endregion

//#region Popper Types

interface PopperInstance {
	forceUpdate(): void;
	destroy(): void;
}

interface PopperStatic {
	createPopper(reference: HTMLElement, popper: HTMLElement, options?: object): PopperInstance;
}

declare const Popper: PopperStatic;

//#endregion

//#region Interop Types

interface DotNetObjectReference {
	invokeMethodAsync(methodName: string, ...args: unknown[]): Promise<void>;
}

interface InternationalFormatInfo {
	locale: string;
	calendar: string;
	numberingSystem: string;
	timeZone: string;
	timeZoneOffset: number;
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
}

interface FormattedDateSamples {
	defaultFormat: string;
	dateOnly: string;
	timeOnly: string;
	full: string;
	withTimeZone: string;
}

interface ScrollbarInfo {
	hasScrollbars: boolean;
	hasStandard: boolean;
	hasCustom: boolean;
	details: {
		standard: {
			hasVertical: boolean;
			hasHorizontal: boolean;
		};
		dimensions: {
			vertical: number;
			horizontal: number;
		};
	};
}

interface DomRect {
	Bottom: number;
	Height: number;
	Left: number;
	Right: number;
	Top: number;
	Width: number;
	X: number;
	Y: number;
}

interface Coordinates {
	X: number;
	Y: number;
}

interface ElementCoordinates {
	left: number;
	top: number;
}

interface ElementDimensions {
	width: number;
	height: number;
}

const enum MouseButton {
	None = -1,
	Left = 0,
	Middle = 1,
	Right = 2,
	Back = 3,
	Forward = 4
}

const enum MouseButtonEventType {
	ContextMenu = "contextmenu",
	Click = "click"
}

interface MouseButtonEventInfo {
	pageX: number;
	pageY: number;
	type: MouseButtonEventType;
	button: MouseButton;
}

//#endregion

// =====================================================================
// Internal Utilities
// =====================================================================

//#region Element Resolution

/**
 * Resolves a DOM element from a selector string or element reference.
 *
 * @param selectorOrElement - CSS selector, element ID, or HTMLElement
 * @returns The resolved HTMLElement or null if not found
 *
 * @remarks
 * Resolution strategy:
 * 1. If HTMLElement provided, returns it directly
 * 2. If string starts with '#', uses querySelector
 * 3. If string matches ID pattern, tries getElementById first, then querySelector
 * 4. Otherwise uses querySelector
 */
function resolveElement<T extends HTMLElement>(selectorOrElement: string | T): T | null {
	if (typeof selectorOrElement !== "string") {
		return selectorOrElement;
	}

	let element: Element | null;

	if (selectorOrElement.charAt(0) === "#") {
		element = document.querySelector(selectorOrElement);
	} else if (/^[a-zA-Z][\w-]*$/.test(selectorOrElement)) {
		element = document.getElementById(selectorOrElement) ?? document.querySelector(selectorOrElement);
	} else {
		element = document.querySelector(selectorOrElement);
	}

	return element instanceof HTMLElement ? (element as T) : null;
}

//#endregion

//#region Scrollbar Detection

/**
 * Detects scrollbar presence and dimensions in the document.
 */
function detectScrollbars(): ScrollbarInfo {
	function hasCustomScrollbars(): boolean {
		const style = document.createElement("style");
		style.textContent = "::-webkit-scrollbar { width: 0; height: 0; }";
		document.head.appendChild(style);
		const hasCustom = window.getComputedStyle(document.body).scrollbarWidth === "none";
		document.head.removeChild(style);
		return hasCustom;
	}

	const outer = document.createElement("div");
	const inner = document.createElement("div");
	outer.style.cssText = "visibility:hidden;overflow:scroll;position:absolute;top:-9999px";
	document.body.appendChild(outer);
	outer.appendChild(inner);

	const verticalWidth1 = window.innerWidth - document.documentElement.clientWidth;
	const verticalWidth2 = outer.offsetWidth - inner.offsetWidth;

	outer.style.overflowY = "hidden";
	const horizontalHeight1 = window.innerHeight - document.documentElement.clientHeight;
	const horizontalHeight2 = outer.offsetHeight - inner.offsetHeight;

	document.body.removeChild(outer);

	const verticalWidth = Math.min(verticalWidth1, verticalWidth2);
	const horizontalHeight = Math.min(horizontalHeight1, horizontalHeight2);
	const hasStandardVertical = verticalWidth > 1;
	const hasStandardHorizontal = horizontalHeight > 1;
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
 * Determines if a scrollbar is visible for an element in the specified direction.
 */
function isElementScrollbarVisible(
	element: Element,
	direction: "horizontal" | "vertical" = "horizontal"
): boolean {
	const style = window.getComputedStyle(element);
	const overflow = direction === "horizontal" ? style.overflowX : style.overflowY;
	const scrollSize = direction === "horizontal" ? element.scrollWidth : element.scrollHeight;
	const clientSize = direction === "horizontal" ? element.clientWidth : element.clientHeight;
	const isRootElement = element === document.documentElement || element === document.body;

	if (isRootElement) {
		const viewportSize =
			direction === "horizontal"
				? window.innerWidth || document.documentElement.clientWidth
				: window.innerHeight || document.documentElement.clientHeight;
		return scrollSize > viewportSize && overflow !== "hidden";
	}

	if (overflow === "hidden") {
		return false;
	}

	if (overflow === "visible") {
		let parent = element.parentElement;
		while (parent) {
			const parentStyle = window.getComputedStyle(parent);
			const parentOverflow = direction === "horizontal" ? parentStyle.overflowX : parentStyle.overflowY;
			if (parentOverflow !== "visible") {
				return scrollSize > clientSize;
			}
			parent = parent.parentElement;
		}
		const viewportSize =
			direction === "horizontal"
				? window.innerWidth || document.documentElement.clientWidth
				: window.innerHeight || document.documentElement.clientHeight;
		return scrollSize > viewportSize;
	}

	return scrollSize > clientSize;
}

//#endregion

// =====================================================================
// Exported Functions
// =====================================================================

//#region Browser Environment

/**
 * Gets the browser's internationalization format information.
 */
export function getInternationalFormats(): InternationalFormatInfo {
	const basicOptions = Intl.DateTimeFormat().resolvedOptions();
	const fullFormatter = new Intl.DateTimeFormat(undefined, {
		weekday: "long",
		era: "long",
		year: "numeric",
		month: "numeric",
		day: "numeric",
		hour: "numeric",
		minute: "numeric",
		second: "numeric",
		timeZoneName: "long"
	});
	const fullOptions = fullFormatter.resolvedOptions();

	return {
		locale: basicOptions.locale || "",
		calendar: basicOptions.calendar || "",
		numberingSystem: basicOptions.numberingSystem || "",
		timeZone: basicOptions.timeZone || "",
		timeZoneOffset: new Date().getTimezoneOffset(),
		hour12: fullOptions.hour12,
		weekday: fullOptions.weekday,
		era: fullOptions.era,
		year: fullOptions.year,
		month: fullOptions.month,
		day: fullOptions.day,
		hour: fullOptions.hour,
		minute: fullOptions.minute,
		second: fullOptions.second,
		timeZoneName: fullOptions.timeZoneName
	};
}

/**
 * Gets the current local time as a string.
 */
export function getCurrentLocalTime(): string {
	return new Date().toString();
}

/**
 * Gets the current UTC time as a string.
 */
export function getCurrentUtcTime(): string {
	return new Date().toUTCString();
}

/**
 * Determines if daylight saving time is currently in effect.
 */
export function isDaylightSavingTime(): string {
	const jan = new Date(new Date().getFullYear(), 0, 1);
	const jul = new Date(new Date().getFullYear(), 6, 1);
	const stdTimezoneOffset = Math.max(jan.getTimezoneOffset(), jul.getTimezoneOffset());
	return new Date().getTimezoneOffset() < stdTimezoneOffset ? "Yes" : "No";
}

/**
 * Gets samples of different date/time formats.
 */
export function getFormattedSamples(): FormattedDateSamples {
	const now = new Date();

	let supportsDateStyle = false;
	try {
		new Intl.DateTimeFormat("en", { dateStyle: "full" } as Intl.DateTimeFormatOptions).format(now);
		supportsDateStyle = true;
	} catch {
		supportsDateStyle = false;
	}

	let fullFormatted: string;
	if (supportsDateStyle) {
		try {
			fullFormatted = new Intl.DateTimeFormat(undefined, {
				dateStyle: "full",
				timeStyle: "long"
			} as Intl.DateTimeFormatOptions).format(now);
		} catch {
			fullFormatted = now.toLocaleString();
		}
	} else {
		try {
			fullFormatted = new Intl.DateTimeFormat(undefined, {
				year: "numeric",
				month: "long",
				day: "numeric",
				hour: "2-digit",
				minute: "2-digit",
				second: "2-digit"
			}).format(now);
		} catch {
			fullFormatted = now.toLocaleString();
		}
	}

	let timeZoneName: string;
	try {
		const formatter = new Intl.DateTimeFormat(undefined, { timeZoneName: "long" });
		const parts = formatter.formatToParts(now);
		const timeZonePart = parts.find((part) => part.type === "timeZoneName");
		timeZoneName = timeZonePart?.value ?? Intl.DateTimeFormat().resolvedOptions().timeZone;
	} catch {
		timeZoneName = Intl.DateTimeFormat().resolvedOptions().timeZone;
	}

	let dateTimeFormat: string;
	try {
		dateTimeFormat = new Intl.DateTimeFormat(undefined, {
			year: "numeric",
			month: "numeric",
			day: "numeric",
			hour: "numeric",
			minute: "2-digit",
			second: "2-digit"
		}).format(now);
	} catch {
		dateTimeFormat = now.toLocaleString();
	}

	return {
		defaultFormat: now.toLocaleString(),
		dateOnly: now.toLocaleDateString(),
		timeOnly: now.toLocaleTimeString(),
		full: fullFormatted,
		withTimeZone: `${dateTimeFormat} (${timeZoneName})`
	};
}

/**
 * Checks if the browser supports Intl.DateTimeFormat timeZone.
 */
export function hasTimeZoneSupport(): boolean {
	return (
		typeof Intl !== "undefined" &&
		"DateTimeFormat" in Intl &&
		typeof new Intl.DateTimeFormat().resolvedOptions().timeZone === "string"
	);
}

/**
 * Checks if the browser supports Date.getTimezoneOffset.
 */
export function hasOffsetSupport(): boolean {
	return typeof new Date().getTimezoneOffset === "function";
}

/**
 * Gets the browser's user agent string.
 */
export function getUserAgent(): string {
	return navigator.userAgent;
}

//#endregion

//#region Cirreum Namespace Accessors

/**
 * Gets the application name.
 */
export function getAppName(): string {
	return window.cirreum?.app?.getName() ?? window.location.hostname.toUpperCase();
}

/**
 * Gets the assembly name.
 */
export function getAssemblyName(): string | null {
	return window.cirreum?.app?.getAssemblyName() ?? null;
}

/**
 * Gets the current theme.
 */
export function getCurrentTheme(): string {
	return window.cirreum?.theme?.getCurrent() ?? "default";
}

/**
 * Gets valid theme names.
 */
export function getValidThemes(): string[] {
	return window.cirreum?.theme?.getValidThemes() ?? ["default"];
}

//#endregion

//#region Media Queries

/**
 * Checks if the app is running in standalone mode (PWA).
 */
export function isStandAlone(): boolean {
	return window.matchMedia("(display-mode: standalone)").matches;
}

/**
 * Gets the system's preferred color scheme.
 */
export function getSystemThemeMode(): string {
	return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
}

/**
 * AbortController for system theme mode monitoring.
 */
let themeModeAbortController: AbortController | null = null;

/**
 * Monitors system theme mode changes.
 */
export function monitorSystemThemeMode(dotnetRef: DotNetObjectReference): void {
	// Clean up any existing listener first
	themeModeAbortController?.abort();

	const darkThemeMq = window.matchMedia("(prefers-color-scheme: dark)");
	const controller = new AbortController();

	darkThemeMq.addEventListener("change", async (e) => {
		const storedTheme = localStorage.getItem("user-theme-mode");
		await dotnetRef.invokeMethodAsync("OnModeChanged", e.matches, storedTheme);
	}, { signal: controller.signal });

	themeModeAbortController = controller;
}

/**
 * Removes system theme mode monitor.
 */
export function removeSystemThemeModeMonitor(): void {
	themeModeAbortController?.abort();
	themeModeAbortController = null;
}

/**
 * Collection of registered Breakpoint Monitors.
 */
const breakpointAbortControllers = new Map<string, AbortController>();

/**
 * Monitors breakpoint changes.
 */
export function monitorBreakpointChanges(
	dotnetRef: DotNetObjectReference,
	minBreakpointSize: string
): void {
	const mq = window.matchMedia(`(min-width: ${minBreakpointSize})`);
	const controller = new AbortController();
	mq.addEventListener("change", async (e) => {
		await dotnetRef.invokeMethodAsync("OnBreakpointChange", e.matches);
	}, { signal: controller.signal });
	breakpointAbortControllers.set(minBreakpointSize, controller);
}

/**
 * Removes breakpoint monitor.
 */
export function removeBreakpointMonitor(minBreakpointSize: string): void {
	breakpointAbortControllers.get(minBreakpointSize)?.abort();
	breakpointAbortControllers.delete(minBreakpointSize);
}

/**
 * Checks if the viewport matches a minimum breakpoint.
 */
export function getCurrentBreakPoint(minBreakpoint: string): boolean {
	return window.matchMedia(`(min-width: ${minBreakpoint})`).matches;
}

//#endregion

//#region Head/Stylesheets

/**
 * Includes a stylesheet if not already present.
 */
export function includeStyleSheet(
	href: string,
	integrity?: string,
	title?: string,
	disabled?: boolean
): void {
	const existing = document.querySelector(`link[href="${href}"]`);
	if (!existing) {
		window.cirreum.assets.loadCss(href, integrity, title, disabled);
	}
}

/**
 * Replaces a stylesheet's href.
 */
export function replaceHeadLink(oldHref: string, newHref: string): void {
	const link = document.querySelector<HTMLLinkElement>(`link[href="${oldHref}"]`);
	if (link) {
		link.href = newHref;
	}
}

//#endregion

//#region Element Focus

const focusableElementsQuery = [
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
].join(",");

/**
 * Focuses the first focusable element within a container.
 */
export function focusFirstElement(parentElement: string | HTMLElement, preventScroll: boolean): void {
	const container = resolveElement(parentElement);
	if (!container) {
		return;
	}

	const focusableElements = container.querySelectorAll<HTMLElement>(focusableElementsQuery);
	if (focusableElements.length > 0) {
		focusableElements[0].focus({ preventScroll });
	}
}

/**
 * Focuses the last focusable element within a container.
 */
export function focusLastElement(parentElement: string | HTMLElement, preventScroll: boolean): void {
	const container = resolveElement(parentElement);
	if (!container) {
		return;
	}

	const focusableElements = container.querySelectorAll<HTMLElement>(focusableElementsQuery);
	if (focusableElements.length > 0) {
		focusableElements[focusableElements.length - 1].focus({ preventScroll });
	}
}

/**
 * Focuses a specific element.
 */
export function focusElement(element: string | HTMLElement, preventScroll: boolean): void {
	const el = resolveElement(element);
	el?.focus({ preventScroll });
}

/**
 * Focuses the next (or previous) focusable element.
 */
export function focusNextElement(
	reverse: boolean = false,
	element: string | null = null,
	preventScroll: boolean = false
): boolean {
	const activeElem = element ? resolveElement(element) ?? document.activeElement : document.activeElement;

	const queryResult = Array.from(document.querySelectorAll<HTMLElement>(focusableElementsQuery)).filter(
		(elem) => elem.offsetWidth > 0 || elem.offsetHeight > 0 || elem === activeElem
	);

	const indexedList = queryResult.filter((elem) => elem.tabIndex > 0).sort((a, b) => a.tabIndex - b.tabIndex);

	const focusable = indexedList.concat(queryResult.filter((elem) => elem.tabIndex === 0));
	const currentIndex = focusable.indexOf(activeElem as HTMLElement);

	const nextElement = reverse
		? focusable[currentIndex - 1] || focusable[focusable.length - 1]
		: focusable[currentIndex + 1] || focusable[0];

	if (nextElement) {
		nextElement.focus({ preventScroll });
		return true;
	}
	return false;
}

//#endregion

//#region Scrollbar Visibility

/**
 * Checks if a vertical scrollbar is visible.
 */
export function isVerticalScrollbarVisible(selector: string): boolean {
	const el = resolveElement(selector);
	return el ? isElementScrollbarVisible(el, "vertical") : false;
}

/**
 * Checks if a horizontal scrollbar is visible.
 */
export function isHorizontalScrollbarVisible(selector: string): boolean {
	const el = resolveElement(selector);
	return el ? isElementScrollbarVisible(el, "horizontal") : false;
}

/**
 * Adds CSS classes to an element if a scrollbar is visible.
 */
export function addElementClassIfScrollbar(
	selector: string,
	vertical: boolean,
	...tokens: string[]
): void {
	const el = resolveElement(selector);
	if (!el) {
		return;
	}

	const scrollbars = detectScrollbars();
	if (scrollbars.hasScrollbars) {
		document.documentElement.style.removeProperty("--scrollbar-width");
	} else {
		document.documentElement.style.setProperty("--scrollbar-width", "0");
	}

	const direction = vertical ? "vertical" : "horizontal";
	if (isElementScrollbarVisible(el, direction)) {
		el.classList.add(...tokens);
	}
}

//#endregion

//#region Element Queries

/**
 * Checks if an element is a child of another element matching a selector.
 */
export function isChildOf(element: Element, selector: string): boolean {
	return element.closest(selector) !== null;
}

/**
 * Sets the indeterminate state of a checkbox.
 */
export function setCheckBoxIndeterminate(element: string | HTMLInputElement, value: boolean): void {
	const el = resolveElement(element);
	if (el && "indeterminate" in el) {
		(el as HTMLInputElement).indeterminate = value;
	}
}

/**
 * Sets an element's attribute.
 */
export function setElementAttribute(element: string | HTMLElement, property: string, value: string): void {
	const el = resolveElement(element);
	el?.setAttribute(property, value);
}

/**
 * Sets an attribute on multiple elements.
 */
export function setElementsAttribute(selector: string, property: string, value: string): void {
	document.querySelectorAll(selector).forEach((el) => {
		el.setAttribute(property, value);
	});
}

/**
 * Gets an element's attribute.
 */
export function getElementAttribute(element: string | HTMLElement, property: string): string {
	const el = resolveElement(element);
	return el?.getAttribute(property) ?? "";
}

/**
 * Gets the innerText of an element.
 */
export function getElementText(element: string | HTMLElement, tryFirstChild: boolean): string {
	try {
		const el = resolveElement(element);
		if (!el) {
			console.warn(`getElementText: Element not found for "${element}"`);
			return "";
		}

		if (tryFirstChild && el.childNodes.length > 0) {
			for (const child of el.childNodes) {
				if (child instanceof HTMLElement) {
					const text = child.innerText.trim();
					if (text) {
						return text;
					}
					break;
				}
			}
		}

		return el.innerText.trim();
	} catch (error) {
		console.error(`Error in getElementText: ${error instanceof Error ? error.message : String(error)}`);
		return "";
	}
}

/**
 * Gets the textContent of an element.
 */
export function getElementTextContent(element: string | HTMLElement, tryFirstChild: boolean): string {
	try {
		const el = resolveElement(element);
		if (!el) {
			console.warn(`getElementTextContent: Element not found for "${element}"`);
			return "";
		}

		if (tryFirstChild && el.childNodes.length > 0) {
			for (const child of el.childNodes) {
				if (child instanceof HTMLElement) {
					const text = child.textContent?.trim() ?? "";
					if (text) {
						return text;
					}
					break;
				}
			}
		}

		return el.textContent?.trim() ?? "";
	} catch (error) {
		console.error(`Error in getElementTextContent: ${error instanceof Error ? error.message : String(error)}`);
		return "";
	}
}

/**
 * Gets an element's ID.
 */
export function getElementId(element: HTMLElement): string {
	return element?.id ?? "";
}

/**
 * Checks if a parent element contains a CSS class.
 */
export function parentContainsClass(child: string | HTMLElement, token: string): boolean {
	const el = resolveElement(child);
	return el?.parentElement?.classList.contains(token) ?? false;
}

/**
 * Checks if an element contains a CSS class.
 */
export function elementContainsClass(element: string | HTMLElement, token: string): boolean {
	const el = resolveElement(element);
	return el?.classList.contains(token) ?? false;
}

/**
 * Adds CSS classes to an element.
 */
export function addElementClasses(element: string | HTMLElement, classString: string): void {
	const el = resolveElement(element);
	if (el && classString) {
		const classes = classString.split(" ").filter(Boolean);
		el.classList.add(...classes);
	}
}

/**
 * Removes CSS classes from an element.
 */
export function removeElementClasses(element: string | HTMLElement, ...tokens: string[]): void {
	const el = resolveElement(element);
	if (el && tokens.length > 0) {
		el.classList.remove(...tokens);
	}
}

/**
 * Toggles a CSS class on an element.
 */
export function toggleElementClass(
	element: string | HTMLElement,
	token: string,
	force?: boolean
): boolean {
	const el = resolveElement(element);
	return el?.classList.toggle(token, force) ?? false;
}

/**
 * Swaps one CSS class for another.
 */
export function swapElementClass(
	element: string | HTMLElement,
	token: string,
	newToken: string
): boolean {
	const el = resolveElement(element);
	return el?.classList.replace(token, newToken) ?? false;
}

/**
 * Sets an element's class attribute.
 */
export function setElementClass(element: string | HTMLElement, value: string): void {
	const el = resolveElement(element);
	el?.setAttribute("class", value);
}

/**
 * Sets an element's style property.
 */
export function setElementStyleProperty(element: string | HTMLElement, property: string, value: string): void {
	const el = resolveElement(element);
	el?.style.setProperty(property, value);
}

/**
 * Scrolls an element into view.
 */
export function scrollIntoView(element: string | HTMLElement, options: ScrollIntoViewOptions): void {
	const el = resolveElement(element);
	el?.scrollIntoView(options);
}

//#endregion

//#region Element Dimensions

/**
 * Sets an element's height.
 */
export function setElementHeight(element: string | HTMLElement, height: string): void {
	const el = resolveElement(element);
	if (el) {
		el.style.height = height;
	}
}

/**
 * Sets an element's max-height.
 */
export function setElementMaxHeight(element: string | HTMLElement, height: string): void {
	const el = resolveElement(element);
	if (el) {
		el.style.maxHeight = height;
	}
}

/**
 * Sets an element's max-height from its scrollHeight.
 */
export function setElementMaxHeightFromScrollHeight(element: string | HTMLElement): void {
	const el = resolveElement(element);
	if (el) {
		el.style.maxHeight = `${el.scrollHeight}px`;
	}
}

/**
 * Gets an element's scrollHeight.
 */
export function getElementScrollHeight(element: string | HTMLElement): number {
	const el = resolveElement(element);
	return el?.scrollHeight ?? -9999;
}

/**
 * Gets an element's scrollWidth.
 */
export function getElementScrollWidth(element: string | HTMLElement): number {
	const el = resolveElement(element);
	return el?.scrollWidth ?? -9999;
}

/**
 * Gets an element's scrollLeft.
 */
export function getElementScrollLeft(element: string | HTMLElement): number {
	const el = resolveElement(element);
	return el?.scrollLeft ?? -9999;
}

/**
 * Sets an element's scrollLeft.
 */
export function setElementScrollLeft(element: string | HTMLElement, value: number): void {
	const el = resolveElement(element);
	if (el) {
		el.scrollLeft = value;
	}
}

/**
 * Gets an element's scrollTop.
 */
export function getElementScrollTop(element: string | HTMLElement): number {
	const el = resolveElement(element);
	return el?.scrollTop ?? -9999;
}

/**
 * Sets an element's scrollTop.
 */
export function setElementScrollTop(element: string | HTMLElement, value: number): void {
	const el = resolveElement(element);
	if (el) {
		el.scrollTop = value;
	}
}

/**
 * Gets an element's bounding client rect.
 */
export function getBoundingClientRect(element: string | HTMLElement): DomRect | null {
	const el = resolveElement(element);
	if (!el) {
		return null;
	}

	const rect = el.getBoundingClientRect();
	return {
		Bottom: rect.bottom,
		Height: rect.height,
		Left: rect.left,
		Right: rect.right,
		Top: rect.top,
		Width: rect.width,
		X: rect.x,
		Y: rect.y
	};
}

/**
 * Gets an accurate bounding rect accounting for transforms.
 */
export function getAccurateBoundingClientRect(element: string | HTMLElement): DomRect | null {
	const el = resolveElement(element);
	if (!el) {
		return null;
	}

	const rect = el.getBoundingClientRect();
	const computedStyle = window.getComputedStyle(el);
	const width = el.offsetWidth;
	const height = el.offsetHeight;

	let offsetX = 0;
	let offsetY = 0;
	const transform = computedStyle.transform;
	if (transform && transform !== "none") {
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
	};
}

/**
 * Gets element coordinates accounting for scroll position.
 */
export function getElementCoordinates(element: string | HTMLElement): ElementCoordinates {
	const el = resolveElement(element);
	if (!el) {
		return { left: window.scrollX, top: window.scrollY };
	}

	const rect = el.getBoundingClientRect();
	return {
		left: rect.left + window.scrollX,
		top: rect.top + window.scrollY
	};
}

/**
 * Gets element dimensions using scroll values.
 */
export function getElementDimensions(element: string | HTMLElement): ElementDimensions {
	const el = resolveElement(element);
	return {
		width: el?.scrollWidth ?? 0,
		height: el?.scrollHeight ?? 0
	};
}

/**
 * Gets viewport dimensions.
 */
export function getViewPortDimensions(): ElementDimensions {
	return {
		width: window.innerWidth || document.documentElement.clientWidth,
		height: window.innerHeight || document.documentElement.clientHeight
	};
}

/**
 * Gets the current scroll position.
 */
export function getScrollPosition(): Coordinates {
	let totalX = window.scrollX;
	let totalY = window.scrollY;

	let currentElement = document.activeElement as HTMLElement | null;
	while (currentElement && currentElement !== document.body) {
		if (currentElement.scrollHeight > currentElement.clientHeight) {
			totalX += currentElement.scrollLeft;
			totalY += currentElement.scrollTop;
		}
		currentElement = currentElement.parentElement;
	}

	return { X: totalX, Y: totalY };
}

/**
 * Gets the footer's height.
 */
export function getFooterHeight(): number {
	const footer = document.querySelector("footer") ?? document.querySelector(".footer");
	if (!footer) {
		return 0;
	}

	const styles = window.getComputedStyle(footer);
	return (
		(footer as HTMLElement).offsetHeight +
		parseFloat(styles.marginTop) +
		parseFloat(styles.marginBottom)
	);
}

//#endregion

//#region Popper.js

const registeredPoppers = new Map<string, PopperInstance>();

/**
 * Creates and shows a Popper instance.
 */
export function showPopper(id: string, host: HTMLElement, popper: HTMLElement, config: object): void {
	if (registeredPoppers.has(id)) {
		console.warn(`showPopper: Duplicate popper id "${id}"`);
		return;
	}

	try {
		const instance = Popper.createPopper(host, popper, config);
		registeredPoppers.set(id, instance);
	} catch (e) {
		console.error(`showPopper error: ${e instanceof Error ? e.message : String(e)}`);
	}
}

/**
 * Forces a Popper instance to update.
 */
export function updatePopper(id: string): void {
	const instance = registeredPoppers.get(id);
	if (instance) {
		try {
			instance.forceUpdate();
		} catch (e) {
			console.error(`updatePopper error: ${e instanceof Error ? e.message : String(e)}`);
		}
	}
}

/**
 * Destroys a Popper instance.
 */
export function destroyPopper(id: string): void {
	const instance = registeredPoppers.get(id);
	if (instance) {
		try {
			instance.destroy();
			registeredPoppers.delete(id);
		} catch (e) {
			console.error(`destroyPopper error: ${e instanceof Error ? e.message : String(e)}`);
		}
	} else {
		console.warn(`destroyPopper: Popper id "${id}" not found`);
	}
}

//#endregion

//#region Mouse Button Listener

class MouseButtonListener {
	private timer: number | null = null;

	constructor(
		private element: HTMLElement,
		private dotnetHelper: DotNetObjectReference,
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
			button
		};
	}

	private handleClick = (e: MouseEvent): void => {
		e.preventDefault();
		if (e.button === this.triggerButton) {
			e.stopPropagation();
			const eventInfo = this.getEventInfo(e.button, e, MouseButtonEventType.Click);
			this.dotnetHelper.invokeMethodAsync("OnEventCallback", eventInfo);
		}
	};

	private handleContextMenu = (e: MouseEvent): void => {
		e.preventDefault();
		if (this.triggerButton === MouseButton.Right) {
			e.stopPropagation();
			const eventInfo = this.getEventInfo(e.button, e, MouseButtonEventType.ContextMenu);
			this.dotnetHelper.invokeMethodAsync("OnEventCallback", eventInfo);
		}
	};

	private handleAuxclick = (e: MouseEvent): void => {
		e.preventDefault();
	};

	private handleTouchStart = (e: TouchEvent): void => {
		if (this.triggerButton === MouseButton.Right) {
			this.startLongPress(e.touches[0]);
		} else {
			const eventInfo = this.getEventInfo(this.triggerButton, e.touches[0], MouseButtonEventType.Click);
			this.dotnetHelper.invokeMethodAsync("OnEventCallback", eventInfo);
		}
	};

	private handleTouchEnd = (e: TouchEvent): void => {
		e.preventDefault();
		this.cancelLongPress();
	};

	private startLongPress(e: Touch): void {
		this.timer = window.setTimeout(() => {
			const eventInfo = this.getEventInfo(MouseButton.None, e, MouseButtonEventType.ContextMenu);
			this.dotnetHelper.invokeMethodAsync("OnEventCallback", eventInfo);
		}, this.delay);
	}

	private cancelLongPress(): void {
		if (this.timer !== null) {
			clearTimeout(this.timer);
			this.timer = null;
		}
	}

	private attachListeners(): void {
		this.element.addEventListener("auxclick", this.handleAuxclick);
		this.element.addEventListener("contextmenu", this.handleContextMenu);
		this.element.addEventListener("click", this.handleClick);
		this.element.addEventListener("touchstart", this.handleTouchStart, { passive: true });
		this.element.addEventListener("touchend", this.handleTouchEnd);
		this.element.addEventListener("touchcancel", this.handleTouchEnd);
	}

	public detachListeners(): void {
		this.cancelLongPress();
		this.element.removeEventListener("auxclick", this.handleAuxclick);
		this.element.removeEventListener("contextmenu", this.handleContextMenu);
		this.element.removeEventListener("click", this.handleClick);
		this.element.removeEventListener("touchstart", this.handleTouchStart);
		this.element.removeEventListener("touchend", this.handleTouchEnd);
		this.element.removeEventListener("touchcancel", this.handleTouchEnd);
	}
}

const registeredMouseButtonListeners = new Map<string, MouseButtonListener>();

/**
 * Adds a mouse button listener to an element.
 */
export function addMouseButtonListener(
	selectorOrElement: string | HTMLElement,
	dotnetHelper: DotNetObjectReference,
	delay: number,
	triggerButton: MouseButton
): void {
	const element = resolveElement(selectorOrElement);
	if (!element) {
		console.warn("addMouseButtonListener: Element not found");
		return;
	}

	if (!element.id) {
		console.warn("addMouseButtonListener: Element does not have an ID");
		return;
	}

	if (registeredMouseButtonListeners.has(element.id)) {
		console.warn(`addMouseButtonListener: Element "${element.id}" already registered`);
		return;
	}

	const listener = new MouseButtonListener(element, dotnetHelper, delay, triggerButton);
	registeredMouseButtonListeners.set(element.id, listener);
}

/**
 * Removes a mouse button listener from an element.
 */
export function removeMouseButtonListener(selectorOrElement: string | HTMLElement): void {
	const element = resolveElement(selectorOrElement);
	if (!element) {
		console.warn("removeMouseButtonListener: Element not found");
		return;
	}

	const listener = registeredMouseButtonListeners.get(element.id);
	if (listener) {
		listener.detachListeners();
		registeredMouseButtonListeners.delete(element.id);
	}
}

//#endregion