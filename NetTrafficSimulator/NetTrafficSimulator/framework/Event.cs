using System;

namespace MFF_NPRG031
{
	/**
	 * Information container about event in discrete simulation
	 */
	public class Event
	{
		/**
		 * Who did the event happened to
		 */
		public Process who;
		/**
		 * What happened
		 */
		public State what;
		/**
		 * When did it happen
		 */
		public int when;

		/**
		 * Create new event
		 * @param who What process to activate
		 * @param what What happens to the process at the given time
		 * @param when When to activate the process
		 */
		public Event(Process who, State what, int when)
		{ this.who = who; this.what = what; this.when = when; }
	}
}

