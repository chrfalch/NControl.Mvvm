using System;
namespace NControl.Mvvm
{
	public interface IStorageProvider
	{
		bool ContainsKey(string key);
		void Set<T>(string key, T value);
		T Get<T>(string key);
	}
}
