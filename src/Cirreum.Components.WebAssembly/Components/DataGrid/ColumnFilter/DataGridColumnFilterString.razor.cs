namespace Cirreum.Components;

using Cirreum.ExpressionBuilder;
using Cirreum.ExpressionBuilder.Interfaces;

public partial class DataGridColumnFilterString<TData> : DataGridColumnFilterBase<TData> {

	public string? FilterValue { get; set; }

	protected override void OnInitialized() {
		base.OnInitialized();
		if (this.Column.Type == typeof(string)) {

			this.Column.FilterControl = this;

			if (this.Column.IsFiltered) {
				this.Operation = this.Column.Filter?.Operator;
				this.FilterValue = Convert.ToString(this.Column.Filter?.Value);
			}

		}
	}

	internal override IFilterStatement? GetFilter() {
		if (this.Operation.NumberOfValues == 0) {
			return new FilterStatement<string>(this.Column.Field, this.Operation, null, null, this.Connector);
		}
		if (this.FilterValue is null) {
			return new FilterStatement<string>(this.Column.Field, this.Operation, this.FilterValue ?? "", null, this.Connector);
		}
		return new FilterStatement<string>(this.Column.Field, this.Operation, this.FilterValue ?? "", null, this.Connector);
	}

}