namespace Cirreum.Components;

/// <summary>
/// Contains the result of a Dialog.
/// </summary>
public class DialogResult {

	/// <summary>
	/// The optional data for the result.
	/// </summary>
	public object? Data { get; }
	/// <summary>
	/// The optional <see cref="Type"/> of the <see cref="Data"/> result.
	/// </summary>
	public Type? DataType { get; }
	/// <summary>
	/// Indicates of the dialog was closed due to be cancelled.
	/// </summary>
	public bool Cancelled { get; }
	/// <summary>
	/// Indicates of the dialog was closed successfully.
	/// </summary>
	public bool Confirmed => !this.Cancelled;

	/// <summary>
	/// Private Constructor.
	/// </summary>
	/// <param name="dataResult"></param>
	/// <param name="dataResultType"></param>
	/// <param name="cancelled"></param>
	private DialogResult(object? dataResult, Type? dataResultType, bool cancelled) {
		this.Data = dataResult;
		this.DataType = dataResultType;
		this.Cancelled = cancelled;
	}

	/// <summary>
	/// A convenience method to return a confirmed dialog result with data.
	/// </summary>
	/// <typeparam name="TResultType">The <see cref="Type"/> of the result data.</typeparam>
	/// <param name="resultData">The data result object.</param>
	/// <returns>A new <see cref="DialogResult"/> configured as <see cref="Confirmed"/>, with the associated <paramref name="resultData"/> object.</returns>
	public static DialogResult Ok<TResultType>(TResultType resultData)
		=> Ok(resultData, typeof(TResultType));

	/// <summary>
	/// A convenience method to return a confirmed dialog result with data.
	/// </summary>
	/// <param name="resultData">The data result object.</param>
	/// <param name="resultDataType">The <see cref="Type"/> of the result data.</param>
	/// <returns>A new <see cref="DialogResult"/> configured as <see cref="Confirmed"/>, with the associated <paramref name="resultData"/> object.</returns>
	public static DialogResult Ok(object? resultData, Type? resultDataType)
		=> new(resultData, resultDataType, false);

	/// <summary>
	/// A convenience method to return a confirmed dialog result.
	/// </summary>
	/// <returns>A new <see cref="DialogResult"/> configured as <see cref="Confirmed"/>.</returns>
	public static DialogResult Ok()
		=> new(null, null, false);

	/// <summary>
	/// A convenience method to return a cancelled dialog result.
	/// </summary>
	/// <returns>A new <see cref="DialogResult"/> configured as <see cref="Cancelled"/>.</returns>
	public static DialogResult Cancel()
		=> new(null, null, true);

	/// <summary>
	/// A convenience method to return a cancelled dialog result with data.
	/// </summary>
	/// <typeparam name="TResultType">The <see cref="Type"/> of the result data.</typeparam>
	/// <param name="resultData">The data result object.</param>
	/// <returns>A new <see cref="DialogResult"/> configured as <see cref="Cancelled"/>, with the associated <paramref name="resultData"/> object.</returns>
	public static DialogResult Cancel<TResultType>(TResultType resultData)
		=> new(resultData, typeof(TResultType), true);

}