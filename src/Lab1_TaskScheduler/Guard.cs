using System;

namespace Lab1_TaskScheduler.Utils
{
	public static class Guard
	{
		public static void Requires(bool condition, string message)
		{
			if (!condition)
			{
				throw new ArgumentException($"Pre-condition failed: {message}");
			}
		}

		public static void Requires<TException>(bool condition, string message) where TException : Exception, new()
		{
			if (!condition)
			{
				throw Activator.CreateInstance(typeof(TException), message) as TException
					?? new TException();
			}
		}
	}
}