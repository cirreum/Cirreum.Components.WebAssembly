namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

[CascadingTypeParameter(nameof(TData))]
public partial class DataGridColumnChooser<TData> {

	private ElementReference ColumnChooserButtonRef;
	private List<DataGridColumnVisibleModel> _columnChooserListItems = [];
	private IReadOnlyList<DataGridColumnVisibleModel> ColumnChooserListItems {
		get {
			if (this._columnChooserListItems.Count == 0) {
				this._columnChooserListItems = this.InternalGrid.GetChoosableColumns();
			}
			return this._columnChooserListItems;
		}
	}

	private Popover? ChooserPopover { get; set; }
	public bool IsPopoverOpen { get; set; }

	[CascadingParameter]
	private InternalGridContext<TData> InternalGridContext { get; set; } = default!;
	private DataGrid<TData> InternalGrid => this.InternalGridContext.Grid;

	[Parameter]
	public bool IsGridBusy { get; set; }

	private string ChooserButtonCss => CssBuilder
		.Default("btn")
			.AddClass("btn-outline-primary")
			.AddClass("btn-sm")
			.AddClass("active", when: this.IsPopoverOpen)
		.Build();

	private void ApplyColumns() {

		this.HideChooser();

		this.InternalGrid.ApplyColumnsVisibility(this._columnChooserListItems);

		this._columnChooserListItems.Clear();

	}
	private void CancelColumns() {

		this.HideChooser();

		this.InternalGrid.CancelColumnVisibility(this._columnChooserListItems);

		this._columnChooserListItems.Clear();

	}

	private void HandleChooserButtonClick() {

		if (this.IsPopoverOpen) {
			this.HideChooser();
			return;
		}

		this.ShowChooser();

	}

	private Task HandleOutsideClick(string targetId) {
		if (this.ElementId != targetId) {
			this.CancelColumns();
		}
		return Task.CompletedTask;
	}

	private void ShowChooser() {
		this.ChooserPopover?.Open();
	}
	private void HideChooser() {
		this.ChooserPopover?.Close();
	}

	#region SelectAll Options

	private bool? AreAllColumnsVisible {
		get {
			if (this.AllColumnsChosen) {
				return true;
			}
			if (this.SomeColumnsChosen) {
				return null;
			}
			return false;
		}
		set {
			// do nothing...
			if (value is null) {
				return;
			}
			if (value is true) {
				foreach (var column in this.ColumnChooserListItems.Where(c => !c.IsFiltered)) {
					column.IsVisible = true;
				}
				return;
			}
			foreach (var column in this.ColumnChooserListItems.Where(c => !c.IsFiltered)) {
				column.IsVisible = false;
			}
		}
	}

	private bool AllColumnsChosen =>
		this.ColumnChooserListItems.Where(c => !c.IsFiltered).All(c => c.IsVisible);

	private bool SomeColumnsChosen =>
		this.ColumnChooserListItems.Where(c => !c.IsFiltered).Any(c => c.IsVisible)
		&& this.ColumnChooserListItems.Where(c => !c.IsFiltered).Any(c => !c.IsVisible);

	private void OnAllColumnsVisibleChanged(bool? value) {
		this.AreAllColumnsVisible = value;
		this.InternalGrid.Update();
	}

	private void OnColumnVisibilityChanged(DataGridColumnVisibleModel model, bool value) {
		model.IsVisible = value;
		this.InternalGrid.Update();
	}

	private void HandleChooserButtonKeyDown(KeyboardEventArgs e) {
		var key = e.Code ?? e.Key;
		if (key == "Escape") {
			this.CancelColumns();
		} else if (key == "Enter") {
			this.ShowChooser();
		}
	}
	private void HandlePopoverKeyDown(KeyboardEventArgs e) {
		var key = e.Code ?? e.Key;
		if (key == "Escape") {
			this.CancelColumns();
		} else if (key == "Enter") {
			this.ApplyColumns();
		}
	}

	#endregion

	private readonly RenderFragment _renderPopover;
	public DataGridColumnChooser() {
		this._renderPopover = this.RenderPopover;
	}
	protected override void OnInitialized() {
		if (this.InternalGridContext is null) {
			throw new InvalidOperationException($"{nameof(DataGridColumnChooser<>)} must be used within a {nameof(DataGrid<>)} component.");
		}
		this.InternalGrid.SetColumnChooserPopup(this._renderPopover);
	}
	protected override async Task OnAfterRenderAsync(bool firstRender) {
		await base.OnAfterRenderAsync(firstRender);
		if (this.IsPopoverOpen) {
			this.ChooserPopover?.Update();
		}
	}

}