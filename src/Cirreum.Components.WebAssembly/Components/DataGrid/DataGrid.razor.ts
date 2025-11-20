
interface DotNetReference {
    invokeMethodAsync(methodName: string, ...args: any[]): Promise<any>;
}

interface DataGridConfig {
    minColumnWidth?: number;
    maxColumnBuffer?: number;
}

interface DataGridOptions extends DataGridConfig {
    keyDownHandler?: DotNetReference;
}

interface ColumnState {
    currentWidth: number;
    minContentWidth: number;
    isVisible: boolean;
}

interface ResizeHandle extends HTMLElement {
    _tableRef: HTMLTableElement;
    _columnRef: HTMLTableCellElement;
    _columnState: ColumnState;
}

class DataGrid {
    private tableElement: HTMLTableElement;
    private options: Required<DataGridConfig>;
    private columnStates = new Map<string, ColumnState>();
    private resizeObserver?: ResizeObserver;
    private dotNetRef?: DotNetReference;
    private isDisposed = false;

    // Feature state tracking
    private features = {
        columnResizeEnabled: false,
        keyboardNavigationEnabled: false,
        footerResizerEnabled: false
    };

    // Event handler references for cleanup
    private boundHandlers = {
        pointerDown: this.handlePointerDown.bind(this),
        doubleClick: this.handleDoubleClick.bind(this),
        keyDown: this.handleRowKeyDown.bind(this),
        resizeObserver: this.updateFooterJustifyContent.bind(this)
    };

    constructor(tableElement: HTMLTableElement, options: DataGridOptions = {}) {
        this.tableElement = tableElement;
        const { keyDownHandler, ...config } = options;

        this.options = {
            minColumnWidth: 16,
            maxColumnBuffer: 16,
            ...config
        };

        // Only initialize columns state, don't attach handlers automatically
        this.initializeColumns();

        // Set up keyboard handler if provided
        if (keyDownHandler) {
            this.addRowKeyDownHandler(keyDownHandler);
        }
    }

    // Column Management
    private initializeColumns(): void {
        const visibleColumns = this.getVisibleColumns();

        visibleColumns.forEach(column => {
            const columnState = this.createColumnState(column);
            this.columnStates.set(column.id, columnState);
        });
    }
    private createColumnState(column: HTMLTableCellElement): ColumnState {
        const contentMinWidth = this.calculateHeaderMinContentWidth(column);
        const userMinWidth = this.getUserDefinedMinWidth(column);
        const effectiveMinWidth = Math.max(
            contentMinWidth,
            userMinWidth,
            this.options.minColumnWidth
        );

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
    private calculateInitialWidth(column: HTMLTableCellElement, minWidth: number): number {
        const existingWidth = column.style.width;

        if (existingWidth && parseInt(existingWidth, 10) > 0) {
            return Math.max(parseInt(existingWidth, 10), minWidth);
        }

        const currentWidth = column.getBoundingClientRect().width;
        return Math.max(currentWidth, minWidth);
    }
    private calculateHeaderMinContentWidth(column: HTMLTableCellElement): number {
        const clone = column.cloneNode(true) as HTMLTableCellElement;

        // Style for measurement
        Object.assign(clone.style, {
            position: 'absolute',
            visibility: 'hidden',
            width: 'auto',
            minWidth: '0',
            maxWidth: 'none',
            whiteSpace: 'nowrap',
            padding: getComputedStyle(column).padding
        });

        // Remove resize handle
        const resizeHandle = clone.querySelector('.col-width-draghandle');
        resizeHandle?.remove();

        document.body.appendChild(clone);
        const contentWidth = clone.scrollWidth;
        document.body.removeChild(clone);

        return Math.ceil(contentWidth) + this.options.maxColumnBuffer;
    }
    private getUserDefinedMinWidth(column: HTMLTableCellElement): number {
        const styleMinWidth = column.style.minWidth;
        if (styleMinWidth && !['auto', '0px', '0'].includes(styleMinWidth)) {
            return parseInt(styleMinWidth, 10) || 0;
        }
        return 0;
    }

    // Resize Handling
    private attachResizeHandlers(): void {
        if (!this.tableElement.tHead) {
            console.warn('DataGrid: Table has no thead element');
            return;
        }

        const handles = this.tableElement.tHead.querySelectorAll<ResizeHandle>('.col-width-draghandle');

        handles.forEach(handle => {
            const column = handle.parentElement as HTMLTableCellElement;
            const columnState = this.columnStates.get(column.id);

            if (!columnState) {
                console.warn(`DataGrid: No column state found for ${column.id}`);
                return;
            }

            // Attach references
            handle._tableRef = this.tableElement;
            handle._columnRef = column;
            handle._columnState = columnState;

            handle.addEventListener('pointerdown', this.boundHandlers.pointerDown);
            handle.addEventListener('dblclick', this.boundHandlers.doubleClick);
        });
    }
    private handlePointerDown(event: PointerEvent): void {
        if (this.isDisposed) return;

        event.preventDefault();
        event.stopPropagation();

        const handle = event.currentTarget as ResizeHandle;
        handle.classList.add('col-width-resizing');

        const originalTarget = event.target as Element;
        originalTarget.setPointerCapture(event.pointerId);

        const startWidth = parseInt(getComputedStyle(handle._columnRef).width, 10);
        const startX = event.pageX;

        const handleMove = (evt: PointerEvent) => {
            evt.preventDefault();
            const calculatedWidth = startWidth + evt.pageX - startX;
            const newWidth = Math.max(calculatedWidth, handle._columnState.minContentWidth);
            handle._columnRef.style.width = `${newWidth}px`;
        };

        const handleEnd = (evt: PointerEvent) => {
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
    private handleDoubleClick(event: MouseEvent): void {
        if (this.isDisposed) return;

        event.preventDefault();
        event.stopPropagation();

        const handle = event.currentTarget as ResizeHandle;

        if (event.ctrlKey) {
            // Ctrl+Double-click: Auto-size ALL columns
            this.autoSizeAllColumns();
        } else {
            // Regular double-click: Auto-size just this column
            this.autoSizeColumn(handle);
        }

    }
    private autoSizeColumn(handle: ResizeHandle): void {

        const column = handle._columnRef;
        const columnState = handle._columnState;

        // Calculate the optimal width based on content
        const optimalWidth = this.calculateOptimalColumnWidth(column);
        // Make sure we don't go smaller than our min-width
        const finalWidth = Math.max(columnState.minContentWidth, optimalWidth);

        // Apply the new width
        column.style.width = `${finalWidth}px`;
        columnState.currentWidth = finalWidth;

        // Compensate other columns to maintain their intended widths
        this.compensateOtherColumns(column.id);
    }

    private autoSizeAllColumns(): void {

        const visibleColumns = this.getVisibleColumns();

        // Auto-size all columns without compensating between them
        visibleColumns.forEach(column => {
            const columnState = this.columnStates.get(column.id);
            if (columnState) {

                const optimalWidth = this.calculateOptimalColumnWidth(column);

                // Make sure we don't go smaller than our min-width
                const finalWidth = Math.max(columnState.minContentWidth, optimalWidth);

                column.style.width = `${finalWidth}px`;
                columnState.currentWidth = finalWidth;
            }
        });

    }
    private calculateOptimalColumnWidth(column: HTMLTableCellElement): number {
        const columnIndex = Array.from(column.parentElement!.children).indexOf(column);
        let maxWidth = this.calculateHeaderMinContentWidth(column);

        // Check all data cells in this column
        const rows = this.tableElement.querySelectorAll('tbody tr');

        rows.forEach(row => {
            const cell = row.children[columnIndex] as HTMLTableCellElement;
            if (cell) {
                const cellWidth = this.calculateCellContentWidth(cell);
                maxWidth = Math.max(maxWidth, cellWidth);
            }
        });

        // Add some padding for better readability
        return Math.ceil(maxWidth) + this.options.maxColumnBuffer;
    }

    private calculateCellContentWidth(cell: HTMLTableCellElement): number {
        // Create a clone to measure content
        const clone = cell.cloneNode(true) as HTMLTableCellElement;

        // Style for measurement
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
    private compensateOtherColumns(excludeColumnId: string): void {
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

    // Footer Management
    private initializeFooterResizer(): void {
        const footer = this.tableElement.querySelector('tfoot') as HTMLElement;
        if (!footer) return;

        if (!this.resizeObserver) {
            this.resizeObserver = new ResizeObserver(entries => {
                entries.forEach(entry => {
                    this.updateFooterJustifyContent(entry.target as HTMLElement);
                });
            });
        }

        this.resizeObserver.observe(footer);
        this.updateFooterJustifyContent(footer);
    }
    private updateFooterJustifyContent(container: HTMLElement): void {
        const gapOffset = 32;
        const items = Array.from(container.children) as HTMLElement[];
        const totalWidth = items.reduce((acc, item) => acc + item.offsetWidth, gapOffset);
        const containerWidth = container.offsetWidth;

        const justifyValue = totalWidth > containerWidth ? 'space-around' : 'space-between';
        container.style.setProperty('--align-items', justifyValue);
    }

    // Keyboard Navigation
    public setDotNetReference(dotNetRef: DotNetReference): void {
        this.dotNetRef = dotNetRef;
    }
    private attachKeyboardHandlers(): void {
        const tbody = this.tableElement.tBodies.item(0);
        if (tbody && this.dotNetRef) {
            tbody.addEventListener('keydown', this.boundHandlers.keyDown);
        }
    }
    private handleRowKeyDown(event: KeyboardEvent): void {
        if (this.isDisposed || !this.dotNetRef) return;

        const keyActions: Record<string, () => void> = {
            'PageDown': () => this.dotNetRef!.invokeMethodAsync('HandlePageDown'),
            'PageUp': () => this.dotNetRef!.invokeMethodAsync('HandlePageUp'),
            'ArrowDown': () => this.dotNetRef!.invokeMethodAsync('HandleArrowDown', event.shiftKey),
            'ArrowUp': () => this.dotNetRef!.invokeMethodAsync('HandleArrowUp', event.shiftKey),
            'Escape': () => this.dotNetRef!.invokeMethodAsync('HandleUnselectAll'),
            'Home': () => this.dotNetRef!.invokeMethodAsync('HandleHome', event.ctrlKey),
            'End': () => this.dotNetRef!.invokeMethodAsync('HandleEnd', event.ctrlKey)
        };

        // Handle Ctrl+A for select all
        if ((event.key === 'a' || event.key === 'A') && event.ctrlKey) {
            keyActions['SelectAll'] = () => this.dotNetRef!.invokeMethodAsync('HandleSelectAll');
        }

        const action = keyActions[event.key] || keyActions['SelectAll'];
        if (action) {
            event.preventDefault();
            event.stopPropagation();
            action();
        }
    }

    // Public API - Dynamic Feature Management
    public addColumnResizeHandlers(): void {
        if (this.isDisposed || this.features.columnResizeEnabled) return;

        this.syncColumns(); // Ensure columns are properly initialized
        this.attachResizeHandlers();
        this.features.columnResizeEnabled = true;
    }
    public removeColumnResizeHandlers(): void {
        if (this.isDisposed || !this.features.columnResizeEnabled) return;

        const handles = this.tableElement.querySelectorAll<HTMLTableCellElement>('.col-width-draghandle');
        handles.forEach(handle => {
            handle.removeEventListener('pointerdown', this.boundHandlers.pointerDown);
            handle.removeEventListener('dblclick', this.boundHandlers.doubleClick);
        });
        this.features.columnResizeEnabled = false;
    }

    public addRowKeyDownHandler(dotNetRef: DotNetReference): void {
        if (this.isDisposed || this.features.keyboardNavigationEnabled) return;

        this.dotNetRef = dotNetRef;
        this.attachKeyboardHandlers();
        this.features.keyboardNavigationEnabled = true;
    }
    public removeRowKeyDownHandler(): void {
        if (this.isDisposed || !this.features.keyboardNavigationEnabled) return;

        const tbody = this.tableElement.tBodies.item(0);
        if (tbody) {
            tbody.removeEventListener('keydown', this.boundHandlers.keyDown);
        }
        this.dotNetRef = undefined;
        this.features.keyboardNavigationEnabled = false;
    }

    public addGridFooterResizer(): void {
        if (this.isDisposed || this.features.footerResizerEnabled) return;

        this.initializeFooterResizer();
        this.features.footerResizerEnabled = true;
    }
    public removeGridFooterResizer(): void {
        if (this.isDisposed || !this.features.footerResizerEnabled) return;

        const footer = this.tableElement.querySelector('tfoot') as HTMLElement;
        if (footer && this.resizeObserver) {
            this.resizeObserver.unobserve(footer);
        }
        this.features.footerResizerEnabled = false;
    }

    // Other Public API methods
    public dispose(): void {
        if (this.isDisposed) return;

        // Remove all features
        this.removeColumnResizeHandlers();
        this.removeRowKeyDownHandler();
        this.removeGridFooterResizer();

        // Cleanup resize observer completely
        if (this.resizeObserver) {
            this.resizeObserver.disconnect();
            this.resizeObserver = undefined;
        }

        // Clear state
        this.columnStates.clear();
        this.dotNetRef = undefined;
        this.isDisposed = true;
    }

    // Utility methods
    private getVisibleColumns(): NodeListOf<HTMLTableCellElement> {
        return this.tableElement.querySelectorAll<HTMLTableCellElement>('th[id]');
    }
    private syncColumns(): void {
        if (this.isDisposed) return;

        const visibleColumns = this.getVisibleColumns();

        // Update visibility and restore/initialize columns
        visibleColumns.forEach(column => {
            const existingState = this.columnStates.get(column.id);
            if (existingState) {
                column.style.width = `${existingState.currentWidth}px`;
                existingState.isVisible = true;
            } else {
                const newState = this.createColumnState(column);
                this.columnStates.set(column.id, newState);
            }
        });

        // Mark missing columns as hidden
        this.columnStates.forEach((state, columnId) => {
            const isInDOM = this.tableElement.querySelector(`th[id="${columnId}"]`);
            if (!isInDOM && state.isVisible) {
                state.isVisible = false;
            }
        });
    }

}

// Registry for tracking instances by table element
const dataGridRegistry = new WeakMap<HTMLTableElement, DataGrid>();

// Helper function to get or create grid instance
function getOrCreateDataGrid(tableElement: HTMLTableElement, options?: DataGridOptions): DataGrid {
    let grid = dataGridRegistry.get(tableElement);
    if (!grid) {
        grid = new DataGrid(tableElement, options);
        dataGridRegistry.set(tableElement, grid);
    }
    return grid;
}

//
// Public Functions
//

export function initColumns(tableElement: HTMLTableElement, options?: DataGridOptions): void {
    getOrCreateDataGrid(tableElement, options);
}

export function addColumnResizeHandlers(tableElement: HTMLTableElement): void {
    const grid = getOrCreateDataGrid(tableElement);
    grid.addColumnResizeHandlers();
}
export function removeColumnResizeHandlers(tableElement: HTMLTableElement): void {
    const grid = dataGridRegistry.get(tableElement);
    grid?.removeColumnResizeHandlers();
}

export function addRowKeyDownHandler(tableElement: HTMLTableElement, dotNetRef: DotNetReference): void {
    const grid = getOrCreateDataGrid(tableElement);
    grid.addRowKeyDownHandler(dotNetRef);
}
export function removeRowKeyDownHandler(tableElement: HTMLTableElement): void {
    const grid = dataGridRegistry.get(tableElement);
    grid?.removeRowKeyDownHandler();
}

export function addGridFooterResizer(tableElement: HTMLTableElement): void {
    const grid = getOrCreateDataGrid(tableElement);
    grid.addGridFooterResizer();
}
export function removeGridFooterResizer(tableElement: HTMLTableElement): void {
    const grid = dataGridRegistry.get(tableElement);
    grid?.removeGridFooterResizer();
}

export function disposeColumns(tableElement: HTMLTableElement): void {
    const grid = dataGridRegistry.get(tableElement);
    if (grid) {
        grid.dispose();
        dataGridRegistry.delete(tableElement);
    }
}