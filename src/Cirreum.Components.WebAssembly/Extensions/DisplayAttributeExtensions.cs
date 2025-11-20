namespace Cirreum.Extensions;

using System.ComponentModel.DataAnnotations;

public static class DisplayAttributeExtensions {

	/// <summary>
	/// Gets the <see cref="DisplayAttribute.Name"/> value if the attribute exists.
	/// </summary>
	/// <param name="value">The <see cref="Enum"/> instance value to evaluate.</param>
	/// <returns>The <paramref name="value"/> as a string or the name if found.</returns>
	public static string ToName<TEnum>(this TEnum value) where TEnum : struct, Enum {

		if (!typeof(TEnum).IsEnum) {
			return $"{value}";
		}

		var name = value.ToString();

		var fieldInfo = value.GetType().GetField(value.ToString() ?? "");
		if (fieldInfo is not null) {
			var attributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
			if (attributes.Length > 0) {
				name = ((DisplayAttribute)attributes[0]).Name;
			}
		}

		return name ?? $"{value}";

	}

	/// <summary>
	/// Gets the <see cref="DisplayAttribute.ShortName"/> value if the attribute exists.
	/// </summary>
	/// <param name="value">The <see cref="Enum"/> instance value to evaluate.</param>
	/// <returns>The <paramref name="value"/> as a string or the short name if found.</returns>
	public static string ToShortName<TEnum>(this TEnum value) where TEnum : struct, Enum {

		if (!typeof(TEnum).IsEnum) {
			return $"{value}";
		}

		var name = value.ToString();

		var fieldInfo = value.GetType().GetField(value.ToString());
		if (fieldInfo is not null) {
			var attributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
			if (attributes.Length > 0) {
				name = ((DisplayAttribute)attributes[0]).ShortName;
			}
		}

		return name ?? $"{value}";

	}

	/// <summary>
	/// Gets the <see cref="DisplayAttribute.Description"/> value if the attribute exists.
	/// </summary>
	/// <param name="value">The <see cref="Enum"/> instance value to evaluate.</param>
	/// <returns>The the description if found; otherwise an empty string.</returns>
	public static string ToDescription<TEnum>(this TEnum value) where TEnum : struct, Enum {

		if (!typeof(TEnum).IsEnum) {
			return string.Empty;
		}

		var name = "";

		var fieldInfo = value.GetType().GetField(value.ToString());
		if (fieldInfo is not null) {
			var attributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
			if (attributes.Length > 0) {
				name = ((DisplayAttribute)attributes[0]).Description;
			}
		}

		return name ?? string.Empty;

	}

}