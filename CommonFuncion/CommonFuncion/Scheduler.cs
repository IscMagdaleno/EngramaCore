using FluentScheduler;

namespace CommonFuncion
{
	public class Scheduler
	{
		public enum Unit
		{
			Seconds,

			Minutes,

			Hours,

			Days
		}

		public static void RunOnce(DateTime datetime, Action action)
		{
			JobManager.AddJob(action, (s) => s.ToRunOnceAt(datetime));
		}

		public static void RunOnce(Action action, int value, Unit unit)
		{
			switch (unit)
			{
				case Unit.Seconds:

					JobManager.AddJob(action, (s) => s.ToRunOnceIn(value).Seconds());

					break;

				case Unit.Minutes:

					JobManager.AddJob(action, (s) => s.ToRunOnceIn(value).Minutes());

					break;

				case Unit.Hours:

					JobManager.AddJob(action, (s) => s.ToRunOnceIn(value).Hours());

					break;

				case Unit.Days:

					JobManager.AddJob(action, (s) => s.ToRunOnceIn(value).Days());

					break;
			}
		}

		public static void RunEvery(int value, Unit unit, Action action)
		{
			switch (unit)
			{
				case Unit.Seconds:

					JobManager.AddJob(action, (s) => s.ToRunEvery(value).Seconds());

					break;

				case Unit.Minutes:

					JobManager.AddJob(action, (s) => s.ToRunEvery(value).Minutes());

					break;

				case Unit.Hours:

					JobManager.AddJob(action, (s) => s.ToRunEvery(value).Hours());

					break;

				case Unit.Days:

					JobManager.AddJob(action, (s) => s.ToRunEvery(value).Days());

					break;
			}
		}
	}
}
