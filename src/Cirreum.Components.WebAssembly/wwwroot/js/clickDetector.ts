
const outsideMethodName = "outsideClick"; // matches the c# named method
const insideMethodName = "insideClick"; // matches the c# named method

interface RegisteredElement {
	element: HTMLElement;
	outsideCallback: (id: string) => void;
	insideCallback: (id: string) => void;
	ignoreOutside: string[];
}

type ClickDetector = {
	registerElement: (element: HTMLElement, outsideCallback: (id: string) => void, insideCallback: (id: string) => void, ignoreOutside: string[]) => void;
	unregisterElement: (element: HTMLElement) => void;
};

const clickDetector: ClickDetector = (() => {

	const registeredElements = new Set<RegisteredElement>();

	const handleAnyClick = (event: MouseEvent | PointerEvent | TouchEvent) => {
		const targetElement = event.target as Element;
		registeredElements.forEach((currentItem) => {
			if (currentItem.element.contains(targetElement)) {
				currentItem.insideCallback(targetElement.id);
			} else if (!currentItem.ignoreOutside.includes(targetElement.id)) {
				currentItem.outsideCallback(targetElement.id);
			}
		});
	};

	const registerElement = (element: HTMLElement, outsideCallback: (id: string) => void, insideCallback: (id: string) => void, ignoreOutside: string[]) => {
		registeredElements.add({ element, outsideCallback, insideCallback, ignoreOutside });
	};

	const unregisterElement = (element: HTMLElement) => {
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

function resolveElementById<T extends HTMLElement>(idOrElement: string | T): T {
	if (typeof idOrElement === 'string') {
		return document.getElementById(idOrElement) as T;
	}
	return idOrElement;
}
export function registerElement(idOrElement: string | HTMLElement, dotnetRef: any, ignoreOutside: string[]) {
	var resolvedElement = resolveElementById(idOrElement);
	if (resolvedElement) {
		clickDetector.registerElement(
			resolvedElement,
			(id: string) => {
				dotnetRef.invokeMethodAsync(outsideMethodName, id);
			},
			(id: string) => {
				dotnetRef.invokeMethodAsync(insideMethodName, id);
			},
			ignoreOutside ?? []
		);
		return;
	}
	console.warn(`unabled to resolve HTMLElement with Id [${idOrElement}]`);
}
export function unregisterElement(idOrElement: string | HTMLElement) {
	var resolvedElement = resolveElementById(idOrElement);
	if (resolvedElement) {
		clickDetector.unregisterElement(resolvedElement);
	}
}