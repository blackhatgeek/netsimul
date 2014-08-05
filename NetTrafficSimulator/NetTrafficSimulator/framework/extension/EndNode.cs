using System;
using System.Collections.Generic;

namespace NetTrafficSimulator
{
	public class EndNode:EndpointNode
	{
		private int sent, received,malreceived,time_wait,server_node_count;
		private Link link;
		private Random r;

		/**
		 * Creates an EndNode with given name and address
		 */
		public EndNode(String n,int address):base(n,address){
			this.sent = 0;
			this.received = 0;
			this.malreceived = 0;
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

		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			server_node_count = model.Servers.GetLength (0);
			switch (state.Actual) {
			case MFF_NPRG031.State.state.SEND:
				if (state.Data != null)
					send (state.Data.Destination,state.Data.Size);
				else if (server_node_count > 0)
					send (selectDestination (model, server_node_count),selectDataSize());
				//else nejsou k dispozici servery
				else
					send (r.Next (),selectDataSize());
				int t = wait_time ();
				time_wait += t;
				this.Schedule (model.K, new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND), model.Time + t);
				break;
			/*case MFF_NPRG031.State.state.WAIT:
				this.Schedule (model.K, new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND), model.Time + wait_time ());
				break;*/
			case MFF_NPRG031.State.state.RECEIVE:
				if (state.Data.Destination == this.Address)
					received++;
				else
					malreceived++;
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
		 * @param destination where to send packet
		 * @param size what size of data to send
		 * @throws InvalidOperationException if link is not connected
		 */
		private void send(int destination,decimal size){
			//must send to existing node!!
			if (link != null) {
				this.link.Carry (new Packet (Address,destination,size), this, this.link.GetPartner (this));
				sent++;
			} else
				throw new InvalidOperationException ("[Node " + Name + "] Link neni pripojen");
		}

		/**
		 * Randomly chooses destination for new packet
		 * @param m framework_model
		 * @param SNC server node counter
		 * @return server's address
		 */ 
		private int selectDestination(MFF_NPRG031.Model m,int SNC){
			return m.Servers[r.Next (SNC-1)].Address;
		}

		/**
		 * Randomly chooses size for new packet
		 * @return packet size
		 */
		private decimal selectDataSize(){
			return (decimal)(r.Next(20)*r.NextDouble ());
		}

		/**
		 * Generates a wait time after sending
		 * @return 5
		 */
		private int wait_time(){
			return 5;
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

