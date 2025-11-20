const outsideMethodName = "outsideClick";
const insideMethodName = "insideClick";
const clickDetector = (() => {
    const registeredElements = new Set();
    const handleAnyClick = (event) => {
        const targetElement = event.target;
        registeredElements.forEach((currentItem) => {
            if (currentItem.element.contains(targetElement)) {
                currentItem.insideCallback(targetElement.id);
            }
            else if (!currentItem.ignoreOutside.includes(targetElement.id)) {
                currentItem.outsideCallback(targetElement.id);
            }
        });
    };
    const registerElement = (element, outsideCallback, insideCallback, ignoreOutside) => {
        registeredElements.add({ element, outsideCallback, insideCallback, ignoreOutside });
    };
    const unregisterElement = (element) => {
        registeredElements.forEach((item) => {
            if (item.element === element) {
                registeredElements.delete(item);
            }
        });
    };
    document.addEventListener('click', handleAnyClick, { passive: true });
    return {
        registerElement,
        unregisterElement
    };
})();
function resolveElementById(idOrElement) {
    if (typeof idOrElement === 'string') {
        return document.getElementById(idOrElement);
    }
    return idOrElement;
}
export function registerElement(idOrElement, dotnetRef, ignoreOutside) {
    var resolvedElement = resolveElementById(idOrElement);
    if (resolvedElement) {
        clickDetector.registerElement(resolvedElement, (id) => {
            dotnetRef.invokeMethodAsync(outsideMethodName, id);
        }, (id) => {
            dotnetRef.invokeMethodAsync(insideMethodName, id);
        }, ignoreOutside ?? []);
        return;
    }
    console.warn(`unabled to resolve HTMLElement with Id [${idOrElement}]`);
}
export function unregisterElement(idOrElement) {
    var resolvedElement = resolveElementById(idOrElement);
    if (resolvedElement) {
        clickDetector.unregisterElement(resolvedElement);
    }
}
