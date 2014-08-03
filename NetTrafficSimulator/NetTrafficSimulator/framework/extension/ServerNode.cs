using System;
using System.Collections.Generic;

namespace NetTrafficSimulator
{
	public class ServerNode:Node,IAddressable
	{
		private readonly int address;
		private int time_waited,process;
		private Link link;
		/**
		 * Creates a ServerNode with given name and address
		 */
		public ServerNode (String name,int address):base(name)
		{
			this.link=null;
			this.address = address;
			this.time_waited = 0;
			this.process = 0;
		}

		/*
		 * Link connecting the ServerNode to the rest of the network
		 */
		public Link Link{
			get{
				return this.link;
			}set{
				this.link = value;
			}
		}

		/*
		 * Address of the ServerNode
		 */
		public int Address{
			get{
				return this.address;
			}
		}

		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			if (state.Actual == MFF_NPRG031.State.state.RECEIVE) {
				int t = wait_time ();
				this.time_waited += t;
				this.process++;
				sendResponse (generateResponse (state.Data), model.Time+t);
			}
			else
				throw new ArgumentException ("[ServerNode "+Name+"] Neplatny stav: "+state);
		}

		/**
		 * Creates a new Packet for sender of the request
		 * @return Packet from this node to source
		 */
		private Packet generateResponse(Packet p){
			return new Packet (this.Address, p.Source);
		}

		/**
		 * Generates a wait time between receiving a packet and sending a response
		 * @return 1
		 */
		private int wait_time(){
			return 1;
		}

		/**
		 * Send generated response at given time
		 * @param p Generated new packet
		 * @param time When to send
		 * @throws ArgumentException Provided packet's source is not this node
		 * @throws InvalidOperationException link not connected
		 */
		private void sendResponse(Packet p,int time){
			if (link != null) {
				if (p.Source == this.Address)
					this.link.Carry (p, this, this.link.GetPartner (this));
				else
					throw new ArgumentException ("[Node " + Name + "] Odchozi packet nepochazi z tohoto node");
			}
			else
				throw new InvalidOperationException ("[Node " + Name + "] Link neni pripojen");
		}

		/**
		 * Empty
		 */
		public override void Run (MFF_NPRG031.Model m)
		{
		}

		//results
		/**
		 * Amount of time spent waiting
		 * Sum of wait_time() provided values counted in ProcessEvent
		 */
		public int TimeWaited{
			get{
				return time_waited;
			}
		}
		/*
		 * Time spend waiting relative to time to run the simulation
		 * @param model Framework model
		 * @return TimeWait to current time provided by model ratio in percents
		 */
		public decimal GetPercentageTimeIdle(MFF_NPRG031.Model model){
			return time_waited / model.Time * 100;
		}

		/**
		 * TimeWaited divided by PacketsProcessed gives us average time ServerNode spent waiting before a response packet was sent
		 */
		public decimal AverageWaitTime{
			get{
				return time_waited / process;
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
	}
}

