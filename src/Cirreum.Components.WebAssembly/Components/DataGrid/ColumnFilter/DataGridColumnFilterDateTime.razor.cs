namespace Cirreum.Components;

using Cirreum.ExpressionBuilder;
using Cirreum.ExpressionBuilder.Interfaces;
using Cirreum.Extensions;
using System;

public partial class DataGridColumnFilterDateTime<TData> {

	public DateTime FilterValue { get; set; }

	public DateTime FilterValue2 { get; set; }

	protected override void OnInitialized() {

		base.OnInitialized();

		if (this.Column.Type is not null) {

			if (this.Column.Type.GetNonNullableType() == typeof(DateTime)) {

				this.Column.FilterControl = this;

				if (this.Column.IsFiltered && this.Column.Filter is not null) {

					this.Operation = this.Column.Filter.Operator;

					if (this.Operation.NumberOfValues > 0) {
						this.FilterValue = this.Column.Filter.Value?.GetCastedInstanceOrDefault<DateTime>()
							?? DateTime.Now;
					}

					if (this.Operation.NumberOfValues > 1) {
						this.FilterValue2 = this.Column.Filter.Value2?.GetCastedInstanceOrDefault<DateTime>()
							?? DateTime.Now;
					}

				} else {

					this.FilterValue = (DateTime)(this.Column.ColumnMinValue() ?? DateTime.Now);

					this.FilterValue2 = (DateTime)(this.Column.ColumnMaxValue() ?? DateTime.Now);

				}

			}

		}

	}

	internal override IFilterStatement GetFilter() {

		var nullableType = Nullable.GetUnderlyingType(this.Column.Type);

		if (nullableType == null) {
			var f1 = this.Operation.NumberOfValues > 0 ? this.FilterValue : default;
			var f2 = this.Operation.NumberOfValues > 1 ? this.FilterValue2 : default;
			return new FilterStatement<DateTime>(this.Column.Field, this.Operation, f1, f2, this.Connector);
		}

		DateTime? f1n = this.Operation.NumberOfValues > 0 ? this.FilterValue : null;
		DateTime? f2n = this.Operation.NumberOfValues > 1 ? this.FilterValue2 : null;
		return new FilterStatement<DateTime?>(this.Column.Field, this.Operation, f1n, f2n, this.Connector);

	}

}