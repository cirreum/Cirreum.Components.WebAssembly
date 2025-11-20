interface IDotNetHelper {
    invokeMethodAsync(methodName: string, ...args: any[]): Promise<any>;
}

interface ScrollInstance {
    scrollHandler: () => void;
    dotNetHelper: IDotNetHelper;
}

const instances = new Map<string, ScrollInstance>();

export function initialize(instanceId: string, dotNetRef: IDotNetHelper, threshold: number): void {
    // Clean up any existing instance with this ID
    dispose(instanceId);

    const scrollHandler = (): void => {
        const visible: boolean = window.scrollY > threshold;
        dotNetRef.invokeMethodAsync('UpdateVisibility', visible);
    };

    instances.set(instanceId, {
        scrollHandler,
        dotNetHelper: dotNetRef
    });

    window.addEventListener('scroll', scrollHandler);
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