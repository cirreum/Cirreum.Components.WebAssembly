namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Represents a type that can be either a string or a RenderFragment.
/// This struct allows for flexible handling of content that can be 
/// either simple text or more complex rendered content.
/// </summary>
public readonly struct StringOrRenderFragment {

	private readonly object _value;

	/// <summary>
	/// Initializes a new instance of the <see cref="StringOrRenderFragment"/> struct.
	/// </summary>
	/// <param name="value">The value to store, either a string or a RenderFragment.</param>
	private StringOrRenderFragment(object value) {
		_value = value;
	}

	/// <summary>
	/// Implicitly converts a string to a <see cref="StringOrRenderFragment"/>.
	/// </summary>
	/// <param name="title">The string to convert.</param>
	public static implicit operator StringOrRenderFragment(string title) => new StringOrRenderFragment(title);

	/// <summary>
	/// Implicitly converts a RenderFragment to a <see cref="StringOrRenderFragment"/>.
	/// </summary>
	/// <param name="fragment">The RenderFragment to convert.</param>
	public static implicit operator StringOrRenderFragment(RenderFragment fragment) => new StringOrRenderFragment(fragment);

	/// <summary>
	/// Converts the stored value to a RenderFragment.
	/// </summary>
	/// <returns>A RenderFragment representing the stored value.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the stored value is neither a string nor a RenderFragment.</exception>
	public RenderFragment ToRenderFragment() {
		return _value switch {
			string s => builder => builder.AddContent(0, s),
			RenderFragment rf => rf,
			_ => throw new InvalidOperationException("Invalid type")
		};
	}

	/// <summary>
	/// Gets the value as a string, if it is not a <see cref="RenderFragment"/>; otherwise an empty string.
	/// </summary>
	/// <returns></returns>
	public override string ToString() {
		return this.IsString ? (string)_value : "";
	}

	/// <summary>
	/// Does this instance represent a <see cref="string"/>.
	/// </summary>
	public bool IsString => _value is string;

	/// <summary>
	/// Does this instance represent a <see cref="RenderFragment"/>.
	/// </summary>
	public bool IsRenderFragment => _value is RenderFragment;

	/// <summary>
	/// Gets the underlying value.
	/// </summary>
	/// <returns><see cref="Object"/> of the underlying valjue.</returns>
	public object GetOriginalValue() => _value;

	/// <summary>
	/// Attempts to get the value as a <see cref="string"/>.
	/// </summary>
	/// <returns>The value as a <see cref="string"/>.</returns>
	/// <remarks>
	/// <para>
	/// Throws an <see cref="InvalidOperationException"/> if the underlying
	/// type of the value is not already a <see cref="string"/>.
	/// </para>
	/// </remarks>
	/// <exception cref="InvalidOperationException"></exception>
	public string AsString() => _value as string ?? throw new InvalidOperationException("Value is not a string");

	/// <summary>
	/// Attempts to get the value as a <see cref="RenderFragment"/>.
	/// </summary>
	/// <returns>The value as a <see cref="RenderFragment"/>.</returns>
	/// <remarks>
	/// <para>
	/// Throws an <see cref="InvalidOperationException"/> if the underlying
	/// type of the value is not already a <see cref="RenderFragment"/>.
	/// </para>
	/// </remarks>
	/// <exception cref="InvalidOperationException"></exception>
	public RenderFragment AsRenderFragment() => _value as RenderFragment ?? throw new InvalidOperationException("Value is not a RenderFragment");


}