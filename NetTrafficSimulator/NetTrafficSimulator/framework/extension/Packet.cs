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

		/**
		 * Creates a packet to travel from the source address specified to the destination address specified
		 * @param source address of the source node
		 * @param destination address of the destination node
		 */
		public Packet (int source,int destination,decimal size)
		{
			this.source = source;
			this.destination = destination;
			this.size = size;
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
	}
}

