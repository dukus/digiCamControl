using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace CameraControl.Core.Translation
{
  /// <summary>
  /// The Translate Markup extension returns a binding to a TranslationData
  /// that provides a translated resource of the specified key
  /// </summary>
  [MarkupExtensionReturnType(typeof(string))]
  public class TranslateExtension : DynamicResourceExtension
  {
    #region Private Members

    private string _key;
    private string _defaultValue;

    #endregion

    #region Construction

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateExtension"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    public TranslateExtension(string key)
    {
      _key = key;
    }

    #endregion

    [ConstructorArgument("key")]
    public string Key
    {
      get { return _key; }
      set { _key = value; }
    }

    public string DefaultValue
    {
      get { return _defaultValue; }
      set { _defaultValue = value; }
    }

    /// <summary>
    /// See <see cref="MarkupExtension.ProvideValue" />
    /// </summary>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (!TranslationManager.Strings.ContainsKey(Key))
        return DefaultValue;
      return TranslationManager.Strings[Key];
    }
  }
}
