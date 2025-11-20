const ESCAPE_KEY = 'Escape';
const ARROW_UP_KEY = 'ArrowUp';
const ARROW_DOWN_KEY = 'ArrowDown';
const ENTER_KEY = 'Enter';
const SPACE_KEY = ' ';
const HANDLE_ARROW_UP = "HandleArrowUpKey";
const HANDLE_ARROW_DOWN = "HandleArrowDownKey";
const HANDLE_ESCAPE = "HandleEscapeKey";
const HANDLE_ENTER = "HandleEnterOrSpace";
class KeyHandler {
    _dotnetRef;
    _toggleButton;
    constructor(dotnetRef, toggleButton) {
        this._dotnetRef = dotnetRef;
        this._toggleButton = toggleButton;
    }
    connect() {
        this._toggleButton.addEventListener("keydown", this);
    }
    disconnect() {
        this._toggleButton.removeEventListener("keydown", this);
    }
    handleEvent(event) {
        if (event.type === "keydown") {
            this.handleKeydownEvent(event);
            return;
        }
    }
    handleKeydownEvent(event) {
        const target = event.target;
        if (this._toggleButton !== target) {
            return;
        }
        const isEscapeEvent = event.key === ESCAPE_KEY;
        const isArrowUpEvent = event.key === ARROW_UP_KEY;
        const isArrowDownEvent = event.key === ARROW_DOWN_KEY;
        const isEnterOrSpace = event.key === ENTER_KEY ||
            event.key === SPACE_KEY;
        if (!(isArrowUpEvent || isArrowDownEvent || isEscapeEvent || isEnterOrSpace)) {
            return;
        }
        event.preventDefault();
        if (isArrowDownEvent) {
            this._dotnetRef.invokeMethodAsync(HANDLE_ARROW_DOWN);
            return;
        }
        if (isArrowUpEvent) {
            this._dotnetRef.invokeMethodAsync(HANDLE_ARROW_UP);
            return;
        }
        if (isEnterOrSpace) {
            this._dotnetRef.invokeMethodAsync(HANDLE_ENTER);
            return;
        }
        this._dotnetRef.invokeMethodAsync(HANDLE_ESCAPE);
    }
}
const registeredMenus = new Map();
export function Connect(id, dotnetRef) {
    if (!id) {
        throw "[ ContextMenuBase | Connect ] missing id.";
    }
    if (registeredMenus.has(id)) {
        return;
    }
    var toggleButton = document.getElementById(id);
    const keyHandler = new KeyHandler(dotnetRef, toggleButton);
    keyHandler.connect();
    registeredMenus.set(id, keyHandler);
}
export function Disconnect(id) {
    if (!id) {
        throw "[ ContextMenuBase | Disconnect ] missing id.";
    }
    var handler = registeredMenus.get(id);
    if (handler) {
        registeredMenus.delete(id);
        handler.disconnect();
        return;
    }
}
