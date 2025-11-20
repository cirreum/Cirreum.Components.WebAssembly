export function raiseInputFileClick(fileInputId) {
    const item = document.getElementById(fileInputId);
    if (item instanceof HTMLInputElement) {
        item.click();
    }
}
export function clearInput(fileInputId) {
    const fileInput = document.getElementById(fileInputId);
    if (fileInput instanceof HTMLInputElement) {
        fileInput.value = "";
    }
}
export function attachClickHandler(buttonId, fileInputId) {
    const button = document.getElementById(buttonId);
    const fileInput = document.getElementById(fileInputId);
    if (button instanceof HTMLElement && fileInput instanceof HTMLElement) {
        button.addEventListener("click", (e) => {
            if (fileInput instanceof HTMLInputElement) {
                fileInput.click();
            }
        });
    }
}
