/****************************** Module Header ******************************\
Module Name:  OnMessageAttribute.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;

namespace NControl.Mvvm
{
	[AttributeUsage(AttributeTargets.Property)]
	public class OnMessageAttribute: Attribute
	{
		public Type MessageType { get; private set; }

		public OnMessageAttribute(Type messageType)
		{
			MessageType = messageType;
		}
	}
}
