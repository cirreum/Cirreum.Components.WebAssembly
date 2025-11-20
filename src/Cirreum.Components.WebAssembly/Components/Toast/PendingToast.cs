namespace Cirreum.Components;

internal struct PendingToast {
	public string Uri { get; set; }
	public ToastInstance Toast { get; set; }
}