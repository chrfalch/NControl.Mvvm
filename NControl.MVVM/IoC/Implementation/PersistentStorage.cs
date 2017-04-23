using System;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace NControl.Mvvm
{
	public class PersistentStorage: IStorageProvider
	{
		public bool ContainsKey(string key)
		{
			return Application.Current.Properties.ContainsKey(key);
		}

		public T Get<T>(string key)
		{
			if (!ContainsKey(key))
				return default(T);

			var jsonValue = (string)Application.Current.Properties[key];
			var retVal = JsonConvert.DeserializeObject<T>(jsonValue);
			return retVal;
		}

		public void Set<T>(string key, T value)
		{
			if (value == null)
			{
				Application.Current.Properties.Remove(key);
				return;
			}

			var jsonValue = JsonConvert.SerializeObject(value);
			Application.Current.Properties[key] = jsonValue;
			Task.Run(async () => await Application.Current.SavePropertiesAsync()).Wait();
		}
	}
}
