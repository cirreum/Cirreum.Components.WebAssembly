namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;

internal sealed class DialogReference : IDialogReference {

	private readonly TaskCompletionSource<DialogResult> _resultCompletion = new();
	private readonly IDialogServiceInternal _dialogService;

	internal string Id { get; }
	internal RenderFragment DialogInstanceFragment { get; }
	internal Dialog? Instance { get; set; }

	internal DialogReference(string dialogInstanceId, RenderFragment dialogInstanceFragment, IDialogServiceInternal dialogService) {
		this.Id = dialogInstanceId;
		this.DialogInstanceFragment = dialogInstanceFragment;
		this._dialogService = dialogService;
	}

	public Task<DialogResult> Result => this._resultCompletion.Task;

	public void Close() => this._dialogService.Close(this);

	public void Close(DialogResult result) => this._dialogService.Close(this, result);

	internal void SetResultsAndCompleteTask(DialogResult result) => this._resultCompletion.TrySetResult(result);

}