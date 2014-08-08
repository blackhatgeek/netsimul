using System;

namespace NetTrafficSimulator
{
	/**
	 * Simulation model holds parameters of a simulation
	 */
	public class SimulationModel
	{
		/**
		 * Time to run simulation
		 */
		private int time;

		private const int DEFAULT_MAX_HOP = 30;
		private int maxHop,eventCount;

		private readonly int events;

		public struct Event{
			public Event(string node1,string node2,int when,decimal size){
				this.node1=node1;
				this.node2=node2;
				this.when=when;
				this.size=size;
			}
			public string node1,node2;
			public int when;
			public decimal size;
		}
		private Event[] evs;

		public SimulationModel (int events)
		{
			maxHop = DEFAULT_MAX_HOP;
			if (events >= 0) {
				this.events = events;
				this.evs = new Event[events];
				this.eventCount = 0;
			} else
				throw new ArgumentOutOfRangeException ("Event number must be non negative");
		}
	
		/**
		 * Time to run simulation, can't be negative
		 * @throws ArgumentOutOfRangeException negative time on set
		 */ 
		public int Time{
			get{
				return this.time;
			}
			set{
				if (value >= 0)
					this.time = value;
				else
					throw new ArgumentOutOfRangeException ("[SimulationModel] Negative time");
			}
		}

		public int MaxHop{
			get{
				return maxHop;
			}
			set{
				if (value > 0)
					this.maxHop = value;
				else
					throw new ArgumentException ("Max hop must be positive");
			}
		}

		public void SetEvent(string node1,string node2,int when,decimal size){
			if (eventCount < events) {
				evs [eventCount] = new Event (node1, node2, when, size);
				eventCount++;
			}else throw new ArgumentException("Event counter overflow");
		}

		public Event[] GetEvents(){
			return evs;
		}
	}
}

