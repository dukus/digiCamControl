using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Schema;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Scripting;
using CameraControl.Core.Wpf;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Path = System.IO.Path;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    /// <summary>
    /// Interaction logic for ScriptTransformView.xaml
    /// </summary>
    public partial class ScriptTransformView : UserControl
    {
        private CompletionWindow completionWindow;
        private List<MyCompletionData> completionData = new List<MyCompletionData>();
        private Dictionary<string, List<string>> _params = new Dictionary<string, List<string>>();
        List<String> keywords = new List<String>();
        public ScriptTransformView()
        {
            InitializeComponent();
            Editor.TextArea.TextEntered += TextArea_TextEntered;
            Editor.TextArea.TextEntering += TextArea_TextEntering;
            LoadData();
        }

        private void LoadData()
        {
            try
            {

                
                XmlDocument doc = new XmlDocument();
                doc.Load(Path.Combine(Settings.ApplicationFolder, "Data", "msl", "MagickScript.xsd"));
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
                XmlNodeList nodes = doc.SelectNodes(@"//xs:group/xs:sequence/xs:choice/xs:element", manager);
                //XmlNodeList nodes = doc.SelectNodes(@"//xs:group[@name='actions']//@name", manager);
                Debug.Assert(nodes != null, "nodes != null");
                foreach (XmlNode node in nodes)
                {
                    String keyword;
                    if ((!String.IsNullOrEmpty(keyword = node.Attributes["name"].Value) && !keywords.Contains(keyword)))
                    {
                        keywords.Add(keyword);
                        if (node.ChildNodes.Count > 0)
                        {
                            foreach (XmlNode childNode in node.ChildNodes[0].ChildNodes)
                            {
                                if (childNode.Attributes.Count > 0)
                                {
                                    if (!_params.ContainsKey(keyword))
                                        _params.Add(keyword, new List<string>());
                                    _params[keyword].Add(childNode.Attributes["name"].Value);
                                }
                            }
                        }
                    }
                }
                foreach (String keyword in keywords)
                {
                    completionData.Add(new MyCompletionData(keyword, ""));
                }   
            }
            catch (Exception ex)
            {
                
            }
        }

        void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "<")
            {
                // open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(Editor.TextArea);
                ServiceProvider.Settings.ApplyTheme(completionWindow);
                completionWindow.CompletionList.ListBox.Foreground = new SolidColorBrush(Colors.Black);
                // provide AvalonEdit with the data:
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                foreach (var myCompletionData in completionData)
                {
                    data.Add(myCompletionData);
                }
                completionWindow.Show();
                completionWindow.Closed += delegate { completionWindow = null; };
            }
            if (e.Text == " ")
            {
                List<string> words = new List<string>();
                string line = Editor.GetLine();
                foreach (string s in keywords)
                {
                    if (line.Contains(s) && _params[s].Count>0)
                    {
                        words.AddRange(_params[s]);
                    }
                }

                if (words.Count > 0)
                {
                    completionWindow = new CompletionWindow(Editor.TextArea);
                    ServiceProvider.Settings.ApplyTheme(completionWindow);
                    completionWindow.CompletionList.ListBox.Foreground = new SolidColorBrush(Colors.Black);
                    // provide AvalonEdit with the data:
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                    foreach (var myCompletionData in words)
                    {
                        data.Add(new MyCompletionData(myCompletionData+"=\"", "",myCompletionData));
                    }
                    completionWindow.Show();
                    completionWindow.Closed += delegate { completionWindow = null; };
                }
            }
        }

        private void Editor_TextChanged(object sender, EventArgs e)
        {
            ((ScriptTransformViewModel) DataContext).Script = Editor.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((ScriptTransformViewModel)DataContext).Load();
            Editor.Text = ((ScriptTransformViewModel)DataContext).Script;
        }



    }
}
