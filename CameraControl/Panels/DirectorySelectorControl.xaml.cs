using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CameraControl.Panels
{
    /// <summary>
    /// Interaction logic for DirectorySelectorControl.xaml
    /// </summary>
    public partial class DirectorySelectorControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty SelectedPathProperty = DependencyProperty.Register(
            "SelectedPath", typeof (string), typeof (DirectorySelectorControl), new PropertyMetadata("",PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyPropertyChangedEventArgs.OldValue != dependencyPropertyChangedEventArgs.NewValue)
            {
                ExpandAndSelectItem(((DirectorySelectorControl)dependencyObject).trvStructure, (string)dependencyPropertyChangedEventArgs.NewValue);
            }
        }

        public string SelectedPath
        {
            get { return this.GetValue(SelectedPathProperty) as string; }
            set { this.SetValue(SelectedPathProperty, value); }
        }

        public DirectorySelectorControl()
        {
            InitializeComponent();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in drives)
                trvStructure.Items.Add(CreateTreeItem(driveInfo));
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            if ((item.Items.Count == 1) && (item.Items[0] is string))
            {
                item.Items.Clear();

                DirectoryInfo expandedDir = null;
                if (item.Tag is DriveInfo)
                    expandedDir = (item.Tag as DriveInfo).RootDirectory;
                if (item.Tag is DirectoryInfo)
                    expandedDir = (item.Tag as DirectoryInfo);
                try
                {
                    foreach (DirectoryInfo subDir in expandedDir.GetDirectories())
                        item.Items.Add(CreateTreeItem(subDir));
                }
                catch { }
            }
        }

        private TreeViewItem CreateTreeItem(object o)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = o.ToString();
            item.Tag = o;
            item.Items.Add("Loading...");
            return item;
        }

        #region Implementation of INotifyPropertyChanged

        public virtual event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        private void TrvStructure_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView) sender;
            TreeViewItem temp = ((TreeViewItem) tree.SelectedItem);

            if (temp == null)
                return;
            var _selectedPath = "";
            string temp1 = "";
            string temp2 = "";
            while (true)
            {
                temp1 = temp.Header.ToString();
                if (temp1.Contains(@"\"))
                {
                    temp2 = "";
                }
                _selectedPath = temp1 + temp2 + _selectedPath;
                if (temp.Parent.GetType() == typeof (TreeView))
                {
                    break;
                }
                temp = ((TreeViewItem) temp.Parent);
                temp2 = @"\";
            }
            SelectedPath = _selectedPath;
        }

        /// <summary>
        /// Finds the provided object in an ItemsControl's children and selects it
        /// </summary>
        /// <param name="parentContainer">The parent container whose children will be searched for the selected item</param>
        /// <param name="itemToSelect">The item to select</param>
        /// <returns>True if the item is found and selected, false otherwise</returns>
        private static bool ExpandAndSelectItem(ItemsControl parentContainer, string itemToSelect)
        {
            //check all items at the current level
            foreach (Object item in parentContainer.Items)
            {
                TreeViewItem currentContainer =
                    parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                //if the data item matches the item we want to select, set the corresponding
                //TreeViewItem IsSelected to true
                if (currentContainer != null)
                {
                    var drive = currentContainer.Tag as DriveInfo;
                    var path = currentContainer.Tag as DirectoryInfo;
                    if (drive != null && drive.RootDirectory.FullName == itemToSelect)
                    {
                        currentContainer.IsSelected = true;
                        currentContainer.BringIntoView();
                        currentContainer.Focus();

                        //the item was found
                        return true;
                    }

                    if (path != null && path.FullName == itemToSelect)
                    {
                        currentContainer.IsSelected = true;
                        currentContainer.BringIntoView();
                        currentContainer.Focus();

                        //the item was found
                        return true;
                    }
                }
            }

            //if we get to this point, the selected item was not found at the current level, so we must check the children
            foreach (Object item in parentContainer.Items)
            {
                TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (currentContainer == null)
                    continue;
                var drive = currentContainer.Tag as DriveInfo;
                var path = currentContainer.Tag as DirectoryInfo;
                if (drive != null && !itemToSelect.StartsWith(drive.RootDirectory.FullName))
                {
                    continue;
                }

                if (path != null && !itemToSelect.StartsWith(path.FullName))
                {
                    continue;
                }

                //if children exist
                if (currentContainer != null && currentContainer.Items.Count > 0)
                {
                    //keep track of if the TreeViewItem was expanded or not
                    bool wasExpanded = currentContainer.IsExpanded;

                    //expand the current TreeViewItem so we can check its child TreeViewItems
                    currentContainer.IsExpanded = true;

                    //if the TreeViewItem child containers have not been generated, we must listen to
                    //the StatusChanged event until they are
                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                    {
                        //store the event handler in a variable so we can remove it (in the handler itself)
                        EventHandler eh = null;
                        eh = new EventHandler(delegate
                        {
                            if (currentContainer.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                            {
                                if (ExpandAndSelectItem(currentContainer, itemToSelect) == false)
                                {
                                    //The assumption is that code executing in this EventHandler is the result of the parent not
                                    //being expanded since the containers were not generated.
                                    //since the itemToSelect was not found in the children, collapse the parent since it was previously collapsed
                                    currentContainer.IsExpanded = false;
                                }

                                //remove the StatusChanged event handler since we just handled it (we only needed it once)
                                currentContainer.ItemContainerGenerator.StatusChanged -= eh;
                            }
                        });
                        currentContainer.ItemContainerGenerator.StatusChanged += eh;
                    }
                    else //otherwise the containers have been generated, so look for item to select in the children
                    {
                        if (ExpandAndSelectItem(currentContainer, itemToSelect) == false)
                        {
                            //restore the current TreeViewItem's expanded state
                            currentContainer.IsExpanded = wasExpanded;
                        }
                        else //otherwise the node was found and selected, so return true
                        {
                            return true;
                        }
                    }
                }
            }

            //no item was found
            return false;
        }

    }
}
