using QuipuTestTask.Models.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

		private string _WebsiteContent;
		public string WebsiteContent
		{
			get => _WebsiteContent;
			set => Set(ref _WebsiteContent, value);
		}

		private ObservableCollection<string> _Tags;
		public ObservableCollection<string> Tags
		{
			get => _Tags;
			set => Set(ref _Tags, value);
		}

		private bool _IsDone;
		public bool IsDone
		{
			get => _IsDone;
			set => Set(ref _IsDone, value);
		}
	}
}
