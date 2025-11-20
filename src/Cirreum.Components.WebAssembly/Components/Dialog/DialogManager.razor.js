const registeredDialogs = new Map();
function resolveElement(element) {
    if (typeof element === 'string') {
        return document.querySelector(element);
    }
    return element;
}
function handlePointerDownUp(event) {
    const dialog = event.currentTarget;
    dialog.focus();
}
function initializeDialogWidth(dialog) {
    if (dialog.style.width !== '') {
        return;
    }
    if (window.innerWidth > 480) {
        dialog.style.width = 'fit-content';
        const computedWidth = window.getComputedStyle(dialog).width;
        dialog.style.width = computedWidth;
    }
}
export function addDraggable(id, options) {
    if (registeredDialogs.has(id)) {
        console.warn("addDraggable: cannot register a duplicate draggable dialog id.");
        return false;
    }
    try {
        const dialog = resolveElement(`#${id}`);
        if (dialog instanceof HTMLElement) {
            initializeDialogWidth(dialog);
            var draggie = new Draggabilly(dialog, options);
            registeredDialogs.set(id, draggie);
            draggie.on('pointerDown', handlePointerDownUp);
            draggie.on('pointerUp', handlePointerDownUp);
            return true;
        }
        console.warn(`addDraggable: dialog with the id of ${id} was not found or is not an HTMLElement.`);
    }
    catch (e) {
        registeredDialogs.delete(id);
        alert(e.message);
    }
    return false;
}
export function removeDraggable(id) {
    const draggie = registeredDialogs.get(id);
    const dialog = resolveElement(`#${id}`);
    if (draggie && dialog instanceof HTMLElement) {
        try {
            draggie.off('pointerDown', handlePointerDownUp);
            draggie.off('pointerUp', handlePointerDownUp);
            draggie.destroy();
            registeredDialogs.delete(id);
        }
        catch (e) {
            console.error(e.message);
        }
    }
}
export function adjustPosition(position, index, dialogInstance) {
    var ele = resolveElement(dialogInstance);
    if (ele && ele instanceof HTMLElement) {
        const vw = Math.max(document.documentElement.clientWidth || 0, window.innerWidth || 0);
        const vh = Math.max(document.documentElement.clientHeight || 0, window.innerHeight || 0);
        const rect = ele.getBoundingClientRect();
        ele.style.setProperty("transform", "none");
        if (position == 0) {
            ele.style.setProperty("top", rect.top + (index * 15) + "px");
            ele.style.setProperty("left", rect.left + (index * 15) + "px");
            return;
        }
        if (position == 1) {
            ele.style.setProperty("top", rect.top + (index * 15) + "px");
            ele.style.setProperty("left", rect.left + (index * 15) + "px");
            return;
        }
        if (position == 2) {
            ele.style.setProperty("top", rect.top + (index * 15) + "px");
            ele.style.setProperty("right", (vw - rect.right) + (index * 15) + "px");
            return;
        }
        if (position == 3) {
            ele.style.setProperty("top", rect.top + (index * 15) + "px");
            ele.style.setProperty("left", rect.left + (index * 15) + "px");
            return;
        }
        if (position == 4) {
            ele.style.setProperty("top", rect.top + (index * 15) + "px");
            ele.style.setProperty("left", rect.left + (index * 15) + "px");
            return;
        }
        if (position == 5) {
            ele.style.setProperty("top", rect.top + (index * 15) + "px");
            ele.style.setProperty("right", (vw - rect.right) + (index * 15) + "px");
            return;
        }
        if (position == 6) {
            ele.style.setProperty("bottom", (vh - rect.bottom) + (index * 15) + "px");
            ele.style.setProperty("left", rect.left + (index * 15) + "px");
            return;
        }
        if (position == 7) {
            ele.style.setProperty("bottom", (vh - rect.bottom) + (index * 15) + "px");
            ele.style.setProperty("left", rect.left + (index * 15) + "px");
            return;
        }
        if (position == 8) {
            ele.style.setProperty("bottom", (vh - rect.bottom) + (index * 15) + "px");
            ele.style.setProperty("right", (vw - rect.right) + (index * 15) + "px");
            return;
        }
    }
}
