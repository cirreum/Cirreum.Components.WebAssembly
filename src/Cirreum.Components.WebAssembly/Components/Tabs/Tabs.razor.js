const KEY_MAPPINGS = {
    SPACE: ' ',
    ENTER: 'Enter',
    ARROW_UP: 'ArrowUp',
    ARROW_DOWN: 'ArrowDown',
    ARROW_LEFT: 'ArrowLeft',
    ARROW_RIGHT: 'ArrowRight',
    HOME: 'Home',
    END: 'End'
};
const HANDLE_METHODS = {
    ARROW_UP: "HandleArrowUpKey",
    ARROW_DOWN: "HandleArrowDownKey",
    ARROW_LEFT: "HandleArrowLeftKey",
    ARROW_RIGHT: "HandleArrowRightKey",
    ENTER: "HandleEnterKey",
    HOME: "HandleHomeKey",
    END: "HandleEndKey"
};
class Tabs {
    tabType;
    tabsInstance;
    dotnetRef;
    scrollContainer;
    tabsElement;
    tabList;
    activeTab;
    vertical;
    scrolledDebouncer = null;
    touchStartX = 0;
    touchStartY = 0;
    resizeObserver = null;
    resizeDebouncer = null;
    MIN_SWIPE_DISTANCE = 20;
    MULTIPLIER_THRESHOLD = 150;
    MULTIPLIER_INCREASE = 3;
    constructor(tabsInstance, dotnetRef, tabType, tabPosition) {
        this.activeTab = null;
        this.resizeObserver = null;
        this.resizeDebouncer = null;
        this.tabsInstance = tabsInstance;
        this.dotnetRef = dotnetRef;
        this.tabType = tabType;
        this.vertical = tabPosition === 'Left' || tabPosition === 'Right';
        this.tabsElement = this.getTabsElement(this.tabsInstance);
        this.scrollContainer = this.getScrollContainer(this.tabsElement);
        this.tabList = this.getTabList(this.scrollContainer);
    }
    getTabsElement(parent) {
        const tabsElement = parent.querySelector('.tab-items');
        if (!tabsElement) {
            console.warn('Tabs element not found');
        }
        return tabsElement;
    }
    getScrollContainer(parent) {
        const scrollContainer = parent.querySelector('.nav-scroll-container');
        if (!scrollContainer) {
            console.warn('Scroll container not found');
        }
        return scrollContainer;
    }
    getTabList(parent) {
        return parent.querySelector('[role="tablist"]');
    }
    connect() {
        if (!this.tabsElement || !this.scrollContainer || !this.tabList) {
            throw new Error('Required elements not found');
        }
        this.scrollContainer.addEventListener('scroll', this.handleTabListScrolled.bind(this));
        this.tabsElement.addEventListener('touchstart', this.handleTouchStart.bind(this), { passive: true });
        this.tabsElement.addEventListener('touchend', this.handleTouchEnd.bind(this), { passive: true });
        this.tabsElement.addEventListener("keydown", this.handleKeydownEvent.bind(this));
        this.connectResizeObserver();
        this.setTouchAction();
    }
    disconnect() {
        this.scrollContainer.removeEventListener('scroll', this.handleTabListScrolled.bind(this));
        if (this.scrolledDebouncer) {
            clearTimeout(this.scrolledDebouncer);
        }
        this.tabsElement.removeEventListener('touchstart', this.handleTouchStart.bind(this));
        this.tabsElement.removeEventListener('touchend', this.handleTouchEnd.bind(this));
        this.tabsElement.removeEventListener("keydown", this.handleKeydownEvent.bind(this));
        this.disconnectResizeObserver();
    }
    changeTabType(newTabType) {
        this.disconnect();
        this.tabType = newTabType;
        this.tabsElement = this.getTabsElement(this.tabsInstance);
        this.scrollContainer = this.getScrollContainer(this.tabsElement);
        this.tabList = this.getTabList(this.scrollContainer);
        this.connect();
    }
    changeTabPosition(tabPosition) {
        this.disconnect();
        this.vertical = tabPosition === 'Left' || tabPosition === 'Right';
        this.tabsElement = this.getTabsElement(this.tabsInstance);
        this.scrollContainer = this.getScrollContainer(this.tabsElement);
        this.tabList = this.getTabList(this.scrollContainer);
        this.connect();
    }
    scrollTo(direction = 1, multiplier = 1) {
        const scrollProperty = this.vertical ? 'scrollTop' : 'scrollLeft';
        const startPosition = this.scrollContainer[scrollProperty];
        const baseScrollAmount = this.vertical ? 60 : 120;
        const totalScrollAmount = baseScrollAmount * multiplier * direction;
        const targetPosition = startPosition + totalScrollAmount;
        this.scrollContainer.scrollTo({
            top: this.vertical ? targetPosition : undefined,
            left: this.vertical ? undefined : targetPosition,
            behavior: 'smooth'
        });
    }
    updateTab() {
        if (!this.activeTab) {
            return;
        }
        this.ensureTabVisible();
        this.updateUnderline();
    }
    updateUnderline() {
        if (this.tabType !== 'Underlines' || !this.tabsElement)
            return;
        const navUnderline = this.tabsElement.querySelector('.nav-underline');
        if (this.activeTab && navUnderline) {
            const rect = this.activeTab.getBoundingClientRect();
            const containerRect = navUnderline.getBoundingClientRect();
            if (this.vertical) {
                const underlineHeight = parseFloat(getComputedStyle(navUnderline).getPropertyValue('--bs-nav-underline-width'));
                const top = rect.top - containerRect.top + (rect.height - underlineHeight) / 2;
                navUnderline.style.setProperty('--bs-nav-underline-top', `${top}px`);
            }
            else {
                const left = rect.left - containerRect.left + (rect.width / 2);
                navUnderline.style.setProperty('--bs-nav-underline-left', `${left}px`);
            }
            navUnderline.dataset.underlineUpdated = '';
        }
    }
    handleTabSelected(activeTab) {
        this.activeTab = activeTab;
        if (this.tabType === 'Underlines') {
            this.updateUnderline();
        }
    }
    handleTabListScrolled() {
        if (!this.tabList)
            return;
        if (this.scrolledDebouncer) {
            clearTimeout(this.scrolledDebouncer);
        }
        this.scrolledDebouncer = window.setTimeout(() => {
            this.scrolledDebouncer = null;
            const visibleIndices = this.getVisibleTabs();
            this.dotnetRef.invokeMethodAsync('HandleTabListScrolled', visibleIndices);
        }, 50);
    }
    getVisibleTabs() {
        if (!this.tabList)
            return [];
        const tabs = Array.from(this.tabList.children);
        const containerRect = this.scrollContainer.getBoundingClientRect();
        const visibleIndices = [];
        void this.scrollContainer.offsetHeight;
        void this.scrollContainer.offsetWidth;
        tabs.forEach((tab, index) => {
            const tabRect = tab.getBoundingClientRect();
            let isFullyVisible;
            if (this.vertical) {
                isFullyVisible = (tabRect.top >= containerRect.top &&
                    (tabRect.bottom - 1) <= containerRect.bottom);
            }
            else {
                isFullyVisible = (tabRect.left >= containerRect.left &&
                    (tabRect.right - 1) <= containerRect.right);
            }
            if (isFullyVisible) {
                visibleIndices.push(index);
            }
        });
        return visibleIndices;
    }
    handleResize() {
        if (this.resizeDebouncer) {
            clearTimeout(this.resizeDebouncer);
        }
        this.resizeDebouncer = window.setTimeout(() => {
            this.resizeDebouncer = null;
            this.ensureTabVisible();
            const visibleIndices = this.getVisibleTabs();
            this.dotnetRef.invokeMethodAsync('HandleTabListScrolled', visibleIndices);
            if (this.tabType === 'Underlines') {
                this.updateUnderline();
            }
        }, 150);
    }
    connectResizeObserver() {
        if (!this.scrollContainer)
            return;
        this.resizeObserver = new ResizeObserver(this.handleResize.bind(this));
        this.resizeObserver.observe(this.tabsInstance);
    }
    disconnectResizeObserver() {
        if (this.resizeObserver) {
            this.resizeObserver.disconnect();
        }
        if (this.resizeDebouncer) {
            clearTimeout(this.resizeDebouncer);
        }
    }
    ensureTabVisible() {
        if (!this.scrollContainer || !this.activeTab) {
            return;
        }
        let scrollAmount = 0;
        const tabRect = this.activeTab.getBoundingClientRect();
        const containerRect = this.scrollContainer.getBoundingClientRect();
        if (this.vertical) {
            if (tabRect.bottom > containerRect.bottom) {
                scrollAmount = tabRect.bottom - containerRect.bottom;
            }
            else if (tabRect.top < containerRect.top) {
                scrollAmount = tabRect.top - containerRect.top;
            }
            this.scrollContainer.scrollTop += scrollAmount;
        }
        else {
            if (tabRect.right > containerRect.right) {
                scrollAmount = tabRect.right - containerRect.right;
            }
            else if (tabRect.left < containerRect.left) {
                scrollAmount = tabRect.left - containerRect.left;
            }
            this.scrollContainer.scrollLeft += scrollAmount;
        }
        const buffer = 5;
        if (this.vertical) {
            this.scrollContainer.scrollTop += Math.sign(scrollAmount) * buffer;
        }
        else {
            this.scrollContainer.scrollLeft += Math.sign(scrollAmount) * buffer;
        }
    }
    handleTouchStart(event) {
        this.touchStartX = event.changedTouches[0].screenX;
        this.touchStartY = event.changedTouches[0].screenY;
    }
    handleTouchEnd(event) {
        if (event.changedTouches.length === 0)
            return;
        const touch = event.changedTouches[0];
        const swipeDistance = this.vertical
            ? touch.screenY - this.touchStartY
            : touch.screenX - this.touchStartX;
        if (Math.abs(swipeDistance) > this.MIN_SWIPE_DISTANCE) {
            const direction = swipeDistance > 0 ? -1 : 1;
            const multiplier = Math.abs(swipeDistance) > this.MULTIPLIER_THRESHOLD
                ? this.MULTIPLIER_INCREASE
                : 1;
            this.scrollTo(direction, multiplier);
        }
    }
    setTouchAction() {
        this.tabsElement.style.touchAction = this.vertical ? 'pan-x' : 'pan-y';
    }
    handleKeydownEvent(event) {
        if (event.target) {
            const targetEle = event.target;
            if (targetEle && targetEle.role !== "tab") {
                return;
            }
        }
        const key = event.key;
        if (!this.isRelevantKey(key))
            return;
        if (this.shouldPreventDefault(key)) {
            event.preventDefault();
            event.stopPropagation();
            const methodName = this.getMethodName(key);
            if (methodName) {
                this.dotnetRef.invokeMethodAsync(methodName);
            }
        }
    }
    isRelevantKey(key) {
        return Object.values(KEY_MAPPINGS).includes(key);
    }
    shouldPreventDefault(key) {
        const verticalKeys = [KEY_MAPPINGS.ARROW_UP, KEY_MAPPINGS.ARROW_DOWN];
        const horizontalKeys = [KEY_MAPPINGS.ARROW_LEFT, KEY_MAPPINGS.ARROW_RIGHT];
        const alwaysPreventKeys = [KEY_MAPPINGS.SPACE, KEY_MAPPINGS.ENTER, KEY_MAPPINGS.HOME, KEY_MAPPINGS.END];
        return ((this.vertical && verticalKeys.includes(key)) ||
            (!this.vertical && horizontalKeys.includes(key)) ||
            alwaysPreventKeys.includes(key));
    }
    getMethodName(key) {
        if (this.vertical && (key === KEY_MAPPINGS.ARROW_LEFT || key === KEY_MAPPINGS.ARROW_RIGHT)) {
            return undefined;
        }
        if (!this.vertical && (key === KEY_MAPPINGS.ARROW_UP || key === KEY_MAPPINGS.ARROW_DOWN)) {
            return undefined;
        }
        switch (key) {
            case KEY_MAPPINGS.ARROW_UP: return HANDLE_METHODS.ARROW_UP;
            case KEY_MAPPINGS.ARROW_DOWN: return HANDLE_METHODS.ARROW_DOWN;
            case KEY_MAPPINGS.ARROW_LEFT: return HANDLE_METHODS.ARROW_LEFT;
            case KEY_MAPPINGS.ARROW_RIGHT: return HANDLE_METHODS.ARROW_RIGHT;
            case KEY_MAPPINGS.ENTER:
            case KEY_MAPPINGS.SPACE: return HANDLE_METHODS.ENTER;
            case KEY_MAPPINGS.HOME: return HANDLE_METHODS.HOME;
            case KEY_MAPPINGS.END: return HANDLE_METHODS.END;
        }
    }
}
const registeredTabs = new Map();
export function connect(instance, dotnetRef, tabType, tabPosition) {
    if (!instance) {
        throw new Error("[Tabs][connect] missing [instance] HTMLElement.");
    }
    if (registeredTabs.has(instance.id)) {
        return;
    }
    const tabs = new Tabs(instance, dotnetRef, tabType, tabPosition);
    tabs.connect();
    registeredTabs.set(instance.id, tabs);
}
export function disconnect(instance) {
    if (!instance) {
        throw new Error("[Tabs][disconnect] missing [instance] HTMLElement.");
    }
    const tabs = registeredTabs.get(instance.id);
    if (tabs) {
        tabs.disconnect();
        registeredTabs.delete(instance.id);
    }
}
export function getVisibleTabs(instance) {
    if (!instance) {
        throw new Error("[Tabs][getVisibleTabs] missing [instance] HTMLElement.");
    }
    const tabs = registeredTabs.get(instance.id);
    if (tabs) {
        return tabs.getVisibleTabs();
    }
    return [];
}
export function scroll(instance, direction) {
    if (!instance) {
        throw new Error("[Tabs][scroll] missing [instance] HTMLElement.");
    }
    const tabs = registeredTabs.get(instance.id);
    if (tabs) {
        tabs.scrollTo(direction);
    }
}
export function onTabSelected(instance, activeTab) {
    if (!instance) {
        throw new Error("[Tabs][onTabSelected] missing [instance] HTMLElement.");
    }
    const tabs = registeredTabs.get(instance.id);
    if (tabs) {
        tabs.handleTabSelected(activeTab);
    }
}
export function changeTabType(instance, tabType) {
    if (!instance) {
        throw new Error("[Tabs][changeTabType] missing [instance] HTMLElement.");
    }
    const tabs = registeredTabs.get(instance.id);
    if (tabs) {
        tabs.changeTabType(tabType);
    }
}
export function changeTabPosition(instance, tabPosition) {
    if (!instance) {
        throw new Error("[Tabs][changeTabPosition] missing [instance] HTMLElement.");
    }
    const tabs = registeredTabs.get(instance.id);
    if (tabs) {
        tabs.changeTabPosition(tabPosition);
    }
}
export function update(instance) {
    if (!instance) {
        throw new Error("[Tabs][update] missing [instance] HTMLElement.");
    }
    const tabs = registeredTabs.get(instance.id);
    if (tabs) {
        tabs.updateTab();
    }
}
