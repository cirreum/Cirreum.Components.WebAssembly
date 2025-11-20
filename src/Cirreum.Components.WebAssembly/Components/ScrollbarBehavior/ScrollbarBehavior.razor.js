function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}
var ScrollbarState;
(function (ScrollbarState) {
    ScrollbarState[ScrollbarState["Hiding"] = 0] = "Hiding";
    ScrollbarState[ScrollbarState["Hidden"] = 1] = "Hidden";
    ScrollbarState[ScrollbarState["Showing"] = 2] = "Showing";
    ScrollbarState[ScrollbarState["Shown"] = 3] = "Shown";
    ScrollbarState[ScrollbarState["Sleeping"] = 4] = "Sleeping";
})(ScrollbarState || (ScrollbarState = {}));
export function addScrollbarBehavior(selector, showDelay, hideDelay) {
    const control = document.documentElement.querySelector(selector);
    if (control) {
        let state = ScrollbarState.Hidden;
        const mouseEnterHandler = async (e) => {
            if (state === ScrollbarState.Showing || state === ScrollbarState.Shown)
                return;
            if (state === ScrollbarState.Sleeping)
                return;
            state = ScrollbarState.Sleeping;
            await sleep(showDelay);
            if (state === ScrollbarState.Sleeping) {
                state = ScrollbarState.Showing;
                control.dataset.scrollbar = 'show';
                state = ScrollbarState.Shown;
            }
        };
        const mouseLeaveHandler = async (e) => {
            if (state === ScrollbarState.Hiding || state === ScrollbarState.Hidden)
                return;
            if (state === ScrollbarState.Sleeping) {
                state = ScrollbarState.Hidden;
            }
            else {
                state = ScrollbarState.Sleeping;
                if (hideDelay > 10) {
                    await sleep(hideDelay);
                }
                if (state === ScrollbarState.Sleeping) {
                    state = ScrollbarState.Hiding;
                    control.dataset.scrollbar = 'hide';
                    state = ScrollbarState.Hidden;
                }
            }
        };
        control.addEventListener('mouseenter', mouseEnterHandler);
        control.addEventListener('mouseleave', mouseLeaveHandler);
        control._mouseenterHandler = mouseEnterHandler;
        control._mouseleaveHandler = mouseLeaveHandler;
        control.dataset.scrollbar = 'hide';
    }
}
export function removeScrollbarBehavior(selector, preserveDataAttribute = false) {
    const control = document.documentElement.querySelector(selector);
    if (control) {
        if (control._mouseenterHandler) {
            control.removeEventListener('mouseenter', control._mouseenterHandler);
            delete control._mouseenterHandler;
        }
        if (control._mouseleaveHandler) {
            control.removeEventListener('mouseleave', control._mouseleaveHandler);
            delete control._mouseleaveHandler;
        }
        if (!preserveDataAttribute) {
            delete control.dataset.scrollbar;
        }
    }
}
