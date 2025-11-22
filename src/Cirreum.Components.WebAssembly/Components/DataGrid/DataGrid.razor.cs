namespace Cirreum.Components;

using Cirreum.Components.Interop;
using Cirreum.ExpressionBuilder;
using Cirreum.ExpressionBuilder.Resources;
using Cirreum.Extensions;
using Cirreum.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// A generic data grid component that displays tabular data with features such as sorting, filtering, 
/// pagination, and customizable column rendering. This component provides a flexible foundation for 
/// displaying collections of strongly-typed data objects.
/// </summary>
/// <typeparam name="TData">
/// The type of data objects that will be displayed in the grid. This type parameter cascades to child 
/// components, allowing columns and other grid elements to maintain strong typing throughout the component hierarchy.
/// </typeparam>
/// <remarks>
/// The DataGrid component supports:
/// <list type="bullet">
/// <item><description>Automatic column generation based on data type properties</description></item>
/// <item><description>Custom column definitions with templated rendering</description></item>
/// <item><description>Built-in sorting and filtering capabilities</description></item>
/// <item><description>Pagination support for large datasets</description></item>
/// <item><description>Responsive design adaptations</description></item>
/// </list>
/// 
/// Example usage:
/// <code>
/// // optionally, Data or QueryableData
/// &lt;DataGrid TData="Employee" Data="employees"&gt;
///     &lt;Column Field="e => e.Name" Header="Employee Name" /&gt;
///     &lt;Column Field="e => e.Department" Header="Department" /&gt;
/// &lt;/DataGrid&gt;
/// </code>
/// </remarks>
[CascadingTypeParameter(nameof(TData))]
public partial class DataGrid<TData> {

	private readonly InternalGridContext<TData> _internalGridContext;
	private readonly RenderFragment _renderColumnHeaders;
	private readonly RenderFragment _renderRows;
	private readonly RenderFragment _renderFooterRow;
	private readonly RenderFragment RenderPageSizer;
	private readonly RenderFragment RenderPageSize;
	private readonly RenderFragment RenderPageNavigation;

	private volatile DataGridColumn<TData>? currentFilteredColumn;
	private volatile RenderFragment? _renderColumnHeaderFilter;
	private RenderFragment? _renderColumnChooserPopup;
	private IJSInProcessObjectReference? module;
	private string ResolvedTitle = "";

	public DataGrid() {
		this.Properties = new PropertyCollection(this.DataType);
		this._internalGridContext = new(this);
		this._renderColumnHeaders = this.RenderColumnHeaders;
		this._renderRows = this.RenderRows;
		this._renderFooterRow = this.RenderFooterRow;
		this.RenderPageSizer = this.RenderPageSizerContent;
		this.RenderPageSize = this.RenderPageSizeContent;
		this.RenderPageNavigation = this.RenderPageNavigationContent;
	}

	#region Services

	[Inject]
	private ISessionStorageService Storage { get; set; } = default!;

	[Inject]
	private IJSAppModule JSApp { get; set; } = default!;

	#endregion

	#region Model

	/// <summary>
	/// Gets or sets if the toolbar is visible.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool ShowToolbar { get; set; } = true;

	/// <summary>
	/// Gets or sets if the <see cref="Title"/> is visible.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool ShowTitle { get; set; } = true;

	/// <summary>
	/// Gets or sets datagrid's title.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public string Title { get; set; } = "";

	/// <summary>
	/// Gets or sets the loading message to display.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see cref="DefaultLoadingMessage"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public string LoadingMessage { get; set; } = DefaultLoadingMessage;
	private string RenderLoadingMessage {
		get {
			return this.LoadingMessage.HasValue() ? this.LoadingMessage : DefaultLoadingMessage;
		}
	}

	/// <summary>
	/// The default loading message string
	/// </summary>
	public const string DefaultLoadingMessage = "Loading...";

	internal readonly string ID = IdGenerator.Next.ToLowerInvariant();

	private readonly string GridID = IdGenerator.Next;

	private string ColumnFilterContainerId => $"{this.ID}_column_filter";
	private string ColumnChooserContainerId => $"{this.ID}_column_chooser";

	private ElementReference GridTableRef;
	private ElementReference GridFooterRef;
	private readonly string GridFooterId = IdGenerator.Next;

	public readonly Type DataType = typeof(TData);

	public PropertyCollection Properties { get; private set; }

	#endregion

	#region Columns

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	private readonly List<DataGridColumn<TData>> Columns = [];

	/// <summary>
	/// Readonly List of Visible Columns.
	/// </summary>
	public IReadOnlyList<DataGridColumn<TData>> VisibleColumns =>
		new ReadOnlyCollection<DataGridColumn<TData>>([.. this.Columns
				.Where(c => c.IsVisible)
				.OrderBy(c => c.ColumnIndex)]);
	internal int VisibleColumnCount => this.Columns.Count(c => c.IsVisible);
	internal List<DataGridColumnVisibleModel> GetChoosableColumns() =>
		[.. this.Columns
					.Where(c => c.VisibleInColumnChooser)
					.OrderBy(c => c.ColumnIndex)
					.Select(c => DataGridColumnVisibleModel.FromDataGridColumn(c))];
	internal void ApplyColumnsVisibility(IEnumerable<DataGridColumnVisibleModel> items) {
		this.RemoveColumnsResizing();
		foreach (var column in this.Columns) {
			var cv = items.FirstOrDefault(c => c.ColumnId.Equals(column.ColumnId));
			if (cv is null) {
				continue;
			}
			column.IsVisible = cv.IsVisible;
		}
		this.ApplyVisibilityIndex();
		this.Update();
	}
	internal void CancelColumnVisibility(IEnumerable<DataGridColumnVisibleModel> items) {
		foreach (var column in this.Columns) {
			var cv = items.FirstOrDefault(c => c.ColumnId.Equals(column.ColumnId));
			if (cv is not null) {
				cv.IsVisible = column.IsVisible;
			}
		}
		this.ApplyVisibilityIndex();
	}
	internal void ApplyVisibilityIndex() {
		var visibleIndex = 1;
		foreach (var column in this.Columns
				.OrderBy(c => c.ColumnIndex)) {
			if (column.IsVisible) {
				column.SetVisibilityIndex(visibleIndex++);
			} else {
				column.SetVisibilityIndex(HIDDEN_VISIBILITY_INDEX);
			}
		}
	}
	const int HIDDEN_VISIBILITY_INDEX = 9999;

	/// <summary>
	/// Set to <see langword="false"/> to prevent the columns width from being resizable.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool ResizableColumns { get; set; } = true;

	/// <summary>
	/// Set to <see langword="false"/> to prevent the columns from being reorderable (drag/drop).
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool Reorderable { get; set; } = true;

	/// <summary>
	/// Set to <see langword="false"/> to prevent the data from being sortable.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool Sortable { get; set; } = true;

	/// <summary>
	/// Gets or sets the css class to set the sort icons color
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: null
	/// </para>
	/// <para>
	/// When <see langword="null"/>, uses the internal default of 'text-info'.
	/// </para>
	/// </remarks>
	[Parameter]
	public string? SortIconColorCss { get; set; }

	private DataGridColumn<TData>? DragSource;
	internal void HandleDragStart(DataGridColumn<TData> column) {
		this.DragSource = column;
	}

	internal void HandleDrop(DataGridColumn<TData> dst) {
		var src = this.DragSource;
		if (src is not null) {
			this.DragSource = null;

			var srcCol = src.ColumnIndex;
			var dstCol = dst.ColumnIndex;
			if (srcCol == dstCol) {
				return;
			}


			// remove all resize handles
			this.RemoveColumnsResizing();

			if (srcCol < dstCol) {
				//
				// move everything to the left starting with the first column after
				// the source column
				//
				// Column.ColumnIndex is 1 based, so by using it as List Index
				// we automatically skip forward to the next column
				for (var i = srcCol; i < dstCol; i++) {
					this.Columns[i].ColumnIndex--;
				}
			} else {
				//
				// move everything to the right, starting with the destination column and
				// skipping the source column
				//
				// Column.ColumnIndex is 1 based, so we have to minus 1 to use it
				// as the proper starting List Index
				for (var i = (dstCol - 1); i < (srcCol - 1); i++) {
					this.Columns[i].ColumnIndex++;
				}
			}

			// assign the final swap
			src.ColumnIndex = dstCol;

			// rebuild the columns
			var temps = new DataGridColumn<TData>[this.Columns.Count];
			this.Columns.CopyTo(temps);
			this.Columns.Clear();
			foreach (var c in temps.OrderBy(c => c.ColumnIndex)) {
				this.Columns.Add(c);
			}
			this.ApplyVisibilityIndex();
			this.Update();

		}
	}

	internal int AssociateColumn(DataGridColumn<TData> column) {
		this.Columns.Add(column);
		return this.Columns.Count;
	}

	internal bool IsFilteredByColumn = false;
	private void ApplyColumnFilters() {
		this.IsFilteredByColumn = false;
		if (this._dataSource is not null) {
			var filterableColumns = this.Columns.Where(c => c.IsFiltered).ToList();
			if (filterableColumns.Count != 0) {
				var filter = new Filter<TData>();
				foreach (var column in filterableColumns) {
					filter.By(column.Filter!);
				}
				this._dataSource = this._dataSource.Where(filter);
				this.IsFilteredByColumn = true;
			}
		}
	}

	private void ApplyColumnSorting() {
		if (this.Sortable && this._dataSource is not null) {
			var sortColumn = this.Columns.Find(x => x.SortColumn);
			if (sortColumn is not null) {
				var sortAccessor = sortColumn.GetPropertyAccessor();
				if (sortAccessor is not null) {
					this._dataSource = sortColumn.SortDescending
						? this._dataSource.OrderByDescending(sortAccessor)
						: this._dataSource.OrderBy(sortAccessor);
				}
			}
		}
	}

	internal void ToggleColumnFilter(DataGridColumn<TData> column) {

		if (this.currentFilteredColumn is not null && this.currentFilteredColumn.ColumnId == column.ColumnId) {
			this.CloseColumnFilter();
			return;
		}

		if (this.currentFilteredColumn is not null) {
			this.CloseColumnFilter();
		}

		this.ShowColumnFilter(column);

	}

	private void ShowColumnFilter(DataGridColumn<TData> column) {
		this.currentFilteredColumn = column;
		this._renderColumnHeaderFilter = column.FilterContent;
		this.RunAfterRender(() => {
			this.currentFilteredColumn?.ShowFilter();
		});
		this.Update();
	}

	internal void CloseColumnFilter() {
		if (this.currentFilteredColumn is not null) {
			this.currentFilteredColumn.HideFilter();
			this.currentFilteredColumn = null;
			this._renderColumnHeaderFilter = null;
			this.Update();
		}
	}

	internal void SetColumnChooserPopup(RenderFragment chooserPopup) {
		this._renderColumnChooserPopup = chooserPopup;
		this.Update();
	}

	#endregion

	#region Rows

	/// <summary>
	/// Optionally defines a value for the @key on each rendered row. Typically this should be used to specify a
	/// unique identifier, such as a primary key value, for each data item.
	///
	/// This allows the grid to preserve the association between row elements and data items based on their
	/// unique identifiers.
	///
	/// If not set, the @key will be the item data instance itself.
	/// </summary>
	[Parameter]
	public Func<TData, object> ItemKey { get; set; } = x => x!;

	/// <summary>
	/// Select Type: None, Single or Multiple
	/// </summary>
	[Parameter]
	public SelectionType SelectionType { get; set; }

	/// <summary>
	/// Contains the Selected rows of <typeparamref name="TData"/>.
	/// </summary>
	[Parameter]
	public List<TData> SelectedItems { get; set; } = [];

	/// <summary>
	/// EventCallback for the Selected Items.
	/// </summary>
	[Parameter]
	public EventCallback<List<TData>> SelectedItemsChanged { get; set; }

	[Parameter]
	public Action<TData>? OnRowClicked { get; set; }

	[Parameter]
	public RenderFragment<TData>? RowContextMenu { get; set; }

	[Parameter]
	public Action<MenuItemEventArgs<TData>>? OnRowContextMenuItemSelected { get; set; }

	[Parameter]
	public Action<TData>? OnRowDoubleClicked { get; set; }

	private readonly Dictionary<string, bool> _rowStates = [];
	private bool _statesLoaded;
	private string _stateVersion = string.Empty; // Track when to reload
	private int versionIndex = int.MinValue;
	private string CalculateStateVersion() {
		// Create a version string based on what would invalidate our cache
		return $"{this.ID}-{this.versionIndex}";
	}
	private async Task LoadRowStates() {
		if (this._statesLoaded) {
			return;
		}

		try {
			this._statesLoaded = false;
			this._rowStates.Clear();

			if (this.VisibleItems == null) {
				return;
			}

			var rowIndex = 0;
			foreach (var item in this.VisibleItems) {
				var toggleStateKey = $"{this.ID}-{this.DetailsPrefix}-{rowIndex}";
				this._rowStates[toggleStateKey] = await this.Storage.ContainsKeyAsync(toggleStateKey) &&
										   await this.Storage.GetItemAsync<bool>(toggleStateKey);
				rowIndex++;
			}
		} finally {
			this._statesLoaded = true;
		}
	}

	private bool preventSelectionOnShiftCtl = false;
	private void OnMouseDownHandler(MouseEventArgs eventArgs) {
		// https://stackoverflow.com/questions/60348208/c-sharp-blazor-how-to-prevent-specific-key-on-input-like-in-js-with-e-preventde
		// how to prevent text selection when Shift/Ctl Key is pressed
		var newValue = eventArgs.ShiftKey || eventArgs.CtrlKey;
		if (this.SelectionType == SelectionType.None) {
			newValue = false;
		}
		if (this.preventSelectionOnShiftCtl == newValue) {
			// true == true, or false == false = no change...
			return;
		}
		this.preventSelectionOnShiftCtl = newValue;
		this.Update();
	}

	internal void HandleContextMenuItemSelected(IMenuItem menuItem) {
		if (this.contextMenuDataHasValue && this.contextMenuData is not null) {
			var localData = this.contextMenuData;
			this.contextMenuData = default;
			this.contextMenuDataHasValue = false;
			this.OnRowContextMenuItemSelected?.Invoke(new(localData, menuItem));
		}
	}

	private Menu? contextMenu;
	private TData? contextMenuData;
	private bool contextMenuDataHasValue;
	private async Task OnContextMenuHandlerAsync(MouseEventArgs e, TData rowData) {
		if (this.contextMenu is not null) {
			if (this.contextMenu.IsShowing) {
				await this.contextMenu.CloseAsync();
			}
			this.contextMenuData = rowData;
			this.contextMenuDataHasValue = true;
			await this.contextMenu.ShowAsync(e.PageY, e.PageX);
		}

	}

	private async Task OnRowClickHandlerAsync(MouseEventArgs eventArgs, TData rowData, int rowIndex) {
		if (eventArgs.Detail == 2) {
			var changedState = false;
			try {
				changedState = await this.ToggleRowDetailAsync(rowIndex);
				if (this.SetCurrentRow(rowIndex)) {
					changedState = true;
					this.RunAfterRender(this.FocusRow);
				}
			} catch {
				// swallow user errors
			} finally {
				if (changedState) {
					this.Update();
				}
				this.OnRowDoubleClicked?.Invoke(rowData);
			}
			return;
		}

		if (eventArgs.Detail == 1) {
			try {
				this.OnRowClicked?.Invoke(rowData);
			} catch {
				// swallow user errors
			} finally {
				if (this.SetCurrentRow(rowIndex)) {
					this.RunAfterRender(this.FocusRow);
					this.Update();
				}
			}
			try {
				if (this.HandleRowSelection(eventArgs, rowData) && this.SelectedItemsChanged.HasDelegate) {
					_ = this.SelectedItemsChanged.InvokeAsync(this.SelectedItems);
				}
			} catch {
			}
		}

	}
	private async Task<bool> ToggleRowDetailAsync(int rowIndex) {
		if (this._detailTemplate != null) {
			var toggleStateKey = $"{this.ID}-{this.DetailsPrefix}-{rowIndex}";
			if (_rowStates.TryGetValue(toggleStateKey, out var isOpen)) {
				try {
					await this.Storage.SetItemAsync(toggleStateKey, !isOpen);
					_rowStates[toggleStateKey] = !isOpen;
					return true;
				} catch {
				}
			} else {
				await this.Storage.SetItemAsync(toggleStateKey, true);
				_rowStates[toggleStateKey] = true;
				return true;
			}
		}
		return false;
	}

	private TData? lastSelectedItem;
	private bool HandleRowSelection(MouseEventArgs eventArgs, TData rowData) {
		return this.SelectionType switch {
			SelectionType.None => false,
			SelectionType.Single => this.HandleSingleRowSelection(eventArgs, rowData),
			SelectionType.Multiple => this.HandleMultipleRowSelection(eventArgs, rowData),
			_ => false,
		};
	}
	private bool HandleSingleRowSelection(MouseEventArgs eventArgs, TData rowData) {

		if (this.SelectedItems is null) {
			return false;
		}

		if (eventArgs.CtrlKey) {
			return this.SelectedItems.Remove(rowData);
		}

		this.SelectedItems.Clear();
		this.SelectedItems.Add(rowData);
		this.lastSelectedItem = rowData;

		return true;

	}
	private bool HandleMultipleRowSelection(MouseEventArgs eventArgs, TData rowData) {

		if (this.VisibleItems.Count == 0) {
			return false;
		}

		if (this.SelectedItems is null) {
			return false;
		}

		if (eventArgs.CtrlKey) {

			if (this.SelectedItems.Remove(rowData) is false) {
				this.SelectedItems.Add(rowData);
				this.lastSelectedItem = rowData;
			}
			return true;

		}

		if (eventArgs.ShiftKey && this.lastSelectedItem is not null) {

			var lastSelectedRow = this.VisibleItems.IndexOf(this.lastSelectedItem);
			if (lastSelectedRow > -1) {

				this.SelectedItems.Clear();

				var selectedRow = this.VisibleItems.IndexOf(rowData);

				if (selectedRow == lastSelectedRow) {
					this.SelectedItems.Add(rowData);
				} else if (selectedRow > lastSelectedRow) {
					for (var i = lastSelectedRow; i <= selectedRow; i++) {
						this.SelectedItems.Add(this.VisibleItems[i]);
					}
				} else {
					for (var i = selectedRow; i <= lastSelectedRow; i++) {
						this.SelectedItems.Add(this.VisibleItems[i]);
					}
				}

				return true;

			}
			// else: moved to another page...start over
		}

		this.SelectedItems.Clear();
		this.SelectedItems.Add(rowData);
		this.lastSelectedItem = rowData;
		return true;

	}
	private bool SetCurrentRow(int newRowIndex) {
		if (this.currentRowIndex != newRowIndex
			&& newRowIndex > -1
			&& newRowIndex < this.VisibleItems.Count) {
			this.currentRowIndex = newRowIndex;
			this.currentRowData = this.VisibleItems[this.currentRowIndex];
			this.currentRowId = $"row-{this.currentRowIndex}-{this.currentRowData!.GetHashCode()}";
			return true;
		}
		return false;
	}
	private bool SelectCurrentRow(bool shiftKey = false) {
		if (this.currentRowData is not null) {
			if (this.HandleRowSelection(new MouseEventArgs { ShiftKey = shiftKey }, this.currentRowData)) {
				if (this.SelectedItemsChanged.HasDelegate) {
					_ = this.SelectedItemsChanged.InvokeAsync(this.SelectedItems);
				} else {
					this.Update();
				}
				return false;
			}
		}
		return true;
	}
	private void FocusRow() {
		if (this.currentRowId.HasValue()) {
			this.JSApp.FocusElement(this.currentRowId, false);
		}
	}

	/// <summary>
	/// Select all visible rows, if <see cref="SelectionType"/> is set to <see cref="SelectionType.Multiple"/>.
	/// </summary>
	public void SelectAll() {

		switch (this.SelectionType) {
			case SelectionType.None:
			case SelectionType.Single:
				return;
			case SelectionType.Multiple:
				if (this.VisibleItems.Count == 0) {
					return;
				}
				if (this.SelectedItems is null) {
					return;
				}
				this.SelectedItems.Clear();
				this.SelectedItems.AddRange(this.VisibleItems);
				if (this.currentRowIndex > -1) {
					this.lastSelectedItem = this.VisibleItems[this.currentRowIndex];
				}
				if (this.SelectedItemsChanged.HasDelegate) {
					this.SelectedItemsChanged.InvokeAsync(this.SelectedItems);
				} else {
					this.Update();
				}
				break;
		}

	}
	/// <summary>
	/// Unselect all visible rows.
	/// </summary>
	public void UnselectAll() {

		switch (this.SelectionType) {
			case SelectionType.None:
				return;
			case SelectionType.Single:
			case SelectionType.Multiple:
				if (this.SelectedItems is null) {
					return;
				}
				this.SelectedItems.Clear();
				this.lastSelectedItem = default;
				if (this.SelectedItemsChanged.HasDelegate) {
					this.SelectedItemsChanged.InvokeAsync(this.SelectedItems);
				} else {
					this.Update();
				}
				break;
		}

	}

	private int currentRowIndex = -1;
	private TData? currentRowData;
	private string? currentRowId;
	private string GetTabIndex(int rowIndex, bool isSelected) => ((this.currentRowIndex == rowIndex) || isSelected) ? "0" : "-1";
	private void HandleArrowDown(bool shift) {
		if (this.SetCurrentRow(this.currentRowIndex + 1)) {
			this.RunAfterRender(this.FocusRow);
			if (this.SelectCurrentRow(shift)) {
				this.Update();
			}
		}
	}
	private void HandleArrowUp(bool shift) {
		if (this.currentRowIndex == 0) {
			return;
		}
		if (this.SetCurrentRow(this.currentRowIndex - 1)) {
			this.RunAfterRender(this.FocusRow);
			if (this.SelectCurrentRow(shift)) {
				this.Update();
			}
		}
	}
	private void HandleSelectAll() {
		this.SelectAll();
	}
	private void HandleUnselectAll() {
		this.UnselectAll();
	}
	private void HandlePageUp() {
		if (this.IsPageable && this.Page > 1) {
			this.SetPagePrivate(this.Page - 1);
			this.RunAfterRender(() => {
				if (this.SetCurrentRow(0)) {
					this.RunAfterRender(this.FocusRow);
					if (this.SelectCurrentRow()) {
						this.Update();
					}
				}
			});
			this.Update();
		}
	}
	private void HandleHome(bool ctl) {
		var pageChanged = false;
		if (ctl) {
			if (this.IsPageable && this.Page > 1) {
				this.SetPagePrivate(1);
				pageChanged = true;
			}
		}
		if (pageChanged) {
			this.RunAfterRender(() => {
				if (this.SetCurrentRow(0)) {
					this.RunAfterRender(this.FocusRow);
					if (this.SelectCurrentRow()) {
						this.Update();
					}
				}
			});
			this.Update();
		} else {
			if (this.SetCurrentRow(0)) {
				this.RunAfterRender(this.FocusRow);
				if (this.SelectCurrentRow()) {
					this.Update();
				}
			}
		}
	}
	private void HandlePageDown() {
		if (this.IsPageable && this.Page < this.TotalPages) {
			this.SetPagePrivate(this.Page + 1);
			this.RunAfterRender(() => {
				if (this.SetCurrentRow(0)) {
					this.RunAfterRender(this.FocusRow);
					if (this.SelectCurrentRow()) {
						this.Update();
					}
				}
			});
			this.Update();
		}
	}
	private void HandleEnd(bool ctl) {
		var pageChanged = false;
		if (ctl) {
			if (this.IsPageable && this.Page < this.TotalPages) {
				this.SetPagePrivate(this.TotalPages);
				pageChanged = true;
			}
		}
		if (pageChanged) {
			this.RunAfterRender(() => {
				if (this.SetCurrentRow(this.VisibleItems.Count - 1)) {
					this.RunAfterRender(this.FocusRow);
					if (this.SelectCurrentRow()) {
						this.Update();
					}
				}
			});
			this.Update();
		} else {
			if (this.SetCurrentRow(this.VisibleItems.Count - 1)) {
				this.RunAfterRender(this.FocusRow);
				if (this.SelectCurrentRow()) {
					this.Update();
				}
			}
		}
	}

	#endregion

	#region DataSource

	private IQueryable<TData>? _dataSource;
	private object? _lastAssignedDataSource;

	/// <summary>
	/// IQueryable data source to display in the table
	/// </summary>
	[Parameter]
	public IQueryable<TData>? QueryableData { get; set; } // WARNING: Do not use in code, always use _dataSource

	/// <summary>
	/// Collection to display in the table
	/// </summary>
	[Parameter]
	public IEnumerable<TData>? Data { get; set; } // WARNING: Do not use in code, always use _dataSource

	public bool HasData => this._dataSource is not null;

	/// <summary>
	/// Collection of data items after filtering, searching, sorting and paging.
	/// </summary>
	internal List<TData> VisibleItems { get; set; } = [];

	/// <summary>
	/// Gets the total number of items in the data grid
	/// after filtering and searching.
	/// </summary>
	public int TotalItems { get; private set; }

	#endregion

	#region Editing

	#region TODO - Implement Real Editing Mode

	/*

	/// <summary>
	/// Is DataGrid in Edit mode
	/// </summary>
	public bool IsEditMode { get; private set; }

	/// <summary>
	/// Toggle the edit mode.
	/// </summary>
	public void ToggleEditMode() {
		IsEditMode = !IsEditMode;
		Update();
	}

	/// <summary>
	/// Place the DataGrid into Edit mode.
	/// </summary>
	public void EnterEditMode() {
		IsEditMode = true;
		Update();
	}

	/// <summary>
	/// Place the DataGrid into Display mode.
	/// </summary>
	public void ExitEditMode() {
		IsEditMode = false;
		Update();
	}

	[Parameter]
	public EventCallback OnCommitEdit { get; set; }

	public bool IsCommittingEdit { get; private set; }

	private bool HasCommitEditHandler {
		get {
			return OnCommitEdit.HasDelegate;
		}
	}

	private async Task OnCommitEditClicked(MouseEventArgs e) {
		try {
			IsCommittingEdit = true;
			if (HasCommitEditHandler) {
				await OnCommitEdit.InvokeAsync();
			}
		} finally {
			IsCommittingEdit = false;
			IsEditMode = false;
			Update();
		}
	}

	[Parameter]
	public EventCallback OnCancelEdit { get; set; }

	private bool HasCancelEditHandler {
		get {
			return OnCancelEdit.HasDelegate;
		}
	}

	private async Task OnCancelEditClicked(MouseEventArgs e) {
		try {
			if (HasCancelEditHandler) {
				await OnCancelEdit.InvokeAsync();
			}
		} finally {
			IsEditMode = false;
			Update();
		}
	}

	*/

	#endregion

	#endregion

	#region Sorting

	public void ResetSort() {
		foreach (var c in this.Columns) {
			c.ResetSort();
		}
	}

	#endregion

	#region Paging

	/// <summary>
	/// Gets or sets if paging is enabled.
	/// </summary>
	[Parameter]
	public bool Pageable { get; set; } = true;
	private bool IsPageable => this.Pageable && this._dataSource is not null;

	/// <summary>
	/// Gets or sets the collection of page sizes (see: <see cref="DataGridPageSizeItem"/>).
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: 
	/// </para>
	/// <para>
	/// <code>
	/// [ new (5, "5"),
	///   DataGridPageSizeItem.Default, // 10
	///   new (25, "25"),
	///   new (50, "50"),
	///   new (100, "100"),
	///   DataGridPageSizeItem.All ]
	/// </code>
	/// </para>
	/// </remarks>
	[Parameter]
	public List<DataGridPageSizeItem> PageSizes { get; set; } = [
		new (5, "5"),
		DataGridPageSizeItem.Default,
		new (25, "25"),
		new (50, "50"),
		new (100, "100"),
		DataGridPageSizeItem.All
	];

	/// <summary>
	/// Gets or sets the initial page size value.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see cref="DataGridPageSizeItem.DefaultPageSize"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public int InitialPageSize { get; set; } = DataGridPageSizeItem.DefaultPageSize;

	/// <summary>
	/// An event callback for when the <see cref="PageSize"/> changes.
	/// </summary>
	[Parameter]
	public EventCallback<int> PageSizeChanged { get; set; }

	/// <summary>
	/// Gets the current page size as s string value.
	/// </summary>
	public string PageSizeDisplay { get; private set; } = "";

	/// <summary>
	/// Gets the current page size.
	/// </summary>
	public int PageSize { get; private set; }

	/// <summary>
	/// Gets the current page number.
	/// </summary>
	public int Page { get; private set; }

	internal int PageFirstItem { get; private set; }

	internal int PageLastItem { get; private set; }

	/// <summary>
	/// Gets the total number of pages
	/// </summary>
	public int TotalPages => this.GetTotalPages(this.TotalItems);

	/// <summary>
	/// Goto the first page.
	/// </summary>
	public void FirstPage() {
		if (this.Page != 1) {
			this.SetPage(1);
		}
	}

	/// <summary>
	/// Goto the next page.
	/// </summary>
	public void NextPage() {
		if (this.Page < this.TotalPages) {
			this.SetPage(this.Page + 1);
		}
	}

	/// <summary>
	/// Goto the previous page.
	/// </summary>
	public void PreviousPage() {
		if (this.Page > 1) {
			this.SetPage(this.Page - 1);
		}
	}

	/// <summary>
	/// Goto the last page.
	/// </summary>
	public void LastPage() {
		this.SetPage(this.TotalPages);
	}

	internal void SetPageSize(int newPageSize) {

		// if the page size has changed, adjust current page
		if (this.IsPageable && this.PageSize != newPageSize) {

			this.Page = -2;

			if (newPageSize < 1) {

				this.PageSize = newPageSize;
				this.SetPage(-1);

			} else {

				double priorPageFirstItem = this.PageFirstItem;
				double priorPageLastItem = this.PageLastItem;

				if (this.PageFirstItem == 1 && this.PageLastItem == this.TotalItems) {
					this.PageSize = newPageSize;
					this.SetPage(1);
				} else if (this.PageSize < newPageSize) {
					var newPageF = priorPageLastItem / newPageSize;
					var newPage = Convert.ToInt32(Math.Ceiling(newPageF));
					if (newPage < 1) {
						newPage = 1;
					}
					this.PageSize = newPageSize;
					this.SetPage(newPage);
				} else {
					var newPageF = priorPageFirstItem / newPageSize;
					var newPage = Convert.ToInt32(Math.Ceiling(newPageF));
					if (newPage < 1) {
						newPage = 1;
					}
					this.PageSize = newPageSize;
					this.SetPage(newPage);
				}

			}

			var valueStr = $"{newPageSize}";
			var selectedPageSize = this.PageSizes.FirstOrDefault(i => i.Value == newPageSize);
			this.PageSizeDisplay = selectedPageSize is not null ? selectedPageSize.Display : valueStr;

			if (this.PageSizeChanged.HasDelegate) {
				this.PageSizeChanged.InvokeAsync(newPageSize);
			}

		}

	}
	void HandlePageSizeChanged(int value) {
		this.SetPageSize(value);
	}

	/// <summary>
	/// Navigate to the desired page.
	/// </summary>
	/// <param name="value">The page number to navigate to.</param>
	public void SetPage(int value) {
		this.SetPagePrivate(value);
		this.Update();
	}
	private void SetPagePrivate(int value) {

		if (this._dataSource == null) {
			throw new Exception($"Cannot set the Page until the DataGrid has data. Ensure either the {nameof(this.Data)} or {nameof(this.QueryableData)} property has been set.");
		}

		// if not IsPageable or PageSize is zero, we return all rows
		if ((this.IsPageable is false || this.PageSize < 1) && this.Page != value) {
			this.currentRowIndex = -1;
			_ = this.ResetDetailsStateAsync();

			this.VisibleItems = [.. this._dataSource];

			this.Page = 1;
			this.PageFirstItem = 1;
			this.PageLastItem = this.VisibleItems.Count;
			this.TotalItems = this.VisibleItems.Count;
			return;
		}

		if (this.IsPageable && this.PageSize > 0 && this.Page != value) {

			if (value < 1) {
				throw new Exception("Cannot set the Page to zero.");
			}
			this.currentRowIndex = -1;

			if (value > this.TotalPages) {
				value = this.TotalPages;
			}

			_ = this.ResetDetailsStateAsync();

			var pageIndex = value - 1;
			var pageLastItem = (pageIndex * this.PageSize) + this.PageSize;

			this.VisibleItems = [.. this._dataSource
				.Skip(pageIndex * this.PageSize)
				.Take(this.PageSize)];

			this.Page = value;
			this.PageFirstItem = (pageIndex * this.PageSize) + 1;
			this.PageLastItem = Math.Min(this.TotalItems, pageLastItem);

		}

	}

	private int GetTotalPages(int totalRecords) {
		if (totalRecords < 1) {
			return 0;
		}
		if (totalRecords < 2) {
			return 1;
		}
		if (this.PageSize > 1) {
			return Convert.ToInt32(Math.Ceiling((double)totalRecords / this.PageSize));
		}
		return this.PageSize;
	}

	private void ApplyPaging() {

		if (this.IsPageable is false) {
			this.Page = -1;
			this.SetPagePrivate(0);
			return;
		}

		var totalCount = this._dataSource?.Count() ?? 0;
		var totalPages = this.GetTotalPages(totalCount);
		var page = this.Page;
		this.Page = -1;

		if (page < 1) {
			page = 1;
		}

		if (page > totalPages) {
			page = 1;
		}

		this.TotalItems = totalCount;

		this.SetPagePrivate(page);

	}

	#endregion

	#region Searching

	/// <summary>
	/// Is the data searchable.
	/// </summary>
	[Parameter]
	public bool Searchable { get; set; } = true;

	/// <summary>
	/// The debounce delay in milliseconds. Default: 500
	/// </summary>
	[Parameter]
	public int SearchDebounceDelay { get; set; } = 500;

	private DataGridSearch<TData>? SearchBarInstance { get; set; }

	private void ApplySearchFilter() {
		if (this.SearchBarInstance is not null) {
			if (this._dataSource is not null) {
				var searchText = this.SearchBarInstance.GridSearchValue;
				if (searchText.HasValue()) {
					var gridSearchExp = this.CreateGridFilterQuery(searchText);
					if (gridSearchExp is not null) {
						this._dataSource = this._dataSource.Where(gridSearchExp);
					}
				}
			}
		}
	}

	private Expression<Func<TData, bool>>? CreateGridFilterQuery(string value) {

		var fields = this.Columns
			.Where(c => c.IsSearchable)
			.Select(c => c.GetPropertyAccessor())
			.Where(p => p is not null);

		return fields.CreateObjectSearch(value);

	}

	#endregion

	#region Footer

	[Parameter]
	public bool ShowFooter { get; set; }

	internal object? GetFooterValue(string field, AggregateType? aggregate) {
		if (!this.ShowFooter) {
			return string.Empty;
		}
		if (!aggregate.HasValue) {
			return string.Empty;
		}
		if (this.VisibleItems.Count == 0) {
			return string.Empty;
		}
		if (string.IsNullOrEmpty(field)) {
			return string.Empty;
		}
		var localField = field;

		var result = aggregate.Value switch {
			AggregateType.Count => this.VisibleItems.Count,
			AggregateType.Min => this.VisibleItems.Min(c => c.GetMemberValue(localField)),
			AggregateType.Max => this.VisibleItems.Max(c => c.GetMemberValue(localField)),
			_ => AggregateQueryable(this.VisibleItems.AsQueryable(), localField, aggregate.Value)
		};

		return result ?? string.Empty;
	}

	private static object? AggregateQueryable(IQueryable source, string member, AggregateType aggregateType) {

		ArgumentNullException.ThrowIfNull(source);

		ArgumentNullException.ThrowIfNull(member);

		// The most common variant of Queryable.Sum() expects a lambda.
		// Since we just have a string to a property, we need to create a
		// lambda from the string in order to pass it to the sum method.

		// Lets create a ((TSource s) => s.Price ). First up, the parameter "s":
		var parameter = Expression.Parameter(source.ElementType, "s");

		// Followed by accessing the Price property of "s" (s.Price):
		var property = source.ElementType.GetProperty(member, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

		if (property == null) {
			return source;
		}

		var getter = Expression.MakeMemberAccess(parameter, property);

		// And finally, we create a lambda from that. First specifying on what
		// to execute when the lambda is called, and finally the parameters of the lambda.
		Expression selector = Expression.Lambda(getter, parameter);

		try {
			// There are a lot of Queryable.Sum() overloads with different
			// return types  (double, int, decimal, double?, int?, etc...).
			// We're going to find one that matches the type of our property.
			var aggregateMethod = typeof(Queryable).GetMethods().First(
				m => m.Name == aggregateType.ToString()
					 && m.ReturnType == property.PropertyType
					 && m.IsGenericMethod);

			// Now that we have the correct method, we need to know how to call the method.
			// Note that the Queryable.Sum<TSource>(source, selector) has a generic type,
			// which we haven't resolved yet. Good thing is that we can use copy the one from
			// our initial source expression.
			var genericAggregateMethod = aggregateMethod.MakeGenericMethod([source.ElementType]);

			// TSource, source and selector are now all resolved. We now know how to call
			// the sum-method. We're not going to call it here, we just express how we're going
			// call it.
			var callExpression = Expression.Call(
				null,
				genericAggregateMethod,
				[source.Expression, Expression.Quote(selector)]);

			// Pass it down to the query provider. This can be a simple LinqToObject-datasource,
			// but also a more complex datasource (such as LinqToSql). Anyway, it knows what to
			// do.
			return source.Provider.Execute(callExpression);

		} catch (Exception) {
			throw new InvalidOperationException($"The {aggregateType} aggregation cannot be used for {member} field. The {member} field must be in numeric data type to perform this operation.");
		}
	}

	internal object? GetColumnMinValue(string field) {
		if (this.VisibleItems.Count == 0) {
			return null;
		}
		if (string.IsNullOrEmpty(field)) {
			return null;
		}
		var localField = field;
		return this.VisibleItems.Min(c => c.GetMemberValue(localField));
	}

	internal object? GetColumnMaxValue(string field) {
		if (this.VisibleItems.Count == 0) {
			return null;
		}
		if (string.IsNullOrEmpty(field)) {
			return null;
		}
		var localField = field;
		return this.VisibleItems.Max(c => c.GetMemberValue(localField));
	}

	#endregion

	#region Templates

	/// <summary>
	/// Set the template to use for the data grid's Title.
	/// </summary>
	/// <param name="titleTemplate"></param>
	/// <remarks>
	/// This overrides the <see cref="Title"/> parameter.
	/// </remarks>
	internal void SetTitleTemplate(DataGridTitleTemplate<TData> titleTemplate) {
		this._titleTemplate = titleTemplate.ChildContent;
	}
	private RenderFragment? _titleTemplate;

	/// <summary>
	/// Set the template to use for empty data
	/// </summary>
	/// <param name="emptyDataTemplate"></param>
	internal void SetEmptyDataTemplate(DataGridEmptyDataTemplate<TData> emptyDataTemplate) {
		this._emptyDataTemplate = emptyDataTemplate.ChildContent;
	}
	private RenderFragment? _emptyDataTemplate;

	/// <summary>
	/// Set the template to use for loading data
	/// </summary>
	/// <param name="loadingDataTemplate"></param>
	internal void SetLoadingDataTemplate(DataGridLoadingDataTemplate<TData> loadingDataTemplate) {
		this._loadingDataTemplate = loadingDataTemplate.ChildContent;
	}
	private RenderFragment? _loadingDataTemplate;

	/// <summary>
	/// Set the template to use for detail
	/// </summary>
	/// <param name="detailTemplate"></param>
	internal void SetDetailTemplate(DataGridDetailTemplate<TData> detailTemplate) {
		this._detailTemplate = detailTemplate.ChildContent;
	}
	private RenderFragment<TData>? _detailTemplate;

	private readonly string DetailsPrefix = "details";
	private async Task ResetDetailsStateAsync() {

		var prefix = $"{this.ID}-{this.DetailsPrefix}-";
		var keysToRemove = new List<string>();

		foreach (var key in this._rowStates.Keys.Where(k => k.StartsWith(prefix))) {
			keysToRemove.Add(key);
		}

		this._rowStates.Clear();
		this.versionIndex++;
		if (keysToRemove.Count > 0) {
			await this.Storage.RemoveItemsAsync(keysToRemove);
		}

	}

	/// <summary>
	/// Set the template to use for a row context menu
	/// </summary>
	/// <param name="rowContextMenuTemplate"></param>
	internal void SetContextMenuTemplate(DataGridRowContextMenuTemplate<TData> rowContextMenuTemplate) {
		this._contextMenuTemplate = rowContextMenuTemplate.ChildContent;
	}
	private RenderFragment? _contextMenuTemplate;
	private bool HasContextMenu => this._contextMenuTemplate is not null;

	#endregion

	#region Styling

	/// <summary>
	/// Gets or sets the datagrid's outer container Css class
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: "card"
	/// </para>
	/// </remarks>
	[Parameter]
	public string ContainerCss { get; set; } = "card";

	private string ContainerClass => CssBuilder.Default("datagrid")
		.AddClass(this.ContainerCss, when: this.ContainerCss.HasValue())
		.AddClass("card", when: this.ContainerCss.IsEmpty())
		.AddClass("datagrid-flush", when: this.Flush)
	.Build();

	/// <summary>
	/// Gets or sets if the datagrid is in rendered in responsive mode
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool Responsive { get; set; } = true;

	/// <summary>
	/// Defaults to 'table-responsive'
	/// </summary>
	[Parameter]
	public string ResponsiveCss { get; set; } = "table-responsive";

	private string ResponsiveCssClass =>
		CssBuilder
			.Empty()
				.AddClass(this.ResponsiveCss, when: this.Responsive && this.ResponsiveCss.HasValue())
			.Build();

	/// <summary>
	/// Set to <see langword="true"/>, to set the table as flush within its container.
	/// </summary>
	[Parameter]
	public bool Flush { get; set; }

	/// <summary>
	/// Applies the table-bordered css class
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool Bordered { get; set; } = true;

	/// <summary>
	/// Applies the table-striped css class
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool Striped { get; set; } = true;

	/// <summary>
	/// Applies the table-hover css class
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool Hover { get; set; } = true;

	/// <summary>
	/// Applies the table-sm css class
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool Small { get; set; } = true;

	private string ResolvedTableClass => CssBuilder
		.Default("table")
			.AddClass("table-flush", when: this.Flush)
			.AddClass("table-striped", when: this.Striped)
			.AddClass("table-bordered", when: this.Bordered)
			.AddClass("table-hover", when: this.Hover)
			.AddClass("table-sm", when: this.Small)
		.Build();

	/// <summary>
	/// Optional Table Head Css Classes. Default: ""
	/// </summary>
	[Parameter]
	public string HeadCss { get; set; } = "";

	/// <summary>
	/// The default column Headers css class list. Example: "bg-primary text-light"; Default: ""
	/// </summary>
	[Parameter]
	public string HeadersCss { get; set; } = "";
	private string DetailsHeaderClassList => CssBuilder
		.Default("row-detail-toggle-header")
			.AddClass(this.HeadersCss, when: string.IsNullOrWhiteSpace(this.HeadersCss) is false)
		.Build();

	/// <summary>
	/// Optional Table Body Css Classes. Default: ""
	/// </summary>
	[Parameter]
	public string BodyCss { get; set; } = "";

	/// <summary>
	/// Optional function to dynamically resolve a row's Css Class. Default: null
	/// </summary>
	/// <remarks>
	/// <para>
	/// A delegate that is supplied with the data for a given row and a bool indicating
	/// if the row/item is currently selected.
	/// </para>
	/// <para>
	/// The value returned form this func is appended to any other internally used Css Classes.
	/// </para>
	/// </remarks>
	[Parameter]
	public Func<TData, bool, string>? RowCss { get; set; }

	/// <summary>
	/// Optional Table Footer Css Classes. Default: ""
	/// </summary>
	[Parameter]
	public string FooterCss { get; set; } = "";

	/// <summary>
	/// Set to <see langword="true"/>, to disable the built-in busy overlay
	/// </summary>
	/// <remarks>
	/// <para>
	/// Allows the developer to use their own busy overlay for the <see cref="OnRefreshData"/>
	/// or <see cref="OnExportData"/> callbacks.
	/// </para>
	/// <para>
	/// Default: <see langword="false"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool DisableBusyOverlay { get; set; }

	/// <summary>
	/// Gets if the DataGrid is currently Busy (Refreshing or Exporting)
	/// </summary>
	public bool IsBusy =>
		this.IsRefreshing ||
		this.IsExporting;

	private string BusyTitle {
		get {
			var action =
				this.IsExporting ? "Exporting" :
				this.IsRefreshing ? "Refreshing" :
				"Busy";

			if (this.ResolvedTitle.HasValue()) {
				return $"{this.ResolvedTitle} {action.ToLowerInvariant()}...";
			}
			return $"{action}...";
		}
	}

	#endregion

	#region Export

	[Parameter]
	public EventCallback OnExportData { get; set; }

	[Parameter]
	public bool IsExporting { get; set; }

	[Parameter]
	public EventCallback<bool> IsExportingChanged { get; set; }

	private bool HasExportDataHandler {
		get {
			return this.OnExportData.HasDelegate;
		}
	}

	private bool ExportDisabled {
		get {
			return
				this.HasExportDataHandler is false ||
				this.VisibleItems.Count == 0;
		}
	}

	private async Task OnExportClicked() {
		if (this.HasExportDataHandler) {
			try {
				if (this.IsExporting is false) {
					this.IsExporting = true;
					await this.IsExportingChanged.InvokeAsync(this.IsExporting);
				}
				await this.OnExportData.InvokeAsync();
			} finally {
				if (this.IsExporting) {
					this.IsExporting = false;
					await this.IsExportingChanged.InvokeAsync(this.IsExporting);
				}
				this.Update();
			}
		}
	}

	#endregion

	#region Refresh

	/// <summary>
	/// A callback used to refresh the data.
	/// </summary>
	/// <remarks>
	/// <para>
	/// If a delegate is connected to this callback, then a Refresh button is displayed in the toolbar area.
	/// </para>
	/// <para>
	/// This is different than <see cref="IsLoading"/> which is typically used during initial load. Then you use
	/// this callback for refreshes.
	/// </para>
	/// <para>
	/// <see cref="IsRefreshing"/> is automatically set to <see langword="true"/> just before the callback is executed.
	/// And will be set back to <see langword="false"/> upon completion.
	/// </para>
	/// </remarks>
	[Parameter]
	public EventCallback OnRefreshData { get; set; }

	/// <summary>
	/// Set to <see langword="false"/> to prevent the Refresh button from being enabled.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// <para>
	/// See <see cref="OnRefreshData"/> which effects if the refresh button is displayed.
	/// </para>
	/// </remarks>
	[Parameter]
	public bool IsRefreshingEnabled { get; set; } = true;

	/// <summary>
	/// Gets if the DataGrid is currently refreshing.
	/// </summary>
	[Parameter]
	public bool IsRefreshing { get; set; }

	/// <summary>
	/// Notified when <see cref="IsRefreshing"/> has been changed.
	/// </summary>
	[Parameter]
	public EventCallback<bool> IsRefreshingChanged { get; set; }

	private bool HasRefreshDataHandler =>
		this.OnRefreshData.HasDelegate;

	private async Task OnRefreshClicked() {
		if (this.HasRefreshDataHandler) {
			try {
				if (this.IsRefreshing is false) {
					this.IsRefreshing = true;
					if (this.IsRefreshingChanged.HasDelegate) {
						await this.IsRefreshingChanged.InvokeAsync(this.IsRefreshing);
					}
				}
				await this.OnRefreshData.InvokeAsync();
			} finally {
				if (this.IsRefreshing) {
					this.IsRefreshing = false;
					if (this.IsRefreshingChanged.HasDelegate) {
						await this.IsRefreshingChanged.InvokeAsync(this.IsRefreshing);
					}
				}
			}

		}
	}

	internal void ApplyCurrentData() {

		if (this.Data is not null || this.QueryableData is not null) {

			this._dataSource = this.Data is not null ?
				this.Data.AsQueryable() :
				this.QueryableData;

			this.ApplyColumnFilters();

			this.ApplySearchFilter();

			this.ApplyColumnSorting();

			this.ApplyPaging();

		} else {

			this.VisibleItems.Clear();

		}

	}

	/// <summary>
	/// Apply or re-apply the current DataGrid State (search, filter, sort etc.) against
	/// the currently assigned data source and re-renders the DataGrid.
	/// </summary>
	/// <remarks>
	/// This does not update/refresh your supplied data source. To re-query, simply perform your base
	/// query (api call etc.) and update the data source Parameter value with your updated/new results.
	/// 
	/// The end user can trigger your implemented query, by listening to the <see cref="OnRefreshData"/>
	/// Event Callback. When assigned, the DataGrid automatically exposes a refresh Button. When clicked,
	/// simply executes your handler that should re-query your backend datasource and re-assign the results
	/// to the DataSource Parameter property.
	/// </remarks>
	public Task RefreshAsync() {
		this.ApplyCurrentData();
		this.Update();
		return Task.CompletedTask;
	}

	/// <summary>
	/// Indicates that your data is not yet available, and will cause the loading overlay to be shown.
	/// </summary>
	/// <remarks>
	/// <para>
	/// When <see langword="true"/> this displays either the built-in default loading UI, or your custom defined
	/// <see cref="DataGridLoadingDataTemplate{TData}"/>.
	/// </para>
	/// <para>
	/// Set this value to <see langword="true"/> when you first begin querying or downloading your data.
	/// Once your data is available, set it to <see langword="false"/>.
	/// </para>
	/// <para>
	/// Typically you would do this during your OnInitialized/Async() method overrides.
	/// </para>
	/// </remarks>
	[Parameter]
	public bool IsLoading { get; set; }
	// NOTE:
	// We assume the consumer would set this to true from the beginning, while they're
	// loading their data. Once completed, they'd update this to 'false', thereby triggering
	// a full lifecycle (SetParameters, OnParametersSet/Async, OnAfterRender/Async etc.).
	//
	// Though the above could be true, there are cases where they already have the data loaded,
	// and will just assign the Data value and never set this to true and back to false. Thereby
	// never causing a re-run of the component lifecycle.

	bool isLoaded = false;
	private void Load() {

		this.versionIndex++;
		this.isLoaded = true;

		var newDataSource = this.QueryableData ?? (object?)this.Data;
		var dataSourceHasChanged = newDataSource != this._lastAssignedDataSource;
		if (dataSourceHasChanged) {
			this._lastAssignedDataSource = newDataSource;
		}
		this.isPageableChanged = false;
		this.isSortableChanged = false;
		this.isSearchableChanged = false;

		this.ApplyCurrentData();

		if (this.IsRefreshing) {
			this.IsRefreshing = false;
			if (this.IsRefreshingChanged.HasDelegate) {
				// we do not wait for the caller to return
				_ = this.IsRefreshingChanged.InvokeAsync(this.IsRefreshing);
			}
		}

		if (this.currentRowIndex == -1 &&
			this.SelectedItems.Count > 0 &&
			this.VisibleItems.Count >= this.SelectedItems.Count) {
			this.SetCurrentRow(this.VisibleItems.IndexOf(this.SelectedItems.First()));
		}

	}

	#endregion

	#region LifeCycle

	private const string JS_MODULE_PATH = "./_content/Cirreum.Components.WebAssembly/Components/DataGrid/DataGrid.razor.js";
	private const string JS_COLUMNS_INIT = "initColumns";
	private const string JS_COLUMNS_ADD_RESIZE = "addColumnResizeHandlers";
	private const string JS_COLUMNS_REMOVE_RESIZE = "removeColumnResizeHandlers";
	private const string JS_FOOTER_ADD_RESIZE = "addGridFooterResizer";
	private const string JS_FOOTER_REMOVE_RESIZE = "removeGridFooterResizer";

	internal bool IsTableLayoutFixed {
		get {
			return (this.ResizableColumns || this.Responsive);
		}
	}
	private string TableLayout
		=> this.IsTableLayoutFixed
			? "fixed"
			: "auto";
	private string TableStyle => $"table-layout: {this.TableLayout}";

	private void CheckSelectionTypeChanged(ParameterView parameters) {
		if (parameters.TryGetValue<SelectionType>(nameof(this.SelectionType), out var value) && this.SelectionType != value) {
			if (this.SelectedItems is not null) {
				if (value == SelectionType.None && this.SelectedItems.Count > 0) {
					this.SelectedItems.Clear();
					if (this.SelectedItemsChanged.HasDelegate && this.Rendered) {
						this.SelectedItemsChanged.InvokeAsync(this.SelectedItems);
					}
				} else if (value == SelectionType.Single && this.SelectedItems.Count > 1) {
					this.SelectedItems.RemoveRange(1, this.SelectedItems.Count - 1);
					if (this.SelectedItemsChanged.HasDelegate && this.Rendered) {
						this.SelectedItemsChanged.InvokeAsync(this.SelectedItems);
					}
				}
			}
		}
	}

	private void CheckTitleChanged(ParameterView parameters) {
		if (parameters.TryGetValue<string>(nameof(this.Title), out var value) && this.Title != value) {
			this.ResolvedTitle = value;
		}
	}
	private void ApplyTitleTemplate() {
		var templatedText = this.JSApp.GetElementText($"#{this.ID} .datagrid-title", true);
		this.ResolvedTitle = templatedText.HasValue()
			? templatedText
			: this.Title;
	}

	private bool isPageableChanged;
	private void CheckIsPageableChanged(ParameterView parameters) {
		this.isPageableChanged = false;
		if (parameters.TryGetValue<bool>(nameof(this.Pageable), out var value) && this.Pageable != value) {
			// force a Data Refresh... see OnParametersSet
			this.isPageableChanged = true;
			// update footer
			if (!value) {
				this.RemoveGridFooterResizer();
			}
		}
	}
	private bool hasGridFooterResizer = false;
	private void AddGridFooterResizer() {
		if (this.hasGridFooterResizer is false) {
			this.module?.InvokeVoid(JS_FOOTER_ADD_RESIZE, this.GridFooterRef);
			this.hasGridFooterResizer = true;
		}
	}
	private void RemoveGridFooterResizer() {
		if (this.hasGridFooterResizer) {
			this.module?.InvokeVoid(JS_FOOTER_REMOVE_RESIZE, this.GridFooterRef);
			this.hasGridFooterResizer = false;
		}
	}

	private bool columnResizeHandlersConnected = false;
	private void CheckColumnsResizingChanged(ParameterView parameters) {
		if (parameters.TryGetValue<bool>(nameof(this.ResizableColumns), out var value) && this.ResizableColumns != value) {
			if (!value) {
				// remove handlers before razor
				// alters the dom and removes the
				// element(s) before we can do our
				// clean up
				this.RemoveColumnsResizing();
			}
		}
	}
	private void AddColumnsResizing() {
		if (this.Columns.Count > 0 && this.ResizableColumns) {
			if (this.columnResizeHandlersConnected is false) {
				if (this.module is not null) {
					this.module.InvokeVoid(JS_COLUMNS_ADD_RESIZE, this.GridTableRef);
					this.columnResizeHandlersConnected = true;
				}
			}
		}
	}
	private void RemoveColumnsResizing() {
		if (this.columnResizeHandlersConnected) {
			this.module?.InvokeVoid(JS_COLUMNS_REMOVE_RESIZE, this.GridTableRef);
			this.columnResizeHandlersConnected = false;
		}
	}

	private bool isSortableChanged;
	private void CheckIsSortableChanged(ParameterView parameters) {
		this.isSortableChanged = false;
		if (parameters.TryGetValue<bool>(nameof(this.Sortable), out var value) && this.Sortable != value) {
			// force a Data Refresh... see OnParametersSet
			this.isSortableChanged = true;
		}
	}

	private bool isSearchableChanged;
	private void CheckIsSearchableChanged(ParameterView parameters) {
		this.isSearchableChanged = false;
		if (parameters.TryGetValue<bool>(nameof(this.Searchable), out var value) && this.Searchable != value) {
			// force a Data Refresh... see OnParametersSet
			this.isSearchableChanged = true;
			if (this.SearchBarInstance is not null) {
				this.SearchBarInstance.GridSearchValue = "";
			}
		}
	}

	private void CheckIsShowToolbarChanged(ParameterView parameters) {
		if (parameters.TryGetValue<bool>(nameof(this.ShowToolbar), out var value) && this.ShowToolbar != value) {
			if (!value && this._renderColumnChooserPopup is not null) {
				this._renderColumnChooserPopup = null;
			}
		}
	}

	DotNetObjectReference<KeyDownHandler>? keyDownHandler;
	public sealed class KeyDownHandler(DataGrid<TData> dataGrid) {

		[JSInvokable]
		public void HandlePageDown() {
			dataGrid.HandlePageDown();
		}
		[JSInvokable]
		public void HandlePageUp() {
			dataGrid.HandlePageUp();
		}
		[JSInvokable]
		public void HandleArrowDown(bool shift) {
			dataGrid.HandleArrowDown(shift);
		}
		[JSInvokable]
		public void HandleArrowUp(bool shift) {
			dataGrid.HandleArrowUp(shift);
		}
		[JSInvokable]
		public void HandleSelectAll() {
			dataGrid.HandleSelectAll();
		}
		[JSInvokable]
		public void HandleUnselectAll() {
			dataGrid.HandleUnselectAll();
		}
		[JSInvokable]
		public void HandleHome(bool ctl) {
			dataGrid.HandleHome(ctl);
		}
		[JSInvokable]
		public void HandleEnd(bool ctl) {
			dataGrid.HandleEnd(ctl);
		}

	}

	private void ApplyJavaScriptAfterRendering() {

		if (this.module is null) {
			return;
		}

		// Extract the Title from Template (overrides Title)
		// we always call this if we have a template
		if (this._titleTemplate is not null) {
			this.ApplyTitleTemplate();
		}

		// Add footer resize observer
		if (this.IsPageable) {
			this.AddGridFooterResizer();
		}

		// Add support for column resizing
		if (this.ResizableColumns) {
			this.AddColumnsResizing();
		}

	}

	protected override bool ShouldRender() {
		if (this.isLoaded && !this._statesLoaded) {
			// Trigger async load if not loaded
			_ = this.LoadRowStates()
				.ContinueWith(t => {
					this.Update();
				});
			return false; // Skip this render
		}
		return true;
	}
	public override async Task SetParametersAsync(ParameterView parameters) {

		// SelectionType changed
		this.CheckSelectionTypeChanged(parameters);

		// Title Changed
		this.CheckTitleChanged(parameters);

		// Pageable changed
		this.CheckIsPageableChanged(parameters);

		// Sortable Changed
		this.CheckIsSortableChanged(parameters);

		// Searchable Changed
		this.CheckIsSearchableChanged(parameters);

		// ResizableColumns changed
		this.CheckColumnsResizingChanged(parameters);

		// ShowToolbar changed
		this.CheckIsShowToolbarChanged(parameters);

		await base.SetParametersAsync(parameters);

	}
	protected override void OnInitialized() {
		var initialPageSizeStr = $"{this.InitialPageSize}";
		this.PageSize = this.InitialPageSize;
		var selectedPageSize = this.PageSizes.FirstOrDefault(i => i.Value == this.PageSize);
		this.PageSizeDisplay = selectedPageSize is not null ? selectedPageSize.Display : initialPageSizeStr;
		this.Page = 1;
	}
	protected override void OnParametersSet() {
		if (this.Data is not null && this.QueryableData is not null) {
			var msg = $"{nameof(DataGrid<TData>)} requires one of {nameof(this.Data)} or {nameof(this.QueryableData)}, but both were specified.";
			throw new InvalidOperationException(msg);
		}

		var newDataSource = this.QueryableData ?? (object?)this.Data;
		var dataSourceHasChanged = newDataSource != this._lastAssignedDataSource;

		if (this.Columns.Count > 0 &&
			this.IsLoading is false) {
			this.ApplyVisibilityIndex();
		}

		if (this.Columns.Count > 0 &&
			this.IsLoading is false &&
			(dataSourceHasChanged ||
			this.isPageableChanged ||
			this.isSortableChanged ||
			this.isSearchableChanged)) {

			// Load the data, now that the user has provided some
			// and set the 'IsLoading' to false which triggered this
			// OnParametersSet to be called.
			this.Load();

		}

	}
	protected override async Task OnParametersSetAsync() {
		// Check if we need to reload states
		var newVersion = this.CalculateStateVersion();
		if (newVersion != this._stateVersion) {
			await this.LoadRowStates();
			this._stateVersion = newVersion;
		}
	}
	protected override async Task OnAfterRenderAsync(bool firstRender) {
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender) {
			this.module = await this.JSApp.InvokeAsync<IJSInProcessObjectReference>("import", JS_MODULE_PATH);
			this.keyDownHandler = DotNetObjectReference.Create(new KeyDownHandler(this));
			this.module.InvokeVoid(JS_COLUMNS_INIT, this.GridTableRef, new {
				minColumnWidth = 20,
				maxColumnBuffer = 16,
				this.keyDownHandler
			});

			// now that we've rendered, cycle again to run our ApplyJavaScriptAfterRendering
			// while we're waiting for the caller to load their data
			this.Update();
			return;
		}

		// Go ahead and the load the data, since IsLoading is false and we have data.
		// Basically means, the user provided data and isn't using the IsLoading feature.
		// aka direct/static Data was provided.
		if (this.isLoaded is false && this.IsLoading is false && (this.Data is not null || this.QueryableData is not null)) {
			this.Load();
			this.Update();
		} else {
			this.ApplyJavaScriptAfterRendering();
		}

	}

	protected override async ValueTask DisposeAsyncCore() {
		await this.ResetDetailsStateAsync();
	}
	protected override void Dispose(bool disposing) {
		if (disposing) {
			if (module != null) {
				module.InvokeVoid("disposeColumns", this.GridTableRef);
				module.Dispose();
			}
			this.keyDownHandler?.Dispose();
		}
		base.Dispose(disposing);
	}

	#endregion

}