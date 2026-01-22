interface IDotNetHelper {
	invokeMethodAsync(methodName: string, ...args: any[]): Promise<any>;
}

interface ScrollInstance {
	scrollHandler: () => void;
	dotNetHelper: IDotNetHelper;
}

const instances = new Map<string, ScrollInstance>();

export function initialize(instanceId: string, dotNetRef: IDotNetHelper, threshold: number): void {
	dispose(instanceId);

	let ticking = false;
	const scrollHandler = (): void => {
		if (!ticking) {
			ticking = true;
			requestAnimationFrame(() => {
				const visible: boolean = window.scrollY > threshold;
				dotNetRef.invokeMethodAsync('UpdateVisibility', visible);
				ticking = false;
			});
		}
	};

	instances.set(instanceId, {
		scrollHandler,
		dotNetHelper: dotNetRef
	});

	window.addEventListener('scroll', scrollHandler, { passive: true });
}

export function scrollToTop(): void {
	window.scrollTo({ top: 0, behavior: 'smooth' });
}

export function dispose(instanceId: string): void {
	const instance = instances.get(instanceId);

	if (instance) {
		window.removeEventListener('scroll', instance.scrollHandler);
		instances.delete(instanceId);
	}
}