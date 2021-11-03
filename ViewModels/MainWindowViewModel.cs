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
using System.Threading;
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
			this.StopSearchTagsCommand = new RelayCommand(OnStopSearchTagsCommandExecuted, CanStopSearchTagsCommandExecute);
		}
		#endregion

		#region Properties
		//Коллекция Websites для привязки к Datagrid
		private ObservableCollection<Website> _Websites;
		public ObservableCollection<Website> Websites
		{
			get => _Websites;
			set => Set(ref _Websites, value);
		}

		//Токен для остановки поиска при нажатии на кнопку
		private CancellationTokenSource _CancellationToken;
		public CancellationTokenSource CancellationToken
		{
			get => _CancellationToken;
			set => Set(ref _CancellationToken, value);
		}
		#endregion

		#region Commands
		public ICommand BrowseFileCommand { get; }
		private bool CanBrowseFileCommandExecute(object p) => true;
		private void OnBrowseFileCommandExecuted(object p)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			//Фильтр только для .txt файлов
			openFileDialog.Filter = "Text files (*.txt)|*.txt";

			if (openFileDialog.ShowDialog() == true)
			{
				string filePath = openFileDialog.FileName;

				//Определяем новую коллекцию из Website's
				this.Websites = new ObservableCollection<Website>();
				//Из каждой найденной ссылки в выбранном файле создаем новый объект Website и помещаем его в коллекцию
				this.FindUrlsByText(File.ReadAllText(filePath)).ForEach(url => this.Websites.Add(new Website { Url = url }));
				if (this.Websites.Count != 0)
					MessageBox.Show($"В выбранном файле обнаружено {this.Websites.Count} ссылок.");
				else
					MessageBox.Show("В выбранном файле не обнаружена ни одна ссылка!");
			}
		}

		//Команда для запуска поиска тэгов
		public ICommand StartSearchTagsCommand { get; }
		private bool CanStartSearchTagsCommandExecute(object p) => this.CancellationToken == null && this.Websites.Count != 0;
		private void OnStartSearchTagsCommandExecuted(object p)
		{
			//Перед запуском поиска на всякий случай выставляем стандартные значения для всех Website's
			foreach (var web in Websites)
			{
				web.IsDone = false;
				web.IsWinner = false;
			}

			MessageBox.Show("Поиск тэгов запущен.");
			this.CancellationToken = new CancellationTokenSource();
			//Для каждого Website ищем тэги
			foreach (var website in this.Websites)
				this.FindTagsFromWebsite(website);
		}

		//Команда для остановки уже начатого поиска тэгов
		public ICommand StopSearchTagsCommand { get; }
		//Кнопка остановки поиска выключена, если поиск не начат или если эта кнопка уже была нажата
		private bool CanStopSearchTagsCommandExecute(object p) => this.CancellationToken != null && this.CancellationToken.IsCancellationRequested != true;
		private void OnStopSearchTagsCommandExecuted(object p)
		{
			//При нажатии на кнопку вызываем Cancel у токена, т.е. останавливаем дальнейший поиск
			this.CancellationToken.Cancel(false);
			MessageBox.Show("Поиск тэгов остановлен.");

			//После остановки поиска ищем Website с наибольшим числом тэгов, чтобы выделить его определенным цветом
			this.FindWinner();
		}
		#endregion

		#region Methods
		private List<string> FindUrlsByText(string sourceText)
		{
			List<string> resultUrls = new List<string>();
			//REGEXP для поиска всех URL из текста
			Regex regex = new Regex(@"(http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");

			MatchCollection matches = regex.Matches(sourceText);
			//Добавляем все найденные URL в результирующий список
			if (matches.Count > 0)
				matches.ToList().ForEach(m => resultUrls.Add(m.Value));

			return resultUrls;
		}

		private async Task FindTagsFromWebsite(Website website)
		{
			//REGEXP для нахождения всех тэгов на сайте
			string pattern = "<a.*?>(.*?)<\\/a>";

			//Асинхронный запуск поиска тэгов с передачей CancellationToken
			await Task.Run(() =>
			{
				website.TagsCount = Regex.Matches(website.WebsiteContent, pattern).Count;
				website.IsDone = true;
			}, this.CancellationToken.Token);

			//Каждый раз проверяем, не последний ли Website мы обработали, и если да, то ищем победителя
			if (this.Websites.Count(w => w.IsDone == true) == this.Websites.Count)
			{
				this.CancellationToken = null;
				this.FindWinner();
			}
		}

		private void FindWinner()
		{
			//Поск Website с максимальным значением TagsCount
			this.Websites.Aggregate((a, b) => a.TagsCount < b.TagsCount ? b : a).IsWinner = true;
		}
		#endregion
	}
}
