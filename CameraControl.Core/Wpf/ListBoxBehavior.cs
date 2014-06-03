using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CameraControl.Core.Wpf
{
  public class ListBoxBehavior
  {
    static Dictionary<ListBox, Capture> Associations =
        new Dictionary<ListBox, Capture>();

    public static bool GetScrollOnNewItem(DependencyObject obj)
    {
      return (bool)obj.GetValue(ScrollOnNewItemProperty);
    }

    public static void SetScrollOnNewItem(DependencyObject obj, bool value)
    {
      obj.SetValue(ScrollOnNewItemProperty, value);
    }

    public static readonly DependencyProperty ScrollOnNewItemProperty =
        DependencyProperty.RegisterAttached(
            "ScrollOnNewItem",
            typeof(bool),
            typeof(ListBoxBehavior),
            new UIPropertyMetadata(false, OnScrollOnNewItemChanged));

    public static void OnScrollOnNewItemChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
      var listBox = d as ListBox;
      if (listBox == null) return;
      bool oldValue = (bool)e.OldValue, newValue = (bool)e.NewValue;
      if (newValue == oldValue) return;
      if (newValue)
      {
        listBox.Loaded += new RoutedEventHandler(ListBox_Loaded);
        listBox.Unloaded += new RoutedEventHandler(ListBox_Unloaded);
      }
      else
      {
        listBox.Loaded -= ListBox_Loaded;
        listBox.Unloaded -= ListBox_Unloaded;
        if (Associations.ContainsKey(listBox))
          Associations[listBox].Dispose();
      }
    }

    static void ListBox_Unloaded(object sender, RoutedEventArgs e)
    {
      var listBox = (ListBox)sender;
      if (Associations.ContainsKey(listBox))
        Associations[listBox].Dispose();
      listBox.Unloaded -= ListBox_Unloaded;
    }

    static void ListBox_Loaded(object sender, RoutedEventArgs e)
    {
      var listBox = (ListBox)sender;
      var incc = listBox.Items as INotifyCollectionChanged;
      if (incc == null) return;
      listBox.Loaded -= ListBox_Loaded;
      Associations[listBox] = new Capture(listBox);
    }

    class Capture : IDisposable
    {
      public ListBox listBox { get; set; }
      public INotifyCollectionChanged incc { get; set; }

      public Capture(ListBox listBox)
      {
        this.listBox = listBox;
        incc = listBox.ItemsSource as INotifyCollectionChanged;
        if (incc != null)
        {
          incc.CollectionChanged +=
              new NotifyCollectionChangedEventHandler(incc_CollectionChanged);
        }
      }

      void incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
          listBox.ScrollIntoView(e.NewItems[0]);
          listBox.SelectedItem = e.NewItems[0];
        }
      }

      public void Dispose()
      {
        if (incc != null)
          incc.CollectionChanged -= incc_CollectionChanged;
      }
    }
  }
}
