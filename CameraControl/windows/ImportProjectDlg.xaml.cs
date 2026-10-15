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
using CameraControl.Core.Classes;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
#endregion

namespace CameraControl.windows
{
	/// <summary>
	/// Interaction logic for ImportProjectDlg.xaml
	/// </summary>
	public partial class ImportProjectDlg : Window, INotifyPropertyChanged
	{
		public static void ImportProject ()
		{
			var dlg = new ImportProjectDlg();
			dlg.ShowDialog();
		}

		public ImportProjectDlg ()
		{
			DataContext = this;
			InitializeComponent();
		}

		public bool ProjectFileExists
		{
			get;
			private set;
		}

		public Visibility HasErrors
			=> _errors == null ? Visibility.Collapsed : Visibility.Visible;

		private string _errors;
		public string Errors
		{
			get => _errors;
			set
			{
				if (_errors != value)
				{
					_errors = value;
					NotifyPropertyChanged(nameof(Errors));
					NotifyPropertyChanged(nameof(HasErrors));
				}
			}
		}

		private void tb_fileName_TextChanged (object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			CheckFilePath();
		}

		private void tb_fileName_KeyUp (object sender, System.Windows.Input.KeyEventArgs e)
		{
			CheckFilePath();
		}

		private void btn_browse_Click (object sender, RoutedEventArgs e)
		{
			var dlg = new OpenFileDialog()
			{
				Filter = "Project file (*.dccproject.zip, *.dccproject)|*.dccproject.zip;*.dccproject|All files|*.*"
			};
			if (dlg.ShowDialog() == true)
			{
				tb_fileName.Text = dlg.FileName;
				CheckFilePath();
			}
		}

		private string _filePath;
		private void CheckFilePath ()
		{
			if (_filePath != tb_fileName.Text)
			{
				_filePath = tb_fileName.Text;
				try
				{
					ProjectFileExists = File.Exists(_filePath);
				}
				catch
				{
				}
				NotifyPropertyChanged(nameof(ProjectFileExists));
			}
		}

		private void btn_import_Click (object sender, RoutedEventArgs e)
		{
			if (!ProjectFileExists)
			{
				return;
			}
			Errors = DCCProject.ImportProject(_filePath, cb_clear_scripts.IsChecked ?? false, cb_clear_objects.IsChecked ?? false);
			if (Errors == null)
			{
				Close();
			}
		}

		#region Implementation of INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void NotifyPropertyChanged (string info)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}
		#endregion


	}
}
