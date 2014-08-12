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
		const int update=3,flush=6,expiry=3;
		static readonly ILog log = LogManager.GetLogger (typeof(NetworkNode));
		readonly int max;//,update_timer=10;//TODO
		Link[] interfaces;
		Dictionary<Packet,Link> schedule;
		RoutingTable rt;
		int[] interface_use_count;
		int interfaces_count,interfaces_used,processed,time_wait,last_process,dropped,rm_received,rm_sent;
		int delay;//, scheduled_update;
		//private Timer update, invalid;

		/**
		 * Creates a new network node with given name and interfaces count
		 * The delay is set fixed as 1
		 * @param name Human readable node name
		 * @param interfaces_count How many ports does the network node have
		 * @param max Max hop count for a packet
		 * @param m Framework model
		 * @throws ArgumentException on negative interfaces_count
		 */
		public NetworkNode (String name,int interfaces_count,int max,MFF_NPRG031.Model m):base(name)
		{
			if (m == null)
				throw new ArgumentNullException ("Model null");
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
				this.rt = new RoutingTable (flush,expiry,max,m);
				this.last_process = 0;
				this.dropped = 0;

				//nastavit se do stavu UPDATE_TIMER
				//poslat REQUESTY vsem okolo


				//this.scheduled_update = -1;
				//this.update = new Timer (this);
				//this.invalid = new Timer (this);
				//this.update.Schedule (model.K, new MFF_NPRG031.State (MFF_NPRG031.State.state.TIMER), model.Time); - SIMU CONTROL
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
		 * If possible add link and appropriate record to routing table
		 * @param l Link to connect
		 * @param model Framework model
		 * @throws ArgumentException if no port is available
		 * @throws ArgumentNullException on l null
		 */
		public void ConnectLink(Link l,MFF_NPRG031.Model model){
			log.Debug ("Connect link");
			if (interfaces_used < interfaces_count) {
				if (l != null) {
					if (l.ConnectedTo (this)) {
						interfaces [interfaces_used] = l;
						interfaces_used++;
						Node n = l.GetPartner (this);
						if(n==null)
							throw new ArgumentNullException ("Node null");
						if (n is EndpointNode) {
							EndpointNode en = n as EndpointNode;
							if (en == null)
								throw new ArgumentNullException ("Endpoint node null");
							rt.SetRecord (en.Address, l, 1);
						}
					}
					else
						throw new ArgumentException ("Link not connected to this NetworkNode");
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
						updateRT (r.Table,r.Link);
					}
					else
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
				if (p != null) {
					if (schedule.TryGetValue (p, out l)) {
						if (l != null) {
							if (p is RoutingMessage) {
								log.Debug ("Sending routing message");
								processed++;
								rm_sent++;
							}
							l.Carry (p, this, l.GetPartner (this));
						} else
							throw new ArgumentNullException ("(" + Name + ") Link null");
					} else
						throw new ArgumentException ("(" + Name + ") Packet was not scheduled for sending - missing record for link to use");
				} else
					throw new ArgumentNullException ("(" + Name + ") Packet null");
				break;
			case MFF_NPRG031.State.state.UPDATE_TIMER:
				//send responses around
				sendResponse (model);
				//schedule timer
				this.Schedule (model.K, new MFF_NPRG031.State (MFF_NPRG031.State.state.UPDATE_TIMER), model.Time + update);
				break;
			default:
				throw new ArgumentException ("[NetworkNode "+Name+"] Neplatny stav: "+state);
			}
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
			if (p is RoutingMessage) {
				log.Error ("Link for routing message");
				throw new Exception ();
				return (p as RoutingMessage).Link;
			} else {
				log.Debug ("Link for destination: " + p.Destination);
				Link link = rt.GetLinkForAddr (p.Destination);
				if (link != null) {
					for (int i=0; i<interfaces_used; i++) {
						if (link.Equals (interfaces [i])) {
							interface_use_count [i]++;
							return link;
						}
					}
					throw new InvalidOperationException ("Routing through invalid link");
				} else {
					log.Warn ("No link for " + p.Destination);
					return null;
				}
			}
			//throw new InvalidOperationException ("Routing through invalid link");
		}

		/**
		 * Given the packet and the link to use, schedule SEND for the link with packet included at time T+delay
		 * @param p Packet to forward
		 * @param l Link to use (result of selectDestination)
		 * @param model the Model
		 */
		private void scheduleForward(Packet p,Link l,MFF_NPRG031.Model model){
			if (l != null) {
				log.Debug ("(" + Name + ") Routing via link " + l + " at " + (model.Time + delay));
				if (schedule.ContainsKey (p))
					throw new InvalidOperationException ("Same packet to schedule twice");
				this.schedule.Add (p, l);
				this.Schedule (model.K, new MFF_NPRG031.State (MFF_NPRG031.State.state.SEND, p), model.Time + delay);
			} else {
				log.Warn ("(" + Name + ") No link to " + p.Destination + " packet dropped");
				dropped++;
			}
		}

		//results
		//TODO: vybrat nekolik nejcasteji vybranych interfacu
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
					return 0.0m;

			}
		}
		/**
		 * Amount of received routing messages
		 */
		public int RoutingMessagesReceived{
			get{
				return rm_received;
			}
		}

		/**
		 * Amount of sent routing messages
		 */
		public int RoutingMessagesSent{
			get{
				return rm_sent;
			}
		}

		/**
		 * What portion of processed packages took routing messages
		 */
		public decimal RoutingMessagesPercentageProcessed{
			get{
				if (processed != 0)
					return (rm_sent + rm_received) / processed*100.0m;
				else
					return 0.0m;
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
					return 0.0m;
			}
		}

		/**
		 * Send requests for routing table
		 */
		public void SendRequest(MFF_NPRG031.Model model){
			for (int i=0; i<interfaces_used; i++) {
				Link l = interfaces [i];
				log.Debug ("(" + Name + ") Scheduling request to " + l.GetPartner (this).Name + " via " + l.Name + " at " + (model.Time + 1));
				scheduleForward (new Request (l), l, model);
			}
		}

		/**
 		 * Send response - our routing table
 		 * @param model Framework model
 		 * @throws ArgmentNullException Model null or interfaces null or RoutingTable null
 		 * @throws Exception Link null
		 */
		private void sendResponse(MFF_NPRG031.Model model){
			if (model == null)
				throw new ArgumentNullException ("Model null ( net. node: " + Name + ")");
			if (interfaces == null)
				throw new ArgumentNullException ("Interfaces null");
			if (rt == null)
				throw new ArgumentNullException ("RT null");
			foreach (Link l in interfaces) {
				if (l != null) {
					log.Debug ("(" + Name + ") Sending response to " + l.GetPartner (this).Name + " via " + l.Name);
					scheduleForward (new Response (l, rt), l, model);
				} else
					throw new Exception ("Link null in send response");
			}
		}

		/**
		 * Send response to particular request
		 * @param l Link to use
		 * @param model Framwork model
		 */
		private void sendResponse(Link l,MFF_NPRG031.Model model){
			log.Debug ("(" + Name + ") Sending response to " + l.GetPartner (this).Name + " via " + l.Name);
			scheduleForward (new Response (l,rt), l, model);
		}


		/**
		 * Check request's link is from our set
		 * @param r RoutingMessage received
		 * @return Sender of r is connected to this NetworkNode
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
		 * @param received Received routing table
		 * @param direction Who delivered the routing table
		 */
		private void updateRT(RoutingTable received,Link direction){
			foreach (Record r in received.GetRecords()) {
				rt.SetRecord (new Record(r.Address,direction,r.Metric+1,this.max,rt));
			}
		}

	}
}

