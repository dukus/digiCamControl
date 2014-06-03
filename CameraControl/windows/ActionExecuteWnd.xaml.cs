using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.windows
{
  /// <summary>
  /// Interaction logic for ActionExecuteWnd.xaml
  /// </summary>
  public partial class ActionExecuteWnd 
  {
    public IMenuAction MenuAction { get; set; }

    public ActionExecuteWnd(IMenuAction action)
    {
      MenuAction = action;
      MenuAction.ProgressChanged += MenuAction_ProgressChanged;
      MenuAction.ActionDone += MenuAction_ActionDone;
      InitializeComponent();
      Title = "Execute action : " + MenuAction.Title;
      ServiceProvider.Settings.ApplyTheme(this);
    }

    void MenuAction_ActionDone(object sender, EventArgs e)
    {
      Dispatcher.Invoke(new Action(delegate
                                     {
                                       progressBar1.IsIndeterminate = false;
                                       button1.Content = "Start";
                                       listBox1.Items.Add("Action done");
                                     }));
    }

    private void MenuAction_ProgressChanged(object sender, EventArgs e)
    {
      ActionEventArgs args = e as ActionEventArgs;
      if (args != null)
      {
        Dispatcher.Invoke(new Action(delegate
                                       {
                                         listBox1.Items.Add(args.Message);
                                         listBox1.ScrollIntoView(listBox1.Items[listBox1.Items.Count - 1]);
                                       }));
      }
    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {
      if(!MenuAction.IsBusy)
      {
        progressBar1.IsIndeterminate = true;
        
        listBox1.Items.Clear();
        Thread thread = new Thread(() => MenuAction.Run(null));
        thread.Start(); 
        button1.Content = "Stop";
      }
      else
      {
        MenuAction.Stop();
      }
    }

  }
}
