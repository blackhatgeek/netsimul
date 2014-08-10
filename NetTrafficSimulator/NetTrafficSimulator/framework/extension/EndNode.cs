using System;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * EndNode generates requests to ServerNodes
	 */
	public class EndNode:EndpointNode
	{
		private int sent, received, last_send;
		private decimal psizesum;
		private static readonly ILog log=LogManager.GetLogger(typeof(EndNode));

		/**
		 * Creates an EndNode with given name and address
		 */
		public EndNode(String n,int address):base(n,address){
			this.sent = 0;
			this.received = 0;
			this.malreceived = 0;
			this.time_wait = 0;
			this.last_send = 0;
			this.psizesum = 0;
		}

		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			switch (state.Actual) {
			case MFF_NPRG031.State.state.SEND:
				time_wait += (model.Time - last_send);
				last_send = model.Time;
				log.Debug ("(" + Name + ") Sending at " + model.Time + " link " + this.Link);
				if (state.Data != null)//preddefinovany event
					send (state.Data);
				else
					throw new ArgumentException ("End node " + Name + " was scheduled with no data to send at "+model.Time);
				break;
			case MFF_NPRG031.State.state.RECEIVE:
				if (state.Data is RoutingMessage)
					log.Debug ("(" + Name + ") Received routing message, never mind");
				else {
					if (state.Data.Destination == this.Address) {
						received++;
						log.Debug ("(" + Name + ") Received at " + model.Time);
					} else {
						malreceived++;
						log.Debug ("(" + Name + ") Received incorrectly at " + model.Time + ": From " + state.Data.Source + " To:" + state.Data.Destination + " Size:" + state.Data.Size);
					}
				}
				break;
			default:
				throw new ArgumentException ("[EndNode "+Name+"] Neplatny stav: "+state);
			}
		}

		/**
		 * Attept to post a new packet to the link, if such exist
		 * @param destination where to send packet
		 * @param size what size of data to send
		 * @throws InvalidOperationException if link is not connected
		 */
		private void send(Packet p){
			//must send to existing node!!
			if (Link != null) {
				psizesum += p.Size;
				this.Link.Carry (p, this, this.Link.GetPartner (this));
				sent++;
			} else
				throw new InvalidOperationException ("[Node " + Name + "] Link neni pripojen");
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
		 * TimeWait divided by PacketsSent gives us average time EndNode spent waiting after a packet was sent
		 */
		public decimal AverageWaitTime{
			get{
				if (sent != 0)
					return (decimal)time_wait / sent;
				else
					return 0;
			}
		}

		public decimal AveragePacketSize{
			get{
				if (sent!=0)
					return psizesum / sent;
				return 0;
			}
		}
	}
}