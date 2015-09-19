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
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

#endregion

namespace CameraControl.Core.Wpf
{
    // original code from : http://blog.thomaslebrun.net/category/avalonedit/#.Uc6TuztM98E
    public static class AvalonEditExtensions
    {
        public static IHighlightingDefinition AddCustomHighlighting(this TextEditor textEditor, Stream xshdStream)
        {
            if (xshdStream == null)
                throw new InvalidOperationException("Could not find embedded resource");

            IHighlightingDefinition customHighlighting;

            // Load our custom highlighting definition
            using (XmlReader reader = new XmlTextReader(xshdStream))
            {
                customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }

            // And register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", null, customHighlighting);

            return customHighlighting;
        }

        public static IHighlightingDefinition AddCustomHighlighting(this TextEditor textEditor, Stream xshdStream,
                                                                    string[] extensions)
        {
            if (xshdStream == null)
                throw new InvalidOperationException("Could not find embedded resource");

            IHighlightingDefinition customHighlighting;

            // Load our custom highlighting definition
            using (XmlReader reader = new XmlTextReader(xshdStream))
            {
                customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }

            // And register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", extensions, customHighlighting);

            return customHighlighting;
        }

        public static string GetWordUnderMouse(this TextDocument document, TextViewPosition position)
        {
            string wordHovered = string.Empty;

            var line = position.Line;
            var column = position.Column;

            var offset = document.GetOffset(line, column);
            if (offset >= document.TextLength)
                offset--;

            var textAtOffset = document.GetText(offset, 1);

            // Get text backward of the mouse position, until the first space
            while (!string.IsNullOrWhiteSpace(textAtOffset))
            {
                wordHovered = textAtOffset + wordHovered;

                offset--;

                if (offset < 0)
                    break;

                textAtOffset = document.GetText(offset, 1);
            }

            // Get text forward the mouse position, until the first space
            offset = document.GetOffset(line, column);
            if (offset < document.TextLength - 1)
            {
                offset++;

                textAtOffset = document.GetText(offset, 1);

                while (!string.IsNullOrWhiteSpace(textAtOffset))
                {
                    wordHovered = wordHovered + textAtOffset;

                    offset++;

                    if (offset >= document.TextLength)
                        break;

                    textAtOffset = document.GetText(offset, 1);
                }
            }

            return wordHovered;
        }

        public static string GetWordBeforeDot(this TextEditor textEditor)
        {
            var wordBeforeDot = string.Empty;

            var caretPosition = textEditor.CaretOffset - 2;

            var lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(caretPosition));

            string text = textEditor.Document.GetText(lineOffset, 1);

            // Get text backward of the mouse position, until the first space
            while (!string.IsNullOrWhiteSpace(text) && text.CompareTo(".") > 0)
            {
                wordBeforeDot = text + wordBeforeDot;

                if (caretPosition == 0)
                    break;

                lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(--caretPosition));

                text = textEditor.Document.GetText(lineOffset, 1);
            }

            return wordBeforeDot;
        }

        public static string GetLine(this TextEditor textEditor)
        {
            var wordBeforeDot = string.Empty;

            var caretPosition = textEditor.CaretOffset - 2;

            var lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(caretPosition));

            string text = textEditor.Document.GetText(lineOffset, 1);

            // Get text backward of the mouse position, until the first space
            while (text.CompareTo("<") != 0)
            {
                wordBeforeDot = text + wordBeforeDot;

                if (caretPosition == 0)
                    break;

                lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(--caretPosition));

                text = textEditor.Document.GetText(lineOffset, 1);
            }

            return wordBeforeDot;
        }
    }
}