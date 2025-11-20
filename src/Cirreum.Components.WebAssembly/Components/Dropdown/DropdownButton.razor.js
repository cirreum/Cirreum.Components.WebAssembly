const TAB_KEY = 'Tab';
const ENTER_KEY = 'Enter';
const ESCAPE_KEY = 'Escape';
const ARROW_UP_KEY = 'ArrowUp';
const ARROW_DOWN_KEY = 'ArrowDown';
const HANDLE_ARROW_UP = "HandleArrowUpKey";
const HANDLE_ARROW_DOWN = "HandleArrowDownKey";
const HANDLE_ESCAPE = "HandleEscapeKey";
const HANDLE_ENTER = "HandleEnterKey";
const HANDLE_TAB = "HandleTabKey";
class KeyHandler {
    _id;
    _dotnetRef;
    _toggleButton;
    _toggleMenu;
    previouslyFocusedElement = null;
    constructor(id, dotnetRef, toggleButton, toggleMenu) {
        this._id = id;
        this._dotnetRef = dotnetRef;
        this._toggleButton = toggleButton;
        this._toggleMenu = toggleMenu;
    }
    connect() {
        this._toggleButton.addEventListener("keydown", this);
        this._toggleMenu.addEventListener("keydown", this);
        this._toggleMenu.addEventListener("keyup", this);
    }
    disconnect() {
        this._toggleButton.removeEventListener("keydown", this);
        this._toggleMenu.removeEventListener("keydown", this);
        this._toggleMenu.removeEventListener("keyup", this);
    }
    handleEvent(event) {
        if (event.type === "keydown") {
            this.handleKeydownEvent(event);
            return;
        }
        if (event.type === "keyup") {
            this.handleKeyupEvent(event);
            return;
        }
    }
    handleKeyupEvent(event) {
        if (event.key === ENTER_KEY && !(event.altKey || event.ctrlKey || event.shiftKey || event.metaKey)) {
            event.preventDefault();
            event.stopPropagation();
            this._dotnetRef.invokeMethodAsync(HANDLE_ENTER);
            return;
        }
    }
    handleKeydownEvent(event) {
        const currentTarget = event.currentTarget;
        const target = event.target;
        if (!target) {
            return;
        }
        const isInput = /input|textarea/i.test(target.tagName);
        const isEscapeEvent = event.key === ESCAPE_KEY;
        const isArrowUpEvent = event.key === ARROW_UP_KEY;
        const isArrowDownEvent = event.key === ARROW_DOWN_KEY;
        if (event.key === TAB_KEY && !(event.altKey || event.ctrlKey || event.shiftKey || event.metaKey)) {
            this._dotnetRef.invokeMethodAsync(HANDLE_TAB, target.id, true);
            return;
        }
        if (event.key === TAB_KEY && event.shiftKey && !(event.altKey || event.ctrlKey || event.metaKey)) {
            this._dotnetRef.invokeMethodAsync(HANDLE_TAB, target.id, false);
            return;
        }
        if (!(isArrowUpEvent || isArrowDownEvent) && !isEscapeEvent) {
            return;
        }
        if (isInput && !isEscapeEvent) {
            return;
        }
        event.preventDefault();
        if (isArrowUpEvent) {
            event.stopPropagation();
            this._dotnetRef.invokeMethodAsync(HANDLE_ARROW_UP);
            return;
        }
        if (isArrowDownEvent) {
            event.stopPropagation();
            this._dotnetRef.invokeMethodAsync(HANDLE_ARROW_DOWN);
            return;
        }
        if (this._toggleMenu.id == currentTarget.id) {
            event.stopPropagation();
            this._dotnetRef.invokeMethodAsync(HANDLE_ESCAPE);
            return;
        }
        const toggleMenuIsShowing = this._toggleMenu.classList.contains("show");
        if (toggleMenuIsShowing) {
            event.stopPropagation();
            this._dotnetRef.invokeMethodAsync(HANDLE_ESCAPE);
            return;
        }
    }
    savePreviousFocus() {
        const exitingFocusedElement = document.activeElement;
        if (exitingFocusedElement) {
            this.previouslyFocusedElement = exitingFocusedElement;
        }
    }
    restorePreviousFocus() {
        if (this.previouslyFocusedElement) {
            this.previouslyFocusedElement.focus();
            return;
        }
    }
}
const registeredDropdowns = new Map();
export function Connect(id, dotnetRef, toggleButton, toggleMenu) {
    if (!id) {
        throw "[ DropdpwnButton | Connect ] missing id.";
    }
    if (registeredDropdowns.has(id)) {
        return;
    }
    const keyHandler = new KeyHandler(id, dotnetRef, toggleButton, toggleMenu);
    keyHandler.connect();
    registeredDropdowns.set(id, keyHandler);
}
export function CaptureFocusedElement(id) {
    if (!id) {
        throw "[ DropdpwnButton | CaptureFocusedElement ] missing id.";
    }
    var handler = registeredDropdowns.get(id);
    if (handler) {
        handler.savePreviousFocus();
    }
}
export function RestoreFocusedElement(id) {
    if (!id) {
        throw "[ DropdpwnButton | RestoreFocusedElement ] missing id.";
    }
    var handler = registeredDropdowns.get(id);
    if (handler) {
        handler.restorePreviousFocus();
    }
}
export function Disconnect(id) {
    if (!id) {
        throw "[ DropdpwnButton | Disconnect ] missing id.";
    }
    var handler = registeredDropdowns.get(id);
    if (handler) {
        registeredDropdowns.delete(id);
        handler.disconnect();
        return;
    }
}
