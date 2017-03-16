using System;
using System.Collections.Generic;
using NControl.Mvvm;

namespace MvvmDemo
{
	public class FeedItem: BaseModel
	{
		public static IEnumerable<FeedItem> FeedRepository = new FeedItem[]{
			new FeedItem { Id = "1", Name = "A skyline blah blah", Image = "https://images.pexels.com/photos/7613/pexels-photo.jpg?w=1260&h=750&auto=compress&cs=tinysrgb" },
			new FeedItem { Id = "2", Name = "Item 2" , Image = "https://images.pexels.com/photos/1139/black-and-white-city-skyline-buildings.jpg?w=1260&h=750&auto=compress&cs=tinysrgb" },
			new FeedItem { Id = "3", Name = "Item 3" , Image = "https://images.pexels.com/photos/285011/pexels-photo-285011.jpeg?w=1260&h=750&auto=compress&cs=tinysrgb" },
			new FeedItem { Id = "4", Name = "Item 1" , Image = "https://images.pexels.com/photos/38203/pexels-photo-38203.jpeg?w=1260&h=750&auto=compress&cs=tinysrgb" },
			new FeedItem { Id = "5", Name = "Item 2" , Image = "https://images.pexels.com/photos/7613/pexels-photo.jpg?w=1260&h=750&auto=compress&cs=tinysrgb" },
			new FeedItem { Id = "6", Name = "Item 3" , Image = "https://images.pexels.com/photos/8218/pexels-photo.jpg?w=1260&h=750&auto=compress&cs=tinysrgb" },
			new FeedItem { Id = "7", Name = "Item 1" , Image = "https://images.pexels.com/photos/12573/photo-1443453489887-98f56bc5bb38.jpeg?w=1260&h=750&auto=compress&cs=tinysrgb" },
			new FeedItem { Id = "8", Name = "Item 2" , Image = "https://images.pexels.com/photos/8247/pexels-photo.jpg?w=1260&h=750&auto=compress&cs=tinysrgb" },
			new FeedItem { Id = "9", Name = "Item 3" , Image = "https://images.pexels.com/photos/7613/pexels-photo.jpg?w=1260&h=750&auto=compress&cs=tinysrgb" },
		};

		public FeedItem()
		{
		}

		public string Id
		{
			get { return GetValue<string>(); }
			set { SetValue<string>(value); }
		}

		public string Image
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string Name
		{
			get { return GetValue<string>(); }
			set { SetValue<string>(value); }
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
