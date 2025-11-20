enum FocusType {
	Fallback = 0,
	First = 1,
	Last = 2
}

class FocusTrapError extends Error {
	constructor(message: string) {
		super(message);
		this.name = 'FocusTrapError';
	}
}

const focusableElementsSelector: string =
	"a:not([tabindex='-1']):not([inert])," +
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

	private container: HTMLElement;
	private focusType: FocusType;
	private handleKeyDown: (event: KeyboardEvent) => void;
	private previouslyFocusedElement: HTMLElement | null = null;
	private focusableElements: HTMLElement[] = [];

	constructor(containerId: string, defaultFocusType: FocusType) {
		const container = document.getElementById(containerId);
		if (!container) {
			throw new FocusTrapError(`Container with id "${containerId}" not found`);
		}
		this.container = container;
		this.focusType = defaultFocusType;
		this.handleKeyDown = this.handleKeyDownEvent.bind(this);
	}

	private updateFocusableElements(): void {
		this.focusableElements = Array.from(
			this.container.querySelectorAll<HTMLElement>(focusableElementsSelector)
		);
	}

	activateTrap() {
		// Save previously focused element
		this.savePreviousFocus();

		// Update focusable elements after DOM is ready
		this.updateFocusableElements();

		// focusType === 0 === fallback/handled in blazor
		const firstNode = this.container.childNodes[0] as HTMLElement;
		if (this.focusType === 0) {
			firstNode.ariaHidden = "false";
		}
		if (this.focusType === 1) {
			firstNode.ariaHidden = "true";
			this.focusFirst(); // first focusable container element
		} else if (this.focusType === 2) {
			firstNode.ariaHidden = "true";
			this.focusLast(); // last focusable container element
		}

		// Connect handlers
		this.container.addEventListener('keydown', this.handleKeyDown, true);
	}

	deactivateTrap() {
		this.container.removeEventListener('keydown', this.handleKeyDown, true);
		this.restorePreviousFocus();
		// Reset internal state
		this.previouslyFocusedElement = null;
		this.focusableElements = [];
	}

	private wrapFocus(backwards: boolean): HTMLElement | null {
		// Update focusable elements in case DOM changed while trap was active
		this.updateFocusableElements();

		if (this.focusableElements.length === 0) return null;

		const currentFocusIndex = this.focusableElements.indexOf(document.activeElement as HTMLElement);
		if (currentFocusIndex === -1) return this.focusableElements[0];

		if (backwards && currentFocusIndex === 0) {
			return this.focusableElements[this.focusableElements.length - 1];
		} else if (!backwards && currentFocusIndex === this.focusableElements.length - 1) {
			return this.focusableElements[0];
		}

		return null;
	}

	private savePreviousFocus() {
		const exitingFocusedElement = document.activeElement as HTMLElement | null;
		if (exitingFocusedElement) {
			this.previouslyFocusedElement = exitingFocusedElement;
		}
	}

	private restorePreviousFocus(): void {
		if (this.previouslyFocusedElement && document.body.contains(this.previouslyFocusedElement)) {
			try {
				this.previouslyFocusedElement.focus();
			} catch (error) {
				// Element might not be focusable anymore (disabled, hidden, etc.)
				console.warn('Failed to restore focus to previous element:', error);
			}
		}
	}

	private focusFirst(): void {
		if (this.focusableElements.length > 0) {
			try {
				this.focusableElements[0].focus();
			} catch (error) {
				console.warn('Failed to focus first element:', error);
			}
		}
	}

	private focusLast(): void {
		if (this.focusableElements.length > 0) {
			try {
				this.focusableElements[this.focusableElements.length - 1].focus();
			} catch (error) {
				console.warn('Failed to focus last element:', error);
			}
		}
	}

	private handleKeyDownEvent(event: KeyboardEvent) {
		if (event.key === 'Tab') {
			var nextElement = this.wrapFocus(event.shiftKey);
			if (nextElement) {
				event.preventDefault();
				try {
					nextElement.focus();
				} catch (error) {
					console.warn('Failed to focus next element during tab wrap:', error);
				}
			}
		}
	}
}

const focusTraps: Map<string, FocusTrap> = new Map<string, FocusTrap>();

export function activate(containerId: string, focusType: FocusType): boolean {
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
	} catch (e) {
		if (e instanceof FocusTrapError) {
			console.error(`Failed to activate focus trap: ${e.message}`);
		} else {
			console.error("Unexpected error activating focus trap", e);
		}
		throw e;
	}
}

export function deactivate(containerId: string): void {
	if (!containerId) {
		throw new FocusTrapError("Missing containerId");
	}
	const focusTrap = focusTraps.get(containerId);
	if (focusTrap) {
		try {
			focusTrap.deactivateTrap();
		} catch (error) {
			console.error(`Error deactivating focus trap for container "${containerId}":`, error);
		} finally {
			focusTraps.delete(containerId);
		}
	} else {
		console.warn(`No focus trap found for container "${containerId}"`);
	}
}