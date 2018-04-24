#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

#endregion

namespace CameraControl.Core.Translation
{
    /// <summary>
    /// The Translate Markup extension returns a binding to a TranslationData
    /// that provides a translated resource of the specified key
    /// </summary>
    [MarkupExtensionReturnType(typeof (string))]
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
            return TranslationManager.Strings[Key].Replace("_","");
        }
    }
}