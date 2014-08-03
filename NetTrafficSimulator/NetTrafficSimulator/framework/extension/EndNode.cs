using System;
using System.Collections.Generic;

namespace NetTrafficSimulator
{
	public class EndNode:Node,IAddressable
	{
		private readonly int address;
		private int sent, received,time_wait;
		private Link link;
		private Random r;

		/**
		 * Creates an EndNode with given name and address
		 */
		public EndNode(String n,int address):base(n){
			this.sent = 0;
			this.received = 0;
			this.address = address;
			this.r = new Random ();
			this.time_wait = 0;
		}
		
		/**
		 * Link connecting the EndNode to the rest of the network
		 */
		public Link Link{
			get{
				return this.link;
			}set{
				this.link = value;
			}
		}

		/**
		 * Network address of the node
		 */
		public int Address{
			get{
				return this.address;
			}
		}

		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			switch (state.Actual) {
			case MFF_NPRG031.State.state.SEND:
				send ();
				int t = wait_time ();
				time_wait += t;
				this.Schedule (model.K, new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND), model.Time + t);
				break;
			/*case MFF_NPRG031.State.state.WAIT:
				this.Schedule (model.K, new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND), model.Time + wait_time ());
				break;*/
			case MFF_NPRG031.State.state.RECEIVE:
				received++;
				break;
			default:
				throw new ArgumentException ("[EndNode "+Name+"] Neplatny stav: "+state);
			}
		}

		public override void Run (MFF_NPRG031.Model m)
		{
			this.Schedule (m.K, new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND), m.Time);
		}

		/**
		 * Attept to post a new packet to the link, if such exist
		 * @throws InvalidOperationException if link is not connected
		 */
		private void send(){
			if (link != null) {
				this.link.Carry (new Packet (address,r.Next()), this, this.link.GetPartner (this));
				sent++;
			} else
				throw new InvalidOperationException ("[Node " + Name + "] Link neni pripojen");
		}

		/**
		 * Generates a wait time after sending
		 * @return 1
		 */
		private int wait_time(){
			return 1;
		}

		//results
		/**
		 * Amount of packets sent
		 */
		public int PacketsSent{
			get{
				return sent;
			}
		}

		/**
		 * Amount of packets received
		 */
		public int PacketsReceived{
			get{
				return received;
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
			return time_wait / model.Time * 100;
		}

		/**
		 * TimeWait divided by PacketsSent gives us average time EndNode spent waiting after a packet was sent
		 */
		public decimal AverageWaitTime{
			get{
				return time_wait / sent;
			}
		}
	}
}

