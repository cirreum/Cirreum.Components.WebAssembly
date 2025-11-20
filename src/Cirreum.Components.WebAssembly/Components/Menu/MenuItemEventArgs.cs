namespace Cirreum.Components;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <param name="Context"></param>
/// <param name="MenuItem"></param>
public record MenuItemEventArgs<TContext>(TContext Context, IMenuItem MenuItem);