using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace ElasticsearchDemo
{
	public class MyTimer
	{
		private Stopwatch stopwatch;
		private bool isRunning;

		public TimeSpan ElapsedTime => stopwatch.Elapsed;

		public event EventHandler<TimeElapsedEventArgs> TimeElapsed;

		public MyTimer()
		{
			stopwatch = new Stopwatch();
			isRunning = false;
		}

		public void Start()
		{
			if (!isRunning)
			{
				stopwatch.Start();
				isRunning = true;
				OnTimeElapsed(TimeSpan.Zero);
			}
		}

		public void Stop()
		{
			if (isRunning)
			{
				stopwatch.Stop();
				isRunning = false;
			}
		}

		public void Reset()
		{
			stopwatch.Reset();
			isRunning = false;
			OnTimeElapsed(TimeSpan.Zero);
		}

		protected virtual void OnTimeElapsed(TimeSpan elapsedTime)
		{
			TimeElapsed?.Invoke(this, new TimeElapsedEventArgs(elapsedTime));
		}
	}
	public class TimeElapsedEventArgs : EventArgs
	{
		public TimeSpan ElapsedTime { get; }

		public TimeElapsedEventArgs(TimeSpan elapsedTime)
		{
			ElapsedTime = elapsedTime;
		}
	}

}
