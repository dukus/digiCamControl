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
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
#endregion

namespace CameraControl.windows
{
	/// <summary>
	/// Interaction logic for ButtonWnd.xaml
	/// </summary>
	public partial class UIButtonWnd : IWindow
	{
		public UIButtonWnd ()
		{
			DataContext = this;
			InitializeComponent();
			_grid_buttons_rows = grid_buttons.RowDefinitions.Count;
			ShowButtons(true);
			ServiceProvider.Project.UIButtons.CollectionChanged += (s, e) => ShowButtons(true);
		}

		#region Implementation of IWindow

		public void ExecuteCommand (string cmd, object param)
		{
			switch (cmd)
			{
				case WindowsCmdConsts.UIButtonWnd_Show:
					Dispatcher.Invoke(new Action(delegate
					{
						Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
						Show();
						Activate();
						Focus();
					}));
					break;
				case WindowsCmdConsts.UIButtonWnd_Hide:
					Hide();
					break;
				case CmdConsts.All_Close:
					Dispatcher.Invoke(new Action(delegate
					{
						Hide();
						Close();
					}));
					break;
			}
		}
		#endregion

		private void MetroWindow_Closing (object sender, CancelEventArgs e)
		{
			if (IsVisible)
			{
				e.Cancel = true;
				ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.UIButtonWnd_Hide);
			}
		}

		private void MetroWindow_Size (object sender, SizeChangedEventArgs e)
		{
			ShowButtons(false);
		}

		private void mnu_services_Click (object sender, RoutedEventArgs e)
		{
			ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ExternalServicesWnd_Show);
		}

		private void mnu_scripts_Click (object sender, RoutedEventArgs e)
		{
			ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ScriptWnd_Show);
		}

		private int _numColumns;
		private readonly int _grid_buttons_rows;
		private void ShowButtons (bool forceRedraw)
		{
			var columns = (int)Math.Floor(Width / 300);
			if (!forceRedraw && _numColumns == columns)
			{
				return;
			}
			_numColumns = columns;

			var groups = new List<(IUIButtonProvider provider, int iRow, int iColumn)>();
			var rowHeights = new List<int>();
			var columnsWithDescription = new HashSet<int>();
			var iColumn = 0;
			var rowsPerColumn = 0;
			var lastRow = _grid_buttons_rows;
			var iSubRow = 0;
			var iFirstProviderOnRow = 0;
			var numGroupsPreviousRows = 0;
			var buttonsPerProvider = ServiceProvider.Project.UIButtons
				.GroupBy(b => b.Provider)
				.ToDictionary(b => b.Key, g => g.ToList());
			var providers = (from p in buttonsPerProvider.Keys
							 orderby p.Name.ToLower()
							 select p).ToList();
			for (var iProvider = 0; iProvider < providers.Count; iProvider++)
			{
				var provider = providers[iProvider];
				var buttons = buttonsPerProvider[provider];
				if (rowsPerColumn == 0)
				{
					rowsPerColumn = buttons.Count + 1;
				}
				else if (iSubRow == 0 && buttons.Count + 1 > rowsPerColumn)
				{
					rowsPerColumn = buttons.Count + 1;
					iProvider = iFirstProviderOnRow - 1;
					iColumn = 0;
					groups.RemoveRange(numGroupsPreviousRows, groups.Count - numGroupsPreviousRows);
					continue;
				}
				var endRow = iSubRow + buttons.Count + 1;
				if (endRow > rowsPerColumn)
				{
					iSubRow = 0;
					if (++iColumn >= _numColumns)
					{
						lastRow += rowsPerColumn + 1;
						iColumn = 0;
						rowHeights.Add(rowsPerColumn);
						rowsPerColumn = 0;
						iFirstProviderOnRow = iProvider;
						numGroupsPreviousRows = groups.Count;
					}
					iProvider--;
					continue;
				}
				groups.Add((provider, lastRow + 1 + iSubRow, iColumn));
				if ((from b in buttons
					 where !string.IsNullOrWhiteSpace(b.Description)
					 select b).Any())
				{
					columnsWithDescription.Add(iColumn);
				}
				iSubRow = endRow;
			}
			if (rowsPerColumn > 0)
			{
				rowHeights.Add(rowsPerColumn);
			}

			foreach (var child in (from UIElement c in grid_buttons.Children
								   where Grid.GetRow(c) >= _grid_buttons_rows
								   select c).ToList())
			{
				grid_buttons.Children.Remove(child);
			}
			if (grid_buttons.RowDefinitions.Count > _grid_buttons_rows)
			{
				grid_buttons.RowDefinitions.RemoveRange(_grid_buttons_rows, grid_buttons.RowDefinitions.Count - _grid_buttons_rows);
			}
			foreach (var rowHeight in rowHeights)
			{
				grid_buttons.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Pixel) });
				for (var iRow = 0; iRow < rowHeight; iRow++)
				{
					grid_buttons.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
				}
			}
			grid_buttons.ColumnDefinitions.Clear();
			for (var iCol = 0; iCol < _numColumns; iCol++)
			{
				grid_buttons.ColumnDefinitions.Add(new ColumnDefinition()
				{
					Width = new GridLength(columnsWithDescription.Contains(iCol) ? 3 : 1, GridUnitType.Star)
				});
			}
			var margin = new Thickness(4);

			foreach (var (provider, iRow, iCol) in groups)
			{
				var buttons = buttonsPerProvider[provider];

				var groupGrid = new Grid();
				foreach (var button in buttons)
				{
					groupGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
				}
				groupGrid.ColumnDefinitions.Add(new ColumnDefinition()
				{
					Width = new GridLength(1, GridUnitType.Star)
				});
				if (columnsWithDescription.Contains(iCol))
				{
					groupGrid.ColumnDefinitions.Add(new ColumnDefinition()
					{
						Width = new GridLength(2, GridUnitType.Star)
					});
				}

				iSubRow = -1;
				foreach (var uiButton in buttons)
				{
					iSubRow++;
					var button = new Button()
					{
						Margin = margin
					};
					button.Click += (s, e) => uiButton.Click();
					if (uiButton is INotifyPropertyChanged)
					{
						button.SetBinding(Button.ContentProperty, new Binding(nameof(uiButton.Title))
						{
							Mode = BindingMode.OneWay,
							Source = uiButton
						});
						button.SetBinding(Button.IsEnabledProperty, new Binding(nameof(uiButton.IsEnabled))
						{
							Mode = BindingMode.OneWay,
							Source = uiButton
						});
					}
					else
					{
						button.Content = uiButton.Title;
						button.IsEnabled = uiButton.IsEnabled;
					}
					Grid.SetRow(button, iSubRow);
					groupGrid.Children.Add(button);

					if (!string.IsNullOrWhiteSpace(uiButton.Description))
					{
						var description = new TextBlock()
						{
							Margin = margin,
							TextWrapping = TextWrapping.Wrap
						};
						if (uiButton is INotifyPropertyChanged)
						{
							description.SetBinding(TextBlock.TextProperty, new Binding(nameof(uiButton.Description))
							{
								Mode = BindingMode.OneWay,
								Source = uiButton
							});
						}
						else
						{
							description.Text = uiButton.Description;
						}
						Grid.SetRow(description, iSubRow);
						Grid.SetColumn(description, 1);
						groupGrid.Children.Add(description);
					}
				}

				var providerName = new Run();
				if (provider is INotifyPropertyChanged)
				{
					providerName.SetBinding(Run.TextProperty, new Binding(nameof(provider.Name))
					{
						Mode = BindingMode.OneWay,
						Source = provider
					});
				}
				else
				{
					providerName.Text = provider.Name;
				}
				var link = new Hyperlink(providerName);
				link.Click += (s, e) => provider.ShowWindow();
				var group = new GroupBox()
				{
					Header = link,
					Content = groupGrid,
					Margin = margin
				};
				Grid.SetRow(group, iRow);
				Grid.SetRowSpan(group, buttons.Count + 1);
				Grid.SetColumn(group, iCol);
				grid_buttons.Children.Add(group);
			}
		}
	}
}
