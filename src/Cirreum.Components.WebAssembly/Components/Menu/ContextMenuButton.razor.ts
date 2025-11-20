const ESCAPE_KEY = 'Escape'
const ARROW_UP_KEY = 'ArrowUp'
const ARROW_DOWN_KEY = 'ArrowDown'
const ENTER_KEY = 'Enter'
const SPACE_KEY = ' '

const HANDLE_ARROW_UP = "HandleArrowUpKey";
const HANDLE_ARROW_DOWN = "HandleArrowDownKey";
const HANDLE_ESCAPE = "HandleEscapeKey";
const HANDLE_ENTER = "HandleEnterOrSpace";

class KeyHandler {

	_dotnetRef: any;
	_toggleButton: HTMLElement;

	constructor(dotnetRef: any, toggleButton: HTMLElement) {
		this._dotnetRef = dotnetRef;
		this._toggleButton = toggleButton;
	}

	connect() {
		this._toggleButton.addEventListener("keydown", this);
	}

	disconnect() {
		this._toggleButton.removeEventListener("keydown", this);
	}

	handleEvent(event: KeyboardEvent) {

		if (event.type === "keydown") {
			this.handleKeydownEvent(event);
			return;
		}

	}

	handleKeydownEvent(event: KeyboardEvent) {
		const target = event.target as HTMLElement;
		if (this._toggleButton !== target) {
			// only if on our togglebutton
			return;
		}

		const isEscapeEvent = event.key === ESCAPE_KEY
		const isArrowUpEvent = event.key === ARROW_UP_KEY;
		const isArrowDownEvent = event.key === ARROW_DOWN_KEY;
		const isEnterOrSpace =
			event.key === ENTER_KEY ||
			event.key === SPACE_KEY;

		// If not an SPACE | ENTER | UP | DOWN | ESCAPE key => not a context menu trigger command
		if (!(isArrowUpEvent || isArrowDownEvent || isEscapeEvent || isEnterOrSpace)) {
			return
		}

		event.preventDefault()

		if (isArrowDownEvent) {
			this._dotnetRef.invokeMethodAsync(HANDLE_ARROW_DOWN);
			return
		}
		if (isArrowUpEvent) {
			this._dotnetRef.invokeMethodAsync(HANDLE_ARROW_UP);
			return
		}
		if (isEnterOrSpace) {
			this._dotnetRef.invokeMethodAsync(HANDLE_ENTER);
			return
		}

		// else escape...
		this._dotnetRef.invokeMethodAsync(HANDLE_ESCAPE);

	}

}
const registeredMenus: Map<string, KeyHandler> = new Map<string, KeyHandler>();
export function Connect(id: string, dotnetRef: any): void {
	if (!id) {
		throw "[ ContextMenuBase | Connect ] missing id.";
	}
	if (registeredMenus.has(id)) {
		return;
	}
	var toggleButton = document.getElementById(id) as HTMLElement;
	const keyHandler = new KeyHandler(dotnetRef, toggleButton);
	keyHandler.connect();
	registeredMenus.set(id, keyHandler);
}
export function Disconnect(id: string): void {
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