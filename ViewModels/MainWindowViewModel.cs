using Microsoft.Win32;
using QuipuTestTask.Commands;
using QuipuTestTask.Models;
using QuipuTestTask.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
			this.Websites = new ObservableCollection<Website>();
			this.BrowseFileCommand = new RelayCommand(OnBrowseFileCommandExecuted, CanBrowseFileCommandExecute);
			this.StartSearchTagsCommand = new RelayCommand(OnStartSearchTagsCommandExecuted, CanStartSearchTagsCommandExecute);
		}
		#endregion

		#region Properties
		private string _FilePath;
		public string FilePath
		{
			get => _FilePath;
			set => Set(ref _FilePath, value);
		}

		private ObservableCollection<Website> _Websites;
		public ObservableCollection<Website> Websites
		{
			get => _Websites;
			set => Set(ref _Websites, value);
		}
		#endregion

		#region Commands
		public ICommand BrowseFileCommand { get; }
		private bool CanBrowseFileCommandExecute(object p) => true;
		private void OnBrowseFileCommandExecuted(object p)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Text files (*.txt)|*.txt";

			if (openFileDialog.ShowDialog() == true)
			{
				this.FilePath = openFileDialog.FileName;
				this.FindUrlsByText(File.ReadAllText(this.FilePath)).ForEach(url => this.Websites.Add(new Website { Url = url, Tags = new ObservableCollection<string>() }));
				if (this.Websites.Count != 0)
					MessageBox.Show($"В выбранном файле обнаружено {this.Websites.Count} ссылок.");
				else
					MessageBox.Show("В выбранном файле не обнаружена ни одна ссылка!");
			}
		}

		public ICommand StartSearchTagsCommand { get; }
		private bool CanStartSearchTagsCommandExecute(object p) => this.Websites.Count != 0;
		private void OnStartSearchTagsCommandExecuted(object p)
		{

		}
		#endregion

		#region Methods
		private List<string> FindUrlsByText(string sourceText)
		{
			List<string> resultUrls = new List<string>();
			Regex regex = new Regex(@"(http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");

			MatchCollection matches = regex.Matches(sourceText);
			if (matches.Count > 0)
				matches.ToList().ForEach(m => resultUrls.Add(m.Value));

			return resultUrls;
		}
		#endregion
	}
}
