namespace Cirreum.Components;

using Cirreum.ExpressionBuilder;
using Cirreum.ExpressionBuilder.Interfaces;
using Cirreum.Extensions;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

public partial class DataGridColumnFilterEnum<TData> {

	public string FilterValue {
		get {
			return this._filterValue?.ToString() ?? "";
		}
		set {
			this._filterValue = (Enum)Enum.Parse(this.Column.Type.GetNonNullableType(), value);
		}
	}
	private Enum? _filterValue;

	protected override void OnInitialized() {

		base.OnInitialized();

		if (this.Column.Type.GetNonNullableType().IsEnum) {

			this.Column.FilterControl = this;

			if (this.Column.IsFiltered) {

				this.Operation = this.Column.Filter?.Operator;

				if (this.Operation.NumberOfValues > 0 && this.Column.Filter is not null) {
					this._filterValue = (Enum)Enum.Parse(this.Column.Type.GetNonNullableType(), $"{this.Column.Filter.Value}");
				}

			} else {

				var enumValue = Enum.GetValues(this.Column.Type.GetNonNullableType()).GetValue(0);
				if (enumValue is not null) {
					this._filterValue = (Enum)enumValue;
				}
			}

		}

	}

	internal override IFilterStatement GetFilter() {
		return new FilterStatement<Enum>(this.Column.Field, this.Operation, this._filterValue, null, this.Connector);
	}

	private List<Enum>? items;
	public IEnumerable<Enum> Items {
		get {
			if (this.items == null) {
				var enums = Enum.GetValues(this.Column.Type.GetNonNullableType());
				this.items = [];
				var enumerator = enums.GetEnumerator();
				while (enumerator.MoveNext()) {
					this.items.Add((Enum)enumerator.Current);
				}
			}
			return this.items;
		}
	}

	public void OnItemChanged(ChangeEventArgs args) {
		this.FilterValue = args.Value?.ToString() ?? "";
	}

}