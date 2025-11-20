
export function raiseInputFileClick(fileInputId: string): void {
	const item = document.getElementById(fileInputId);
	if (item instanceof HTMLInputElement) {
		item.click();
	}
}

export function clearInput(fileInputId: string): void {
	const fileInput = document.getElementById(fileInputId);
	if (fileInput instanceof HTMLInputElement) {
		fileInput.value = "";
	}
}

export function attachClickHandler(buttonId: string, fileInputId: string): void {
	const button = document.getElementById(buttonId);
	const fileInput = document.getElementById(fileInputId);

	if (button instanceof HTMLElement && fileInput instanceof HTMLElement) {
		button.addEventListener("click", (e: Event) => {
			if (fileInput instanceof HTMLInputElement) {
				fileInput.click();
			}
		});
	}
}