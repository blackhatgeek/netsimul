using System;

namespace NetTrafficSimulator
{
	/**
	 * Information about data on the network
	 */
	public class Packet
	{
		private readonly int source, destination;
		private readonly decimal size;
		private int hopcounter;

		/**
		 * Creates a packet to travel from the source address specified to the destination address specified
		 * @param source address of the source node
		 * @param destination address of the destination node
		 * @param size data size
		 */
		public Packet (int source,int destination,decimal size)
		{
			this.source = source;
			this.destination = destination;
			this.size = size;
			this.hopcounter = 0;
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
	}
}

