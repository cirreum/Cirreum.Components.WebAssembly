export function scrollNodeIntoView(treeview: string, treeviewNode: string, options: { debug?: boolean, smoothScroll?: boolean } = {}): void {

	// Set default values for options
	const defaultOptions = { debug: false, smoothScroll: false };
	const finalOptions = { ...defaultOptions, ...options };

	const container = document.getElementById(treeview);
	const node = document.getElementById(treeviewNode);

	if (container && node) {

		// Find the div inside the LI that represents the actual node content
		const nodeContent = node.querySelector('.treeview-node') as HTMLElement;
		if (!nodeContent) {
			console.log('Node content not found');
			return;
		}

		const getNodePosition = () => {
			const containerRect = container.getBoundingClientRect();
			const nodeRect = nodeContent.getBoundingClientRect();
			return {
				relativeTop: nodeRect.top - containerRect.top,
				relativeBottom: nodeRect.top - containerRect.top + nodeRect.height
			};
		};

		const buffer = 4;
		const visibleHeight = container.clientHeight;

		const logScrollInfo = (prefix: string = '') => {
			if (!finalOptions.debug) return;
			const { relativeTop, relativeBottom } = getNodePosition();
			console.log(`${prefix}Scroll calculation:`);
			console.log('	relativeTop:', relativeTop);
			console.log('	relativeBottom:', relativeBottom);
			console.log('	nodeHeight:', nodeContent.offsetHeight);
			console.log('	containerHeight:', visibleHeight);
			console.log('	currentScrollTop:', container.scrollTop);
		};

		logScrollInfo('Initial ');

		const { relativeTop, relativeBottom } = getNodePosition();

		if (relativeTop >= buffer && relativeBottom <= (visibleHeight - buffer)) {
			if (finalOptions.debug) console.log('Node is fully visible, no scrolling needed.');
			return;
		}

		let newScrollTop = container.scrollTop;

		if (relativeTop < buffer) {
			newScrollTop = container.scrollTop + relativeTop - buffer;
		} else if (relativeBottom > visibleHeight - buffer) {
			newScrollTop = container.scrollTop + (relativeBottom - visibleHeight + buffer);
		}

		newScrollTop = Math.max(0, Math.min(newScrollTop, container.scrollHeight - visibleHeight));

		if (Math.abs(newScrollTop - container.scrollTop) > 1) {
			if (finalOptions.debug) console.log('Scrolling to:', newScrollTop);
			if (finalOptions.smoothScroll) {
				container.scrollTo({
					top: newScrollTop,
					behavior: 'smooth'
				});
			} else {
				container.scrollTop = newScrollTop;
			}
		} else if (finalOptions.debug) {
			console.log('Scroll change too small, no action taken.');
		}

	}
}