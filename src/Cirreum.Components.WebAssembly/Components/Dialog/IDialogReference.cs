namespace Cirreum.Components;

/// <summary>
/// Defines a dialog reference object including an optional <see cref="DialogResult"/>.
/// </summary>
public interface IDialogReference {

	/// <summary>
	/// An awaitable Dialog Result.
	/// </summary>
	Task<DialogResult> Result { get; }

	/// <summary>
	/// Close the dialog with no result.
	/// </summary>
	void Close();

	/// <summary>
	/// Close the dialog with a result.
	/// </summary>
	/// <param name="result">The <see cref="DialogResult"/> of the dialog.</param>
	void Close(DialogResult result);

}