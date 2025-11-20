namespace Cirreum.Components;
public interface IClickDetectorService {
    void Register(string elementId, Func<string, Task>? outsideClick, Func<string, Task>? insideClick = null, string[]? ignoreOutside = null);
    void Unregister(string elementId);
}