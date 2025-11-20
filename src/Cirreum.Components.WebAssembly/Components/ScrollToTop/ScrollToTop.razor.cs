namespace Cirreum.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

public partial class ScrollToTop {

	private IJSObjectReference? module;
	private DotNetObjectReference<ScrollToTop>? dotNetRef;
	private bool isVisible = false;
	private readonly string instanceId = IdGenerator.Next;

	[Parameter] public string CssClass { get; set; } = "scroll-to-top";

	[Parameter] public string Style { get; set; } = "";

	[Parameter] public int ShowAfterPixels { get; set; } = 300;

	[Parameter] public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Bootstrap color variant for the button.
	/// </summary>
	[Parameter] public ButtonColor Color { get; set; } = ButtonColor.Primary;

	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	protected override async Task OnAfterFirstRenderAsync() {
		if (this.IsDisposed) {
			return;
		}

		try {

			const string jsPath = "./_content/Cirreum.Blazor.Components/Components/ScrollToTop/ScrollToTop.razor.js";
			this.module = await this.JSApp.InvokeAsync<IJSInProcessObjectReference>("import", this.DisposalToken, jsPath);

			this.dotNetRef = DotNetObjectReference.Create(this);
			await this.module.InvokeVoidAsync("initialize", this.instanceId, this.dotNetRef, this.ShowAfterPixels);

		} catch (OperationCanceledException) when (this.DisposalToken.IsCancellationRequested) {
			// Expected during disposal
		} catch (JSDisconnectedException) {
			// Browser disconnected during module load
		}
	}

	[JSInvokable]
	public void UpdateVisibility(bool visible) {
		if (this.isVisible != visible) {
			this.isVisible = visible;
			this.StateHasChanged();
		}
	}

	private string ComputedCss => CssBuilder
		.Default(this.CssClass)
			.AddClass("btn", when: this.Color != ButtonColor.None)
			.AddClass(this.Color.ToName(), when: this.Color != ButtonColor.None)
			.AddClass("visible", when: this.isVisible)
		.Build();

	private string ComputedStyle => StyleBuilder
		.Empty()
			.AddStyleIfNotEmpty(this.Style)
		.Build();

	private async Task ScrollTop(MouseEventArgs args) {
		if (this.module != null) {
			await this.module.InvokeVoidAsync("scrollToTop");
		}
	}

	protected override async ValueTask DisposeAsyncCore() {
		if (this.module != null) {
			await this.module.InvokeVoidAsync("dispose", this.instanceId);
			await this.module.DisposeAsync();
		}
		this.dotNetRef?.Dispose();
	}

}
