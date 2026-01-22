export function clearInput(fileInputId: string): void {
	const fileInput = document.getElementById(fileInputId);
	if (fileInput instanceof HTMLInputElement) {
		fileInput.value = "";
	}
}

const fileInputAbortControllers = new Map<string, AbortController>();
export function attachClickHandler(buttonId: string, fileInputId: string): void {
	const button = document.getElementById(buttonId);
	const fileInput = document.getElementById(fileInputId);

	if (!(button instanceof HTMLElement) || !(fileInput instanceof HTMLInputElement)) {
		return;
	}

	// Clean up existing handler if any
	fileInputAbortControllers.get(buttonId)?.abort();

	const controller = new AbortController();
	button.addEventListener("click", () => fileInput.click(), { signal: controller.signal });
	fileInputAbortControllers.set(buttonId, controller);
}
export function detachClickHandler(buttonId: string): void {
	fileInputAbortControllers.get(buttonId)?.abort();
	fileInputAbortControllers.delete(buttonId);
}