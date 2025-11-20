namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// PopperJS Options.
/// </summary>
public record PopperJSOptions {

	/// <summary>
	/// Gets the source element that triggers the popper element.
	/// </summary>
	public ElementReference TriggerElement { get; init; }

	/// <summary>
	/// Gets the popper element.
	/// </summary>
	public ElementReference PopperElement { get; init; }

	/// <summary>
	/// Gets the popperJs placement
	/// </summary>
	public Placement Placement { get; init; } = Placement.Bottom;

	/// <summary>
	/// Gets the array of popperJs modifiers
	/// </summary>
	public List<PopperModifer> Modifiers { get; init; } = [];

	/// <summary>
	/// Default is to display dynamically, unless this
	/// is set to true.
	/// </summary>
	public bool DisplayStatic { get; init; }

}

public record PopperModifer {

	public PopperModifer(string name, bool enabled) {
		this.Name = name;
		this.Enabled = enabled;
	}
	public PopperModifer(string name, object options) {
		this.Name = name;
		this.Options = options;
	}
	public PopperModifer(string name, bool enabled, object options) {
		this.Name = name;
		this.Enabled = enabled;
		this.Options = options;
	}

	/// <summary>
	/// Gets the name of the modifier.
	/// </summary>
	[JsonPropertyName("name")]
	public string Name { get; init; }

	/// <summary>
	/// Gets if the modifier is enabled. Default: <see langword="true"/>
	/// </summary>
	[JsonPropertyName("enabled")]
	public bool Enabled { get; init; } = true;

	/// <summary>
	/// Gets the options for the modifier.
	/// </summary>
	[JsonPropertyName("options")]
	public object? Options { get; init; }

}