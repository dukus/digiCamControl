using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace CameraControl.Controls
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

        public static IHighlightingDefinition AddCustomHighlighting(this TextEditor textEditor, Stream xshdStream, string[] extensions)
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
