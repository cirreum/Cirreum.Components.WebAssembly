namespace Cirreum.Components;

using Cirreum.ExpressionBuilder.Common;
using Cirreum.ExpressionBuilder.Helpers;
using Cirreum.ExpressionBuilder.Interfaces;
using Cirreum.ExpressionBuilder.Resources;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

[CascadingTypeParameter(nameof(TData))]
public abstract class DataGridColumnFilterBase<TData> : ComponentBase {

	private readonly List<OperationInfo> operations = [];

	protected ElementReference initialFocusElement;
	private bool initialFocusElementSet;

	/// <summary>
	/// The associated <see cref="DataGridColumn{TData}"/> this filer is for.
	/// </summary>
	[CascadingParameter]
	public DataGridColumn<TData> Column { get; set; } = default!;

	/// <summary>
	/// Gets or sets a collection of additional attributes that will be applied to the created element.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

	/// <summary>
	/// Gets the appropriate collection of comparison operations
	/// available for the <see cref="Type"/> of data
	/// the <see cref="DataGridColumn{TData}"/> is displaying.
	/// </summary>
	public List<OperationInfo> Options {
		get {
			return this.operations;
		}
	}

	/// <summary>
	/// Gets or sets the user selected comparison operation.
	/// </summary>
	[AllowNull]
	public IOperator Operation { get; set; }

	internal abstract IFilterStatement? GetFilter();

	/// <summary>
	/// Gets the <see cref="Connector"/> for this filter. Default: <see cref="Connector.And"/>
	/// </summary>
	public virtual Connector Connector {
		get {
			return Connector.And;
		}
	}

	/// <summary>
	/// Handles the onchange event for the selected comparison operation.
	/// </summary>
	/// <param name="args">The <see cref="ChangeEventArgs"/>.</param>
	public virtual void OnOperationChanged(ChangeEventArgs args) {
		this.Operation = OperationHelper.GetOperationByName($"{args.Value}");
	}

	protected override void OnInitialized() {
		if (this.Column.Type is not null) {
			var ops = OperationHelper.SupportedOperators(this.Column.Type)
				   .Select(o => new OperationInfo(
					   Id: o.ToString(),
					   Name: o.GetDescription(OperationStrings.ResourceManager)
				   ))
				   .ToList();
			this.Operation = OperationHelper.GetOperationByName(ops.First().Id);
			this.operations.AddRange(ops);
		}
	}

	protected override async Task OnAfterRenderAsync(bool firstRender) {
		await base.OnAfterRenderAsync(firstRender);
		if (this.initialFocusElementSet is false
			&& this.Operation.NumberOfValues > 0
			&& this.initialFocusElement.Context is not null) {
			this.initialFocusElementSet = true;
			await this.initialFocusElement.FocusAsync(true);
		}
	}

}


/// <summary>
/// A filter value operation for an Option selector.
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
public record OperationInfo(string Id, string Name);