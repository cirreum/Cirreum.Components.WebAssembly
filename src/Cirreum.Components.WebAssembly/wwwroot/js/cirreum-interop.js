var MouseButton;
(function (MouseButton) {
    MouseButton[MouseButton["None"] = -1] = "None";
    MouseButton[MouseButton["Left"] = 0] = "Left";
    MouseButton[MouseButton["Middle"] = 1] = "Middle";
    MouseButton[MouseButton["Right"] = 2] = "Right";
    MouseButton[MouseButton["Back"] = 3] = "Back";
    MouseButton[MouseButton["Forward"] = 4] = "Forward";
})(MouseButton || (MouseButton = {}));
function resolveElement(selectorOrElement) {
    if (typeof selectorOrElement === 'string') {
        let element;
        if (selectorOrElement.charAt(0) === '#') {
            element = document.querySelector(selectorOrElement);
        }
        else if (/^[a-zA-Z][\w-]*$/.test(selectorOrElement)) {
            element = document.getElementById(selectorOrElement);
            if (!element) {
                element = document.querySelector(selectorOrElement);
            }
        }
        else {
            element = document.querySelector(selectorOrElement);
        }
        return (element instanceof HTMLElement) ? element : null;
    }
    return selectorOrElement;
}
function detectScrollbars() {
    function hasCustomScrollbars() {
        const style = document.createElement('style');
        style.textContent = '::-webkit-scrollbar { width: 0; height: 0; }';
        document.head.appendChild(style);
        const hasCustom = window.getComputedStyle(document.body).scrollbarWidth === 'none';
        document.head.removeChild(style);
        return hasCustom;
    }
    const outer = document.createElement('div');
    const inner = document.createElement('div');
    outer.style.visibility = 'hidden';
    outer.style.overflow = 'scroll';
    outer.style.position = 'absolute';
    outer.style.top = '-9999px';
    document.body.appendChild(outer);
    outer.appendChild(inner);
    const verticalWidth1 = window.innerWidth - document.documentElement.clientWidth;
    const verticalWidth2 = outer.offsetWidth - inner.offsetWidth;
    outer.style.overflowY = 'hidden';
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
function isElementScrollbarVisible(element, direction = 'horizontal') {
    const style = window.getComputedStyle(element);
    const overflow = direction === 'horizontal' ? style.overflowX : style.overflowY;
    const scrollSize = direction === 'horizontal' ? element.scrollWidth : element.scrollHeight;
    const clientSize = direction === 'horizontal' ? element.clientWidth : element.clientHeight;
    const isRootElement = element === document.documentElement || element === document.body;
    if (isRootElement) {
        const viewportSize = direction === 'horizontal' ?
            window.innerWidth || document.documentElement.clientWidth :
            window.innerHeight || document.documentElement.clientHeight;
        return scrollSize > viewportSize && overflow !== 'hidden';
    }
    if (overflow === 'hidden') {
        return false;
    }
    if (overflow === 'visible') {
        let parent = element.parentElement;
        while (parent) {
            const parentStyle = window.getComputedStyle(parent);
            const parentOverflow = direction === 'horizontal' ? parentStyle.overflowX : parentStyle.overflowY;
            if (parentOverflow !== 'visible') {
                return scrollSize > clientSize;
            }
            parent = parent.parentElement;
        }
        const viewportSize = direction === 'horizontal' ?
            window.innerWidth || document.documentElement.clientWidth :
            window.innerHeight || document.documentElement.clientHeight;
        return scrollSize > viewportSize;
    }
    return scrollSize > clientSize;
}
export function getInternationalFormats() {
    const basicOptions = Intl.DateTimeFormat().resolvedOptions();
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
    const fullOptions = fullFormatter.resolvedOptions();
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
export function getCurrentLocalTime() {
    return new Date().toString();
}
export function getCurrentUtcTime() {
    return new Date().toUTCString();
}
export function isDaylightSavingTime() {
    const jan = new Date(new Date().getFullYear(), 0, 1);
    const jul = new Date(new Date().getFullYear(), 6, 1);
    const stdTimezoneOffset = Math.max(jan.getTimezoneOffset(), jul.getTimezoneOffset());
    return new Date().getTimezoneOffset() < stdTimezoneOffset ? 'Yes' : 'No';
}
export function getFormattedSamples() {
    const now = new Date();
    let supportsNewOptions = false;
    try {
        new Intl.DateTimeFormat('en', { dateStyle: 'full' }).format(now);
        supportsNewOptions = true;
    }
    catch (e) {
        supportsNewOptions = false;
    }
    let fullFormatted;
    if (supportsNewOptions) {
        try {
            fullFormatted = new Intl.DateTimeFormat(undefined, {
                dateStyle: 'full',
                timeStyle: 'long'
            }).format(now);
        }
        catch (e) {
            fullFormatted = now.toLocaleString();
        }
    }
    else {
        try {
            fullFormatted = new Intl.DateTimeFormat(undefined, {
                year: 'numeric',
                month: 'long',
                day: 'numeric',
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit'
            }).format(now);
        }
        catch (e) {
            fullFormatted = now.toLocaleString();
        }
    }
    let withTimeZoneFormatted;
    let timeZoneName;
    try {
        const formatter = new Intl.DateTimeFormat(undefined, { timeZoneName: 'long' });
        const parts = formatter.formatToParts(now);
        const timeZonePart = parts.find(part => part.type === 'timeZoneName');
        timeZoneName = timeZonePart ? timeZonePart.value :
            Intl.DateTimeFormat().resolvedOptions().timeZone;
    }
    catch (e) {
        timeZoneName = Intl.DateTimeFormat().resolvedOptions().timeZone;
    }
    let dateTimeFormat;
    try {
        dateTimeFormat = new Intl.DateTimeFormat(undefined, {
            year: 'numeric',
            month: 'numeric',
            day: 'numeric',
            hour: 'numeric',
            minute: '2-digit',
            second: '2-digit'
        }).format(now);
    }
    catch (e) {
        dateTimeFormat = now.toLocaleString();
    }
    const withTimeZone = `${dateTimeFormat} (${timeZoneName})`;
    return {
        defaultFormat: now.toLocaleString(),
        dateOnly: now.toLocaleDateString(),
        timeOnly: now.toLocaleTimeString(),
        full: fullFormatted,
        withTimeZone: withTimeZone
    };
}
export function hasTimeZoneSupport() {
    return typeof Intl !== 'undefined' &&
        'DateTimeFormat' in Intl &&
        typeof new Intl.DateTimeFormat().resolvedOptions().timeZone === 'string';
}
export function hasOffsetSupport() {
    return typeof new Date().getTimezoneOffset === 'function';
}
export function getUserAgent() {
    return navigator.userAgent;
}
export function getAuthInfo() {
    return window.authConfig;
}
if (typeof window.loadCss !== "function") {
    window.loadCss = (href, integrity, title, disabled) => {
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
            console.error('Error occurred while loading stylesheet', href);
        };
        linkCss.href = href;
        document.head.appendChild(linkCss);
    };
}
export function isStandAlone() {
    return window.matchMedia('(display-mode: standalone)').matches;
}
export function getSystemTheme() {
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}
export function monitorThemeChanges(dotnetRef) {
    const darkThemeMq = window.matchMedia("(prefers-color-scheme: dark)");
    darkThemeMq.addEventListener("change", async (e) => {
        const storedTheme = localStorage.getItem("theme");
        await dotnetRef.invokeMethodAsync("OnThemeChange", e.matches, storedTheme);
    });
}
export function getCurrentBreakPoint(minBreakpoint) {
    return window.matchMedia(`(min-width: ${minBreakpoint})`).matches;
}
export function monitorBreakpointChanges(dotnetRef, minBreakpointSize) {
    const breakpointMq = window.matchMedia(`(min-width: ${minBreakpointSize})`);
    breakpointMq.addEventListener("change", async (e) => {
        await dotnetRef.invokeMethodAsync("OnBreakpointChange", e.matches);
    });
}
export function includeStyleSheet(href, integrity, title, disabled) {
    var elink = document.documentElement.querySelector(`link[href="${href}"]`);
    if (elink === null) {
        window.loadCss(href, integrity, title, disabled);
    }
}
export function replaceHeadLink(oldHref, newHref) {
    var elink = document.documentElement.querySelector(`link[href="${oldHref}"]`);
    if (elink !== null) {
        elink.href = newHref;
        return;
    }
}
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
].join(',');
export function focusFirstElement(parentElement, preventScroll) {
    if (parentElement) {
        const container = resolveElement(parentElement);
        if (container) {
            const focusableElements = container.querySelectorAll(focusableElementsQuery);
            if (focusableElements.length === 0) {
                return;
            }
            const firstElement = focusableElements[0];
            firstElement.focus({ preventScroll });
        }
    }
}
export function focusLastElement(parentElement, preventScroll) {
    if (parentElement) {
        const container = resolveElement(parentElement);
        if (container) {
            const focusableElements = container.querySelectorAll(focusableElementsQuery);
            if (focusableElements.length === 0) {
                return;
            }
            if (focusableElements.length === 1) {
                focusableElements[0].focus({ preventScroll });
                return;
            }
            focusableElements[focusableElements.length - 1].focus({ preventScroll });
        }
    }
}
export function focusElement(element, preventScroll) {
    const ele = resolveElement(element);
    if (ele) {
        ele.focus({ preventScroll });
    }
}
export function focusNextElement(reverse = false, element = null, preventScroll) {
    let activeElem = element
        ? resolveElement(element) ?? document.activeElement
        : document.activeElement;
    function innerFocusNextElement(reverse, activeElem) {
        let queryResult = Array.from(document.querySelectorAll(focusableElementsQuery))
            .filter(elem => {
            if (elem instanceof HTMLElement) {
                return elem.offsetWidth > 0 || elem.offsetHeight > 0 || elem === activeElem;
            }
            return false;
        });
        const indexedList = queryResult
            .filter(elem => elem instanceof HTMLElement && elem.tabIndex > 0)
            .sort((a, b) => a.tabIndex - b.tabIndex);
        const focusable = indexedList.concat(queryResult.filter(elem => elem instanceof HTMLElement && elem.tabIndex === 0));
        const currentIndex = focusable.indexOf(activeElem);
        const nextElement = reverse
            ? (focusable[currentIndex - 1] || focusable[focusable.length - 1])
            : (focusable[currentIndex + 1] || focusable[0]);
        if (nextElement instanceof HTMLElement) {
            nextElement.focus({ preventScroll });
            return true;
        }
        return false;
    }
    if (!innerFocusNextElement(reverse, activeElem)) {
        return false;
    }
    return true;
}
export function isVerticalScrollbarVisible(selector) {
    const ele = resolveElement(selector);
    if (ele) {
        return isElementScrollbarVisible(ele, "vertical");
    }
    return false;
}
export function isHorizontalScrollbarVisible(selector) {
    const ele = resolveElement(selector);
    if (ele) {
        return isElementScrollbarVisible(ele, "horizontal");
    }
    return false;
}
export function addElementClassIfScrollbar(selector, vertical, ...tokens) {
    const ele = resolveElement(selector);
    if (ele) {
        const scrollbars = detectScrollbars();
        if (scrollbars.hasScrollbars) {
            document.documentElement.style.removeProperty('--scrollbar-width');
        }
        else {
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
export function isChildOf(element, selector) {
    return element.closest(selector) !== null;
}
export function setCheckBoxIndeterminate(element, value) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            localEl.indeterminate = value;
        }
    }
}
export function setElementAttribute(element, property, value) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            localEl.setAttribute(property, value);
        }
    }
}
export function setElementsAttribute(selector, property, value) {
    if (selector) {
        document.documentElement.querySelectorAll(selector).forEach((ele) => {
            ele.setAttribute(property, value);
        });
    }
}
export function getElementAttribute(element, property) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            return localEl.getAttribute(property) ?? "";
        }
    }
    return "";
}
export function getElementText(element, tryFirstChild) {
    try {
        const localEl = resolveElement(element);
        if (!localEl) {
            console.warn(`getElementText: Element not found for selector "${element}"`);
            return "";
        }
        if (tryFirstChild && localEl.childNodes.length > 0) {
            for (const child of localEl.childNodes) {
                if (child instanceof HTMLElement) {
                    const text = child.innerText.trim();
                    if (text)
                        return text;
                    break;
                }
            }
        }
        return localEl.innerText.trim();
    }
    catch (error) {
        console.error(`Error in getElementText: ${error instanceof Error ? error.message : String(error)}`);
        return "";
    }
}
export function getElementTextContent(element, tryFirstChild) {
    try {
        const localEl = resolveElement(element);
        if (!localEl) {
            console.warn(`getElementTextContent: Element not found for selector "${element}"`);
            return "";
        }
        if (tryFirstChild && localEl.childNodes.length > 0) {
            for (const child of localEl.childNodes) {
                if (child instanceof HTMLElement) {
                    const text = child.textContent?.trim() ?? "";
                    if (text)
                        return text;
                    break;
                }
            }
        }
        return localEl.textContent?.trim() ?? "";
    }
    catch (error) {
        console.error(`Error in getElementTextContent: ${error instanceof Error ? error.message : String(error)}`);
        return "";
    }
}
export function getElementId(element) {
    if (element) {
        return element.id;
    }
    return "";
}
export function parentContainsClass(child, token) {
    if (child) {
        const localEl = resolveElement(child);
        if (localEl) {
            return localEl.parentElement?.classList.contains(token) ?? false;
        }
    }
    return false;
}
export function elementContainsClass(element, token) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            return localEl.classList.contains(token);
        }
    }
    return false;
}
export function addElementClasses(element, classString) {
    const el = resolveElement(element);
    if (el && classString) {
        const classes = classString.split(' ');
        el.classList.add(...classes);
    }
}
export function removeElementClasses(element, ...tokens) {
    if (tokens) {
        const localEl = resolveElement(element);
        if (localEl) {
            localEl.classList.remove(...tokens);
        }
    }
}
export function toggleElementClass(element, token, force) {
    if (element && token) {
        const localEl = resolveElement(element);
        if (localEl) {
            return localEl.classList.toggle(token, force);
        }
    }
    return false;
}
export function swapElementClass(element, token, newToken) {
    if (element && token && newToken) {
        const localEl = resolveElement(element);
        if (localEl) {
            return localEl.classList.replace(token, newToken);
        }
    }
    return false;
}
export function setElementClass(element, value) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            localEl.setAttribute("class", value);
        }
    }
}
export function setElementStyleProperty(element, property, value) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            localEl.style.setProperty(property, value);
        }
    }
}
export function scrollIntoView(element, options) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            localEl.scrollIntoView(options);
        }
    }
}
export function setElementHeight(element, height) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            localEl.style["height"] = height;
        }
    }
}
export function setElementMaxHeight(element, height) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            localEl.style["maxHeight"] = height;
        }
    }
}
export function setElementMaxHeightFromScrollHeight(element) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            localEl.style["maxHeight"] = `${localEl.scrollHeight}px`;
        }
    }
}
export function getElementScrollHeight(element) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            return localEl.scrollHeight | 0;
        }
    }
    return -9999;
}
export function getElementScrollWidth(element) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            return localEl.scrollWidth | 0;
        }
    }
    return -9999;
}
export function setElementScrollLeft(element, value) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            localEl.scrollLeft = value;
        }
    }
}
export function getElementScrollLeft(element) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            return localEl.scrollLeft | 0;
        }
    }
    return -9999;
}
export function setElementScrollTop(element, value) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            localEl.scrollTop = value;
        }
    }
}
export function getElementScrollTop(element) {
    let result = -9999;
    if (element) {
        const localEl = resolveElement(element);
        if (localEl) {
            result = localEl.scrollTop | 0;
        }
    }
    return result;
}
export function getBoundingClientRect(element) {
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
            };
            return result;
        }
    }
    return null;
}
export function getAccurateBoundingClientRect(element) {
    if (element) {
        const localEl = resolveElement(element);
        if (localEl && typeof localEl.getBoundingClientRect === "function") {
            const rect = localEl.getBoundingClientRect();
            const computedStyle = window.getComputedStyle(localEl);
            const width = localEl.offsetWidth;
            const height = localEl.offsetHeight;
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
            };
        }
    }
    return null;
}
export function getElementCoordinates(element) {
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
export function getElementDimensions(element) {
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
export function getViewPortDimensions() {
    return {
        width: window.innerWidth || document.documentElement.clientWidth,
        height: window.innerHeight || document.documentElement.clientHeight
    };
}
export function getScrollPosition() {
    const x = window.scrollX;
    const y = window.scrollY;
    let totalX = x;
    let totalY = y;
    let currentElement = document.activeElement;
    while (currentElement && currentElement !== document.body) {
        if (currentElement.scrollHeight > currentElement.clientHeight) {
            totalX += currentElement.scrollLeft;
            totalY += currentElement.scrollTop;
        }
        currentElement = currentElement.parentElement;
    }
    return { X: totalX, Y: totalY };
}
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
const registeredPoppers = new Map();
export function destroyPopper(id) {
    const instance = registeredPoppers.get(id);
    if (instance) {
        try {
            instance.destroy();
            registeredPoppers.delete(id);
        }
        catch (e) {
            console.error(e.message);
        }
    }
    else {
        console.warn("popper id " + id + " not found");
    }
}
export function updatePopper(id) {
    const instance = registeredPoppers.get(id);
    if (instance) {
        try {
            instance.forceUpdate();
        }
        catch (e) {
            console.error(e.message);
        }
    }
}
export function showPopper(id, host, popper, config) {
    if (registeredPoppers.has(id)) {
        console.warn("showPopper: cannot register a duplicate popper id.");
        return;
    }
    try {
        const instance = Popper.createPopper(host, popper, config);
        registeredPoppers.set(id, instance);
    }
    catch (e) {
        console.error(e.message);
    }
}
var MouseButtonEventType;
(function (MouseButtonEventType) {
    MouseButtonEventType["ContextMenu"] = "contextmenu";
    MouseButtonEventType["Click"] = "click";
})(MouseButtonEventType || (MouseButtonEventType = {}));
class MouseButtonListener {
    element;
    dotnetHelper;
    delay;
    triggerButton;
    timer = null;
    constructor(element, dotnetHelper, delay, triggerButton) {
        this.element = element;
        this.dotnetHelper = dotnetHelper;
        this.delay = delay;
        this.triggerButton = triggerButton;
        this.attachListeners();
    }
    getEventInfo(button, e, type) {
        return {
            pageX: e.pageX,
            pageY: e.pageY,
            type,
            button: button
        };
    }
    handleClick = (e) => {
        e.preventDefault();
        if (e.button === this.triggerButton) {
            e.stopPropagation();
            const eventInfo = this.getEventInfo(e.button, e, MouseButtonEventType.Click);
            this.dotnetHelper.invokeMethodAsync('OnEventCallback', eventInfo);
        }
    };
    handleContextMenu = (e) => {
        e.preventDefault();
        if (this.triggerButton === MouseButton.Right) {
            e.stopPropagation();
            const eventInfo = this.getEventInfo(e.button, e, MouseButtonEventType.ContextMenu);
            this.dotnetHelper.invokeMethodAsync('OnEventCallback', eventInfo);
        }
    };
    handleAuxclick = (e) => {
        e.preventDefault();
    };
    handleTouchStart = (e) => {
        if (this.triggerButton === MouseButton.Right) {
            this.startLongPress(e.touches[0]);
        }
        else {
            const eventInfo = this.getEventInfo(this.triggerButton, e.touches[0], MouseButtonEventType.Click);
            this.dotnetHelper.invokeMethodAsync('OnEventCallback', eventInfo);
        }
    };
    handleTouchEnd = (e) => {
        e.preventDefault();
        this.cancelLongPress();
    };
    startLongPress = (e) => {
        this.timer = window.setTimeout(() => {
            const eventInfo = this.getEventInfo(MouseButton.None, e, MouseButtonEventType.ContextMenu);
            this.dotnetHelper.invokeMethodAsync('OnEventCallback', eventInfo);
        }, this.delay);
    };
    cancelLongPress = () => {
        if (this.timer !== null) {
            clearTimeout(this.timer);
            this.timer = null;
        }
    };
    attachListeners() {
        this.element.addEventListener('auxclick', this.handleAuxclick);
        this.element.addEventListener('contextmenu', this.handleContextMenu);
        this.element.addEventListener('click', this.handleClick);
        this.element.addEventListener('touchstart', this.handleTouchStart, { passive: true });
        this.element.addEventListener('touchend', this.handleTouchEnd);
        this.element.addEventListener('touchcancel', this.handleTouchEnd);
    }
    detachListeners() {
        this.cancelLongPress();
        this.element.removeEventListener('auxclick', this.handleAuxclick);
        this.element.removeEventListener('contextmenu', this.handleContextMenu);
        this.element.removeEventListener('click', this.handleClick);
        this.element.removeEventListener('touchstart', this.handleTouchStart);
        this.element.removeEventListener('touchend', this.handleTouchEnd);
        this.element.removeEventListener('touchcancel', this.handleTouchEnd);
    }
}
const registeredMouseButtonListeners = new Map();
export function addMouseButtonListener(selectorOrElement, dotnetHelper, delay, triggerButton) {
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
export function removeMouseButtonListener(selectorOrElement) {
    const element = resolveElement(selectorOrElement);
    if (!element) {
        console.warn("removeMouseButtonListener: Element not found");
        return;
    }
    if (!registeredMouseButtonListeners.has(element.id)) {
        return;
    }
    const listener = registeredMouseButtonListeners.get(element.id);
    if (listener) {
        listener.detachListeners();
        registeredMouseButtonListeners.delete(element.id);
    }
}
