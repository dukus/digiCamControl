using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using CameraControl.Core;

namespace CameraControl.Classes
{
    [MarkupExtensionReturnType(typeof (string))]
    public class CommandShortcutExtensioncs: DynamicResourceExtension
    {
                #region Private Members

        #endregion
                public CommandShortcutExtensioncs(string key)
        {
            Key = key;
        }

        
        [ConstructorArgument("key")]
        public string Key { get; set; }

        public string DefaultValue { get; set; }

        /// <summary>
        /// See <see cref="MarkupExtension.ProvideValue" />
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (ServiceProvider.Settings == null)
                return "";
            foreach (var act in ServiceProvider.Settings.Actions)
            {
                if (act.Name == Key)
                    return (act.Ctrl ? "CTRL + " : "") + (act.Alt ? "ALT + " : "") + (act.Key == "None" ? "" : act.Key);
            }

            return "";
        }
    }
}
