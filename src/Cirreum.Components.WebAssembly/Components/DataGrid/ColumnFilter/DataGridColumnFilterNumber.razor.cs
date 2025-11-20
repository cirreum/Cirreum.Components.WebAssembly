namespace Cirreum.Components;

using Cirreum.ExpressionBuilder;
using Cirreum.ExpressionBuilder.Interfaces;
using Cirreum.Extensions;
using System;
using System.Globalization;

public partial class DataGridColumnFilterNumber<TData> : DataGridColumnFilterBase<TData> {

	private string _filterValue = "";
	private string _filterValue2 = "";

	public string FilterValue {
		get => this._filterValue;
		set => this._filterValue = value ?? "";
	}

	public string FilterValue2 {
		get => this._filterValue2;
		set => this._filterValue2 = value ?? "";
	}

	protected override void OnInitialized() {
		base.OnInitialized();

		if (this.Column?.Type == null) {
			return;
		}

		var targetType = this.Column.Type.GetNonNullableType();
		if (!targetType.IsNumeric() || targetType.IsEnum) {
			return;
		}

		this.Column.FilterControl = this;

		this.InitializeFilterValues();
	}

	private void InitializeFilterValues() {
		if (this.Column.IsFiltered && this.Column.Filter != null) {

			this.Operation = this.Column.Filter.Operator;

			if (this.Operation?.NumberOfValues > 0) {
				this.FilterValue = FormatValue(this.Column.Filter.Value);
			}

			if (this.Operation?.NumberOfValues > 1) {
				this.FilterValue2 = FormatValue(this.Column.Filter.Value2);
			}

		} else {
			this.FilterValue = FormatValue(this.Column.ColumnMinValue());
			this.FilterValue2 = FormatValue(this.Column.ColumnMaxValue());
		}
	}

	private static string FormatValue(object? value) {
		return value?.ToString() ?? "";
	}

	internal override IFilterStatement? GetFilter() {
		if (this.Operation == null || this.Column?.Type == null) {
			return null;
		}

		try {
			return CreateFilterStatement(
				this.Column.Type,
				this.Column.Field,
				this.Operation,
				this.FilterValue,
				this.FilterValue2);
		} catch (Exception) {
			// Log error or handle invalid input gracefully
			return null;
		}
	}

	private static IFilterStatement CreateFilterStatement(
		Type columnType,
		string field,
		IOperator operation,
		string value1,
		string value2) {

		var targetType = columnType.GetNonNullableType();
		var isNullable = Nullable.GetUnderlyingType(columnType) != null;

		return Type.GetTypeCode(targetType) switch {
			TypeCode.Byte => CreateTypedFilter<byte>(field, operation, value1, value2, isNullable),
			TypeCode.SByte => CreateTypedFilter<sbyte>(field, operation, value1, value2, isNullable),
			TypeCode.UInt16 => CreateTypedFilter<ushort>(field, operation, value1, value2, isNullable),
			TypeCode.UInt32 => CreateTypedFilter<uint>(field, operation, value1, value2, isNullable),
			TypeCode.UInt64 => CreateTypedFilter<ulong>(field, operation, value1, value2, isNullable),
			TypeCode.Int16 => CreateTypedFilter<short>(field, operation, value1, value2, isNullable),
			TypeCode.Int32 => CreateTypedFilter<int>(field, operation, value1, value2, isNullable),
			TypeCode.Int64 => CreateTypedFilter<long>(field, operation, value1, value2, isNullable),
			TypeCode.Decimal => CreateTypedFilter<decimal>(field, operation, value1, value2, isNullable),
			TypeCode.Double => CreateTypedFilter<double>(field, operation, value1, value2, isNullable),
			TypeCode.Single => CreateTypedFilter<float>(field, operation, value1, value2, isNullable),
			_ => throw new InvalidOperationException($"Unsupported numeric type: {targetType}")
		};
	}

	private static IFilterStatement CreateTypedFilter<T>(
		string field,
		IOperator operation,
		string value1,
		string value2,
		bool isNullable) where T : struct {

		var parsedValue1 = ParseValue<T>(value1, operation.NumberOfValues > 0);
		var parsedValue2 = ParseValue<T>(value2, operation.NumberOfValues > 1);

		if (isNullable) {
			return new FilterStatement<T?>(field, operation, parsedValue1, parsedValue2, ExpressionBuilder.Common.Connector.And);
		} else {
			return new FilterStatement<T>(field, operation,
				parsedValue1 ?? default,
				parsedValue2 ?? default,
				ExpressionBuilder.Common.Connector.And);
		}
	}

	private static T? ParseValue<T>(string value, bool isRequired) where T : struct {
		if (!isRequired || string.IsNullOrWhiteSpace(value)) {
			return null;
		}

		try {
			return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
		} catch {
			return default(T);
		}
	}

}