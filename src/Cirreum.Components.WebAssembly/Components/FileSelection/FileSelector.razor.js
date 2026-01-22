export function clearInput(fileInputId) {
    const fileInput = document.getElementById(fileInputId);
    if (fileInput instanceof HTMLInputElement) {
        fileInput.value = "";
    }
}
const fileInputAbortControllers = new Map();
export function attachClickHandler(buttonId, fileInputId) {
    const button = document.getElementById(buttonId);
    const fileInput = document.getElementById(fileInputId);
    if (!(button instanceof HTMLElement) || !(fileInput instanceof HTMLInputElement)) {
        return;
    }
    fileInputAbortControllers.get(buttonId)?.abort();
    const controller = new AbortController();
    button.addEventListener("click", () => fileInput.click(), { signal: controller.signal });
    fileInputAbortControllers.set(buttonId, controller);
}
export function detachClickHandler(buttonId) {
    fileInputAbortControllers.get(buttonId)?.abort();
    fileInputAbortControllers.delete(buttonId);
}
