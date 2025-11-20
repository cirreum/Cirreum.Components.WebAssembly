namespace Cirreum.Components;

internal interface IDialogServiceInternal : IDialogService {

	void Close(DialogReference dialog);

	void Close(DialogReference dialog, DialogResult result);

	event Action<DialogReference>? OnDialogInstanceAdded;

	event Action<DialogReference, DialogResult>? OnDialogCloseRequested;

}