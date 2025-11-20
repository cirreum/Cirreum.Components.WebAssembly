namespace Cirreum.Extensions;

using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0079 // Remove unnecessary suppression
internal static class RenderTreeBuilderExtensions {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SuppressMessage("Blazor", "ASP0006:Do not use non-literal sequence numbers")]
	public static void AddAttributeIfNotNullOrEmpty(
		this RenderTreeBuilder builder,
		int sequence,
		string name,
		string? value) {
		if (value.HasValue()) {
			builder.AddAttribute(sequence, name, value);
		}
	}
}
#pragma warning restore IDE0079 // Remove unnecessary suppression