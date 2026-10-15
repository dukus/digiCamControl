using System;

namespace CameraControl.Core.Translation
{
	public class TranslationExtensionWithProjectSuffix : TranslateExtension
	{
		public TranslationExtensionWithProjectSuffix (string key)
			: base(key)
		{
		}

		public override object ProvideValue (IServiceProvider serviceProvider)
		{
			if (string.IsNullOrEmpty(Key))
				return DefaultValue;
			return base.ProvideValue(serviceProvider) + " ●";
		}
	}
}
