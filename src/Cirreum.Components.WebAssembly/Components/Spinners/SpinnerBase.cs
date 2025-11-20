namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

public class SpinnerBase : ComponentBase {

	[Parameter]
	public string? Color { get; set; }

	[Parameter]
	public bool Center { get; set; } = true;

	[Parameter]
	public string? Size { get; set; }

	[Parameter(CaptureUnmatchedValues = true)]
	public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; } = new Dictionary<string, object>();

}