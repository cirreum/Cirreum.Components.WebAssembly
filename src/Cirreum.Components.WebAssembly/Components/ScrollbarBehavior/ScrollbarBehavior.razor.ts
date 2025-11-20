
interface HTMLElementWithEventHandler extends HTMLElement {
	_mouseenterHandler: any;
	_mouseleaveHandler: any;
}
function sleep(ms: number) {
	return new Promise(resolve => setTimeout(resolve, ms));
}

enum ScrollbarState {
	Hiding,
	Hidden,
	Showing,
	Shown,
	Sleeping
}

export function addScrollbarBehavior(selector: string, showDelay: number, hideDelay: number) {

	const control = document.documentElement.querySelector(selector) as HTMLElementWithEventHandler;

	if (control) {

		let state: ScrollbarState = ScrollbarState.Hidden;

		// Define the event handlers
		const mouseEnterHandler = async (e: MouseEvent) => {
			if (state === ScrollbarState.Showing || state === ScrollbarState.Shown) return;
			if (state === ScrollbarState.Sleeping) return;
			state = ScrollbarState.Sleeping;
			await sleep(showDelay);
			if (state === ScrollbarState.Sleeping) {
				state = ScrollbarState.Showing;
				control.dataset.scrollbar = 'show';
				state = ScrollbarState.Shown;
			}
		}

		const mouseLeaveHandler = async (e: MouseEvent) => {
			if (state === ScrollbarState.Hiding || state === ScrollbarState.Hidden) return;
			if (state === ScrollbarState.Sleeping) {
				state = ScrollbarState.Hidden;
			} else {
				state = ScrollbarState.Sleeping;
				if (hideDelay > 10) {
					await sleep(hideDelay);
				}
				if (state === ScrollbarState.Sleeping) {
					state = ScrollbarState.Hiding;
					// Set the hide state
					control.dataset.scrollbar = 'hide';
					state = ScrollbarState.Hidden;
				}
			}
		}

		// Add the event listeners
		control.addEventListener('mouseenter', mouseEnterHandler);
		control.addEventListener('mouseleave', mouseLeaveHandler);

		// Store references to the event handlers for later removal
		control._mouseenterHandler = mouseEnterHandler;
		control._mouseleaveHandler = mouseLeaveHandler;

		// Initialize with the default (hidden)
		control.dataset.scrollbar = 'hide';

	}
}

export function removeScrollbarBehavior(selector: string, preserveDataAttribute: boolean = false) {

	const control = document.documentElement.querySelector(selector) as HTMLElementWithEventHandler;

	if (control) {

		// Remove the event listeners using the stored references
		if (control._mouseenterHandler) {
			control.removeEventListener('mouseenter', control._mouseenterHandler);
			delete control._mouseenterHandler;
		}
		if (control._mouseleaveHandler) {
			control.removeEventListener('mouseleave', control._mouseleaveHandler);
			delete control._mouseleaveHandler;
		}

		// Clean up data attribute
		if (!preserveDataAttribute) {
			delete control.dataset.scrollbar;
		}
	}

}