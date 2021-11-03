using QuipuTestTask.Models.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace QuipuTestTask.Models
{
	public class Website : Entity
	{
		private string _Url;
		public string Url
		{
			get => _Url;
			set => Set(ref _Url, value);
		}

		private int _TagsCount;
		public int TagsCount
		{
			get => _TagsCount;
			set => Set(ref _TagsCount, value);
		}

		//Завершен ли поиск тэгов по этому сайту?
		private bool _IsDone;
		public bool IsDone
		{
			get => _IsDone;
			set => Set(ref _IsDone, value);
		}

		//Найдено ли на этом сайте больше всего тэгов?
		private bool _IsWinner;
		public bool IsWinner
		{
			get => _IsWinner;
			set => Set(ref _IsWinner, value);
		}

		//Содержимое сайта
		public string WebsiteContent => new WebClient().DownloadString(this.Url);
	}
}
