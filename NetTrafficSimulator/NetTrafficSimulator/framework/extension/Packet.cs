using System;

namespace NetTrafficSimulator
{
	public class Packet
	{
		private readonly int source, destination;
		/**
		 * Creates a packet to travel from the source address specified to the destination address specified
		 * @param source address of the source node
		 * @param destination address of the destination node
		 */
		public Packet (int source,int destination)
		{
			this.source = source;
			this.destination = destination;
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
	}
}

