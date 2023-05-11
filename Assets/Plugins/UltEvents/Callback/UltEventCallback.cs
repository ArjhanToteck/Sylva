using System;

namespace UltEvents
{
	partial class UltEvent
	{
		public T InvokeWithCallback<T>()
		{
			return (T)PersistentCallsList[0].Invoke();
		}
	}
}