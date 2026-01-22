const scrollbarBehaviors = new Map();
var ScrollbarState;
(function (ScrollbarState) {
    ScrollbarState[ScrollbarState["Hiding"] = 0] = "Hiding";
    ScrollbarState[ScrollbarState["Hidden"] = 1] = "Hidden";
    ScrollbarState[ScrollbarState["Showing"] = 2] = "Showing";
    ScrollbarState[ScrollbarState["Shown"] = 3] = "Shown";
    ScrollbarState[ScrollbarState["Sleeping"] = 4] = "Sleeping";
})(ScrollbarState || (ScrollbarState = {}));
export function addScrollbarBehavior(selector, showDelay, hideDelay) {
    const control = document.querySelector(selector);
    if (!(control instanceof HTMLElement)) {
        return;
    }
    removeScrollbarBehavior(selector);
    const controller = new AbortController();
    const signal = controller.signal;
    let state = ScrollbarState.Hidden;
    let pendingTimeout = null;
    const entry = {
        controller,
        get pendingTimeout() { return pendingTimeout; },
        set pendingTimeout(val) { pendingTimeout = val; }
    };
    const clearPendingTimeout = () => {
        if (pendingTimeout !== null) {
            clearTimeout(pendingTimeout);
            pendingTimeout = null;
        }
    };
    const mouseEnterHandler = () => {
        if (state === ScrollbarState.Showing || state === ScrollbarState.Shown)
            return;
        clearPendingTimeout();
        state = ScrollbarState.Sleeping;
        pendingTimeout = window.setTimeout(() => {
            if (state === ScrollbarState.Sleeping && !signal.aborted) {
                state = ScrollbarState.Showing;
                control.dataset.scrollbar = 'show';
                state = ScrollbarState.Shown;
            }
        }, showDelay);
    };
    const mouseLeaveHandler = () => {
        if (state === ScrollbarState.Hiding || state === ScrollbarState.Hidden)
            return;
        clearPendingTimeout();
        if (state === ScrollbarState.Sleeping) {
            state = ScrollbarState.Hidden;
        }
        else {
            state = ScrollbarState.Sleeping;
            const delay = hideDelay > 10 ? hideDelay : 0;
            pendingTimeout = window.setTimeout(() => {
                if (state === ScrollbarState.Sleeping && !signal.aborted) {
                    state = ScrollbarState.Hiding;
                    control.dataset.scrollbar = 'hide';
                    state = ScrollbarState.Hidden;
                }
            }, delay);
        }
    };
    control.addEventListener('mouseenter', mouseEnterHandler, { signal });
    control.addEventListener('mouseleave', mouseLeaveHandler, { signal });
    control.dataset.scrollbar = 'hide';
    scrollbarBehaviors.set(selector, entry);
}
export function removeScrollbarBehavior(selector, preserveDataAttribute = false) {
    const entry = scrollbarBehaviors.get(selector);
    if (entry) {
        if (entry.pendingTimeout !== null) {
            clearTimeout(entry.pendingTimeout);
        }
        entry.controller.abort();
        scrollbarBehaviors.delete(selector);
    }
    if (!preserveDataAttribute) {
        const control = document.querySelector(selector);
        if (control instanceof HTMLElement) {
            delete control.dataset.scrollbar;
        }
    }
}
