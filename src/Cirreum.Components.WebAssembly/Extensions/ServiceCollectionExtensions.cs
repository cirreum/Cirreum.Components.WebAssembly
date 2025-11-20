namespace Cirreum.Extensions;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions {

	/// <summary>
	/// Adds all the optional services for Components. See remarks.
	/// </summary>
	/// <param name="services"></param>
	/// <remarks>
	/// <see cref="IMenuService"/>, <see cref="IToastService"/>, 
	/// and <see cref="IDialogService"/>
	/// </remarks>
	public static IServiceCollection AddCoreComponents(
		this IServiceCollection services) {

		services
			.AddToastService()
			.AddDialogService()
			.AddMenuService();

		return services;

	}

	static IServiceCollection AddToastService(this IServiceCollection services) {
		services.AddScoped<IToastService, ToastService>();
		return services;
	}

	static IServiceCollection AddDialogService(this IServiceCollection services) {

		services.AddScoped<IDialogServiceInternal, DialogService>();
		services.AddScoped<IDialogService>(sp => sp.GetRequiredService<IDialogServiceInternal>());

		return services;

	}

	static IServiceCollection AddMenuService(this IServiceCollection services) {
		services.AddScoped<IMenuService, MenuService>();
		return services;
	}

}