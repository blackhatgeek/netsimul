using System;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * Server node is receiver of data sent by EndNode. When data arrives it sends back a response packet of same size
	 */
	public class ServerNode:EndpointNode
	{
		private static readonly ILog log=LogManager.GetLogger(typeof(ServerNode));
		private int process,last_send;
		/**
		 * Creates a ServerNode with given name and address
		 */
		public ServerNode (String name,int address):base(name,address)
		{
			this.last_send = -1;
			this.process = 0;
			this.time_wait = 0;
		}

		/**
		 * On RECEIVE schedule sending of generated response after waiting for generated delay time
		 * On SEND passes generated packet to link, if possible
		 * @throws ArgumentException Provided packet's source is not this node on SEND or state is invalid
		 * @throws InvalidOperationException link not connected on SEND
		 */
		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			if (state.Actual == MFF_NPRG031.State.state.RECEIVE) {
				if (state.Data is RoutingMessage)
					log.Debug ("(" + Name + ") Received routing message, never mind");
				else {
					if (state.Data.Destination == this.Address) {
						int t = wait_time (model.Time,model);
						time_wait += t;
						this.process++;
						log.Debug ("(" + Name + ") Received at time " + model.Time + " waiting for " + t + ", total waited " + time_wait + " total processed " + process + " sending at " + (model.Time + t));
						sendResponse (generateResponse (state.Data), model.Time + t, model);
					} else {
						malreceived++;
					}
				}
			} else if (state.Actual == MFF_NPRG031.State.state.SEND) {
				if (Link != null) {
					if (state.Data.Source == this.Address)
						this.Link.Carry (state.Data, this, this.Link.GetPartner (this));
					else
						throw new ArgumentException ("[Node " + Name + "] Odchozi packet nepochazi z tohoto node");
				}
				else
					throw new InvalidOperationException ("[Node " + Name + "] Link neni pripojen");
			}
			else
				throw new ArgumentException ("[ServerNode "+Name+"] Neplatny stav: "+state);
			base.ProcessEvent (state, model);
		}

		/**
		 * Creates a new Packet for sender of the request
		 * @return Packet from this node to source
		 */
		private Packet generateResponse(Packet p){
			return new Packet (this.Address, p.Source,p.Size);
		}

		/**
		 * Send generated response at given time
		 * @param p Generated new packet
		 * @param time When to send
		 * @param m framework model
		 */
		private void sendResponse(Packet p,int time,MFF_NPRG031.Model m){
			this.Schedule (m.K, new MFF_NPRG031.State (MFF_NPRG031.State.state.SEND, p), time);
		}

		//results
		/**
		 * TimeWaited divided by PacketsProcessed gives us average time ServerNode spent waiting before a response packet was sent
		 */
		public decimal AverageWaitTime{
			get{
				if (process != 0)
					return (decimal)time_wait / process*1.0m;
				else
					return 0.0m;
			}
		}

		/**
		 * Amount of packets processed
		 */
		public int PacketsProcessed{
			get{
				return process;
			}
		}

		/**
		 * Generates a wait time
		 * @return 1
		 */
		protected int wait_time(int max,MFF_NPRG031.Model m){
			int r = new Random ().Next (max);
			int wait =  r>0?r:1;
			log.Debug ("Random wait:" + wait);
			if (last_send != -1) {
				int shift = (last_send - m.Time) > 0 ? (last_send - m.Time) : 0;
				log.Debug ("Last send:" + last_send + "\tTime:" + m.Time + "\tSpan" + (shift) + "\tWait shift:" + (wait + shift));
				wait += shift;//kolik casu zbyva do posledniho odeslani + novy cekaci cas = celkovy cas nutny na cekani
			}
			last_send = m.Time + wait;
			return wait;
		}

	}
}

