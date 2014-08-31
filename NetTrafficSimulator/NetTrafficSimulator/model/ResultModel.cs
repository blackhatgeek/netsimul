using System;
using System.Collections.Generic;

namespace NetTrafficSimulator
{
	/**
	 * Result model holds results of simulation
	 */
	public class ResultModel
	{
		/**
		 */
		private class EndNodeResult
		{
			string name;
			int address, packetsSent, packetsReceived,packetsMalreceived,timeWaited;
			decimal timeIdle, avgWaitTime,avgPSize;

			public EndNodeResult(string name, int address,int packetsSent, int packetsReceived, int packetsMalreceived,int timeWaited, decimal timeIdle, decimal avgWaitTime,decimal avgPSize){
				this.name=name;
				this.address=address;
				this.packetsSent=packetsSent;
				this.packetsReceived=packetsReceived;
				this.packetsMalreceived=packetsMalreceived;
				this.timeWaited=timeWaited;
				this.timeIdle=timeIdle;
				this.avgWaitTime=avgWaitTime;
				this.avgPSize=avgPSize;
			}

			public string Name{
				get{
					return name;
				}
			}

			public int Address{
				get{
					return address;
				}
			}

			public int PacketsSent{
				get{
					return packetsSent;
				}
			}

			public int PacketsReceived{
				get{
					return packetsReceived;
				}
			}

			public int PacketsMalreceived{
				get{
					return packetsMalreceived;
				}
			}

			public int TimeWaited{
				get{
					return timeWaited;
				}
			}

			public decimal TimeIdle{
				get{
					return timeIdle;
				}
			}

			public decimal AvgWaitTime {
				get {
					return avgWaitTime;
				}
			}

			public decimal AvgPSize{
				get{
					return avgPSize;
				}
			}
		}

		/**
		 */
		private class ServerNodeResult
		{
			string name;
			int address,packetsProcessed,packetsMalreceived,timeWaited;
			decimal timeIdle,avgWaitTime;
			public ServerNodeResult(string name,int address,int packetsProcessed,int packetsMalreceived,int timeWaited,decimal timeIdle,decimal avgWaitTime){
				this.name=name;
				this.address=address;
				this.packetsProcessed=packetsProcessed;
				this.packetsMalreceived=packetsMalreceived;
				this.timeWaited=timeWaited;
				this.timeIdle=timeIdle;
				this.avgWaitTime=avgWaitTime;

			}

			public string Name{
				get{
					return name;
				}
			}

			public int Address {
				get {
					return address;
				}
			}

			public int PacketsProcessed {
				get {
					return packetsProcessed;
				}
			}

			public int PacketsMalreceived{
				get{
					return packetsMalreceived;
				}
			}

			public int TimeWaited {
				get {
					return timeWaited;
				}
			}

			public decimal TimeIdle{
				get {
					return timeIdle;
				}
			}

			public decimal AvgWaitTime{
				get{
					return avgWaitTime;
				}
			}
		}

		/**
		 */
		private class NetworkNodeResult
		{
			string name;
			int packetsProcessed;
			int timeWaited;
			int dropped;
			int route_sent,route_received;
			decimal timeIdle;
			decimal avgWaitTime;
			decimal percPacketsDropped;
			decimal percRoute;
			decimal dapro;
			decimal pedapro;
			NetworkNode.IfaceUse[] iface_use;

			public NetworkNodeResult(string name,int packetsProcessed,int timeWaited,decimal timeIdle,decimal avgWaitTime,int dropped,
			                         decimal precPDropped,int rsent,int rrec,decimal prm, NetworkNode.IfaceUse[] iface_use, 
			                         decimal dapro, decimal pedapro){
				this.name=name;
				this.packetsProcessed=packetsProcessed;
				this.timeWaited=timeWaited;
				this.timeIdle=timeIdle;
				this.avgWaitTime=avgWaitTime;
				this.dropped=dropped;
				this.percPacketsDropped=precPDropped;
				this.route_sent=rsent;
				this.route_received=rrec;
				this.percRoute=prm;
				this.iface_use = iface_use;
				this.dapro = dapro;
				this.pedapro = pedapro;
			}

			public string Name{
				get{
					return name;
				}
			}

			public int PacketsProcessed{
				get{
					return packetsProcessed;
				}
			}

			public int TimeWaited{
				get{
					return timeWaited;
				}
			}

			public decimal TimeIdle {
				get {
					return timeIdle;
				}
			}

			public decimal AvgWaitTime{
				get{
					return avgWaitTime;
				}
			}

			public int PacketsDropped{
				get{
					return dropped;
				}
			}

			public decimal PercPacketsDropped{
				get{
					return percPacketsDropped;
				}
			}

			public int RouteSent{
				get{
					return route_sent;
				}
			}

			public int RouteReceived{
				get{
					return route_received;
				}
			}

			public decimal PercentageRoutePackets{
				get{
					return percRoute;
				}
			}

			public NetworkNode.IfaceUse[] IfaceUse{
				get{
					return iface_use;
				}
			}

			public decimal DataProcessed{
				get{
					return dapro;
				}
			}

			public decimal DataProcessedPerTic{
				get{
					return pedapro;
				}
			}
		}

		/**
		 */
		private	class LinkResult
		{
			string name;
			int packetsCarried;
			int activeTime;
			int passiveTime;
			decimal idleTime;
			decimal dataCarried;
			decimal dataPerTic;
			decimal usage;


			decimal dataSent,dataLost,pDataLost,pDataDelivered,pLostInCary;

			public LinkResult(string name,int packetsCarried, int activeTime, int passiveTime, decimal idleTime,decimal dataCarried,decimal dataPerTic,decimal usage, decimal dataSent, decimal dataLost, 
			                  decimal pDataLost,decimal pDataDelivered,decimal pLostInCarry){
				this.name=name;
				this.packetsCarried=packetsCarried;
				this.activeTime=activeTime;
				this.passiveTime=passiveTime;
				this.idleTime=idleTime;
				this.dataCarried=dataCarried;
				this.dataPerTic=dataPerTic;
				this.usage=usage;
				this.dataLost=dataLost;
				this.dataSent=dataSent;
				this.pDataLost=pDataLost;
				this.pDataDelivered=pDataDelivered;
				this.pLostInCary=pLostInCarry;
			}

			public string Name{
				get{
					return name;
				}
			}

			public int PacketsCarried {
				get {
					return packetsCarried;
				}
			}

			public int ActiveTime {
				get {
					return activeTime;
				}
			}

			public int PassiveTime {
				get {
					return passiveTime;
				}
			}

			public decimal IdleTime{
				get{
					return idleTime;
				}
			}

			public decimal DataCarried{
				get{
					return dataCarried;
				}
			}

			public decimal DataPerTic{
				get{
					return dataPerTic;
				}
			}

			public decimal Usage{
				get{
					return usage;
				}
			}
			//decimal dataSent, decimal dataLost, 
			//decimal pDataLost,decimal pDataDelivered,decimal pLostInCarry
			public decimal DataSent{
				get{
					return dataSent;
				}
			}

			public decimal DataLost{
				get{
					return dataLost;
				}
			}

			public decimal PercentageDataLost{
				get{
					return pDataLost;
				}
			}

			public decimal PercentageDataDelivered{
				get{
					return pDataDelivered;
				}
			}

			public decimal PercentageDataLostInCarry{
				get{
					return pLostInCary;
				}
			}
		}

		private const String NN = "Network node";
		private const String SN = "Server node";
		private const String EN = "End node";
		private const String LN = "Link";
		private const string NF = " not found";


		//jmena
		private string[] endNodes;
		private string[] serverNodes;
		private string[] networkNodes;
		private string[] links;

		//hledani podle jmena
		private Dictionary<string,EndNodeResult> endNodeNames;
		private Dictionary<string,ServerNodeResult> serverNodeNames;
		private Dictionary<string,NetworkNodeResult> networkNodeNames;
		private Dictionary<string,LinkResult> linkNames;

		//vysledky trasovanych packetu
		//private LinkedList<LinkedList<KeyValuePair<Node,int>>> traces;
		private LinkedList<KeyValuePair<string,int>>[] traces;

		private int endNodeLimit, endNodeCount, serverNodeLimit, serverNodeCount, networkNodeLimit, networkNodeCount,linkLimit,linkCount,traced,tracer;

		/**
		 * Create result model for nodes amounts specified
		 * @param endNodes amount of end nodes
		 * @param serverNodes amount of server nodes
		 * @param networkNodes amount of network nodes
		 * @param linkNodes amount of links
		 * @param traced amount of traced packets
		 */
		public ResultModel (int endNodes,int serverNodes,int networkNodes,int linkNodes,int traced)
		{
			this.endNodes = new string[endNodes];
			this.endNodeLimit = endNodes;
			this.endNodeCount = 0;
			this.endNodeNames = new Dictionary<string, EndNodeResult> ();

			this.serverNodes = new string[serverNodes];
			this.serverNodeLimit = serverNodes;
			this.serverNodeCount = 0;
			this.serverNodeNames = new Dictionary<string, ServerNodeResult> ();

			this.networkNodes = new string[networkNodes];
			this.networkNodeLimit = networkNodes;
			this.networkNodeCount = 0;
			this.networkNodeNames = new Dictionary<string, NetworkNodeResult> ();

			this.links = new string[linkNodes];
			this.linkLimit = linkNodes;
			this.linkCount = 0;
			this.linkNames = new Dictionary<string, LinkResult> ();

			this.traced = traced;
			this.traces = new LinkedList<KeyValuePair<string,int>>[traced];
			this.tracer = 0;
		}
		/**
		 * Records results of an end node, if possible
		 * @param name node name
		 * @param address node network address
		 * @param packetsSent amount of packets sent
		 * @param packetsReceived amount of packets received
		 * @param packetsMalreceived amount of packets received with destination address different fron eno node's address
		 * @param timeWaited amount of time spent waiting
		 * @param timeIdle percentage of time spent waiting
		 * @param avgWaitTime average wait time
		 * @param avgPSize average packet size
		 * @throws ArgumentException End node counter exceeded amouunt of end nodes set in constructor
		 */
		public void SetNewEndNodeResult(string name,int address,int packetsSent,int packetsReceived,int packetsMalreceived,int timeWaited, decimal timeIdle, decimal avgWaitTime,decimal avgPSize){
			if (endNodeCount < endNodeLimit) {
				endNodes [endNodeCount] = name;
				if(endNodeNames.ContainsKey(name))
					endNodeNames.Remove(name);
				endNodeNames.Add (name, new EndNodeResult (name, address, packetsSent, packetsReceived, packetsMalreceived, timeWaited, timeIdle, avgWaitTime,avgPSize));
				endNodeCount++;
			} else
				throw new ArgumentException ("End node counter exceeded");
		}
		/**
		 * Records results of a server node, if possible
		 * @param name node name
		 * @param address node network address
		 * @param packetsProcessed amount of packets processed
		 * @param packetsMalreceived amount of packets received with destination address different fron eno node's address
		 * @param timeWaited amount of time waited
		 * @param timeIdle percentage of time spent waiting
		 * @param avgWaitTime average wait time
		 * @throws ArgumentException Server node counter exceeded amount of server nodes set in constructor
		 */
		public void SetNewServerNodeResult(string name,int address,int packetsProcessed, int packetsMalreceived, int timeWaited,decimal timeIdle,decimal avgWaitTime){
			if (serverNodeCount < serverNodeLimit) {
				serverNodes [serverNodeCount] = name;
				if (serverNodeNames.ContainsKey (name))
					serverNodeNames.Remove (name);
				serverNodeNames.Add (name, new ServerNodeResult (name, address, packetsProcessed, packetsMalreceived, timeWaited, timeIdle, avgWaitTime));
				serverNodeCount++;
			} else
				throw new ArgumentException ("Server node counter exceeded");
		}
		/**
		 * Records results of a network node, if possible
		 * @param name node name
		 * @param packetsProcessed amount of packets processed
		 * @param timeWaited amount of time spend waiting for incomming packet
		 * @param timeIdle percentage of time spent waiting
		 * @param avgWaitTime average wait time
		 * @param dropped packets dropped due to hop count
		 * @param percPacketsDropped packets dropped relative to packets processed
		 * @param rsent routing packets sent
		 * @param rreceived routing packets received
		 * @param rpack percentage of routing packets in packetsProcessed
		 * @param iface_use interface use statistics
		 * @param dapro amount data processed
		 * @param pedapro percentage of data processed
		 * @throws ArgumentException Network node counter exceeded amount of network nodes set in constructor
		 */
		public void SetNewNetworkNodeResult(string name,int packetsProcessed,int timeWaited,decimal timeIdle,decimal avgWaitTime,int dropped,decimal percPacketsDropped,int rsent,int rreceived,decimal rpack,NetworkNode.IfaceUse[] iface_use,decimal dapro,decimal pedapro){
			if (networkNodeCount < networkNodeLimit) {
				networkNodes [networkNodeCount] = name;
				if (networkNodeNames.ContainsKey (name))
					networkNodeNames.Remove (name);
				networkNodeNames.Add (name, new NetworkNodeResult (name, packetsProcessed, timeWaited, timeIdle, avgWaitTime, dropped, percPacketsDropped,rsent,rreceived,rpack,iface_use,dapro,pedapro));
				networkNodeCount++;
			} else
				throw new ArgumentException ("Network node counter exceeded");
		}
		/**
		 * Records results of a link, if possible
		 * @param name
		 * @param packetsCarried
		 * @param activeTime
		 * @param passiveTime
		 * @param idleTime
		 * @param dataCarried
		 * @param dataPerTic
		 * @param usage
		 * @param dataSent
		 * @param dataLost
		 * @param pDataLost
		 * @param pDataDelivered
		 * @param pLostInCarry
		 * @throws ArgumentException
		 */
		public void SetNewLinkResult(string name,int packetsCarried, int activeTime, int passiveTime, decimal idleTime,decimal dataCarried,decimal dataPerTic,decimal usage, decimal dataSent, decimal dataLost, 
		                             decimal pDataLost,decimal pDataDelivered, decimal pLostInCarry){
			if (linkCount < linkLimit) {
				links [linkCount] = name;
				linkNames.Add (name, new LinkResult (name, packetsCarried, activeTime, passiveTime, idleTime,dataCarried,dataPerTic,usage,dataSent,dataLost,pDataLost,pDataDelivered,pLostInCarry));
				linkCount++;
			} else
				throw new ArgumentException ("Link counter exceeded: "+linkCount);
		}

		/**
		 */
		public void SetPacketTrace(Packet p){
			if (p.Traced) {
				if (tracer < traced) {
					this.traces [tracer] = p.Trace;
					tracer++;
				} else
					throw new ArgumentException ("Trace overflow");
			}else
					throw new ArgumentException ("Packet not traced!");
		}

		/**
		 * Array of available EndNode's names for queries
		 */
		public string[] EndNodeNames{
			get{
				return endNodes;
			}
		}
		/**
		 * Array of avaaliable NetworkNode's names for queries
		 */
		public string[] NetworkNodeNames{
			get{
				return networkNodes;
			}
		}
		/**
		 * Array of available ServerNode's names for queries
		 */
		public string[] ServerNodeNames{
			get{
				return serverNodes;
			}
		}
		/**
		 * Array of available Link's names for queries
		 */
		public string[] LinkNames{
			get{
				return links;
			}
		}

		/**
		 */
		private EndNodeResult getENR(string name){
			EndNodeResult enr;
			if (endNodeNames.TryGetValue (name, out enr))
				return enr;
			else
				throw new ArgumentException (EN+NF);
		}
		/**
		 * If there exists an EndNode with the name provided, return it's network address
		 * @param name EndNode name
		 * @return EndNode network address
		 * @throws ArgumentException Node not found
		 */
		public int GetEndNodeAddress(string name){
				return getENR(name).Address;
		}
		/**
		 * If there exists an EndNode with the name provided return amount of packets sent by the node
		 * @param name EndNode name
		 * @return Amount of packets the EndNode sent
		 * @throws ArgumentException node not found
		 */
		public int GetEndNodePacketsSent(string name){
			return getENR(name).PacketsSent;
		}
		/**
		 * If there exists an EndNode with the name provided return amount of packets received by the node.
		 * @param name EndNode name
		 * @return Amount of packets the EndNode sent
		 * @throws ArgumentException Node not found
		 */
		public int GetEndNodePacketsReceived(string name){
			return getENR(name).PacketsReceived;
		}
		/**
		 * If there exists an EndNode with the name provided return amount of packets received by the EndNode, where destination address was not the EndNode's address
		 * @param name EndNode name
		 * @return amount of packets incorrectly received by the node
		 * @throws ArgumentException Node not found
		 */
		public int GetEndNodePacketsMalreceived(string name){
			return getENR(name).PacketsMalreceived;
		}
		/** 
		 * If there exists an EndNode with the name provided return amount of time the node spent waiting
		 * @param name EndNode name
		 * @return Amount of time the EndNode spent waiting
		 * @throws ArgumentException Node not found
		 */
		public int GetEndNodeTimeWaited(string name){
			return getENR(name).TimeWaited;
		}
		/**
		 * If there exists an EndNode with the name provided return percentage of time the node spent waiting
		 * @param name EndNode name
		 * @return Percentage of time the EndNode spent waiting
		 * @throws ArgumentException node not found
		 */
		public decimal GetEndNodePercentTimeIdle(string name){
			return getENR(name).TimeIdle;
		}
		/**
		 * If there exists an EndNode with the name provided return the average time the node spent waiting
		 * @param name EndNode name
		 * @return Average time the node spent waiting
		 * @throws ArgumentException EndNode not found
		 */
		public decimal GetEndNodeAverageWaitTime(string name){
			return getENR(name).AvgWaitTime;
		}
		/**
		 * If there exists an EndNode with the name provided return average size of packet the node generated
		 * @param name EndNode name
		 * @return average size of packet data generated by the node
		 * @throws ArgumentException node not found
		 */
		public decimal GetEndNodeAveragePacketSize(string name){
			return getENR(name).AvgPSize;
		}

		/**
		 * For ServerNode name try to retrieve ServerNodeResult
		 * @param name ServerNode name
		 * @return ServerNodeResult
		 * @throws ArgumentException Server node not found
		 */
		private ServerNodeResult getSNR(string name){
			ServerNodeResult snr;
			if (serverNodeNames.TryGetValue (name, out snr))
				return snr;
			else
				throw new ArgumentException (SN+NF);
		}
		/**
		 * If there exists a ServerNode with the name provided, return it's network address
		 * @param name ServerNode name
		 * @return ServerNode network address
		 * @throws ArgumentException server node not found
		 */
		public int GetServerNodeAddress(string name){
			return getSNR (name).Address;
		}
		/**
		 * If there exists a ServerNode with the name provided return the amount of packets processed by the node
		 * @param name ServerNode name
		 * @return amount of packets processed by the ServerNode
		 * @throws ArgumentException ServerNode not found
		 */
		public int GetServerNodePacketsProcessed(string name){
			return getSNR(name).PacketsProcessed;
		}
		/**
		 * If there exists a ServerNode with the name provided return amount of packets received by the ServerNode where destination address was different than ServerNode's address
		 * @param name ServerNode name
		 * @return amount of packets incorrectly received by the ServerNode
		 * @throws ArgumentException Server node not found
		 */
		public int GetServerNodePacketsMalreceived(string name){
			return getSNR(name).PacketsMalreceived;
		}
		/**
		 * If there exists a ServerNode with the name provided return amount of time the node spent waiting
		 * @param name ServerNode name
		 * @return Amount of time the ServerNode spent waiting
		 * @throws ArgumentException Node not found
		 */
		public int GetServerNodeTimeWaited(string name){
			return getSNR(name).TimeWaited;
		}
		/**
		 * If there exists a ServerNode with the name provided return percentage of time the node spent waiting
		 * @param name ServerNode name
		 * @return Percentage of time the ServerNode spent waiting
		 * @throws ArgumentException node not found
		 */
		public decimal GetServerNodePercentTimeIdle(string name){
			return getSNR(name).TimeIdle;
		}
		/**
		 * If there exists a ServerNode with the name provided return average time the node spent waiting
		 * @param name ServerNode name
		 * @return average time the node spent waiting
		 * @throws ArgumentException ServerNode not found
		 */
		public decimal GetServerNodeAverageWaitTime(string name){
			return getSNR(name).AvgWaitTime;
		}

		/**
		 * If there exists a NetworkNode with the name provided, return it's network node result
		 * @param name NetworkNode name
		 * @return NetworkNodeResult
		 * @throws ArgumentException network node not found
		 */
		private NetworkNodeResult getNNR(string name){
			NetworkNodeResult nnr;
			if (networkNodeNames.TryGetValue (name, out nnr))
				return nnr;
			else
				throw new ArgumentException (NN+NF);
		}
		/**
		 * If there exists a NetworkNode with the name provided return the amount of packets processed by the node
		 * @param name NetworkNode name
		 * @return amount of packets processed by the NetworkNode
		 * @throws ArgumentException NetworkNode not found
		 */
		public int GetNetworkNodePacketsProcessed(string name){
			return getNNR (name).PacketsProcessed;
		}
		/**
		 * If there exists a NetworkNode with the name provided return amount of time the node spent waiting
		 * @param name NetworkNode name
		 * @return Amount of time the EndNode spent waiting
		 * @throws ArgumentException Node not found
		 */
		public int GetNetworkNodeTimeWaited(string name){
			return getNNR(name).TimeWaited;
		}
		/**
		 * If there exists a NetworkNode with the name provided return percentage of time the node spent waiting
		 * @param name NetworkNode name
		 * @return Percentage of time the NetworkNode spent waiting
		 * @throws ArgumentException node not found
		 */
		public decimal GetNetworkNodePercentTimeIdle(string name){
			return getNNR(name).TimeIdle;
		}
		/**
		 * If there exists a NetworkNode with the name provided return the average time the node spent waiting
		 * @param name NetworkNode name
		 * @return Average time the node spent waiting
		 * @throws ArgumentException NetworkNode not found
		 */
		public decimal GetNetworkNodeAverageWaitTime(string name){
			return getNNR (name).AvgWaitTime;
		}
		/**
		 * If there exists a NetworkNode with the name provided return the amount of packets dropped due to hop count
		 * @param name NetwokNode name
		 * @return amount of packets dropped due to hop count
		 * @throws ArgumentException Network node not found
		 */
		public int GetNetworkNodePacketsDropped(string name){
			return getNNR (name).PacketsDropped;
		}
		/**
		 * If there exists a NetworkNode with the name provided return the percentage of packets dropped due to hop count relative to amount of packets processed
		 * @param name NetwokNode name
		 * @return percentage of packets dropped due to hop count
		 * @throws ArgumentException Network node not found
		 */
		public decimal GetNetworkNodePercentagePacketsDropped(string name){
			return getNNR (name).PercPacketsDropped;
		}
		/**
		 * If there exists a NetworkNode with the name provided return the amount of routing packets sent
		 * @param name NetworkNode name
		 * @return amount of routing packets sent
		 * @throws ArgumentException Network node not found
		 */
		public int GetNetworkNodeRoutingPacketsSent(string name){
			return getNNR(name).RouteSent;
		}
		/**
		 * If there exists a NetworkNode with the name provided return the amount of routing packets received
		 * @param name NetworkNode name
		 * @return amount of routing packets received
		 * @throws ArgumentException Network node not found
		 */
		public int GetNetworkNodeRoutingPacketsReceived(string name){
			return getNNR (name).RouteReceived;
		}
		/**
		 * If there exists a NetworkNode with the name provided return the percentage of routing packets relative to total packets processed by the network node
		 * @param name NetworkNode name
		 * @return percentage of routing packets processed
		 * @throws ArgumentExcepion Network node not found
		 */
		public decimal GetNetworkNodePercentageRoutingPackets(string name){
			return getNNR (name).PercentageRoutePackets;
		}

		public NetworkNode.IfaceUse[] GetNetworkNodeIfaceUse(string name){
			return getNNR (name).IfaceUse;
		}

		public decimal GetNetworkNodeDataProcessed(string name){
			return getNNR (name).DataProcessed;
		}

		public decimal GetNetworkNodeDataProcessedPerTic(string name){
			return getNNR (name).DataProcessedPerTic;
		}

		/**
		 * If there exists a Link with the name provided retrieve it's link result
		 * @param name link name
		 * @return LinkResult
		 * @throws ArgumentException Link not found
		 */
		private LinkResult getLR(string name){
			LinkResult lr;
			if (linkNames.TryGetValue (name, out lr))
				return lr;
			else
				throw new ArgumentException (LN);
		}
		/**
		 * If there exists a link with the name provided return amount of packets carried by the link
		 * @param name Link name
		 * @return amount of packets varried by the link
		 * @throws ArgumentException link not found
		 */
		public int GetLinkPacketsCarried(string name){
			return getLR (name).PacketsCarried;
		}
		/**
		 * If there exists a link with the name provided return how much time the link was active
		 * @param name Link name
		 * @return How much time was the link active
		 * @throws ArgumentException Link not found
		 */
		public int GetLinkActiveTime(string name){
			return getLR(name).ActiveTime;
		}
		/**
		 * If there exists a link with the name provided return how much time the link was not active
		 * @param name Link name
		 * @return How much time the link was not active
		 * @throws ArgumentException Link not found
		 */
		public int GetLinkPassiveTime(string name){
			return getLR(name).PassiveTime;
		}
		/**
		 * If there exists a link with the name provided return the percentage how much time the link was not active
		 * @param name Link name
		 * @return percentage how much time the link was not active
		 * @throws ArgumentException Link not found
		 */
		public decimal GetLinkIdleTimePercentage(string name){
			return getLR(name).IdleTime;
		}

		/**
		 * If there exists a link with the name provided return the amount of data carried by the link
		 * @param name link name
		 * @return amount of data carried by the link
		 * @throws ArgumentException link not found
		 */
		public decimal GetLinkDataCarried(string name){
			return getLR (name).DataCarried;
		}

		/**
		 * If there exists a link with the name provided return the average amount of data carried by link per one unit of simulation time
		 * @param name Link name
		 * @return average amount of data carried per unit of time
		 * @throws ArgumentException link not found
		 */
		public decimal GetLinkDataPerTic(string name){
			return getLR (name).DataPerTic;
		}

		/**
		 * If there exists a link with the name provided return link usage as percentage of data carried to link capacity
		 * @param name link name
		 * @return link usage
		 * @throws ArgumentException link not found
		 */ 
		public decimal GetLinkUsage(string name){
			return getLR (name).Usage;
		}

		/**
		 * If there exists a link with the name provided return the amount of data sent by link
		 * @param name link name
		 * @return amount of data sent
		 * @throws ArgumentException link not found
		 */
		public decimal GetLinkDataSent(string name){
			return getLR (name).DataSent;
		}

		/**
		 * If there exists a link with the name provided return the amount of data lost by link
		 * @param name link name
		 * @return amount of data lost by link
		 * @throws ArgumentException link not found
		 */
		public decimal GetLinkDataLost(string name){
			return getLR (name).DataLost;
		}

		/**
		 * If there exists a link with the name provided return the amount of data lost by the link as percentage to the amount of data processed
		 * @param name link name
		 * @return percentage of data lost
		 * @throws ArgumentException link not found
		 */
		public decimal GetLinkPercentageDataLost(string name){
			return getLR (name).PercentageDataLost;
		}
		/**
		 * If there exists a link with the name provided return the amount of data delivered by the link as percentage to the amount of data processed
		 * @param name link name
		 * @return percentage of data delivered
		 * @throws ArgumentException link not found
		 */
		public decimal GetLinkPercentageDataDelivered(string name){
			return getLR (name).PercentageDataDelivered;
		}
		/**
		 * If there exists a link with the name provided return the amount of data lost because link was not "Active" when carry was called as percentage to the amount of data processed
		 * @param name link name
		 * @return percentage of data lost because link was not active when received the data
		 * @throws ArgumentException link not found
		 */
		public decimal GetLinkPercentageDataLostInCarry(string name){
			return getLR (name).PercentageDataLostInCarry;
		}

		/**
		 */
		public LinkedList<KeyValuePair<string,int>>[] GetPacketTraces(){
			return traces;
		}

		public void RemoveEndNodeResult(string name){
			endNodeNames.Remove (name);
			endNodeCount--;
		}

		public void RemoveServerNodeResult(string name){
			serverNodeNames.Remove (name);
			serverNodeCount--;
		}

		public void RemoveNetworkNodeResult(string name){
			networkNodeNames.Remove (name);
			networkNodeCount--;
		}

		public void RemoveLinkResult(string name){
			linkNames.Remove (name);
			linkCount--;
		}
	}
}