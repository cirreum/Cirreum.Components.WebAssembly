namespace Cirreum.Components;

using Cirreum.ExpressionBuilder;
using Cirreum.ExpressionBuilder.Interfaces;
using Cirreum.Extensions;

public partial class DataGridColumnFilterBoolean<TData> {

	public string FilterValue {
		get {
			return this._filterValue.ToString();
		}
		set {
			this._filterValue = bool.Parse(value);
		}
	}
	private bool _filterValue;

	protected override void OnInitialized() {

		base.OnInitialized();

		if (this.Column.Type is not null) {
			if (this.Column.Type.GetNonNullableType() == typeof(bool)) {

				this.Column.FilterControl = this;

				if (this.Column.IsFiltered) {

					this.Operation = this.Column.Filter?.Operator;
					if (this.Operation is not null && this.Operation.NumberOfValues > 0) {
						var v1 = (bool?)this.Column.Filter?.Value;
						if (v1.HasValue) {
							this._filterValue = v1.Value;
						}
					}

				} else {

					if (this.Operation is not null && this.Operation.NumberOfValues > 0) {
						this._filterValue = false;
					}

				}

			}
		}

	}

	internal override IFilterStatement? GetFilter() {

		Type? nullableType = null;

		if (this.Column.Type is not null) {
			nullableType = this.Column.Type.GetNonNullableType();
		}

		if (nullableType is null && this.Operation is not null) {
			var f1 = this.Operation.NumberOfValues > 0 && this._filterValue;
			bool f2 = default;
			return new FilterStatement<bool>(this.Column.Field, this.Operation, f1, f2, this.Connector);
		}

		if (this.Operation is not null) {
			bool? f1n = this.Operation.NumberOfValues > 0 ? this._filterValue : null;
			bool? f2n = null;
			return new FilterStatement<bool?>(this.Column.Field, this.Operation, f1n, f2n, this.Connector);
		}

		return null;

	}

}