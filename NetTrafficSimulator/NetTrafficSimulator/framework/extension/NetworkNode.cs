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
		readonly int max;
		Link[] interfaces;
		Dictionary<Packet,Link> schedule;
		//Dictionary<int,int> route;
		RoutingTable rt;
		int[] interface_use_count;
		int interfaces_count,interfaces_used,processed,time_wait,last_process,dropped,rm_received,rm_sent;
		int delay;

		/**
		 * Creates a new network node with given name and interfaces count
		 * The delay is set fixed as 1
		 * @param name Human readable node name
		 * @param interfaces_count How many ports does the network node have
		 * @throws ArgumentException on negative interfaces_count
		 */
		public NetworkNode (String name,int interfaces_count,int max):base(name)
		{
			this.max = max;
			if (interfaces_count >= 0) {
				this.interfaces = new Link[interfaces_count];
				this.interface_use_count = new int[interfaces_count];
				this.interfaces_count = interfaces_count;
				this.interfaces_used = 0;
				this.delay = 1;
				this.processed = 0;
				this.time_wait = 0;
				this.schedule = new Dictionary<Packet, Link> ();
				//this.route=new Dictionary<int,int>();
				rt = new RoutingTable (max);
				this.last_process = 0;
				this.dropped = 0;
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
							log.Debug("Set record to routing table: "+(n as EndpointNode).Address+" via "+l.Name+" (1)");
							rt.SetRecord((n as EndpointNode).Address,l,1);
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
				log.Debug ("(" + Name + ") Receiving. Time: " + model.Time + " Last process:" + last_process + " Waited:" + time_wait + " Processed:" + processed);
				this.last_process = model.Time;

				if (state.Data == null)
					throw new ArgumentNullException ("Packet null");
				state.Data.HopInc ();
				log.Debug ("(" + Name + ") Data hop counter: " + state.Data.Hop + " max:" + max);
				if (state.Data.Hop <= max) {
					if (state.Data is Request) {
						rm_received++;
						Request r = state.Data as Request;
						log.Debug ("(" + Name + ") Received routing message - request");
						if (!verifyRM (r))
							throw new Exception ("Invalid request - link not present in interfaces: " + r.Link.Name);
						sendResponse (r.Link, model);
					} else if (state.Data is Response) {
						rm_received++;
						log.Debug ("(" + Name + ") Received routing message - response");
						Response r = state.Data as Response;
						if (!verifyRM (r))
							throw new Exception ("Invalid response - link not present in interfaces: " + r.Link.Name);
						updateRT (r.Link,model,r.Table);
					}
					scheduleForward (state.Data, selectDestination (state.Data), model);
				} else {
					dropped++;
					log.Debug ("Hop counter over max .. packet dropped");
				}
				break;
			case MFF_NPRG031.State.state.SEND:
				log.Debug ("(" + Name + ") Sending.");
				Packet p = state.Data;
				Link l;
				if (schedule.TryGetValue (p, out l)) {
					if (p is RoutingMessage) {
						processed++;
						rm_sent++;
					}
					l.Carry (p, this, l.GetPartner (this));
				}else
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
			log.Debug ("Link for destination: " + p.Destination);
			Link link=rt.GetLinkForAddress (p.Destination);
			for (int i=0; i<interfaces_used; i++) {
				if (link.Equals (interfaces [i])) {
					interface_use_count [i]++;
					return link;
				}
			}
			throw new InvalidOperationException ("Routing through invalid link");
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
				return time_wait / model.Time*100.0m;
			else
				return 100.0m;
		}
		/**
		 * Average wait time
		 */
		public decimal AverageWaitTime {
			get {
				if (processed != 0)
					return (decimal)time_wait / processed;
				else
					return 0;

			}
		}
		public int RoutingMessagesReceived{
			get{
				return rm_received;
			}
		}
		public int RoutingMessagesSent{
			get{
				return rm_sent;
			}
		}
		public decimal RoutingMessagesPercentageProcessed{
			get{
				if (processed != 0)
					return (rm_sent + rm_received) / processed*100.0m;
				else
					return 0;
			}
		}

		/**
		 * How many packets were dropped due to hop count
		 */ 
		public int PacketsDropped{
			get{
				return dropped;
			}
		}

		/**
		 * How many packets were dropped due to hop count relative to packets processed
		 */
		public decimal PercentagePacketsDropped{
			get{
				int i = processed - rm_sent;
				if (i != 0)
					return (decimal)dropped / i*100.0m;
				else
					return 0;
			}
		}

		/**
		 * When link switches state, it notifies connected NetworkNodes using this method
		 * If link became active: send request for routing table to neighbour
		 * If link became passive: remove related elements from routing table and send updated table to neighbours
		 */ 
		public void LinkSwitchTrigger(Link l,MFF_NPRG031.Model model){
			if (l.Active) {
				//send request
				sendRequest (l,model);
			} else {
				//update table
				rt.RemoveLink (l);
				//send updated table
				sendResponse (l, model);

			}
		}

		/**
		 * Send request for routing table
		 */ 
		private void sendRequest(Link l,MFF_NPRG031.Model model){
			log.Debug ("(" + Name + ") Scheduling request to " + l.GetPartner (this).Name + " via " + l.Name+" at "+(model.Time+1));
			scheduleForward (new Request (l), l, model);
		}

		/**
		 * Send response - our routing table
		 */ 
		private void sendResponse(Link link,MFF_NPRG031.Model model){
			foreach (Link l in interfaces) {
				if (!l.Equals (link)) {
					log.Debug ("(" + Name + ") Sending response to " + l.GetPartner (this).Name + " via " + l.Name);
					scheduleForward (new Response (l,rt), l, model);
				}
			}
		}

		/**
		 * Check request's link is from our set
		 */
		private bool verifyRM(RoutingMessage r){
			bool ok = false;
			foreach(Link l in interfaces){
				if (l.Equals (r.Link)) {
					ok = true;
					break;
				}
			}
			return ok;
		}

		/**
		 * Merge received routing table into our
		 */
		private void updateRT(Link l,MFF_NPRG031.Model m,RoutingTable received){
			bool change=false;
			foreach(RoutingTable.Record r in received.GenerateRecordTable()){
				change=change | rt.SetRecord (r.Addr, r.Link, r.Metric+1);
			}
			if (change)
				sendResponse (l, m);
		}
	}
}

