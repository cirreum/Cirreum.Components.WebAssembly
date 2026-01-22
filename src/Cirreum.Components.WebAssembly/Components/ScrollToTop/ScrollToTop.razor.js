const instances = new Map();
export function initialize(instanceId, dotNetRef, threshold) {
    dispose(instanceId);
    let ticking = false;
    const scrollHandler = () => {
        if (!ticking) {
            ticking = true;
            requestAnimationFrame(() => {
                const visible = window.scrollY > threshold;
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
