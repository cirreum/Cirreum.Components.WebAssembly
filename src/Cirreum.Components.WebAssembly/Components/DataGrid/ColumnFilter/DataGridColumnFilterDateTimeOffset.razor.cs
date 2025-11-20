namespace Cirreum.Components;

using Cirreum.ExpressionBuilder;
using Cirreum.ExpressionBuilder.Interfaces;
using Cirreum.Extensions;
using System;

public partial class DataGridColumnFilterDateTimeOffset<TData> {

	public DateTimeOffset FilterValue { get; set; }

	public DateTimeOffset FilterValue2 { get; set; }

	protected override void OnInitialized() {

		base.OnInitialized();

		if (this.Column.Type.GetNonNullableType() == typeof(DateTimeOffset)) {

			this.Column.FilterControl = this;

			if (this.Column.IsFiltered) {

				this.Operation = this.Column.Filter?.Operator;

				if (this.Operation.NumberOfValues > 0) {
					this.FilterValue =
						this.Column.Filter?.Value?.GetCastedInstanceOrDefault<DateTimeOffset>() ?? DateTimeOffset.Now;
				}

				if (this.Operation.NumberOfValues > 1) {
					this.FilterValue2 =
						this.Column.Filter?.Value2?.GetCastedInstanceOrDefault<DateTimeOffset>() ?? DateTimeOffset.Now;
				}

			} else {

				this.FilterValue = (DateTimeOffset)(this.Column.ColumnMinValue() ?? DateTimeOffset.Now);

				this.FilterValue2 = (DateTimeOffset)(this.Column.ColumnMaxValue() ?? DateTimeOffset.Now);

			}

		}

	}

	internal override IFilterStatement GetFilter() {

		var nullableType = Nullable.GetUnderlyingType(this.Column.Type);

		if (nullableType == null) {
			var f1 = this.Operation.NumberOfValues > 0 ? this.FilterValue : default;
			var f2 = this.Operation.NumberOfValues > 1 ? this.FilterValue2 : default;
			return new FilterStatement<DateTimeOffset>(this.Column.Field, this.Operation, f1, f2, this.Connector);
		}

		DateTimeOffset? f1n = this.Operation.NumberOfValues > 0 ? this.FilterValue : null;
		DateTimeOffset? f2n = this.Operation.NumberOfValues > 1 ? this.FilterValue2 : null;
		return new FilterStatement<DateTimeOffset?>(this.Column.Field, this.Operation, f1n, f2n, this.Connector);

	}

}