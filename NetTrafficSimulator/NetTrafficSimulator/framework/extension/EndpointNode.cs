using System;

namespace NetTrafficSimulator
{
	/**
	 * Abstract EndpointNode. EndpointNode is node where packets are generated or where packets are to flow
	 */
	public abstract class EndpointNode:Node,IAddressable
	{
		private readonly int address;
		public EndpointNode(String name,int address):base(name){
			this.address=address;
		}

		/**
		 * Network address of the node
		 */
		public int Address{
			get{
				return this.address;
			}
		}
	}
}

