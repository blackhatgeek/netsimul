using System;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * Information about data on the network
	 */
	public class Packet
	{
		static readonly ILog log = LogManager.GetLogger (typeof(Packet));
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
			log.Debug ("New packet from " + source + " to " + destination + " of size " + size);
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

		public int Hop{
			get{
				return this.hopcounter;
			}
		}

		public void HopInc(){
			this.hopcounter++;
		}
	}
}

