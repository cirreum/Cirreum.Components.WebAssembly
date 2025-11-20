const instances = new Map();
export function initialize(instanceId, dotNetRef, threshold) {
    dispose(instanceId);
    const scrollHandler = () => {
        const visible = window.scrollY > threshold;
        dotNetRef.invokeMethodAsync('UpdateVisibility', visible);
    };
    instances.set(instanceId, {
        scrollHandler,
        dotNetHelper: dotNetRef
    });
    window.addEventListener('scroll', scrollHandler);
}
export function scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
}
export function dispose(instanceId) {
    const instance = instances.get(instanceId);
    if (instance) {
        window.removeEventListener('scroll', instance.scrollHandler);
        instances.delete(instanceId);
    }
}
