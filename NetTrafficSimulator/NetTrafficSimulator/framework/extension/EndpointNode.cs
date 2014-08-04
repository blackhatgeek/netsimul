using System;

namespace NetTrafficSimulator
{
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

