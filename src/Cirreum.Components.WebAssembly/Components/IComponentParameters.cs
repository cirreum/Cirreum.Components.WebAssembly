namespace Cirreum.Components;

/// <summary>
/// Defines a custom collection to store Parameter's and their associated values.
/// </summary>
public interface IComponentParameters : IEnumerable<KeyValuePair<string, object>> {

	/// <summary>
	/// Named index access to get a parameter value.
	/// </summary>
	/// <param name="parameterName">The parameter name.</param>
	/// <returns>THe parameter value.</returns>
	object this[string parameterName] { get; set; }

	/// <summary>
	/// Adds or updates, the value within the parameters.
	/// </summary>
	/// <param name="parameterName">The parameter name.</param>
	/// <param name="value">The parameter value.</param>
	void Add(string parameterName, object value);

	/// <summary>
	/// Get an existing parameter value.
	/// </summary>
	/// <typeparam name="T">The Type of the value.</typeparam>
	/// <param name="parameterName">The name of the value to get.</param>
	/// <returns>The found value if found; otherwise throws an <see cref="InvalidOperationException"/>.</returns>
	/// <exception cref="InvalidOperationException"><paramref name="parameterName"/> does not exist.</exception>
	T Get<T>(string parameterName);

	/// <summary>
	/// Tries to get a parameter value; if it exists.
	/// </summary>
	/// <typeparam name="T">The Type of the value.</typeparam>
	/// <param name="parameterName">The name of the value to get.</param>
	/// <returns>The found value; otherwise the <typeparamref name="T"/> default which could be null.</returns>
	/// <exception cref="InvalidOperationException"><paramref name="parameterName"/> does not exist.</exception>
	T? TryGet<T>(string parameterName);

	/// <summary>
	/// Gets a dictionary of the existing parameters, and optionally inserting any instance level properties.
	/// </summary>
	/// <param name="insertProperties"><see langword="true"/> to insert properties this instance.</param>
	/// <returns>A dictionary containing the parameters, optionally including class level properties.</returns>
	Dictionary<string, object> GetDictionary(bool insertProperties = false);

}