// Constants
const KEY_MAPPINGS = {
	SPACE: ' ',
	ENTER: 'Enter',
	ARROW_UP: 'ArrowUp',
	ARROW_DOWN: 'ArrowDown',
	ARROW_LEFT: 'ArrowLeft',
	ARROW_RIGHT: 'ArrowRight',
	HOME: 'Home',
	END: 'End'
} as const;

type KeyMapping = typeof KEY_MAPPINGS[keyof typeof KEY_MAPPINGS];

const HANDLE_METHODS = {
	ARROW_UP: "HandleArrowUpKey",
	ARROW_DOWN: "HandleArrowDownKey",
	ARROW_LEFT: "HandleArrowLeftKey",
	ARROW_RIGHT: "HandleArrowRightKey",
	ENTER: "HandleEnterKey",
	HOME: "HandleHomeKey",
	END: "HandleEndKey"
} as const;

type HandleMethod = typeof HANDLE_METHODS[keyof typeof HANDLE_METHODS];

type DotNetReference = {
	invokeMethodAsync<T = void>(methodName: string, ...args: any[]): Promise<T>;
};

type TabType = 'Tabs' | 'Pills' | 'Underlines';
type TabPosition = 'Top' | 'Right' | 'Bottom' | 'Left';

class Tabs {
	private tabType: TabType;
	private readonly tabsInstance: HTMLElement;
	private readonly dotnetRef: DotNetReference;
	private scrollContainer: HTMLElement;
	private tabsElement: HTMLElement;
	private tabList: HTMLElement | null;
	private activeTab: HTMLElement | null;
	private vertical: boolean;
	private scrolledDebouncer: number | null = null;
	private touchStartX: number = 0;
	private touchStartY: number = 0;
	private resizeObserver: ResizeObserver | null = null;
	private resizeDebouncer: number | null = null;

	private readonly MIN_SWIPE_DISTANCE: number = 20; // Minimum distance to trigger a swipe
	private readonly MULTIPLIER_THRESHOLD = 150;
	private readonly MULTIPLIER_INCREASE = 3;

	constructor(tabsInstance: HTMLElement, dotnetRef: DotNetReference, tabType: TabType, tabPosition: TabPosition) {

		// Initialize defaults
		this.activeTab = null;
		this.resizeObserver = null;
		this.resizeDebouncer = null;
		this.tabsInstance = tabsInstance;
		this.dotnetRef = dotnetRef;
		this.tabType = tabType;

		// Assign properties
		this.vertical = tabPosition === 'Left' || tabPosition === 'Right';
		this.tabsElement = this.getTabsElement(this.tabsInstance);
		this.scrollContainer = this.getScrollContainer(this.tabsElement);
		this.tabList = this.getTabList(this.scrollContainer);

	}

	private getTabsElement(parent: HTMLElement): HTMLElement {
		const tabsElement = parent.querySelector('.tab-items');
		if (!tabsElement) {
			console.warn('Tabs element not found');
		}
		return tabsElement as HTMLElement;
	}
	private getScrollContainer(parent: HTMLElement): HTMLElement {
		const scrollContainer = parent.querySelector('.nav-scroll-container');
		if (!scrollContainer) {
			console.warn('Scroll container not found');
		}
		return scrollContainer as HTMLElement;
	}
	private getTabList(parent: HTMLElement): HTMLElement | null {
		return parent.querySelector('[role="tablist"]');
	}

	public connect(): void {

		// Validate required elements
		if (!this.tabsElement || !this.scrollContainer || !this.tabList) {
			throw new Error('Required elements not found');
		}

		// connect normal event handlers
		this.scrollContainer.addEventListener('scroll', this.handleTabListScrolled.bind(this));
		this.tabsElement.addEventListener('touchstart', this.handleTouchStart.bind(this), { passive: true });
		this.tabsElement.addEventListener('touchend', this.handleTouchEnd.bind(this), { passive: true });
		this.tabsElement.addEventListener("keydown", this.handleKeydownEvent.bind(this));

		// setup resize observer
		this.connectResizeObserver();

		// set the touch action based on orientation
		this.setTouchAction();

	}

	public disconnect(): void {

		this.scrollContainer.removeEventListener('scroll', this.handleTabListScrolled.bind(this));
		if (this.scrolledDebouncer) {
			clearTimeout(this.scrolledDebouncer);
		}

		this.tabsElement.removeEventListener('touchstart', this.handleTouchStart.bind(this));
		this.tabsElement.removeEventListener('touchend', this.handleTouchEnd.bind(this));
		this.tabsElement.removeEventListener("keydown", this.handleKeydownEvent.bind(this));

		this.disconnectResizeObserver();

	}


	public changeTabType(newTabType: TabType): void {

		this.disconnect();

		this.tabType = newTabType;

		this.tabsElement = this.getTabsElement(this.tabsInstance);
		this.scrollContainer = this.getScrollContainer(this.tabsElement);
		this.tabList = this.getTabList(this.scrollContainer);

		this.connect();

	}

	public changeTabPosition(tabPosition: TabPosition): void {

		this.disconnect();

		this.vertical = tabPosition === 'Left' || tabPosition === 'Right';

		this.tabsElement = this.getTabsElement(this.tabsInstance);
		this.scrollContainer = this.getScrollContainer(this.tabsElement);
		this.tabList = this.getTabList(this.scrollContainer);

		this.connect();

	}

	public scrollTo(direction: number = 1, multiplier: number = 1): void {

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

	public updateTab() {
		if (!this.activeTab) {
			return;
		}
		this.ensureTabVisible();
		this.updateUnderline();
	}

	public updateUnderline(): void {
		if (this.tabType !== 'Underlines' || !this.tabsElement) return;

		const navUnderline = this.tabsElement.querySelector('.nav-underline') as HTMLElement;

		if (this.activeTab && navUnderline) {
			const rect = this.activeTab.getBoundingClientRect();
			const containerRect = navUnderline.getBoundingClientRect();

			if (this.vertical) {
				const underlineHeight = parseFloat(getComputedStyle(navUnderline).getPropertyValue('--bs-nav-underline-width'));
				const top = rect.top - containerRect.top + (rect.height - underlineHeight) / 2;
				navUnderline.style.setProperty('--bs-nav-underline-top', `${top}px`);
			} else {
				const left = rect.left - containerRect.left + (rect.width / 2);
				navUnderline.style.setProperty('--bs-nav-underline-left', `${left}px`);
			}

			navUnderline.dataset.underlineUpdated = ''; // This triggers the transition
		}
	}


	public handleTabSelected(activeTab: HTMLElement) {
		this.activeTab = activeTab;
		if (this.tabType === 'Underlines') {
			this.updateUnderline();
		}
	}

	private handleTabListScrolled(): void {
		if (!this.tabList) return;
		if (this.scrolledDebouncer) {
			clearTimeout(this.scrolledDebouncer);
		}
		this.scrolledDebouncer = window.setTimeout(() => {
			this.scrolledDebouncer = null;
			const visibleIndices = this.getVisibleTabs();
			this.dotnetRef.invokeMethodAsync('HandleTabListScrolled', visibleIndices);
		}, 50);
	}
	public getVisibleTabs(): number[] {
		if (!this.tabList) return [];

		const tabs = Array.from(this.tabList.children) as HTMLElement[];
		const containerRect = this.scrollContainer.getBoundingClientRect();
		const visibleIndices: number[] = [];

		// Force layout recalculation
		void this.scrollContainer.offsetHeight;
		void this.scrollContainer.offsetWidth;

		tabs.forEach((tab, index) => {

			const tabRect = tab.getBoundingClientRect();
			let isFullyVisible: boolean;

			if (this.vertical) {
				isFullyVisible = (
					tabRect.top >= containerRect.top &&
					(tabRect.bottom - 1) <= containerRect.bottom
				);
			} else {
				isFullyVisible = (
					tabRect.left >= containerRect.left &&
					(tabRect.right - 1) <= containerRect.right
				);
			}

			if (isFullyVisible) {
				visibleIndices.push(index);
			}

		});

		return visibleIndices;

	}

	private handleResize(): void {
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
	private connectResizeObserver(): void {
		if (!this.scrollContainer) return;

		this.resizeObserver = new ResizeObserver(this.handleResize.bind(this));
		this.resizeObserver.observe(this.tabsInstance);
	}
	private disconnectResizeObserver(): void {
		if (this.resizeObserver) {
			this.resizeObserver.disconnect();
		}

		if (this.resizeDebouncer) {
			clearTimeout(this.resizeDebouncer);
		}
	}
	private ensureTabVisible(): void {
		if (!this.scrollContainer || !this.activeTab) {
			return;
		}

		let scrollAmount = 0;

		const tabRect = this.activeTab.getBoundingClientRect();
		const containerRect = this.scrollContainer.getBoundingClientRect();

		if (this.vertical) {
			// Vertical scrolling
			if (tabRect.bottom > containerRect.bottom) {
				// Scroll down
				scrollAmount = tabRect.bottom - containerRect.bottom;
			} else if (tabRect.top < containerRect.top) {
				// Scroll up
				scrollAmount = tabRect.top - containerRect.top;
			}
			this.scrollContainer.scrollTop += scrollAmount;
		} else {
			// Horizontal scrolling
			if (tabRect.right > containerRect.right) {
				// Scroll right
				scrollAmount = tabRect.right - containerRect.right;
			} else if (tabRect.left < containerRect.left) {
				// Scroll left
				scrollAmount = tabRect.left - containerRect.left;
			}
			this.scrollContainer.scrollLeft += scrollAmount;
		}

		// Optionally, you can add a small buffer to ensure the tab is fully visible
		const buffer = 5; // 5 pixels buffer
		if (this.vertical) {
			this.scrollContainer.scrollTop += Math.sign(scrollAmount) * buffer;
		} else {
			this.scrollContainer.scrollLeft += Math.sign(scrollAmount) * buffer;
		}

	}

	private handleTouchStart(event: TouchEvent): void {
		this.touchStartX = event.changedTouches[0].screenX;
		this.touchStartY = event.changedTouches[0].screenY;
	}
	private handleTouchEnd(event: TouchEvent): void {
		if (event.changedTouches.length === 0) return;

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
	private setTouchAction(): void {
		this.tabsElement.style.touchAction = this.vertical ? 'pan-x' : 'pan-y';
	}

	private handleKeydownEvent(event: KeyboardEvent): void {

		if (event.target) {
			const targetEle = event.target as HTMLElement;
			if (targetEle && targetEle.role !== "tab") {
				return;
			}
		}

		const key = event.key as KeyMapping;

		if (!this.isRelevantKey(key)) return;

		if (this.shouldPreventDefault(key)) {
			event.preventDefault();
			event.stopPropagation();

			const methodName = this.getMethodName(key);
			if (methodName) {
				this.dotnetRef.invokeMethodAsync(methodName);
			}
		}

	}
	private isRelevantKey(key: string): key is KeyMapping {
		return Object.values(KEY_MAPPINGS).includes(key as KeyMapping);
	}
	private shouldPreventDefault(key: KeyMapping): boolean {
		const verticalKeys: KeyMapping[] = [KEY_MAPPINGS.ARROW_UP, KEY_MAPPINGS.ARROW_DOWN];
		const horizontalKeys: KeyMapping[] = [KEY_MAPPINGS.ARROW_LEFT, KEY_MAPPINGS.ARROW_RIGHT];
		const alwaysPreventKeys: KeyMapping[] = [KEY_MAPPINGS.SPACE, KEY_MAPPINGS.ENTER, KEY_MAPPINGS.HOME, KEY_MAPPINGS.END];

		return (
			(this.vertical && verticalKeys.includes(key)) ||
			(!this.vertical && horizontalKeys.includes(key)) ||
			alwaysPreventKeys.includes(key)
		);
	}
	private getMethodName(key: KeyMapping): HandleMethod | undefined {

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


const registeredTabs = new Map<string, Tabs>();

export function connect(instance: HTMLElement, dotnetRef: DotNetReference, tabType: TabType, tabPosition: TabPosition): void {
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

export function disconnect(instance: HTMLElement): void {
	if (!instance) {
		throw new Error("[Tabs][disconnect] missing [instance] HTMLElement.");
	}
	const tabs = registeredTabs.get(instance.id);
	if (tabs) {
		tabs.disconnect();
		registeredTabs.delete(instance.id);
	}
}

export function getVisibleTabs(instance: HTMLElement): number[] {
	if (!instance) {
		throw new Error("[Tabs][getVisibleTabs] missing [instance] HTMLElement.");
	}
	const tabs = registeredTabs.get(instance.id);
	if (tabs) {
		return tabs.getVisibleTabs();
	}
	return [];
}

export function scroll(instance: HTMLElement, direction: number): void {
	if (!instance) {
		throw new Error("[Tabs][scroll] missing [instance] HTMLElement.");
	}
	const tabs = registeredTabs.get(instance.id);
	if (tabs) {
		tabs.scrollTo(direction);
	}
}

export function onTabSelected(instance: HTMLElement, activeTab: HTMLElement): void {
	if (!instance) {
		throw new Error("[Tabs][onTabSelected] missing [instance] HTMLElement.");
	}
	const tabs = registeredTabs.get(instance.id) as Tabs;
	if (tabs) {
		tabs.handleTabSelected(activeTab);
	}
}

export function changeTabType(instance: HTMLElement, tabType: TabType): void {
	if (!instance) {
		throw new Error("[Tabs][changeTabType] missing [instance] HTMLElement.");
	}
	const tabs = registeredTabs.get(instance.id) as Tabs;
	if (tabs) {
		tabs.changeTabType(tabType);
	}
}

export function changeTabPosition(instance: HTMLElement, tabPosition: TabPosition): void {
	if (!instance) {
		throw new Error("[Tabs][changeTabPosition] missing [instance] HTMLElement.");
	}
	const tabs = registeredTabs.get(instance.id);
	if (tabs) {
		tabs.changeTabPosition(tabPosition);
	}
}

export function update(instance: HTMLElement): void {
	if (!instance) {
		throw new Error("[Tabs][update] missing [instance] HTMLElement.");
	}
	const tabs = registeredTabs.get(instance.id);
	if (tabs) {
		tabs.updateTab();
	}
}
