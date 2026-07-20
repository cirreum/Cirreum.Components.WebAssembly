var FocusType;
(function (FocusType) {
    FocusType[FocusType["Fallback"] = 0] = "Fallback";
    FocusType[FocusType["First"] = 1] = "First";
    FocusType[FocusType["Last"] = 2] = "Last";
})(FocusType || (FocusType = {}));
class FocusTrapError extends Error {
    constructor(message) {
        super(message);
        this.name = 'FocusTrapError';
    }
}
const focusableElementsSelector = "a:not([tabindex='-1']):not([inert])," +
    "area:not([tabindex='-1']):not([inert])," +
    "button:not([disabled]):not([tabindex='-1']):not([inert])," +
    "input:not([disabled]):not([tabindex='-1']):not([inert])," +
    "select:not([disabled]):not([tabindex='-1']):not([inert])," +
    "textarea:not([disabled]):not([tabindex='-1']):not([inert])," +
    "iframe:not([tabindex='-1']):not([inert])," +
    "details:not([tabindex='-1']):not([inert])," +
    "[tabindex]:not([tabindex='-1'])," +
    "[contentEditable=true]:not([tabindex='-1']):not([inert])";
class FocusTrap {
    container;
    focusType;
    handleKeyDown;
    previouslyFocusedElement = null;
    focusableElements = [];
    constructor(containerId, defaultFocusType) {
        const container = document.getElementById(containerId);
        if (!container) {
            throw new FocusTrapError(`Container with id "${containerId}" not found`);
        }
        this.container = container;
        this.focusType = defaultFocusType;
        this.handleKeyDown = this.handleKeyDownEvent.bind(this);
    }
    updateFocusableElements() {
        this.focusableElements = Array.from(this.container.querySelectorAll(focusableElementsSelector));
    }
    activateTrap() {
        this.savePreviousFocus();
        this.updateFocusableElements();
        const firstNode = this.container.childNodes[0];
        if (this.focusType === 0) {
            firstNode.ariaHidden = "false";
        }
        if (this.focusType === 1) {
            firstNode.ariaHidden = "true";
            this.focusFirst();
        }
        else if (this.focusType === 2) {
            firstNode.ariaHidden = "true";
            this.focusLast();
        }
        this.container.addEventListener('keydown', this.handleKeyDown, true);
    }
    deactivateTrap() {
        this.container.removeEventListener('keydown', this.handleKeyDown, true);
        this.restorePreviousFocus();
        this.previouslyFocusedElement = null;
        this.focusableElements = [];
    }
    wrapFocus(backwards) {
        this.updateFocusableElements();
        if (this.focusableElements.length === 0)
            return null;
        const currentFocusIndex = this.focusableElements.indexOf(document.activeElement);
        if (currentFocusIndex === -1)
            return this.focusableElements[0];
        if (backwards && currentFocusIndex === 0) {
            return this.focusableElements[this.focusableElements.length - 1];
        }
        else if (!backwards && currentFocusIndex === this.focusableElements.length - 1) {
            return this.focusableElements[0];
        }
        return null;
    }
    savePreviousFocus() {
        const exitingFocusedElement = document.activeElement;
        if (exitingFocusedElement) {
            this.previouslyFocusedElement = exitingFocusedElement;
        }
    }
    restorePreviousFocus() {
        if (this.previouslyFocusedElement && document.body.contains(this.previouslyFocusedElement)) {
            try {
                this.previouslyFocusedElement.focus();
            }
            catch (error) {
                console.warn('Failed to restore focus to previous element:', error);
            }
        }
    }
    focusFirst() {
        if (this.focusableElements.length > 0) {
            try {
                this.focusableElements[0].focus();
            }
            catch (error) {
                console.warn('Failed to focus first element:', error);
            }
        }
    }
    focusLast() {
        if (this.focusableElements.length > 0) {
            try {
                this.focusableElements[this.focusableElements.length - 1].focus();
            }
            catch (error) {
                console.warn('Failed to focus last element:', error);
            }
        }
    }
    handleKeyDownEvent(event) {
        if (event.key === 'Tab') {
            var nextElement = this.wrapFocus(event.shiftKey);
            if (nextElement) {
                event.preventDefault();
                try {
                    nextElement.focus();
                }
                catch (error) {
                    console.warn('Failed to focus next element during tab wrap:', error);
                }
            }
        }
    }
}
const focusTraps = new Map();
export function activate(containerId, focusType) {
    if (!containerId) {
        throw new FocusTrapError("Missing containerId");
    }
    if (focusTraps.has(containerId)) {
        console.warn(`Focus trap for container "${containerId}" already exists`);
        return false;
    }
    try {
        const focusTrap = new FocusTrap(containerId, focusType);
        focusTrap.activateTrap();
        focusTraps.set(containerId, focusTrap);
        return true;
    }
    catch (e) {
        if (e instanceof FocusTrapError) {
            console.error(`Failed to activate focus trap: ${e.message}`);
        }
        else {
            console.error("Unexpected error activating focus trap", e);
        }
        throw e;
    }
}
export function deactivate(containerId) {
    if (!containerId) {
        throw new FocusTrapError("Missing containerId");
    }
    const focusTrap = focusTraps.get(containerId);
    if (focusTrap) {
        try {
            focusTrap.deactivateTrap();
        }
        catch (error) {
            console.error(`Error deactivating focus trap for container "${containerId}":`, error);
        }
        finally {
            focusTraps.delete(containerId);
        }
    }
    else {
        console.warn(`No focus trap found for container "${containerId}"`);
    }
}
