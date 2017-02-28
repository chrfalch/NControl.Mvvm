using System;
namespace NControl.Mvvm
{
	public interface ISizeProvider
	{
		double Get(string name);
		void Set(string name, double value);
	}
}
