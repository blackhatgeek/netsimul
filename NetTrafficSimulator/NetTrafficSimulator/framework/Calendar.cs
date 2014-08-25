using System;
using System.Collections.Generic;
using log4net;

namespace MFF_NPRG031
{
	/**
	 * Calendar holds information about Events
	 */
	public class Calendar
	{
		/**
		 * Calendar is a list of events
		 */
		List<Event> calendar = new List<Event>();
		const int MAX = 0x7FFFFFFF;
		readonly int TTR;
		static readonly ILog log=LogManager.GetLogger(typeof(Calendar));
		/**
		 * Create new empty calendar
		 */
		public Calendar(int time_to_run)
		{
			this.TTR=time_to_run;
			calendar.Clear();
		}
		/**
		 * @return is calendar empty?
		 */
		public bool Empty
		{
			get{
				return calendar.Count == 0;
			}
		}

		/**
		 * Returns first event from calendar and removes it from calendar
		 * @return first event from calendar
		 */
		public Event First()
		{
			if (Empty) return null;
			int t = MAX;
			Event first = null;
			foreach (Event e in calendar)
				if (e.when < t) { first = e; t = e.when; }
			calendar.Remove(first);
			return first;
		}
		/**
		 * Schedules an event by adding it to calendar if the event is to happen within simulation time
		 * @param e Event to schedule
		 */ 
		public void Schedule(Event e)
		{
			if ((e.when >= 0) && (e.when <= TTR))
				calendar.Add (e);
			else
				log.Debug ("Not scheduling event for time " + e.when);
		}

		/**
		 * Removes process from calendar
		 * @param p process
		 */
		public void Remove(Process p)
		{
			foreach (Event u in calendar)
				if (u.who == p) { calendar.Remove(u); break; }
		}
	}
}

