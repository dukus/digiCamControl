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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#endregion

namespace CameraControl.windows
{
	/// <summary>
	/// Interaction logic for ExternalObjectsWnd.xaml
	/// </summary>
	public partial class ExternalServicesWnd : IWindow, INotifyPropertyChanged
	{
		public ExternalServicesWnd ()
		{
			DataContext = this;
			InitializeComponent();
			foreach (var provider in ServiceProvider.PluginManager.ExternalObjectProviders)
			{
				cb_provider.Items.Add(new ComboBoxItem()
				{
					Content = provider.DisplayName,
					Tag = provider,
					ToolTip = provider.Description
				});
			}
			_grid_properties_rows = grid_properties.RowDefinitions.Count;

			foreach (var provider in ServiceProvider.Project.ExternalServiceProviders)
			{
				AddToList(provider.Name, provider);
			}
			ServiceProvider.Project.ExternalServiceProviders.CollectionChanged += AddRemoveExternalServiceProviders;
		}

		#region Implementation of IWindow

		public void ExecuteCommand (string cmd, object param)
		{
			switch (cmd)
			{
				case WindowsCmdConsts.ExternalServicesWnd_Show:
				case WindowsCmdConsts.ExternalServicesWnd_ShowExternalObject:
					{
						var externalObject = param as IScriptExternalObject;
						var externalService = param as IExternalServiceProvider;
						Dispatcher.Invoke(new Action(delegate
						{
							Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
							Show();
							Activate();
							Focus();
							ListBoxItem toSelect = null;
							if (externalObject != null)
							{
								toSelect = (from ListBoxItem li in lst_objects.Items
											where (li.Tag as DCCProject.ExternalObject)?.ScriptObject == externalObject
											   || (li.Tag as IExternalServiceProvider)?.ScriptObject == externalObject
											select li)?.FirstOrDefault();
							}
							if (toSelect == null && externalService != null)
							{
								toSelect = (from ListBoxItem li in lst_objects.Items
											where li.Tag == externalService
											select li)?.FirstOrDefault();
							}
							if (toSelect != null)
							{
								SelectedObject = toSelect;
							}
						}));
						break;
					}
				case WindowsCmdConsts.ExternalServicesWnd_Hide:
					Hide();
					break;
				case CmdConsts.All_Close:
					Dispatcher.Invoke(new Action(delegate
					{
						Hide();
						Close();
					}));
					break;
				case WindowsCmdConsts.ExternalServicesWnd_RemoveAllExternalObjects:
					{
						Dispatcher.Invoke(new Action(delegate
						{
							for (var i = lst_objects.Items.Count - 1; i >= 0; i--)
							{
								if ((lst_objects.Items[i] as ListBoxItem).Tag is DCCProject.ExternalObject)
								{
									lst_objects.Items.RemoveAt(i);
								}
							}
						}));
						break;
					}
				case WindowsCmdConsts.ExternalServicesWnd_VerifyExternalObjectsPresent:
					{
						Dispatcher.Invoke(new Action(delegate
						{
							for (var i = lst_objects.Items.Count - 1; i >= 0; i--)
							{
								if ((lst_objects.Items[i] as ListBoxItem).Tag is DCCProject.ExternalObject externalObject
									&& !ServiceProvider.Project.ExternalObjects.Contains(externalObject))
								{
									lst_objects.Items.RemoveAt(i);
								}
							}
							foreach (var externalObject in ServiceProvider.Project.ExternalObjects)
							{
								if (!(from ListBoxItem li in lst_objects.Items
									  where li.Tag == externalObject
									  select li).Any())
								{
									AddToList(externalObject.ScriptObject.Name, externalObject);
									if (!externalObject.ScriptObject.IsActivated)
									{
										NotifyPropertyChanged(nameof(AnyActivationRequired));
									}
								}
							}
						}));
						break;
					}
			}
		}
		#endregion

		private void MetroWindow_Closing (object sender, CancelEventArgs e)
		{
			if (IsVisible)
			{
				e.Cancel = true;
				ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ExternalServicesWnd_Hide);
			}
			else
			{
				ServiceProvider.Project.ExternalServiceProviders.CollectionChanged -= AddRemoveExternalServiceProviders;
			}
		}

		private string _selectedObjectTitle = " ";
		public string SelectedObjectTitle
		{
			get => _selectedObjectTitle;
			private set
			{
				if (_selectedObjectTitle != value)
				{
					_selectedObjectTitle = value;
					NotifyPropertyChanged(nameof(SelectedObjectTitle));
					NotifyPropertyChanged(nameof(PropertiesVisibility));
				}
			}
		}

		public Visibility PropertiesVisibility
			=> string.IsNullOrWhiteSpace(SelectedObjectTitle) ? Visibility.Collapsed : Visibility.Visible;

		private ListBoxItem _selectedObject;
		private ListBoxItem SelectedObject
		{
			get => _selectedObject;
			set
			{
				if (_selectedObject != value)
				{
					_selectedObject = value;
					if (lst_objects.SelectedItem != _selectedObject)
					{
						lst_objects.SelectedItem = _selectedObject;
					}
					if (_selectedObject != null)
					{
						SelectedObjectTitle = (_selectedObject.Tag as IExternalServiceProvider)?.Name
							?? (_selectedObject.Tag as DCCProject.ExternalObject)?.ScriptObject.Name;
						if (_selectedObject.Tag is DCCProject.ExternalObject external)
						{
							foreach (var param in external.ScriptObject.Provider.Parameters)
							{
								param.RefreshEnumerationValues();
							}
							ShowExternalObjectParameters(external.ScriptObject.Provider, external.ScriptObject.ProviderParameters, false);
							grid_properties.Visibility = Visibility.Visible;
						}
						else if (_selectedObject.Tag is IExternalServiceProvider provider)
						{
							cb_provider.SelectedItem = (from ComboBoxItem i in cb_provider.Items
														where i.Tag == provider
														select i).FirstOrDefault();
							ShowExternalServiceProviderParameters(provider);
							grid_properties.Visibility = Visibility.Visible;
						}
					}
					NotifyPropertyChanged(nameof(IsExternalObjectSelected));
					NotifyPropertyChanged(nameof(IsActivationRequired));
				}
			}
		}

		public bool IsExternalObjectSelected
			=> _selectedObject?.Tag is DCCProject.ExternalObject;

		public bool AnyActivationRequired
			=> lst_objects != null
			&& (from ListBoxItem li in lst_objects.Items
				where !((li.Tag as DCCProject.ExternalObject)?.ScriptObject.IsActivated ?? true)
				select li).Any();

		public bool IsActivationRequired
			=> !((SelectedObject?.Tag as DCCProject.ExternalObject)?.ScriptObject.IsActivated ?? true);

		#region Implementation of INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		public virtual void NotifyPropertyChanged (string info)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}
		#endregion

		private void mnu_export_Click (object sender, RoutedEventArgs e)
		{
			ExportProjectDlg.ExportProject();
		}

		private void mnu_import_Click (object sender, RoutedEventArgs e)
		{
			ImportProjectDlg.ImportProject();
		}

		private void lst_objects_Selected (object sender, RoutedEventArgs e)
		{
			_editedExternalObjectProvider = null;
			if (lst_objects.SelectedItem is ListBoxItem selected)
			{
				SelectedObject = selected;
			}
			else
			{
				SelectedObjectTitle = "";
				SelectedObject = null;
				grid_properties.Visibility = Visibility.Collapsed;
			}
		}

		private void mnu_new_Click (object sender, RoutedEventArgs e)
		{
			SelectedObject = null;
			SelectedObjectTitle = "Create new object";
			_editedExternalObjectProvider = null;
			cb_provider.SelectedItem = null;
			ShowExternalObjectParameters(null, new Dictionary<ScriptExternalObjectCreateParameter, object>(), true);
			grid_properties.Visibility = Visibility.Visible;
		}

		private void mnu_delete_Click (object sender, RoutedEventArgs e)
		{
			if (IsExternalObjectSelected)
			{
				ServiceProvider.Project.RemoveExternalObject((SelectedObject.Tag as DCCProject.ExternalObject).ScriptObject);
				lst_objects.Items.Remove(SelectedObject);
			}
		}

		private const string _activated_suffix = " 🗸";

		private void mnu_activate_Click (object sender, RoutedEventArgs e)
		{
			if (SelectedObject?.Tag is DCCProject.ExternalObject externalObject
				&& !externalObject.ScriptObject.IsActivated)
			{
				try
				{
					externalObject.Activate();
				}
				catch (Exception ex)
				{
					tb_activation_error.Text = $"Activation failed: {ex.Message}";
					tb_activation_error.Visibility = Visibility.Visible;
					return;
				}
				if (!externalObject.ScriptObject.IsActivated)
				{
					tb_activation_error.Text = "Activation failed";
					tb_activation_error.Visibility = Visibility.Visible;
					return;
				}
				else
				{
					tb_activation_error.Visibility = Visibility.Collapsed;
				}
				NotifyPropertyChanged(nameof(AnyActivationRequired));
				NotifyPropertyChanged(nameof(IsActivationRequired));
				SelectedObject.Content = externalObject.ScriptObject.Name + _activated_suffix;
			}
		}

		private void mnu_activate_all_Click (object sender, RoutedEventArgs e)
		{
			foreach (ListBoxItem item in lst_objects.Items)
			{
				if (item.Tag is DCCProject.ExternalObject externalObject
					&& !externalObject.ScriptObject.IsActivated)
				{
					string error = null;
					try
					{
						externalObject.Activate();
						if (!externalObject.ScriptObject.IsActivated)
						{
							error = "Activation failed";
						}
						else
						{
							item.Content = externalObject.ScriptObject.Name + _activated_suffix;
						}
					}
					catch (Exception ex)
					{
						error = $"Activation failed: {ex.Message}";
					}
					if (error != null)
					{
						SelectedObject = item;
						tb_activation_error.Text = error;
						tb_activation_error.Visibility = Visibility.Visible;
						break;
					}
				}
			}
			NotifyPropertyChanged(nameof(AnyActivationRequired));
		}

		private void mnu_buttons_Click (object sender, RoutedEventArgs e)
		{
			ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.UIButtonWnd_Show);
		}

		private void mnu_scripts_Click (object sender, RoutedEventArgs e)
		{
			ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ScriptWnd_Show);
		}

		private void cb_provider_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if ((cb_provider.SelectedItem as ComboBoxItem)?.Tag is IScriptExternalObjectProvider provider
				&& provider != _editedExternalObjectProvider)
			{
				foreach (var param in provider.Parameters)
				{
					param.RefreshEnumerationValues();
				}
				ShowExternalObjectParameters(provider, new Dictionary<ScriptExternalObjectCreateParameter, object>(), true);
			}
		}

		private ListBoxItem AddToList (string name, object element)
		{
			ListBoxItem item = new ListBoxItem()
			{
				Tag = element
			};
			if (element is IExternalServiceProvider provider && element is INotifyPropertyChanged)
			{
				var binding = new Binding(nameof(provider.IsActivated))
				{
					Mode = BindingMode.OneWay,
					Source = provider,
					Converter = new ExternalServiceName(provider.Name, () => provider.IsActivated)
				};
				item.SetBinding(ListBoxItem.ContentProperty, binding);
			}
			else
			{
				var wrapper = element as DCCProject.ExternalObject;
				var binding = new Binding(nameof(wrapper.IsActivated))
				{
					Mode = BindingMode.OneWay,
					Source = wrapper,
					Converter = new ExternalServiceName(wrapper.ScriptObject.Name, () => wrapper.IsActivated)
				};
				item.SetBinding(ListBoxItem.ContentProperty, binding);
			}
			for (var i = 0; i < lst_objects.Items.Count; i++)
			{
				if (string.Compare(name, (string)(lst_objects.Items[i] as ListBoxItem).Content, true) <= 0)
				{
					lst_objects.Items.Insert(i, item);
					return item;
				}
			}
			lst_objects.Items.Add(item);
			return item;
		}
		private sealed class ExternalServiceName : IValueConverter
		{
			private readonly string _name;
			private readonly Func<bool> _isActivated;

			internal ExternalServiceName (string name, Func<bool> isActivated)
			{
				_name = name;
				_isActivated = isActivated;
			}

			public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
			{
				return _name + (_isActivated() ? _activated_suffix : "");
			}

			public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}

		private void AddRemoveExternalServiceProviders (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				for (var i = lst_objects.Items.Count - 1; i >= 0; i--)
				{
					if (e.OldItems.Contains((lst_objects.Items[i] as ListBoxItem).Tag))
					{
						lst_objects.Items.RemoveAt(i);
					}
				}
			}
			if (e.NewItems != null)
			{
				foreach (IExternalServiceProvider provider in e.NewItems)
				{
					AddToList(provider.Name, provider);
				}
			}
		}

		private readonly int _grid_properties_rows;

		private void InitialiseParameterGrid (object provider, bool allowEdit)
		{
			foreach (var child in (from UIElement c in grid_properties.Children
								   where Grid.GetRow(c) >= _grid_properties_rows
								   select c).ToList())
			{
				grid_properties.Children.Remove(child);
			}
			if (grid_properties.RowDefinitions.Count > _grid_properties_rows)
			{
				grid_properties.RowDefinitions.RemoveRange(_grid_properties_rows, grid_properties.RowDefinitions.Count - _grid_properties_rows);
			}
			cb_provider.IsEnabled = allowEdit;
			if (allowEdit)
			{
				cb_provider.Visibility = Visibility.Visible;
				cb_provider.SelectedItem = (from ComboBoxItem i in cb_provider.Items
											where i.Tag == provider
											select i).FirstOrDefault();
				tb_provider.Visibility = Visibility.Collapsed;
			}
			else
			{
				cb_provider.Visibility = Visibility.Collapsed;
				tb_provider.Text = (provider as IScriptExternalObjectProvider)?.DisplayName
					?? (provider as IExternalServiceProvider).Name;
				tb_provider.Visibility = Visibility.Visible;
			}
			tb_activation_error.Visibility = Visibility.Collapsed;
		}

		private void ShowExternalServiceProviderParameters (IExternalServiceProvider provider)
		{
			InitialiseParameterGrid(provider, false);

			var iRow = _grid_properties_rows;
			grid_properties.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });

			var button = new Button()
			{
				Content = "Details",
				Margin = new Thickness(4)
			};
			button.Click += (s, e) => provider.ShowWindow();
			Grid.SetRow(button, iRow);
			Grid.SetColumn(button, 1);
			grid_properties.Children.Add(button);
		}

		private IScriptExternalObjectProvider _editedExternalObjectProvider;
		private Dictionary<ScriptExternalObjectCreateParameter, object> _editedExternalObjectParameters;
		private Button _recreate_object;

		private void ShowExternalObjectParameters (IScriptExternalObjectProvider provider, IReadOnlyDictionary<ScriptExternalObjectCreateParameter, object> parameterValues, bool allowEdit)
		{
			_create_name = null;
			_create_error = null;
			_recreate_object = null;
			_editedExternalObjectProvider = provider;
			_editedExternalObjectParameters = provider == null ? null : new Dictionary<ScriptExternalObjectCreateParameter, object>();
			InitialiseParameterGrid(provider, allowEdit);

			if (provider is null)
			{
				return;
			}

			var iRow = _grid_properties_rows - 1;
			void _AddRow ()
			{
				iRow++;
				grid_properties.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
			}
			var margin = new Thickness(4);

			if (!string.IsNullOrWhiteSpace(provider.Description))
			{
				_AddRow();

				var label = new TextBlock()
				{
					Text = provider.Description,
					Margin = margin,
					TextWrapping = TextWrapping.Wrap
				};
				Grid.SetRow(label, iRow);
				Grid.SetColumn(label, 1);
				grid_properties.Children.Add(label);
			}

			if (allowEdit)
			{
				_AddRow();

				var label = new TextBlock() { Text = "Name", Margin = margin };
				Grid.SetRow(label, iRow);
				grid_properties.Children.Add(label);

				_create_name = new TextBox()
				{
					Margin = margin,
				};
				Grid.SetRow(_create_name, iRow);
				Grid.SetColumn(_create_name, 1);
				grid_properties.Children.Add(_create_name);
			}

			var allowUpdate = false;
			foreach (var param in provider.Parameters)
			{
				_AddRow();

				var allowEditParam = allowEdit || !param.IsConstant;
				if (!allowEdit)
				{
					allowUpdate |= allowEditParam;
					if (parameterValues.TryGetValue(param, out var paramValue))
					{
						_editedExternalObjectParameters[param] = paramValue;
					}
				}

				UIElement inputElement;
				switch (param.Type)
				{
					case ScriptExternalObjectCreateParameter.ValueType.Boolean:
						{
							var input = new CheckBox()
							{
								IsChecked = (bool)param.GetValue(parameterValues),
								Margin = margin,
								IsEnabled = allowEditParam,
							};
							if (allowEditParam)
							{
								input.Click += (s, e) =>
								{
									_editedExternalObjectParameters[param] = input.IsChecked;
									if (_recreate_object != null)
									{
										_recreate_object.Visibility = Visibility.Visible;
									}
								};
							}
							inputElement = input;
						}
						break;

					case ScriptExternalObjectCreateParameter.ValueType.Long:
						{
							var currentValue = param.GetValue(parameterValues)?.ToString() ?? "";
							if (!allowEditParam || param.EnumerationValues == null || param.EnumerationValues.Count == 0)
							{
								var input = new TextBox()
								{
									Text = currentValue,
									Margin = margin,
									IsEnabled = allowEditParam,
								};
								if (allowEditParam)
								{
									input.TextChanged += (s, e) =>
									{
										_editedExternalObjectParameters[param] = string.IsNullOrWhiteSpace(input.Text)
											? null
											: long.TryParse(input.Text.Trim(), out var value)
												? (object)value
												: _invalidLongValue;
										if (_recreate_object != null)
										{
											_recreate_object.Visibility = Visibility.Visible;
										}
									};
								}
								inputElement = input;
							}
							else
							{
								var input = new ComboBox()
								{
									Margin = margin
								};
								foreach (var e in param.EnumerationValues)
								{
									input.Items.Add(new ComboBoxItem()
									{
										Content = e,
										IsSelected = e == currentValue
									});
								}
								input.SelectionChanged += (s, e) =>
								{
									_editedExternalObjectParameters[param] = input.SelectedItem is null
										? null
										: long.TryParse((input.SelectedItem as ComboBoxItem).Content as string, out var value)
											? (object)value
											: null;
									if (_recreate_object != null)
									{
										_recreate_object.Visibility = Visibility.Visible;
									}
								};
								inputElement = input;
							}
						}
						break;


					case ScriptExternalObjectCreateParameter.ValueType.String:
						{
							var currentValue = param.GetValue(parameterValues)?.ToString() ?? "";
							if (!allowEditParam || param.EnumerationValues == null || param.EnumerationValues.Count == 0)
							{
								var input = new TextBox()
								{
									Text = currentValue,
									Margin = margin,
									IsEnabled = allowEditParam,
								};
								if (allowEditParam)
								{
									input.TextChanged += (s, e) =>
									{
										_editedExternalObjectParameters[param] = string.IsNullOrWhiteSpace(input.Text) ? null : input.Text;
										if (_recreate_object != null)
										{
											_recreate_object.Visibility = Visibility.Visible;
										}
									};
								}
								inputElement = input;
							}
							else
							{
								var input = new ComboBox()
								{
									Margin = margin,
								};
								foreach (var e in param.EnumerationValues)
								{
									input.Items.Add(new ComboBoxItem()
									{
										Content = e,
										IsSelected = e == currentValue
									});
								}
								input.SelectionChanged += (s, e) =>
								{
									_editedExternalObjectParameters[param] = input.SelectedItem is null ? null : (input.SelectedItem as ComboBoxItem).Content as string;
									if (_recreate_object != null)
									{
										_recreate_object.Visibility = Visibility.Visible;
									}
								};
								inputElement = input;
							}
						}
						break;
					default:
						continue;
				}

				{
					var label = new TextBlock() { Text = param.Name, Margin = margin };
					Grid.SetRow(label, iRow);
					grid_properties.Children.Add(label);

					Grid.SetRow(inputElement, iRow);
					Grid.SetColumn(inputElement, 1);
					grid_properties.Children.Add(inputElement);
				}

				if (!string.IsNullOrWhiteSpace(param.Description))
				{
					_AddRow();

					var label = new TextBlock()
					{
						Text = param.Description.Trim(),
						TextWrapping = TextWrapping.Wrap,
						Margin = margin
					};
					Grid.SetRow(label, iRow);
					Grid.SetColumn(label, 1);
					grid_properties.Children.Add(label);
				}
			}

			if (allowEdit || allowUpdate)
			{
				_AddRow();

				_create_error = new TextBlock()
				{
					Visibility = Visibility.Collapsed,
					Margin = margin
				};
				Grid.SetRow(_create_error, iRow);
				Grid.SetColumn(_create_error, 1);
				grid_properties.Children.Add(_create_error);

				_AddRow();

				var button = new Button()
				{
					Content = allowEdit ? "Create" : "Re-create",
					Margin = margin,
					Visibility = allowEdit ? Visibility.Visible : Visibility.Collapsed,
				};
				button.Click += (s, e) => CreateNewObject(allowUpdate);
				Grid.SetRow(button, iRow);
				Grid.SetColumn(button, 1);
				grid_properties.Children.Add(button);
				if (allowUpdate)
				{
					_recreate_object = button;
				}
			}
		}

		private TextBlock _create_error;
		private TextBox _create_name;

		private void CreateNewObject (bool replaceCurrent)
		{
			_create_error.Text = "";
			_create_error.Visibility = Visibility.Collapsed;
			void _SetError (string error)
			{
				_create_error.Text = error;
				_create_error.Visibility = Visibility.Visible;
			}
			var current = replaceCurrent ? (SelectedObject.Tag as DCCProject.ExternalObject).ScriptObject : null;

			if (_editedExternalObjectParameters == null || _create_error == null)
			{
				return;
			}
			string name;
			if (replaceCurrent)
			{
				name = current.Name;
			}
			else
			{
				if (string.IsNullOrWhiteSpace(_create_name.Text))
				{
					_SetError("No name specified.");
					return;
				}
				name = _create_name.Text.Trim();
				if (!ServiceProvider.Project.IsValidExternalObjectName(name))
				{
					_SetError($"The name '{name}' is already in use.");
					return;
				}
			}

			if (_editedExternalObjectProvider == null)
			{
				_SetError("No provider selected.");
				return;
			}

			foreach (var parameter in _editedExternalObjectProvider.Parameters)
			{
				object value = null;
				if (parameter.IsRequired && parameter.DefaultValue == null &&
					(!_editedExternalObjectParameters.TryGetValue(parameter, out value)
					|| value is null
					|| (value is string sValue && string.IsNullOrWhiteSpace(sValue))
					))
				{
					_SetError($"A value for '{parameter.Name}' is required.");
				}
				else if (value == _invalidLongValue)
				{
					_SetError($"The value for '{parameter.Name}' must be an integer number.");
				}
			}
			if (_create_error.Text.Length > 0)
			{
				return;
			}

			DCCProject.ExternalObject externalObject;
			try
			{
				externalObject = ServiceProvider.Project.CreateExternalObject(_editedExternalObjectProvider, name, _editedExternalObjectParameters);
			}
			catch (Exception ex)
			{
				_SetError($"Object could not be created: {ex.Message}");
				return;
			}
			if (externalObject == null)
			{
				_SetError("Object could not be created.");
				return;
			}
			if (replaceCurrent)
			{
				var item = SelectedObject;
				item.Content = externalObject.ScriptObject.Name + (externalObject.ScriptObject.IsActivated ? _activated_suffix : "");
				item.Tag = externalObject;
				ServiceProvider.Project.RemoveExternalObject(current);
				if (_recreate_object != null)
				{
					_recreate_object.Visibility = Visibility.Collapsed;
				}
			}
			else
			{
				AddToList(externalObject.ScriptObject.Name, externalObject).IsSelected = true;
			}
			NotifyPropertyChanged(nameof(AnyActivationRequired));
			NotifyPropertyChanged(nameof(IsActivationRequired));
		}
		private static readonly object _invalidLongValue = new object();
	}
}
