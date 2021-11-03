using Microsoft.Win32;
using QuipuTestTask.Commands;
using QuipuTestTask.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuipuTestTask.ViewModels
{
	public class MainWindowViewModel : BaseViewModel
	{
		#region Constructor
		public MainWindowViewModel()
		{
			this.BrowseFileCommand = new RelayCommand(OnBrowseFileCommandExecuted, CanBrowseFileCommandExecute);
		}
		#endregion

		#region Properties
		private string _FilePath;
		public string FilePath
		{
			get => _FilePath;
			set => Set(ref _FilePath, value);
		}

		private string _FileContent;
		public string FileContent
		{
			get => _FileContent;
			set
			{
				Set(ref _FileContent, value);
				MessageBox.Show($"Содержимое файла {this.FilePath}:\n{this.FileContent}");
			}
		}
		#endregion

		#region Commands
		public ICommand BrowseFileCommand { get; }
		private bool CanBrowseFileCommandExecute(object p) => true;
		public void OnBrowseFileCommandExecuted(object p)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Text files (*.txt)|*.txt";

			if (openFileDialog.ShowDialog() == true)
			{
				this.FilePath = openFileDialog.FileName;
				this.FileContent = File.ReadAllText(this.FilePath);
			}
		}
		#endregion

		#region Methods
		#endregion
	}
}
