namespace Cirreum.Components;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Represents a duration of time primarily used for timeouts and intervals.
/// All values are stored internally as milliseconds for consistency.
/// </summary>
/// <remarks>
/// This struct provides type-safe handling of timeout durations to prevent confusion between
/// seconds and milliseconds. It supports implicit conversion from strings (e.g. "25s" or "1000ms")
/// and integration with System.TimeSpan.
/// 
/// Internal storage is always in milliseconds regardless of how the duration was created.
/// </remarks>
[JsonConverter(typeof(JsonConverter))]
public readonly struct TimeoutDuration {
	private readonly int _milliseconds;

	private TimeoutDuration(int milliseconds) {
		this._milliseconds = milliseconds;
	}

	/// <summary>
	/// Creates a TimeoutDuration from a millisecond value.
	/// </summary>
	/// <param name="ms">The number of milliseconds.</param>
	/// <returns>A new TimeoutDuration instance.</returns>
	public static TimeoutDuration FromMilliseconds(int ms) => new(ms);

	/// <summary>
	/// Creates a TimeoutDuration from a seconds value.
	/// The value will be converted to milliseconds internally.
	/// </summary>
	/// <param name="seconds">The number of seconds.</param>
	/// <returns>A new TimeoutDuration instance.</returns>
	public static TimeoutDuration FromSeconds(int seconds) => new(seconds * 1000);

	/// <summary>
	/// Default timeout duration of 5 seconds.
	/// </summary>
	public static TimeoutDuration Default => FromSeconds(5);

	/// <summary>
	/// Short timeout duration of 3 seconds.
	/// </summary>
	public static TimeoutDuration Short => FromSeconds(3);

	/// <summary>
	/// Long timeout duration of 25 seconds.
	/// </summary>
	public static TimeoutDuration Long => FromSeconds(25);

	/// <summary>
	/// Parses a string representation of a duration.
	/// </summary>
	/// <param name="value">String in format "XXms" for milliseconds or "XXs" for seconds (e.g. "500ms" or "30s").</param>
	/// <returns>A new TimeoutDuration instance.</returns>
	/// <exception cref="FormatException">Thrown when the string format is invalid.</exception>
	/// <remarks>
	/// Supported formats:
	/// - "XXms" for milliseconds (e.g. "500ms")
	/// - "XXs" for seconds (e.g. "30s")
	/// Case insensitive and tolerant of whitespace.
	/// </remarks>
	public static TimeoutDuration Parse(string value) {
		value = value.Trim().ToLower();
		if (value.EndsWith("ms")) {
			return FromMilliseconds(int.Parse(value[..^2]));
		}

		if (value.EndsWith('s')) {
			return FromSeconds(int.Parse(value[..^1]));
		}

		throw new FormatException("Duration must end with ms or s");
	}

	/// <summary>
	/// Attempts to parse a string representation of a duration.
	/// </summary>
	/// <param name="value">String to parse.</param>
	/// <param name="result">When this method returns, contains the parsed TimeoutDuration if successful, or Default if parsing failed.</param>
	/// <returns>true if parsing was successful; otherwise, false.</returns>
	public static bool TryParse(string value, out TimeoutDuration result) {
		try {
			result = Parse(value);
			return true;
		} catch {
			result = Default;
			return false;
		}
	}

	/// <summary>
	/// Implicitly converts TimeoutDuration to milliseconds as an integer.
	/// </summary>
	public static implicit operator int(TimeoutDuration duration) => duration._milliseconds;

	/// <summary>
	/// Implicitly converts a string to TimeoutDuration using Parse.
	/// </summary>
	public static implicit operator TimeoutDuration(string duration) => Parse(duration);

	/// <summary>
	/// Implicitly converts TimeoutDuration to System.TimeSpan.
	/// </summary>
	public static implicit operator TimeSpan(TimeoutDuration duration)
		=> TimeSpan.FromMilliseconds(duration._milliseconds);

	/// <summary>
	/// Gets the duration in milliseconds.
	/// </summary>
	/// <returns>The total number of milliseconds.</returns>
	public int ToMilliseconds() => this._milliseconds;

	/// <summary>
	/// Gets the duration in seconds.
	/// </summary>
	/// <returns>The total number of seconds as a double for fractional second precision.</returns>
	public double ToSeconds() => this._milliseconds / 1000.0;

	/// <summary>
	/// Returns the string representation of this duration in seconds.
	/// </summary>
	/// <returns>A string in the format "XXs" (e.g. "25s").</returns>
	public override string ToString() => $"{this.ToSeconds()}s";

	/// <summary>
	/// JSON converter for TimeoutDuration that supports both string and number formats.
	/// </summary>
	public class JsonConverter : JsonConverter<TimeoutDuration> {
		/// <summary>
		/// Reads TimeoutDuration from JSON. Supports both string format ("25s") and raw millisecond numbers.
		/// </summary>
		public override TimeoutDuration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			return reader.TokenType switch {
				JsonTokenType.String => Parse(reader.GetString()!),
				JsonTokenType.Number => FromMilliseconds(reader.GetInt32()),
				_ => throw new JsonException($"Unexpected token type {reader.TokenType}"),
			};
		}

		/// <summary>
		/// Writes TimeoutDuration to JSON in string format (e.g. "25s").
		/// </summary>
		public override void Write(Utf8JsonWriter writer, TimeoutDuration value, JsonSerializerOptions options) {
			writer.WriteStringValue(value.ToString());
		}
	}

}