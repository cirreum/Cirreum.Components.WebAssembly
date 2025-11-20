class DataGrid {
    tableElement;
    options;
    columnStates = new Map();
    resizeObserver;
    dotNetRef;
    isDisposed = false;
    features = {
        columnResizeEnabled: false,
        keyboardNavigationEnabled: false,
        footerResizerEnabled: false
    };
    boundHandlers = {
        pointerDown: this.handlePointerDown.bind(this),
        doubleClick: this.handleDoubleClick.bind(this),
        keyDown: this.handleRowKeyDown.bind(this),
        resizeObserver: this.updateFooterJustifyContent.bind(this)
    };
    constructor(tableElement, options = {}) {
        this.tableElement = tableElement;
        const { keyDownHandler, ...config } = options;
        this.options = {
            minColumnWidth: 16,
            maxColumnBuffer: 16,
            ...config
        };
        this.initializeColumns();
        if (keyDownHandler) {
            this.addRowKeyDownHandler(keyDownHandler);
        }
    }
    initializeColumns() {
        const visibleColumns = this.getVisibleColumns();
        visibleColumns.forEach(column => {
            const columnState = this.createColumnState(column);
            this.columnStates.set(column.id, columnState);
        });
    }
    createColumnState(column) {
        const contentMinWidth = this.calculateHeaderMinContentWidth(column);
        const userMinWidth = this.getUserDefinedMinWidth(column);
        const effectiveMinWidth = Math.max(contentMinWidth, userMinWidth, this.options.minColumnWidth);
        const initialWidth = this.calculateInitialWidth(column, effectiveMinWidth);
        column.style.width = `${initialWidth}px`;
        const actualWidth = parseInt(getComputedStyle(column).width, 10);
        if (actualWidth !== initialWidth) {
            column.style.width = `${actualWidth}px`;
        }
        return {
            currentWidth: actualWidth,
            minContentWidth: effectiveMinWidth,
            isVisible: true
        };
    }
    calculateInitialWidth(column, minWidth) {
        const existingWidth = column.style.width;
        if (existingWidth && parseInt(existingWidth, 10) > 0) {
            return Math.max(parseInt(existingWidth, 10), minWidth);
        }
        const currentWidth = column.getBoundingClientRect().width;
        return Math.max(currentWidth, minWidth);
    }
    calculateHeaderMinContentWidth(column) {
        const clone = column.cloneNode(true);
        Object.assign(clone.style, {
            position: 'absolute',
            visibility: 'hidden',
            width: 'auto',
            minWidth: '0',
            maxWidth: 'none',
            whiteSpace: 'nowrap',
            padding: getComputedStyle(column).padding
        });
        const resizeHandle = clone.querySelector('.col-width-draghandle');
        resizeHandle?.remove();
        document.body.appendChild(clone);
        const contentWidth = clone.scrollWidth;
        document.body.removeChild(clone);
        return Math.ceil(contentWidth) + this.options.maxColumnBuffer;
    }
    getUserDefinedMinWidth(column) {
        const styleMinWidth = column.style.minWidth;
        if (styleMinWidth && !['auto', '0px', '0'].includes(styleMinWidth)) {
            return parseInt(styleMinWidth, 10) || 0;
        }
        return 0;
    }
    attachResizeHandlers() {
        if (!this.tableElement.tHead) {
            console.warn('DataGrid: Table has no thead element');
            return;
        }
        const handles = this.tableElement.tHead.querySelectorAll('.col-width-draghandle');
        handles.forEach(handle => {
            const column = handle.parentElement;
            const columnState = this.columnStates.get(column.id);
            if (!columnState) {
                console.warn(`DataGrid: No column state found for ${column.id}`);
                return;
            }
            handle._tableRef = this.tableElement;
            handle._columnRef = column;
            handle._columnState = columnState;
            handle.addEventListener('pointerdown', this.boundHandlers.pointerDown);
            handle.addEventListener('dblclick', this.boundHandlers.doubleClick);
        });
    }
    handlePointerDown(event) {
        if (this.isDisposed)
            return;
        event.preventDefault();
        event.stopPropagation();
        const handle = event.currentTarget;
        handle.classList.add('col-width-resizing');
        const originalTarget = event.target;
        originalTarget.setPointerCapture(event.pointerId);
        const startWidth = parseInt(getComputedStyle(handle._columnRef).width, 10);
        const startX = event.pageX;
        const handleMove = (evt) => {
            evt.preventDefault();
            const calculatedWidth = startWidth + evt.pageX - startX;
            const newWidth = Math.max(calculatedWidth, handle._columnState.minContentWidth);
            handle._columnRef.style.width = `${newWidth}px`;
        };
        const handleEnd = (evt) => {
            originalTarget.releasePointerCapture(evt.pointerId);
            document.removeEventListener('pointermove', handleMove);
            document.removeEventListener('pointerup', handleEnd);
            document.removeEventListener('pointercancel', handleEnd);
            const finalWidth = parseInt(getComputedStyle(handle._columnRef).width, 10);
            handle._columnState.currentWidth = finalWidth;
            this.compensateOtherColumns(handle._columnRef.id);
            handle.classList.remove('col-width-resizing');
        };
        document.addEventListener('pointermove', handleMove);
        document.addEventListener('pointerup', handleEnd);
        document.addEventListener('pointercancel', handleEnd);
    }
    handleDoubleClick(event) {
        if (this.isDisposed)
            return;
        event.preventDefault();
        event.stopPropagation();
        const handle = event.currentTarget;
        if (event.ctrlKey) {
            this.autoSizeAllColumns();
        }
        else {
            this.autoSizeColumn(handle);
        }
    }
    autoSizeColumn(handle) {
        const column = handle._columnRef;
        const columnState = handle._columnState;
        const optimalWidth = this.calculateOptimalColumnWidth(column);
        const finalWidth = Math.max(columnState.minContentWidth, optimalWidth);
        column.style.width = `${finalWidth}px`;
        columnState.currentWidth = finalWidth;
        this.compensateOtherColumns(column.id);
    }
    autoSizeAllColumns() {
        const visibleColumns = this.getVisibleColumns();
        visibleColumns.forEach(column => {
            const columnState = this.columnStates.get(column.id);
            if (columnState) {
                const optimalWidth = this.calculateOptimalColumnWidth(column);
                const finalWidth = Math.max(columnState.minContentWidth, optimalWidth);
                column.style.width = `${finalWidth}px`;
                columnState.currentWidth = finalWidth;
            }
        });
    }
    calculateOptimalColumnWidth(column) {
        const columnIndex = Array.from(column.parentElement.children).indexOf(column);
        let maxWidth = this.calculateHeaderMinContentWidth(column);
        const rows = this.tableElement.querySelectorAll('tbody tr');
        rows.forEach(row => {
            const cell = row.children[columnIndex];
            if (cell) {
                const cellWidth = this.calculateCellContentWidth(cell);
                maxWidth = Math.max(maxWidth, cellWidth);
            }
        });
        return Math.ceil(maxWidth) + this.options.maxColumnBuffer;
    }
    calculateCellContentWidth(cell) {
        const clone = cell.cloneNode(true);
        Object.assign(clone.style, {
            position: 'absolute',
            visibility: 'hidden',
            width: 'auto',
            minWidth: '0',
            maxWidth: 'none',
            whiteSpace: 'nowrap',
            padding: getComputedStyle(cell).padding,
            fontSize: getComputedStyle(cell).fontSize,
            fontFamily: getComputedStyle(cell).fontFamily,
            fontWeight: getComputedStyle(cell).fontWeight
        });
        document.body.appendChild(clone);
        const contentWidth = clone.scrollWidth;
        document.body.removeChild(clone);
        return contentWidth;
    }
    compensateOtherColumns(excludeColumnId) {
        const visibleColumns = this.getVisibleColumns();
        visibleColumns.forEach(column => {
            if (column.id !== excludeColumnId) {
                const columnState = this.columnStates.get(column.id);
                if (columnState) {
                    const actualWidth = parseInt(getComputedStyle(column).width, 10);
                    if (columnState.currentWidth !== actualWidth) {
                        column.style.width = `${columnState.currentWidth}px`;
                    }
                }
            }
        });
    }
    initializeFooterResizer() {
        const footer = this.tableElement.querySelector('tfoot');
        if (!footer)
            return;
        if (!this.resizeObserver) {
            this.resizeObserver = new ResizeObserver(entries => {
                entries.forEach(entry => {
                    this.updateFooterJustifyContent(entry.target);
                });
            });
        }
        this.resizeObserver.observe(footer);
        this.updateFooterJustifyContent(footer);
    }
    updateFooterJustifyContent(container) {
        const gapOffset = 32;
        const items = Array.from(container.children);
        const totalWidth = items.reduce((acc, item) => acc + item.offsetWidth, gapOffset);
        const containerWidth = container.offsetWidth;
        const justifyValue = totalWidth > containerWidth ? 'space-around' : 'space-between';
        container.style.setProperty('--align-items', justifyValue);
    }
    setDotNetReference(dotNetRef) {
        this.dotNetRef = dotNetRef;
    }
    attachKeyboardHandlers() {
        const tbody = this.tableElement.tBodies.item(0);
        if (tbody && this.dotNetRef) {
            tbody.addEventListener('keydown', this.boundHandlers.keyDown);
        }
    }
    handleRowKeyDown(event) {
        if (this.isDisposed || !this.dotNetRef)
            return;
        const keyActions = {
            'PageDown': () => this.dotNetRef.invokeMethodAsync('HandlePageDown'),
            'PageUp': () => this.dotNetRef.invokeMethodAsync('HandlePageUp'),
            'ArrowDown': () => this.dotNetRef.invokeMethodAsync('HandleArrowDown', event.shiftKey),
            'ArrowUp': () => this.dotNetRef.invokeMethodAsync('HandleArrowUp', event.shiftKey),
            'Escape': () => this.dotNetRef.invokeMethodAsync('HandleUnselectAll'),
            'Home': () => this.dotNetRef.invokeMethodAsync('HandleHome', event.ctrlKey),
            'End': () => this.dotNetRef.invokeMethodAsync('HandleEnd', event.ctrlKey)
        };
        if ((event.key === 'a' || event.key === 'A') && event.ctrlKey) {
            keyActions['SelectAll'] = () => this.dotNetRef.invokeMethodAsync('HandleSelectAll');
        }
        const action = keyActions[event.key] || keyActions['SelectAll'];
        if (action) {
            event.preventDefault();
            event.stopPropagation();
            action();
        }
    }
    addColumnResizeHandlers() {
        if (this.isDisposed || this.features.columnResizeEnabled)
            return;
        this.syncColumns();
        this.attachResizeHandlers();
        this.features.columnResizeEnabled = true;
    }
    removeColumnResizeHandlers() {
        if (this.isDisposed || !this.features.columnResizeEnabled)
            return;
        const handles = this.tableElement.querySelectorAll('.col-width-draghandle');
        handles.forEach(handle => {
            handle.removeEventListener('pointerdown', this.boundHandlers.pointerDown);
            handle.removeEventListener('dblclick', this.boundHandlers.doubleClick);
        });
        this.features.columnResizeEnabled = false;
    }
    addRowKeyDownHandler(dotNetRef) {
        if (this.isDisposed || this.features.keyboardNavigationEnabled)
            return;
        this.dotNetRef = dotNetRef;
        this.attachKeyboardHandlers();
        this.features.keyboardNavigationEnabled = true;
    }
    removeRowKeyDownHandler() {
        if (this.isDisposed || !this.features.keyboardNavigationEnabled)
            return;
        const tbody = this.tableElement.tBodies.item(0);
        if (tbody) {
            tbody.removeEventListener('keydown', this.boundHandlers.keyDown);
        }
        this.dotNetRef = undefined;
        this.features.keyboardNavigationEnabled = false;
    }
    addGridFooterResizer() {
        if (this.isDisposed || this.features.footerResizerEnabled)
            return;
        this.initializeFooterResizer();
        this.features.footerResizerEnabled = true;
    }
    removeGridFooterResizer() {
        if (this.isDisposed || !this.features.footerResizerEnabled)
            return;
        const footer = this.tableElement.querySelector('tfoot');
        if (footer && this.resizeObserver) {
            this.resizeObserver.unobserve(footer);
        }
        this.features.footerResizerEnabled = false;
    }
    dispose() {
        if (this.isDisposed)
            return;
        this.removeColumnResizeHandlers();
        this.removeRowKeyDownHandler();
        this.removeGridFooterResizer();
        if (this.resizeObserver) {
            this.resizeObserver.disconnect();
            this.resizeObserver = undefined;
        }
        this.columnStates.clear();
        this.dotNetRef = undefined;
        this.isDisposed = true;
    }
    getVisibleColumns() {
        return this.tableElement.querySelectorAll('th[id]');
    }
    syncColumns() {
        if (this.isDisposed)
            return;
        const visibleColumns = this.getVisibleColumns();
        visibleColumns.forEach(column => {
            const existingState = this.columnStates.get(column.id);
            if (existingState) {
                column.style.width = `${existingState.currentWidth}px`;
                existingState.isVisible = true;
            }
            else {
                const newState = this.createColumnState(column);
                this.columnStates.set(column.id, newState);
            }
        });
        this.columnStates.forEach((state, columnId) => {
            const isInDOM = this.tableElement.querySelector(`th[id="${columnId}"]`);
            if (!isInDOM && state.isVisible) {
                state.isVisible = false;
            }
        });
    }
}
const dataGridRegistry = new WeakMap();
function getOrCreateDataGrid(tableElement, options) {
    let grid = dataGridRegistry.get(tableElement);
    if (!grid) {
        grid = new DataGrid(tableElement, options);
        dataGridRegistry.set(tableElement, grid);
    }
    return grid;
}
export function initColumns(tableElement, options) {
    getOrCreateDataGrid(tableElement, options);
}
export function addColumnResizeHandlers(tableElement) {
    const grid = getOrCreateDataGrid(tableElement);
    grid.addColumnResizeHandlers();
}
export function removeColumnResizeHandlers(tableElement) {
    const grid = dataGridRegistry.get(tableElement);
    grid?.removeColumnResizeHandlers();
}
export function addRowKeyDownHandler(tableElement, dotNetRef) {
    const grid = getOrCreateDataGrid(tableElement);
    grid.addRowKeyDownHandler(dotNetRef);
}
export function removeRowKeyDownHandler(tableElement) {
    const grid = dataGridRegistry.get(tableElement);
    grid?.removeRowKeyDownHandler();
}
export function addGridFooterResizer(tableElement) {
    const grid = getOrCreateDataGrid(tableElement);
    grid.addGridFooterResizer();
}
export function removeGridFooterResizer(tableElement) {
    const grid = dataGridRegistry.get(tableElement);
    grid?.removeGridFooterResizer();
}
export function disposeColumns(tableElement) {
    const grid = dataGridRegistry.get(tableElement);
    if (grid) {
        grid.dispose();
        dataGridRegistry.delete(tableElement);
    }
}
