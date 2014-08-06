using System;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * NetworkNode is routing packets through the network
	 */
	public class NetworkNode:Node
	{
		static readonly ILog log = LogManager.GetLogger (typeof(NetworkNode));
		Link[] interfaces;
		Dictionary<Packet,Link> schedule;
		Dictionary<int,int> route;
		int[] interface_use_count;
		int interfaces_count,interfaces_used,processed,time_wait,last_process;
		int delay;

		/**
		 * Creates a new network node with given name and interfaces count
		 * The delay is set fixed as 1
		 * @param name Human readable node name
		 * @param interfaces_count How many ports does the network node have
		 * @throws ArgumentException on negative interfaces_count
		 */
		public NetworkNode (String name,int interfaces_count):base(name)
		{
			if (interfaces_count >= 0) {
				this.interfaces = new Link[interfaces_count];
				this.interface_use_count = new int[interfaces_count];
				this.interfaces_count = interfaces_count;
				this.interfaces_used = 0;
				this.delay = 1;
				this.processed = 0;
				this.time_wait = 0;
				this.schedule = new Dictionary<Packet, Link> ();
				this.route=new Dictionary<int,int>();
				this.last_process = 0;
			} else
				throw new ArgumentException ("[NetworkNode] Negative interface count");
		}

		/**
		 * Amount of ports of the NetworkNode
		 */
		public int Interfaces{
			get{
				return interfaces.Length;
			}
		}

		/**
		 * If possible, note a new link in use on first interface available
		 * @param l Link to connect
		 * @throws ArgumentException if no port is available
		 * @throws ArgumentNullException on l null
		 */
		public void ConnectLink(Link l){
			if (interfaces_used < interfaces_count) {
				if (l != null) {
					try{
						Node n=l.GetPartner(this);
						interfaces [interfaces_used] = l;
						interfaces_used++;
						if(n is EndpointNode){
							//if(route.ContainsKey((n as EndpointNode).Address)
							//TODO: rozhodnout o lepsi trase ale druha route musi byt v zaloze!!!
							route.Add((n as EndpointNode).Address,interfaces_used);
						}
					}catch(ArgumentException){
						throw new ArgumentException ("Link not connected to this NetworkNode");
					}

				} else
					throw new ArgumentNullException ("Link null");
			} else
				throw new ArgumentException ("No port available");
		}

		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			switch (state.Actual) {
			case MFF_NPRG031.State.state.RECEIVE:
				this.time_wait += model.Time - last_process;
				processed++;
				log.Debug ("("+Name+") Receiving. Time: "+model.Time+" Last process:"+last_process+" Waited:"+time_wait+" Processed:"+processed);
				this.last_process = model.Time;
				
				if (state.Data == null)
					throw new ArgumentNullException ("Packet null");
				scheduleForward (state.Data, selectDestination (state.Data), model);
				break;
			case MFF_NPRG031.State.state.SEND:
				log.Debug ("("+Name+") Sending.");
				Packet p = state.Data;
				Link l;
				if (schedule.TryGetValue (p, out l))
					l.Carry (p, this, l.GetPartner (this));
				else
					throw new ArgumentException ("("+Name+") Packet was not scheduled for sending - missing record for link to use");
				break;
			default:
				throw new ArgumentException ("[NetworkNode "+Name+"] Neplatny stav: "+state);
			}
		}

		/**
		 * Empty
		 */
		public override void Run (MFF_NPRG031.Model m)
		{
		}

		/**
		 * If there's a link connected, forward a packet:
		 * If destination is an EndpointNode directly connected to the NetworkNode, use that connection
		 * Otherwise use the first interface as default route
		 * @param p Packet to route
		 * @return which way to go
		 * @throws InvalidOperationException No link connected
		 */
		private Link selectDestination(Packet p){
			if (interfaces_used > 0) {
				int l;
				//if we are delivering to endpoint node, deliver directly
				//otherwise use "default route"
				if (route.TryGetValue (p.Destination, out l)) {
					//TODO: pokud ale link neni active, ale jsou k dispozici dalsi cesty ...
					interface_use_count [l-1]++;
					return interfaces [l-1];
				} else {
					interface_use_count [0]++;
					return interfaces [0];
				}
			}
			else
				throw new InvalidOperationException ("No link connected");
		}

		/**
		 * Given the packet and the link to use, schedule SEND for the link with packet included at time T+delay
		 * @param p Packet to forward
		 * @param l Link to use (result of selectDestination)
		 * @param model the Model
		 */
		private void scheduleForward(Packet p,Link l,MFF_NPRG031.Model model){
			log.Debug ("("+Name+") Routing via link " + l + " at " + (model.Time + delay));
			if (schedule.ContainsKey (p))
				throw new InvalidOperationException ("Same packet to schedule twice");
			this.schedule.Add (p, l);
			this.Schedule (model.K, new MFF_NPRG031.State (MFF_NPRG031.State.state.SEND, p), model.Time + delay);
			//l.Schedule(model.K,new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND,p),model.Time+delay);
		}

		//results
		//TODO: vybrat nekolik nejcasteji vybranych interfacu
		//TODO: delay je konstantni - bude-li random, tak prumerny
		/**
		 * Amount of packets processed
		 */
		public int PacketsProcessed{
			get{
				return processed;
			}
		}
		/**
		 * Amount of time passed between two ProcessEvent calls - summed
		 */
		public int TimeWaited{
			get{
				return time_wait;
			}
		}
		/**
		 * How much time was the network node idle
		 */
		public decimal GetPercentageTimeIdle(MFF_NPRG031.Model model){
			if (model.Time != 0)
				return time_wait / model.Time;
			else
				return 100;
		}
		/**
		 * Average wait time
		 */
		public decimal AverageWaitTime {
			get {
				if (processed != 0)
					return time_wait / processed;
				else
					return 0;

			}
		}
	}
}

