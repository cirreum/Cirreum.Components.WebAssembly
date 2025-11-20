namespace Cirreum.Components;

using Cirreum.ExpressionBuilder.Interfaces;
using Cirreum.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq.Expressions;

[CascadingTypeParameter(nameof(TData))]
public sealed partial class DataGridColumn<TData> {

	/// <summary>
	/// Gets a reference to the enclosing <see cref="DataGrid{TData}" />.
	/// </summary>
	private DataGrid<TData> Grid => this.InternalGridContext.Grid;
	internal readonly RenderFragment HeaderContent;
	internal readonly RenderFragment<TData> CellContent;
	internal readonly RenderFragment FilterContent;
	internal DataGridColumnFilterBase<TData>? FilterControl { get; set; }
	internal Type Type { get; private set; } = default!;
	internal bool TypeIsNumeric { get; private set; }
	internal bool TypeIsAlphaNumeric { get; private set; }
	internal bool TypeIsOther { get; private set; }
	internal bool TypeIsBoolean { get; private set; }
	internal bool TypeIsEnum { get; private set; }
	internal bool TypeIsDateTime { get; private set; }
	/// <summary>
	/// The 1-Based Sort Order of this Column relative to the other Columns
	/// </summary>
	internal int ColumnIndex { get; set; }
	internal readonly string ColumnId = IdGenerator.Next;

	public DataGridColumn() {
		this.HeaderContent = this.RenderHeaderContent;
		this.CellContent = item => builder => this.RenderCellContent(builder, item);
		this.FilterContent = this.RenderFilterContainerContent;
	}

	[CascadingParameter]
	internal InternalGridContext<TData> InternalGridContext { get; set; } = default!;


	#region Templates

	/// <summary>
	/// RenderFragment to support user supplied rendering.
	/// </summary>
	[Parameter]
	public RenderFragment<TData>? ChildContent { get; set; }

	#endregion

	#region Default Rendering

	private static readonly ConcurrentDictionary<string, string> _formatCache = new();
	internal string Render(TData data) {

		if (this.IsVisible is false) {
			return string.Empty;
		}

		if (data == null) {
			return string.Empty;
		}

		if (string.IsNullOrWhiteSpace(this.Field)) {
			return string.Empty;
		}

		var propGetValue = DelegateCache<TData>.GetPropertyAccessor(this.Field);
		if (propGetValue == null) {
			return string.Empty;
		}

		object? value;

		try {
			value = propGetValue(data);
		} catch (NullReferenceException) {
			value = null;
		}

		if (value == null) {
			return string.Empty;
		}

		if (string.IsNullOrEmpty(this.Format)) {
			return value.ToString() ?? string.Empty;
		}

		var formatString = _formatCache.GetOrAdd(this.Format, f => $"{{0:{f}}}");
		return string.Format(CultureInfo.CurrentUICulture, formatString, value);

	}

	internal Expression<Func<TData, object>>? GetPropertyAccessor() {
		if (string.IsNullOrWhiteSpace(this.Field) is false) {
			return ExpressionCache<TData>.GetPropertyExpression(this.Field);
		}
		return null;
	}
	private static string GetItemId(TData item) => item?.GetHashCode().ToString() ?? "";

	#endregion

	#region Event Handlers


	internal void HandleColumnFilterClick() {
		if (this.Grid.HasData) {
			this.Grid.ToggleColumnFilter(this);
		}
	}

	internal Task HandleOutsideClick(string targetId) {
		if (this.HeaderFilterId != targetId) {
			this.Grid.CloseColumnFilter();
		}
		return Task.CompletedTask;
	}

	string noop = "";
	// we need this to attach to the event.
	internal void HandleDragOver() {
		this.noop = "";
	}
	// we need this to attach to the event.
	internal void ResizeHandlerClicked() {
		if (this.noop != null) {
			this.noop = "";
		}
	}

	internal void HandleDragStart(DragEventArgs args) {
		args.DataTransfer.EffectAllowed = "move";
		args.DataTransfer.DropEffect = "move";
		this.Grid.HandleDragStart(this);
	}
	internal void HandleDrop() {
		this.Grid.HandleDrop(this);
	}


	#endregion

	#region Options

	/// <summary>
	/// The name of the class property or field
	/// used to populate the column's value.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This is also the columns default header text.
	/// </para>
	/// <para>
	/// See <see cref="Header"/> and <see cref="HeaderTemplate"/> for other options.
	/// </para>
	/// </remarks>
	[Parameter]
	public string Field { get; set; } = "";

	/// <summary>
	/// Optional value to use for the column's header text. If not provided,
	/// the <see cref="Field"/> will be used.
	/// </summary>
	[Parameter]
	public string Header { get; set; } = "";

	/// <summary>
	/// Optional template to use for the column's header text.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The sorting, filtering and column resizing are provided out of the box, so this
	/// template is only useful for custom display purposes, such as icons or fonts or coloring.
	/// </para>
	/// </remarks>
	[Parameter]
	public RenderFragment<DataGridColumn<TData>>? HeaderTemplate { get; set; }

	/// <summary>
	/// Resolved header display text (header or field)
	/// </summary>
	internal string HeaderText {
		get {
			return string.IsNullOrWhiteSpace(this.Header) ? this.Field : this.Header;
		}
	}

	/// <summary>
	/// The desired Width of the column in pixels.
	/// </summary>
	/// <remarks>
	/// Must be a value greater than or equal to 16(px).
	/// <para>
	/// To hide a column, set <see cref="Visible"/> to <see langword="false"/>.
	/// </para>
	/// </remarks>
	[Parameter]
	public int? Width { get; set; }
	private const int width_minimum = 16;

	/// <summary>
	/// The desired Minimum Width of the column in pixels.
	/// </summary>
	[Parameter]
	public int? MinWidth { get; set; }

	/// <summary>
	/// Gets or sets if the column is visible within the datagrid.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool Visible { get; set; } = true;
	internal bool IsVisible { get; set; }
	private int VisibleIndex { get; set; }
	internal void SetVisibilityIndex(int newIndex) {
		this.VisibleIndex = newIndex;
		this.ApplyHeaderStyle();
	}

	/// <summary>
	/// Gets or sets if the column is resizable.
	/// </summary>
	/// <remarks>
	/// This value is ignored, if <see cref="DataGrid{TData}.ResizableColumns"/>
	/// is set to <see langword="false"/>.
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool Resizable { get; set; } = true;

	/// <summary>
	/// Gets or sets if this column is available in the Column Chooser
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool VisibleInColumnChooser { get; set; } = true;

	/// <summary>
	/// Gets or sets if the Column can be sorted
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool Sortable { get; set; } = true;
	private bool CanSort = false;
	internal bool IsSortable => this.CanSort && this.IsVisible;
	void ApplyCanSort() {
		if (this.Type == null) {

			if (string.IsNullOrWhiteSpace(this.Field)) {
				this.CanSort = false;
				return;
			}

			var member = this.Grid.Properties[this.Field];
			if (member == null) {
				this.CanSort = false;
				return;
			}

		}

		this.CanSort = this.Sortable;

	}

	/// <summary>
	/// Gets or sets if the Column can be filtered
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool Filterable { get; set; } = true;
	internal bool IsFilterable => this.Filterable && this.IsVisible &&
		(this.Grid.VisibleItems.Count > 0 || this.Grid.IsFilteredByColumn);

	/// <summary>
	/// Gets or sets if the Column can be searched via the datagrid's search bar
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="true"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool Searchable { get; set; } = true;
	internal bool IsSearchable => this.Searchable && this.IsVisible;


	/// <summary>
	/// Gets or sets the optional <see cref="AggregateType"/> for the footer. It can only be applied to
	/// numerical fields (e.g. int, long decimal, double, etc.).
	/// </summary>
	[Parameter]
	public AggregateType? Aggregate { get; set; }

	/// <summary>
	/// Gets or sets the string format for values when no template is used
	/// </summary>
	[Parameter]
	public string Format { get; set; } = "";

	/// <summary>
	/// Gets or sets if this Column is the default Column that is sorted
	/// </summary>
	[Parameter]
	public bool? DefaultSortColumn { get; set; }

	/// <summary>
	/// Gets or sets if the default sort order is descending.
	/// </summary>
	[Parameter]
	public bool? DefaultSortDescending { get; set; }

	/// <summary>
	/// Gets or sets the Horizontal <see cref="TextAlign"/>
	/// </summary>
	[Parameter]
	public TextAlign Align { get; set; }

	/// <summary>
	/// Set to <see langword="true"/>, to allow the text to wrap within the cell
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: <see langword="false"/>
	/// </para>
	/// </remarks>
	[Parameter]
	public bool AllowTextWrap { get; set; } = false;

	/// <summary>
	/// Optional cell Css class(es)
	/// </summary>
	[Parameter]
	public string CellCss { get; set; } = "";
	private string _cellCss = "";

	private bool _cellClassListIsDirty = true;
	private string? _cellClassList;
	internal string? CellClassList {
		get {
			if (this._cellClassListIsDirty) {
				this._cellClassList = CssBuilder
					.Empty()
						.AddClassIfNotEmpty(this._cellCss)
						.AddClass("sorting_1", when: this.SortColumn)
					.NullIfEmpty();
				this._cellClassListIsDirty = false;
			}
			return this._cellClassList;
		}
	}

	/// <summary>
	/// Cell Custom Style
	/// </summary>
	internal string? CellStyle = "";

	/// <summary>
	/// Optional Css Classes for this column's header (th)
	/// </summary>
	/// <remarks>
	/// See <see cref="DataGrid{T}.HeadersCss"/> for the default for all columns.
	/// </remarks>
	[Parameter]
	public string HeaderCss { get; set; } = "";

	/// <summary>
	/// Optional column header content Css class(es)
	/// </summary>
	[Parameter]
	public string HeaderContentCss { get; set; } = "";

	private string? HeaderCssClasses => CssBuilder
			.Empty()
				.AddClassIfNotEmpty(this.Grid.HeadersCss)
				.AddClassIfNotEmpty(this.HeaderCss)
			.NullIfEmpty();

	private string HeaderContentCssClasses = "";

	private string HeaderStyle = "";
	private void ApplyHeaderStyle() {
		this.HeaderStyle = StyleBuilder
			.Empty()
				.AddStyle("min-width", $"{this.MinWidth}px", this.MinWidth.HasValue)
				.AddStyle("width", $"{this.Width}px", this.CanSetWidth)
			.Build();
	}

	/// <summary>
	/// Determines whether this column's width can be set based on the grid's layout mode and column position.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In fixed table layout mode: width can be set on any column that has a width value.
	/// </para>
	/// <para>
	/// In auto table layout mode: width can only be set on columns that have a width value and
	/// are not the last columns. This is to allow the last column to flex and fill remaining space.
	/// </para>
	/// </remarks>
	/// <value>
	/// <see langword="true"/> if the column has a width value and either the grid uses fixed table layout 
	/// or this is not the last visible column in auto table layout mode; otherwise, <see langword="false"/>.
	/// </value>
	private bool CanSetWidth =>
		this.Width is not null
		&& ((this.VisibleIndex < this.Grid.VisibleColumnCount) || this.Grid.IsTableLayoutFixed);

	/// <summary>
	/// Determines whether this column can be resized by the user based on grid settings, column properties, and layout mode.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In fixed table layout mode: any visible, resizable column can be resized when grid allows column resizing.
	/// </para>
	/// <para>
	/// In auto table layout mode: only visible, resizable columns that are not the last columns can be resized. 
	/// This prevents resizing the last column which needs to flex and fill remaining space.
	/// </para>
	/// </remarks>
	/// <value>
	/// <see langword="true"/> if the grid allows resizable columns, this column is visible and resizable, 
	/// and either the grid uses fixed table layout or this is not the last visible column in auto table layout mode; 
	/// otherwise, <see langword="false"/>.
	/// </value>
	private bool CanResize =>
	   this.Grid.ResizableColumns
		   && this.IsVisible
		   && this.Resizable
	   && ((this.VisibleIndex < this.Grid.VisibleColumnCount) || this.Grid.IsTableLayoutFixed);

	private readonly string HeaderFilterId = IdGenerator.Next;

	private string HeaderFilterIconClass => CssBuilder
		.Default("bi")
			.AddClass("bi-search", when: this.IsFiltered is false)
			.AddClass("bi-funnel-fill", when: this.IsFiltered)
		.Build();

	private string HeaderFilterCss =>
		CssBuilder.Default("filterable")
			.AddClass("is-filtered", when: this.IsFiltered)
			.AddClass("disabled", when: this.Grid.HasData is false)
		.Build();
	private string? HeaderFilterAriaLabel =>
		ValueBuilder.Empty()
			.AddValue($"filter {this.HeaderText}...", when: this.HeaderText.HasValue() && this.Grid.HasData)
		.NullIfEmpty();

	/// <summary>
	/// Gets or sets the Column's Footer Css Class
	/// </summary>
	[Parameter]
	public string FooterClass { get; set; } = "";

	internal string? FooterCss => CssBuilder
		.Empty()
			.AddClassIfNotEmpty(this.FooterClass)
		.NullIfEmpty();

	internal string? FooterStyle => StyleBuilder
		.Empty()
			.AddStyle("text-align", $"{this.Align.ToName()}", when: this.Align > 0)
		.NullIfEmpty();

	private string AriaSort =>
		this.SortColumn ? (this.SortDescending ? "descending" : "ascending") : "none";

	private string Draggable => (
		this.IsFilterVisible is false &&
		this.Grid.Reorderable)
		.ToString().ToLowerInvariant();

	#endregion

	#region Sorting

	/// <summary>
	/// True if this is the current Sort Column
	/// </summary>
	public bool SortColumn => this._sortColumn;
	private bool _sortColumn;
	private void SetSortColumn(bool sortColumn) {
		if (this._sortColumn != sortColumn) {
			this._sortColumn = sortColumn;
			this._cellClassListIsDirty = true;
		}
	}

	/// <summary>
	/// Direction of sorting
	/// </summary>
	public bool SortDescending { get; private set; }

	/// <summary>
	/// Sort by this column
	/// </summary>
	public void SortBy() {

		if (this.IsSortable && !this.IsFilterVisible && this.Grid is not null) {

			if (this.SortColumn) {
				this.SortDescending = !this.SortDescending;
			}

			this.Grid.ResetSort();

			this.SetSortColumn(true);

			this.Grid.ApplyCurrentData();
			this.Grid.Update();

		}

	}

	internal void ResetSort() {
		this.SetSortColumn(false);
	}

	internal const string DefaultSortIconColorCss = "text-primary";
	internal string? SortIconTitle => ValueBuilder
		.Empty()
			.AddValue("sort ascending...", when: this.IsSortable && !this.SortColumn)
			.AddValue("sorted ascending", when: this.SortColumn && !this.SortDescending)
			.AddValue("sorted descending", when: this.SortColumn && this.SortDescending)
		.NullIfEmpty();

	internal string SortIconClass => CssBuilder
		.Default("mx-1 bi")
			.AddClass("bi-sort-alpha-down", when: this.IsSortAlphaAsc)
			.AddClass("bi-sort-alpha-down-alt", when: this.IsSortAlphaDesc)
			.AddClass("bi-sort-numeric-down", when: this.IsSortNumericAsc)
			.AddClass("bi-sort-numeric-down-alt", when: this.IsSortNumericDesc)
			.AddClass("bi-sort-down-alt", when: this.IsSortOtherAsc) // this icon is backwards compared to the others
			.AddClass("bi-sort-down", when: this.IsSortOtherDesc) // this icon is backwards compared to the others
			.AddClass("bi-sort-alpha-down invisible", when: !this.SortColumn)
			.AddClass(this.Grid.SortIconColorCss!, when: !string.IsNullOrWhiteSpace(this.Grid.SortIconColorCss))
			.AddClass(DefaultSortIconColorCss, when: string.IsNullOrWhiteSpace(this.Grid.SortIconColorCss))
		.Build();

	private bool IsSortAlphaAsc =>
			this.SortColumn &&
			this.SortDescending is false &&
			this.TypeIsAlphaNumeric;
	private bool IsSortAlphaDesc =>
			this.SortColumn &&
			this.SortDescending &&
			this.TypeIsAlphaNumeric;

	private bool IsSortNumericAsc =>
			this.SortColumn &&
			this.SortDescending is false &&
			this.TypeIsNumeric;
	private bool IsSortNumericDesc =>
			this.SortColumn &&
			this.SortDescending &&
			this.TypeIsNumeric;

	private bool IsSortOtherAsc =>
			this.SortColumn &&
			this.SortDescending is false && (
			this.TypeIsOther);
	private bool IsSortOtherDesc =>
			this.SortColumn &&
			this.SortDescending && (
			this.TypeIsOther);

	#endregion

	#region Filtering

	/// <summary>
	/// If the <see cref="Type"/> is a <see cref="DateTimeOffset"/>, specifies
	/// the minimum date/time allowed when filtering.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: 9-22-1970 11:44:02 +0
	/// </para>
	/// </remarks>
	[Parameter]
	public DateTimeOffset MinDateTimeOffset { get; set; } = DateTimeOffset.Parse("9-22-1970 11:44:02 +0");

	/// <summary>
	/// If the <see cref="Type"/> is a <see cref="DateTimeOffset"/>, specifies
	/// the maximum date/time allowed when filtering.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: 9-22-1970 11:44:02 +0
	/// </para>
	/// </remarks>
	[Parameter]
	public DateTimeOffset MaxDateTimeOffset { get; set; } = DateTimeOffset.Parse("9-22-2070 11:44:02 +0");


	/// <summary>
	/// If the <see cref="Type"/> is a <see cref="DateTime"/>, specifies
	/// the minimum date/time allowed when filtering.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: 9-22-1970 11:44:02
	/// </para>
	/// </remarks>
	[Parameter]
	public DateTime MinDateTime { get; set; } = DateTime.Parse("9-22-1970 11:44:02");

	/// <summary>
	/// If the <see cref="Type"/> is a <see cref="DateTime"/>, specifies
	/// the maximum date/time allowed when filtering.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Default: 9-22-1970 11:44:02
	/// </para>
	/// </remarks>
	[Parameter]
	public DateTime MaxDateTime { get; set; } = DateTime.Parse("9-22-2070 11:44:02");

	/// <summary>
	/// Gets or sets the current filter for the column.
	/// </summary>
	public IFilterStatement? Filter { get; set; }

	/// <summary>
	/// Gets if the filter condition has been set for this column.
	/// </summary>
	public bool IsFiltered {
		get {
			return this.Filter != null && this.IsVisible;
		}
	}

	/// <summary>
	/// Gets or sets the reference to the Filter Element
	/// </summary>
	private ElementReference FilterElement;

	/// <summary>
	/// Place custom controls which implement IFilter
	/// </summary>
	[Parameter]
	public RenderFragment<DataGridColumn<TData>>? CustomFilter { get; set; }


	/// <summary>
	/// Is the Filter popup open
	/// </summary>
	public bool IsFilterVisible => filterPopover?.IsOpen ?? false;
	private string IsFilterOpenAria => this.IsFilterVisible.ToAttributeValue();

	private void KeyPress(KeyboardEventArgs e) {
		var key = e.Code ?? e.Key;
		if (key == "Escape") {
			this.CancelFilter();
		} else if (key == "Enter") {
			this.ApplyFilter();
		}
	}

	private void ApplyFilter() {

		this.Grid.CloseColumnFilter();

		if (this.FilterControl != null) {
			var newFilter = this.FilterControl.GetFilter();
			if (newFilter is not null && this.Filter is not null) {
				var existingStr = this.Filter.ToString();
				if (existingStr.HasValue() && existingStr.Equals(newFilter.ToString()) is false) {
					this.Filter = newFilter;
					this.Grid.ApplyCurrentData();
					this.Grid.Update();
				}
			} else {
				this.Filter = newFilter;
				this.Grid.ApplyCurrentData();
				this.Grid.Update();
			}
		}

	}

	private void ClearFilter() {

		this.Grid.CloseColumnFilter();

		if (this.FilterControl != null) {
			if (this.Filter is not null) {
				this.Filter = null;
				this.Grid.ApplyCurrentData();
				this.Grid.Update();
			}
		}

	}

	private void CancelFilter() {
		this.Grid.CloseColumnFilter();
	}

	private Popover filterPopover = default!;

	internal void ShowFilter() {
		this.RunAfterRender(() => {
			this.filterPopover.Open();
		});
		this.Update();
	}
	internal void HideFilter() {
		this.filterPopover.Close();
	}

	#endregion

	#region Footer

	/// <summary>
	/// Set a custom Footer value for the Column.
	/// </summary>
	[Parameter]
	public string? CustomFooterValue { get; set; }

	/// <summary>
	/// Returns aggregation of this column for the table footer based on given type: Sum, Average, Count, Min, or Max.
	/// </summary>
	/// <returns>string results</returns>
	internal string GetFooterValue() {

		if (this.Grid.ShowFooter is false) {
			return string.Empty;
		}

		if (this.Aggregate.HasValue is false) {
			return string.Empty;
		}
		if (string.IsNullOrEmpty(this.Field)) {
			return string.Empty;
		}
		if (this.Grid is null) {
			return string.Empty;
		}

		Task.Yield(); // allow the UI thread to process pending queue

		var val = this.Grid.GetFooterValue(this.Field, this.Aggregate);
		var formatString = _formatCache.GetOrAdd(this.Format, f => $"{{0:{f}}}");
		var final = string.Format(CultureInfo.CurrentCulture, formatString, val);
		return final;

	}

	internal object? ColumnMaxValue() {
		if (string.IsNullOrEmpty(this.Field)) {
			return null;
		}
		return this.Grid.GetColumnMaxValue(this.Field);
	}
	internal object? ColumnMinValue() {
		if (string.IsNullOrEmpty(this.Field)) {
			return null;
		}
		return this.Grid.GetColumnMinValue(this.Field);
	}


	#endregion

	#region Life Cycle

	void InitializeFieldType() {

		if (this.Type == null) {

			if (string.IsNullOrWhiteSpace(this.Field)) {
				this.Filterable = false;
				this.CanSort = false;
				this.Searchable = false;
				return;
			}

			var member = this.Grid.Properties[this.Field];
			if (member == null) {
				this.Filterable = false;
				this.CanSort = false;
				this.Searchable = false;
				return;
			}

			this.Type = member.MemberType;

			var typeIsNumeric = this.Type.IsNumeric();
			this.TypeIsBoolean = this.Type == typeof(bool) || this.Type == typeof(bool?);
			this.TypeIsEnum = this.Type.IsEnum;
			this.TypeIsDateTime = this.Type.IsDateAndOrTime();
			this.TypeIsOther = this.TypeIsBoolean || this.TypeIsEnum || this.TypeIsDateTime;
			this.TypeIsNumeric = this.TypeIsOther is false && typeIsNumeric;
			this.TypeIsAlphaNumeric = this.TypeIsOther is false && typeIsNumeric is false;

		}

	}

	protected override void OnInitialized() {

		this.IsVisible = this.Visible;

		this.InitializeFieldType();

		this.ColumnIndex = this.Grid.AssociateColumn(this);

		this._cellCss = this.CellCss;

	}

	protected override void OnParametersSet() {

		this.ApplyCanSort();

		if (this.DefaultSortDescending.HasValue) {
			this.SortDescending = this.DefaultSortDescending.Value;
		}

		if (this.DefaultSortColumn.HasValue && this.IsVisible) {
			this.SetSortColumn(this.DefaultSortColumn.Value);
		}

		if (this.MinWidth.HasValue && this.MinWidth < width_minimum) {
			throw new ArgumentOutOfRangeException(
				nameof(this.MinWidth),
				$"{nameof(this.MinWidth)} must be at least {width_minimum}.");
		}

		if (this.Width.HasValue && this.Width < width_minimum) {
			throw new ArgumentOutOfRangeException(
				nameof(this.Width),
				$"{nameof(this.Width)} must be at least {width_minimum}.");
		}

		if (this.MinWidth.HasValue && this.Width.HasValue && this.MinWidth > this.Width) {
			throw new ArgumentException(
				$"{nameof(this.MinWidth)} cannot be greater than {nameof(this.Width)}.");
		}


		if (this._cellCss != this.CellCss) {
			this._cellCss = this.CellCss;
			this._cellClassListIsDirty = true;
		}

		this.ApplyHeaderStyle();

		this.HeaderContentCssClasses = CssBuilder
			.Default(this.HeaderContentCss)
				.AddClass("sortable", when: this.CanSort)
				.AddClass("me-auto", when: this.Align == TextAlign.Start || this.Align == TextAlign.None)
				.AddClass("ms-auto", when: this.Align == TextAlign.End)
				.AddClass("mx-auto", when: this.Align == TextAlign.Center)
			.Build();

		this.CellStyle = StyleBuilder
			.Empty()
				.AddStyle("text-align", $"{this.Align.ToName()}", when: this.Align > 0)
				.AddStyle("text-overflow", "ellipsis", when: this.AllowTextWrap is false)
				.AddStyle("white-space", "nowrap", when: this.AllowTextWrap is false)
				.AddStyle("overflow", "hidden", when: this.AllowTextWrap is false)
			.NullIfEmpty();

	}

	#endregion

}