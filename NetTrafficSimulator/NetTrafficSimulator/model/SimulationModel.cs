using System;
using System.Collections.Generic;

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

		private HashSet<string> randomTalkers;

		/**
		 * Information about a simulation event
		 */
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

		/**
		 * Create new simulation model with defined events count
		 */
		public SimulationModel (int events)
		{
			maxHop = DEFAULT_MAX_HOP;
			this.randomTalkers = new HashSet<string> ();
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

		/**
		 * MaxHop for packet
		 * @throws ArgumentException - Max hop must be positive - on set
		 */
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

		/**
		 * Set event in simulation: EndNode sends to ServerNode
		 * @param node1 node
		 * @param node2 node
		 * @param when time
		 * @param size packet size
		 * @throws ArgumentException event counter overflow
		 */
		public void SetEvent(string node1,string node2,int when,decimal size){
			if (eventCount < events) {
				evs [eventCount] = new Event (node1, node2, when, size);
				eventCount++;
			}else throw new ArgumentException("Event counter overflow");
		}

		/**
		 * Returns array of scheduled events
		 * @return events scheduled
		 */
		public Event[] GetEvents(){
			return evs;
		}

		/**
		 * Mark node as random talker - no verifications are done here
		 * @param nodeName node name
		 */
		public void SetRandomTalker(string nodeName){
			randomTalkers.Add (nodeName);
		}

		/**
		 * Is node with the name provided marked as random talker?
		 * @param node node name
		 * @return node is marked as random talker
		 */
		public bool IsRandomTalker(string node){
			return randomTalkers.Contains (node);
		}
	}
}

