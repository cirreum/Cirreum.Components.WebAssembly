const TAB_KEY = 'Tab'
const ENTER_KEY = 'Enter'
const ESCAPE_KEY = 'Escape'
const ARROW_UP_KEY = 'ArrowUp'
const ARROW_DOWN_KEY = 'ArrowDown'

const HANDLE_ARROW_UP = "HandleArrowUpKey";
const HANDLE_ARROW_DOWN = "HandleArrowDownKey";
const HANDLE_ESCAPE = "HandleEscapeKey";
const HANDLE_ENTER = "HandleEnterKey";
const HANDLE_TAB = "HandleTabKey";

class KeyHandler {

	_id: string;
	_dotnetRef: any;
	_toggleButton: HTMLElement;
	_toggleMenu: HTMLElement;
	private previouslyFocusedElement: HTMLElement | null = null;

	constructor(id: string, dotnetRef: any, toggleButton: HTMLElement, toggleMenu: HTMLElement) {
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

	handleEvent(event: KeyboardEvent) {

		if (event.type === "keydown") {
			this.handleKeydownEvent(event);
			return;
		}

		if (event.type === "keyup") {
			this.handleKeyupEvent(event);
			return;
		}

	}

	handleKeyupEvent(event: KeyboardEvent) {

		if (event.key === ENTER_KEY && !(event.altKey || event.ctrlKey || event.shiftKey || event.metaKey)) {
			// close...
			event.preventDefault()
			event.stopPropagation()
			this._dotnetRef.invokeMethodAsync(HANDLE_ENTER);
			return;
		}

	}

	handleKeydownEvent(event: KeyboardEvent) {
		// If not an UP | DOWN | ESCAPE key => not a dropdown command
		// If input/textarea && if key is other than ESCAPE => not a dropdown command
		const currentTarget = event.currentTarget as HTMLElement;
		const target = event.target as HTMLElement;
		if (!target) {
			return;
		}

		const isInput = /input|textarea/i.test(target.tagName)
		const isEscapeEvent = event.key === ESCAPE_KEY
		const isArrowUpEvent = event.key === ARROW_UP_KEY;
		const isArrowDownEvent = event.key === ARROW_DOWN_KEY;

		if (event.key === TAB_KEY && !(event.altKey || event.ctrlKey || event.shiftKey || event.metaKey)) {
			// tab forwards...
			this._dotnetRef.invokeMethodAsync(HANDLE_TAB, target.id, true);
			return;
		}

		if (event.key === TAB_KEY && event.shiftKey && !(event.altKey || event.ctrlKey || event.metaKey)) {
			// tab backwards...
			this._dotnetRef.invokeMethodAsync(HANDLE_TAB, target.id, false);
			return;
		}

		if (!(isArrowUpEvent || isArrowDownEvent) && !isEscapeEvent) {
			return
		}

		if (isInput && !isEscapeEvent) {
			return
		}

		event.preventDefault()

		if (isArrowUpEvent) {
			event.stopPropagation()
			// arrow up key
			this._dotnetRef.invokeMethodAsync(HANDLE_ARROW_UP);
			return
		}
		if (isArrowDownEvent) {
			event.stopPropagation()
			// arrow down key
			this._dotnetRef.invokeMethodAsync(HANDLE_ARROW_DOWN);
			return
		}

		// else is escape...

		// on the popup menu
		if (this._toggleMenu.id == currentTarget.id) {
			event.stopPropagation()
			this._dotnetRef.invokeMethodAsync(HANDLE_ESCAPE);
			return;
		}

		// or, is showing the popup menu, but it's not focused
		const toggleMenuIsShowing = this._toggleMenu.classList.contains("show");
		if (toggleMenuIsShowing) {
			event.stopPropagation()
			this._dotnetRef.invokeMethodAsync(HANDLE_ESCAPE);
			return;
		}

	}

	savePreviousFocus() {
		const exitingFocusedElement = document.activeElement as HTMLElement | null;
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


const registeredDropdowns: Map<string, KeyHandler> = new Map<string, KeyHandler>();
export function Connect(id: string, dotnetRef: any, toggleButton: HTMLElement, toggleMenu: HTMLElement): void {
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
export function CaptureFocusedElement(id: string): void {
	if (!id) {
		throw "[ DropdpwnButton | CaptureFocusedElement ] missing id.";
	}
	var handler = registeredDropdowns.get(id);
	if (handler) {
		handler.savePreviousFocus();
	}
}
export function RestoreFocusedElement(id: string): void {
	if (!id) {
		throw "[ DropdpwnButton | RestoreFocusedElement ] missing id.";
	}
	var handler = registeredDropdowns.get(id);
	if (handler) {
		handler.restorePreviousFocus();
	}
}
export function Disconnect(id: string): void {
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