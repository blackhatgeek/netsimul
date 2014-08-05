using System;
using System.Collections.Generic;

namespace MFF_NPRG031
{
	/**
	 * Calendar holds information about Events
	 */
	public class Calendar
	{
		List<Event> calendar = new List<Event>();
		const int MAX = 0x7FFFFFFF;
		public Calendar()
		{
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
		 * Schedules an event by adding it to calendar
		 * @param e Event to schedule
		 */ 
		public void Schedule(Event e)
		{
			calendar.Add(e);
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

		/**
		 * Returns inner calendar for testing
		 * @return list of events
		 */ 
		public List<Event> GetK(){
			return calendar;
		}
	}
}

