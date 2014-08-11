using System;

namespace NetTrafficSimulator
{
	/**
	 * Abstract EndpointNode. EndpointNode is node where packets are generated or where packets are to flow
	 */
	public abstract class EndpointNode:Node,IAddressable
	{
		private readonly int address;
		private Link link;
		protected int malreceived,time_wait;
		public EndpointNode(String name,int address):base(name){
			this.address=address;
			this.link = null;
		}

		/**
		 * Network address of the node
		 */
		public int Address{
			get{
				return this.address;
			}
		}

		/*
		 * Link connecting the EndpointNode to the rest of the network
		 */
		public Link Link{
			get{
				return this.link;
			}set{
				this.link = value;
			}
		}

		/**
		 * Amount of packets received, where destination was other than EndNode's address
		 */
		public int PacketsMalreceived{
			get{
				return malreceived;
			}
		}

		/**
		 * Amount of time spend waiting
		 * sum of wait_time() provided values counted in ProcessEvent
		 */
		public int TimeWaited{
			get{
				return time_wait;
			}
		}

		/**
		 * Time spend waiting relative to time to run the simulation
		 * @param model Framework model
		 * @return TimeWait to current time provided by model ratio in percents
		 */
		public decimal GetPercentageTimeIdle(MFF_NPRG031.Model model){
			if (model.Time != 0)
				return (decimal)time_wait / model.Time * 100;
			else
				return 100;
		}
	}
}

