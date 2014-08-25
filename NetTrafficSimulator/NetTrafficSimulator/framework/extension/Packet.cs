using System;
using System.Collections.Generic;

namespace NetTrafficSimulator
{
	/**
	 * Information about data on the network
	 */
	public class Packet
	{
		private readonly int source, destination;
		private readonly decimal size;
		private readonly bool trace;
		private LinkedList<KeyValuePair<string,int>> journey;
		private int hopcounter;

		/**
		 * Creates a packet to travel from the source address specified to the destination address specified which is not to be traced
		 * @param source address of the source node
		 * @param destination address of the destination node
		 * @param size data size
		 */
		public Packet (int source,int destination,decimal size):this(source,destination,size,false)
		{
		}

		/**
		 * Creates a packet to travel from the source address specified to the destination address specified
		 * @param source address of the source node
		 * @param destination address of the destination node
		 * @param size data size
		 * @param trace trace this packet?
		 */
		public Packet (int source, int destination,decimal size,bool trace){
			this.trace = trace;
			this.source = source;
			this.destination = destination;
			this.size = size;
			this.hopcounter = 0;
			this.trace = false;
			if(trace)
				this.journey = new LinkedList<KeyValuePair<string,int>> ();
		}

		/**
		 * Address of the source node
		 */
		public int Source{
			get{
				return this.source;
			}
		}

		/**
		 * Address of the destination node
		 */
		public int Destination{
			get{
				return this.destination;
			}
		}

		/**
		 * Packet data size
		 */
		public decimal Size {
			get {
				return this.size;
			}
		}

		/**
		 * Hop counter value - through how many NetworkNodes did the packet travelled
		 */
		public int Hop{
			get{
				return this.hopcounter;
			}
		}

		/**
		 * Increment hop counter value by one
		 */
		public void HopInc(){
			this.hopcounter++;
		}

		/**
		 * Is packet traced - for traced packet we keep a trace - list of nodes the packet went through
		 */
		public bool Traced{
			get{
				return this.trace;
			}
		}

		/**
		 * Add node packet went through to trace records
		 * @param n node
		 * @param time time
		 */
		public void SetNodePassedThrough(Node n,int time){
			if (this.trace)
				this.journey.AddLast (new KeyValuePair<string, int>(n.Name,time));
			else
				throw new ArgumentException ("Packet not set as traced!");
		}

		/**
		 * Packet trace is list of pair: node name, time
		 */
		public LinkedList<KeyValuePair<string,int>> Trace{
			get{
				return this.journey;
			}
		}
	}
}

