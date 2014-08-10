using System;

namespace MFF_NPRG031
{
	/**
	 * Abstract process of discrete simulation
	 */
	public abstract class Process
	{
		/**
		 * Schedule an event to happen in future
		 * @param calendar Calendar to schedule the event
		 * @param what In what state the process will be
		 * @param when When the event is to happen
		 */
		public void Schedule(Calendar calendar, State what, int when)
		{
			calendar.Schedule(new Event(this, what, when));
		}
		/**
		 * Remove the process from calendar
		 * @param calendar Calendar to remove process from
		 */
		public void ZrusPlan(Calendar calendar)
		{
			calendar.Remove(this);
		}

		/**
		 * Abstract method ProcessEvent for process to react on event hapened
		 * @param state In what state the process is now
		 * @param model Our simulation model
		 */
		abstract public void ProcessEvent(State state, Model model);
	}
}

