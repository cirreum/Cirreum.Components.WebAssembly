namespace Cirreum.Components;

using System.Collections;

public class ComponentParameters : IComponentParameters {

	private readonly Dictionary<string, object> Parameters;

	public ComponentParameters() {
		this.Parameters = [];
	}

	/// <inheritdoc/>
	public object this[string parameterName] {
		get => this.Get<object>(parameterName);
		set => this.Parameters[parameterName] = value;
	}

	/// <inheritdoc/>
	public void Add(string parameterName, object value)
		=> this.Parameters[parameterName] = value;

	/// <inheritdoc/>
	public T Get<T>(string parameterName) {
		if (this.Parameters.TryGetValue(parameterName, out var value)) {
			return (T)value;
		}

		throw new KeyNotFoundException($"{parameterName} does not exist.");
	}

	/// <inheritdoc/>
	public T? TryGet<T>(string parameterName) {
		if (this.Parameters.TryGetValue(parameterName, out var value)) {
			return (T)value;
		}

		return default;
	}

	/// <inheritdoc/>
	public Dictionary<string, object> GetDictionary(bool insertProperties = false) {
		if (insertProperties) {
			foreach (var property in this.GetType().GetProperties()) {
				this.Parameters[property.Name] = property.GetValue(this)!;
			}
		}
		return this.Parameters;
	}

	/// <inheritdoc/>
	public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		=> this.Parameters.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator()
		=> this.Parameters.GetEnumerator();

	/// <inheritdoc/>
	public override int GetHashCode() {
		unchecked {
			// Overflow is fine, just wrap
			var hash = 17;
			foreach (var kvp in this.Parameters) {
				hash = hash * 23 + (kvp.Key?.GetHashCode() ?? 0);
				hash = hash * 23 + (kvp.Value?.GetHashCode() ?? 0);
			}
			return hash;
		}
	}

	/// <inheritdoc/>
	public override bool Equals(object? obj) {
		if (obj is ComponentParameters other) {
			return this.Parameters.Count == other.Parameters.Count &&
				   !this.Parameters.Except(other.Parameters).Any();
		}
		return false;
	}

}