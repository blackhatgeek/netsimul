using System;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * NetworkNode is routing packets through the network using Routing Information Protocol
	 */
	public class NetworkNode:Node
	{
		readonly int update;
		static readonly ILog log = LogManager.GetLogger (typeof(NetworkNode));
		readonly int max;
		Link[] interfaces;
		Dictionary<Packet,Link> schedule;
		Dictionary<Packet,int> iface_pkt;
		RoutingTable rt;
		int[] iface_psent;
		decimal[] iface_dsent;
		int interfaces_count,interfaces_used,processed,time_wait,last_process,dropped,rm_received,rm_sent;
		decimal dproc;
		int last_send;
		Link def_r;

		/**
		 * Creates a new network node with given name and interfaces count
		 * Timers are set to default values: update = 3, expiry = 3, flush = 6
		 * @param name Human readable node name
		 * @param interfaces_count How many ports does the network node have
		 * @param max Max hop count for a packet
		 * @param m Framework model
		 * @throws ArgumentException on negative interfaces_count
		 */
		public NetworkNode (String name,int interfaces_count,int max,MFF_NPRG031.Model m):this(name,interfaces_count,max,m,3,3,6)
		{

		}

		/**
		 * Creates a new network node with given name, interfaces count and timers
		 * @param name Human readable node name
		 * @param interfaces_count How many ports does the network node have
		 * @param max Max hop count for a packet
		 * @param m Framework model
		 * @param update Update timer
		 * @param expiry Expiry timer
		 * @param flush Flush timer
		 * @throws ArgumentException on negative interfaces_count
		 */
		public NetworkNode(String name,int interfaces_count,int max,MFF_NPRG031.Model m,int update,int expiry,int flush):base(name){
			if (m == null)
				throw new ArgumentNullException ("Model null");
			this.max = max;
			if (interfaces_count >= 0) {
				this.interfaces = new Link[interfaces_count];
				this.iface_psent = new int[interfaces_count];
				this.iface_dsent = new decimal[interfaces_count];
				this.interfaces_count = interfaces_count;
				this.interfaces_used = 0;
				this.processed = 0;
				this.time_wait = 0;
				this.schedule = new Dictionary<Packet, Link> ();
				this.iface_pkt = new Dictionary<Packet, int> ();
				this.rt = new RoutingTable (flush,expiry,max,m);
				this.last_process = 0;
				this.dropped = 0;
				this.update = update;
				this.dproc = 0.0m;
				this.last_send = -1;
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
		 * If possible add link and appropriate record to routing table as non-default route
		 * @param l Link to connect
		 * @param model Framework model
		 * @throws ArgumentException if no port is available
		 * @throws ArgumentNullException on l null
		 */
		public void ConnectLink(Link l,MFF_NPRG031.Model model){
			this.ConnectLink(l,model,false);
		}

		/**
		 * If possible add link and appropriate record to routing table
		 * @param l link to connect
		 * @param model Framework model
		 * @param defroute link to be used as default route?
		 * @throws ArgumentException if no port is available, link not connected to this NetworkNode
		 * @throws ArgumentNullException on link name null, node null, endpoint node null, link null
		 */
		public void ConnectLink(Link l,MFF_NPRG031.Model model,bool defroute){
			log.Debug ("Connect link");
			if (interfaces_used < interfaces_count) {
				if (l != null) {
					if (l.ConnectedTo (this)) {
						if (l.Name == null)
							throw new ArgumentNullException ("Link name null");
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
						if (defroute) {
							log.Debug ("Mark default route");
							def_r = l;
						}
					}
					else
						throw new ArgumentException ("Link not connected to this NetworkNode");
				} else
					throw new ArgumentNullException ("Link null");
			} else
				throw new ArgumentException ("No port available");
		}

		/**
		 * On RECEIVE, increase hop counter and either drop or process data - if data is not routing message, schedule forward, 
		 * if data is request, send response, if data is response, update routing table
		 * On SEND pass data to link
		 * On UPDATE_TIMER, send response and schedule update timer
		 * @throws ArgumentNullException packet null on RECEIVE or SEND, link null on SEND
		 * @throws InvalidOperationException invalid request or response as link is not present in interfaces
		 * @throws ArgumentException no internal record for link to use on SEND, state not SEND, RECEIVE or UPDATE_TIMER
		 */
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
							throw new InvalidOperationException ("Invalid request - link not present in interfaces: " + r.Link.Name);
						sendResponse (r.Link, model);
					} else if (state.Data is Response) {
						rm_received++;
						log.Debug ("(" + Name + ") Received routing message - response");
						Response r = state.Data as Response;
						if (!verifyRM (r))
							throw new InvalidOperationException ("Invalid response - link not present in interfaces: " + r.Link.Name);
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
							} else {
								int itf;
								if (iface_pkt.TryGetValue (p, out itf)) {
									dproc += p.Size;
									iface_psent [itf]++;
									iface_dsent [itf] += p.Size;
								} else
									throw new ArgumentNullException ("No iface");
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
				throw new ArgumentException ("[NetworkNode "+Name+"] Invalid state: "+state);
			}
			base.ProcessEvent (state, model);
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
				return (p as RoutingMessage).Link;
			} else {
				log.Debug ("Link for destination: " + p.Destination);
				Link link = rt.GetLinkForAddr (p.Destination);
				if (link != null) {
					for (int i=0; i<interfaces_used; i++) {
						if (link.Equals (interfaces [i])) {
							this.iface_pkt.Add (p, i);
							return link;
						}
					}
					throw new InvalidOperationException ("Routing through invalid link");
				} else {
					log.Debug ("No link for " + p.Destination+", trying to use default route ");
					return def_r;
				}
			}
		}

		/**
		 * Given the packet and the link to use, schedule SEND for the link with packet included at time T+wait_time
		 * @param p Packet to forward
		 * @param l Link to use (result of selectDestination)
		 * @param model the Model
		 * @throws InvalidOperationException same packet to schedule twice
		 */
		private void scheduleForward(Packet p,Link l,MFF_NPRG031.Model model){
			if (l != null) {
				int wait = wait_time (model.Time, model);
				log.Debug ("(" + Name + ") Routing via link " + l + " at " + (model.Time + wait));
				if (schedule.ContainsKey (p))
					throw new InvalidOperationException ("Same packet to schedule twice");
				this.schedule.Add (p, l);
				this.Schedule (model.K, new MFF_NPRG031.State (MFF_NPRG031.State.state.SEND, p), model.Time + wait);
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
 		 * @throws ArgmentNullException Model null or interfaces null or RoutingTable null or Link null
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
					throw new ArgumentNullException ("Link null in send response");
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

		public struct IfaceUse{
			public readonly string lname;
			public readonly int psent;
			public readonly decimal dsent,ppsent,pdsent;
			public int rpsent, rdsent;

			public IfaceUse(string link, int packets, decimal data,decimal percPackets,decimal percData){
				this.lname = link;
				this.psent = packets;
				this.dsent = data;
				this.ppsent = percPackets;
				this.pdsent = percData;
				this.rpsent = 0;
				this.rdsent = 0;
			}
		}

		public IfaceUse[] GetLinkUsage(){
			log.Debug ("Get link usage (" + Name+")");
			IfaceUse[] res = new IfaceUse[interfaces_used];
			log.Debug ("Populate array");
			int up = processed - rm_sent - rm_received;
			log.Debug ("UP:" + up);
			for (int i=0; i<interfaces_used; i++) {
				log.Debug ("Interface " + i+"\niface_psent:"+iface_psent[i]+"\niface_dsent:"+iface_dsent[i]);
				decimal pp = (up>0)? (decimal)(iface_psent[i])/up*100.0m : 0.0m;
				decimal pd = (dproc>0)? (decimal)(iface_dsent[i])/dproc*100.0m : 0.0m;
				res [i] = new IfaceUse (interfaces [i].Name, iface_psent [i], iface_dsent [i],pp,pd);
			}
			log.Debug ("Sort packets");
			sortUsagePackets (ref res, 0, interfaces_used - 1);
			for (int i=0; i<interfaces_used; i++) {
				res [i].rpsent = interfaces_used - i;
				log.Debug (res [i].rpsent);
			}
			log.Debug ("Sort data");
			sortUsageData (ref res, 0, interfaces_used - 1);
			for (int i=0; i<interfaces_used; i++) {
				res [i].rdsent = interfaces_used-i;
				log.Debug (res [i].rdsent);
			}
			return res;
		}

		public decimal DataProcessed{
			get{
				return dproc;
			}
		}

		public decimal DataProcessedPerTic(MFF_NPRG031.Model fm){
			if (fm != null) {
				return (fm.Time>0)?(decimal)(dproc/fm.Time)*1.0m:0.0m;
			} else
				throw new ArgumentException ("Model null");
		}

		//setridi podle psent, poradi ulozi do rpsent, pouzije QS
		private void sortUsagePackets(ref IfaceUse[] iu,int start,int stop){
			log.Debug ("sUP(" + start + "," + stop);
			int k, i, j;
			IfaceUse x;
			i = start;
			j = stop;
			k = iu [(start + stop) / 2].psent;
			do{
				while(iu[i].psent<k) i++;
				while(iu[j].psent>k) j--;
				if(i<j){
					x = iu[i];
					iu[i]=iu[j];
					iu[j]=x;
					i++;
					j--;
				} else if (i==j){
					i++;
					j--;

				}
			} while (i<=j);
			if (start < j)
				sortUsagePackets (ref iu, start, j);
			if (i < stop)
				sortUsagePackets (ref iu, i, stop);
		} 

		//setridi podle dsent, poradi ulozi do dsent, pouzije QS
		private void sortUsageData(ref IfaceUse[] iu, int start, int stop){
			log.Debug ("sUD(" + start + "," + stop);
			decimal k; 
			int i, j;
			IfaceUse x;
			i = start;
			j = stop;
			k = iu [(start + stop) / 2].dsent;
			do{
				while(iu[i].dsent<k) i++;
				while(iu[j].dsent>k) j--;
				if(i<j){
					x = iu[i];
					iu[i]=iu[j];
					iu[j]=x;
					i++;
					j--;
				} else if (i==j){
					i++;
					j--;

				}
			} while (i<=j);
			if (start < j)
				sortUsagePackets (ref iu, start, j);
			if (i < stop)
				sortUsagePackets (ref iu, i, stop);
		}

		/**
		 * Generates a wait time
		 * @return random wait time
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

