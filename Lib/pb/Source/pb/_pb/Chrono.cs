using System;
using System.Security.Permissions;

namespace pb
{
    public class Chrono : MarshalByRefObject, IChrono
	{
		#region variable Chrono
		private string gsName = null;
		private TimeSpan gtsTotalTime = new TimeSpan(0);
		private int giCount = 0;
		private bool gbIsStarted = false;
		private DateTime gdtStart = new DateTime(0);
		private DateTime gdtStop = new DateTime(0);
		private DateTime gdtIntermediate1 = new DateTime(0);
		private DateTime gdtIntermediate2 = new DateTime(0);
		#endregion

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            // from http://stackoverflow.com/questions/2410221/appdomain-and-marshalbyrefobject-life-time-how-to-avoid-remotingexception
            return null;
        }

		#region property Chrono
		#region Name
		public string Name
		{
			get { return gsName; }
			set { gsName = value; }
		}
		#endregion

		#region IsStarted
		public bool IsStarted
		{
			get { return gbIsStarted; }
		}
		#endregion

		#region StartTime
		public DateTime StartTime
		{
			get { return gdtStart; }
		}
		#endregion

		#region StopTime
		public DateTime StopTime
		{
			get { return gdtStop; }
		}
		#endregion

		#region IntermediateTime1
		public DateTime IntermediateTime1
		{
			get { return gdtIntermediate1; }
		}
		#endregion

		#region IntermediateTime2
		public DateTime IntermediateTime2
		{
			get { return gdtIntermediate2; }
		}
		#endregion

		#region IntermediateTime
		public DateTime IntermediateTime
		{
			get { return gdtIntermediate2; }
		}
		#endregion

		#region Intermediate
		public TimeSpan Intermediate
		{
			get { return GetTimeSpan(gdtIntermediate1, gdtIntermediate2); }
		}
		#endregion

		#region IntermediateString
		public string IntermediateString
		{
			get { return TimeSpanToString(Intermediate); }
		}
		#endregion

		#region Time
		public TimeSpan Time
		{
			get { return GetTimeSpan(gdtStart, gdtIntermediate2); }
		}
		#endregion

		#region TimeString
		public string TimeString
		{
			get { return TimeSpanToString(Time); }
		}
		#endregion

		#region TotalTime
		public TimeSpan TotalTime
		{
			get { return gtsTotalTime; }
		}
		#endregion

		#region TotalTimeString
		public string TotalTimeString
		{
			get { return TimeSpanToString(gtsTotalTime); }
		}
		#endregion

		#region Count
		public int Count
		{
			get { return giCount; }
		}
		#endregion

		#region AverageTime
		public TimeSpan AverageTime
		{
			get
			{
				if (giCount != 0)
					return new TimeSpan((long)Math.Round((decimal)gtsTotalTime.Ticks / (decimal)giCount));
				else
					return new TimeSpan(0);
			}
		}
		#endregion

		#region AverageTimeString
		public string AverageTimeString
		{
			get { return TimeSpanToString(AverageTime); }
		}
		#endregion
		#endregion

		#region Start()
		public void Start()
		{
			Start(DateTime.Now);
		}
		#endregion

		#region Start(DateTime dt)
		public void Start(DateTime dt)
		{
			gbIsStarted = true;
			gdtStart = gdtIntermediate1 = gdtIntermediate2 = dt;
			gdtStop = new DateTime(0);
		}
		#endregion

		#region Stop()
		public void Stop()
		{
			Stop(DateTime.Now);
		}
		#endregion

		#region Stop(DateTime dt)
		public void Stop(DateTime dt)
		{
			if (gbIsStarted)
			{
				gdtStop = dt;
				gtsTotalTime = gtsTotalTime.Add(GetTimeSpan(gdtStart, gdtStop));
				giCount++;
				gbIsStarted = false;
			}
		}
		#endregion

		#region RazTotal
		private void RazTotal()
		{
			gtsTotalTime = new TimeSpan(0);
			giCount = 0;
		}
		#endregion

		#region SetIntermediateTime()
		public void SetIntermediateTime()
		{
			SetIntermediateTime(DateTime.Now);
		}
		#endregion

		#region SetIntermediateTime(DateTime dt)
		public void SetIntermediateTime(DateTime dt)
		{
			gdtIntermediate1 = gdtIntermediate2;
			gdtIntermediate2 = dt;
			if (gdtStart.Ticks == 0) gdtStart = dt;
			if (gdtIntermediate1.Ticks == 0) gdtIntermediate1 = dt;
		}
		#endregion

		#region static functions
		#region GetTimeSpan
		public static TimeSpan GetTimeSpan(DateTime dtStart, DateTime dtStop)
		{
			if (dtStart.Ticks != 0 && dtStop.Ticks != 0)
				return dtStop.Subtract(dtStart);
			else
				return new TimeSpan(0);
		}
		#endregion

		#region GetTimeSpanString
		public static string GetTimeSpanString(DateTime dtStart, DateTime dtStop)
		{
			return TimeSpanToString(GetTimeSpan(dtStart, dtStop));
		}
		#endregion

		#region TimeSpanToString
		public static string TimeSpanToString(TimeSpan ts)
		{
			//return string.Format("{0:00}:{1:00}:{2:00}.{3:0000000}", (int)ts.TotalHours, ts.Minutes, ts.Seconds, ts.Ticks % 10000000);
			return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", (int)ts.TotalHours, ts.Minutes, ts.Seconds, ts.Milliseconds);
		}
		#endregion
		#endregion
	}
}
