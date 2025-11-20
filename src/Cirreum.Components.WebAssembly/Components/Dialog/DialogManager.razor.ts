interface Position {
	x: number;
	y: number;
}

interface DraggabillyOptions {
	axis?: "x" | "y" | undefined;
	containment?: Element | string | boolean | undefined;
	grid?: [number, number] | undefined;
	handle?: string | undefined;
}

type DraggabillyClickEventName = "dragStart" | "dragEnd" | "pointerDown" | "pointerUp" | "staticClick";

type DraggabillyMoveEventName = "dragMove" | "pointerMove";

declare class Draggabilly {

	position: Position;

	constructor(element: Element | string, options?: DraggabillyOptions);

	on(
		eventName: DraggabillyClickEventName,
		listener: (event: Event, pointer: MouseEvent | Touch) => void,
	): Draggabilly;

	on(
		eventName: DraggabillyMoveEventName,
		listener: (event: Event, pointer: MouseEvent | Touch, moveVector: Position) => void,
	): Draggabilly;

	off(
		eventName: DraggabillyClickEventName,
		listener: (event: Event, pointer: MouseEvent | Touch) => void,
	): Draggabilly;

	off(
		eventName: DraggabillyMoveEventName,
		listener: (event: Event, pointer: MouseEvent | Touch, moveVector: Position) => void,
	): Draggabilly;

	once(
		eventName: DraggabillyClickEventName,
		listener: (event: Event, pointer: MouseEvent | Touch) => void,
	): Draggabilly;

	once(
		eventName: DraggabillyMoveEventName,
		listener: (event: Event, pointer: MouseEvent | Touch, moveVector: Position) => void,
	): Draggabilly;

	enable(): void;

	disable(): void;

	destroy(): void;
}

const registeredDialogs: Map<string, Draggabilly> = new Map<string, Draggabilly>();
function resolveElement(element: string | HTMLElement): HTMLElement {
	if (typeof element === 'string') {
		return document.querySelector(element) as HTMLElement;
	}
	return element;
}
function handlePointerDownUp(event: Event): void {
	const dialog = event.currentTarget as HTMLElement;
	dialog.focus();
}
function initializeDialogWidth(dialog: HTMLElement) {

	if (dialog.style.width !== '') {
		// if there is custom/user provided width, then do not alter
		return;
	}

	if (window.innerWidth > 480) {
		// For larger screens, prevent resizing when dragged

		// Force a reflow to ensure the dialog adjusts to its content
		dialog.style.width = 'fit-content';

		// Read the computed width
		const computedWidth = window.getComputedStyle(dialog).width;

		// Set width explicitly
		dialog.style.width = computedWidth;

	}

}
export function addDraggable(id: string, options?: DraggabillyOptions): boolean {
	if (registeredDialogs.has(id)) {
		console.warn("addDraggable: cannot register a duplicate draggable dialog id.");
		return false;
	}

	try {
		const dialog = resolveElement(`#${id}`);
		if (dialog instanceof HTMLElement) {

			// Adjust dialog width immediately upon initialization
			initializeDialogWidth(dialog);

			var draggie = new Draggabilly(dialog, options);
			registeredDialogs.set(id, draggie);
			draggie.on('pointerDown', handlePointerDownUp);
			draggie.on('pointerUp', handlePointerDownUp);

			return true;

		}

		console.warn(`addDraggable: dialog with the id of ${id} was not found or is not an HTMLElement.`);

	} catch (e: any) {
		registeredDialogs.delete(id);
		alert(e.message);
	}

	return false;
}
export function removeDraggable(id: string) {
	const draggie = registeredDialogs.get(id);
	const dialog = resolveElement(`#${id}`);
	if (draggie && dialog instanceof HTMLElement) {
		try {
			draggie.off('pointerDown', handlePointerDownUp);
			draggie.off('pointerUp', handlePointerDownUp);
			draggie.destroy();
			registeredDialogs.delete(id);
		} catch (e: any) {
			console.error(e.message);
		}
	}
}
export function adjustPosition(position: number, index: number, dialogInstance: string | HTMLElement) {

	var ele = resolveElement(dialogInstance);
	if (ele && ele instanceof HTMLElement) {

		const vw = Math.max(document.documentElement.clientWidth || 0, window.innerWidth || 0);
		const vh = Math.max(document.documentElement.clientHeight || 0, window.innerHeight || 0);
		const rect = ele.getBoundingClientRect();

		ele.style.setProperty("transform", "none");

		if (position == 0) {
			// center
			ele.style.setProperty("top", rect.top + (index * 15) + "px");
			ele.style.setProperty("left", rect.left + (index * 15) + "px");
			return;
		}
		if (position == 1) {
			// centerLeft
			ele.style.setProperty("top", rect.top + (index * 15) + "px");
			ele.style.setProperty("left", rect.left + (index * 15) + "px");
			return;
		}
		if (position == 2) {
			// centerRight
			ele.style.setProperty("top", rect.top + (index * 15) + "px");
			ele.style.setProperty("right", (vw - rect.right) + (index * 15) + "px");
			return;
		}
		if (position == 3) {
			// top
			ele.style.setProperty("top", rect.top + (index * 15) + "px");
			ele.style.setProperty("left", rect.left + (index * 15) + "px");
			return;
		}
		if (position == 4) {
			// topleft
			ele.style.setProperty("top", rect.top + (index * 15) + "px");
			ele.style.setProperty("left", rect.left + (index * 15) + "px");
			return;
		}
		if (position == 5) {
			// topright
			ele.style.setProperty("top", rect.top + (index * 15) + "px");
			ele.style.setProperty("right", (vw - rect.right) + (index * 15) + "px");
			return;
		}
		if (position == 6) {
			// bottom
			ele.style.setProperty("bottom", (vh - rect.bottom) + (index * 15) + "px");
			ele.style.setProperty("left", rect.left + (index * 15) + "px");
			return;
		}
		if (position == 7) {
			// bottomleft
			ele.style.setProperty("bottom", (vh - rect.bottom) + (index * 15) + "px");
			ele.style.setProperty("left", rect.left + (index * 15) + "px");
			return;
		}
		if (position == 8) {
			// bottomright
			ele.style.setProperty("bottom", (vh - rect.bottom) + (index * 15) + "px");
			ele.style.setProperty("right", (vw - rect.right) + (index * 15) + "px");
			return;
		}
	}

}