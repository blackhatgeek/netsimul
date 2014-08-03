using System;
using System.Collections.Generic;

namespace NetTrafficSimulator
{
	public class ResultModel
	{
		private class EndNodeResult
		{
			string name;
			int address, packetsSent, packetsReceived, timeWaited;
			decimal timeIdle, avgWaitTime;

			public EndNodeResult(string name, int address,int packetsSent, int packetsReceived, int timeWaited, decimal timeIdle, decimal avgWaitTime){
				this.name=name;
				this.address=address;
				this.packetsSent=packetsSent;
				this.packetsReceived=packetsReceived;
				this.timeWaited=timeWaited;
				this.timeIdle=timeIdle;
				this.avgWaitTime=avgWaitTime;
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
		}

		private class ServerNodeResult
		{
			string name;
			int address,packetsProcessed,timeWaited;
			decimal timeIdle,avgWaitTime;
			public ServerNodeResult(string name,int address,int packetsProcessed,int timeWaited,decimal timeIdle,decimal avgWaitTime){
				this.name=name;
				this.address=address;
				this.packetsProcessed=packetsProcessed;
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

		private class NetworkNodeResult
		{
			string name;
			int packetsProcessed;
			int timeWaited;
			decimal timeIdle;
			decimal avgWaitTime;

			public NetworkNodeResult(string name,int packetsProcessed,int timeWaited,decimal timeIdle,decimal avgWaitTime){
				this.name=name;
				this.packetsProcessed=packetsProcessed;
				this.timeWaited=timeWaited;
				this.timeIdle=timeIdle;
				this.avgWaitTime=avgWaitTime;
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
		}

		private	class LinkResult
		{
			string name;
			int packetsCarried;
			int packetsDropped;
			decimal dropPerct;
			int activeTime;
			int passiveTime;
			decimal idleTime;

			public LinkResult(string name,int packetsCarried, int packetsDropped, decimal dropPerct, int activeTime, int passiveTime, decimal idleTime){
				this.name=name;
				this.packetsCarried=packetsCarried;
				this.packetsDropped=packetsDropped;
				this.dropPerct=dropPerct;
				this.activeTime=activeTime;
				this.passiveTime=passiveTime;
				this.idleTime=idleTime;
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

			public int PacketsDropped{
				get{
					return packetsDropped;
				}
			}

			public decimal DropPercentage{
				get{
					return dropPerct;
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
		}

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

		private int endNodeLimit, endNodeCount, serverNodeLimit, serverNodeCount, networkNodeLimit, networkNodeCount,linkLimit,linkCount;

		/**
		 * Create result model for nodes amounts specified
		 * @param endNodes amount of end nodes
		 * @param serverNodes amount of server nodes
		 * @param networkNodes amount of network nodes
		 * @param linkNodes amount of links
		 */
		public ResultModel (int endNodes,int serverNodes,int networkNodes,int linkNodes)
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

			this.links = new string[linkCount];
			this.linkLimit = linkCount;
			this.linkCount = 0;
			this.linkNames = new Dictionary<string, LinkResult> ();
		}
		/**
		 * Records results of an end node, if possible
		 * @param name node name
		 * @param address node network address
		 * @param packetsSent amount of packets sent
		 * @param packetsReceived amount of packets received
		 * @param timeWaited amount of time spent waiting
		 * @param timeIdle percentage of time spent waiting
		 * @param avgWaitTime average wait time
		 * @throws ArgumentException End node counter exceeded amouunt of end nodes set in constructor
		 */
		public void SetNewEndNodeResult(string name,int address,int packetsSent,int packetsReceived,int timeWaited, decimal timeIdle, decimal avgWaitTime){
			if (endNodeCount < endNodeLimit) {
				endNodes [endNodeCount] = name;
				endNodeNames.Add (name, new EndNodeResult (name, address, packetsSent, packetsReceived, timeWaited, timeIdle, avgWaitTime));
				endNodeCount++;
			} else
				throw new ArgumentException ("End node counter exceeded");
		}
		/**
		 * Records results of a server node, if possible
		 * @param name node name
		 * @param address node network address
		 * @param packetsProcessed amount of packets processed
		 * @param timeWaited amount of time waited
		 * @param timeIdle percentage of time spent waiting
		 * @param avgWaitTime average wait time
		 * @throws ArgumentException Server node counter exceeded amount of server nodes set in constructor
		 */
		public void SetNewServerNodeResult(string name,int address,int packetsProcessed,int timeWaited,decimal timeIdle,decimal avgWaitTime){
			if (serverNodeCount < serverNodeLimit) {
				serverNodes [serverNodeCount] = name;
				serverNodeNames.Add (name, new ServerNodeResult (name, address, packetsProcessed, timeWaited, timeIdle, avgWaitTime));
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
		 * @throws ArgumentException Network node counter exceeded amount of network nodes set in constructor
		 */
		public void SetNewNetworkNodeResult(string name,int packetsProcessed,int timeWaited,decimal timeIdle,decimal avgWaitTime){
			if (networkNodeCount < networkNodeLimit) {
				networkNodes [networkNodeCount] = name;
				networkNodeNames.Add (name, new NetworkNodeResult (name, packetsProcessed, timeWaited, timeIdle, avgWaitTime));
				networkNodeCount++;
			} else
				throw new ArgumentException ("Network node counter exceeded");
		}
		/**
		 * Records results of a link, if possible
		 * @param name
		 * @param packetsCarried
		 * @param packetsDropped
		 * @param dropPerct
		 * @param activeTime
		 * @param passiveTime
		 * @param idleTime
		 * @throws ArgumentException
		 */
		public void SetNewLinkResult(string name,int packetsCarried, int packetsDropped, decimal dropPerct, int activeTime, int passiveTime, decimal idleTime){
			if (linkCount < linkLimit) {
				links [linkCount] = name;
				linkNames.Add (name, new LinkResult (name, packetsCarried, packetsDropped, dropPerct, activeTime, passiveTime, idleTime));
				linkCount++;
			} else
				throw new ArgumentException ("Link counter exceeded");
		}

		public string[] EndNodeNames{
			get{
				return endNodes;
			}
		}
		public string[] NetworkNodeNames{
			get{
				return networkNodes;
			}
		}
		public string[] ServerNodeNames{
			get{
				return serverNodes;
			}
		}
		public string[] LinkNames{
			get{
				return links;
			}
		}

		public int GetEndNodeAddress(string name){
			EndNodeResult enr;
			if (endNodeNames.TryGetValue (name, out enr))
				return enr.Address;
			else
				throw new ArgumentException ("Node not found");
		}
		public int GetEndNodePacketsSent(string name){
			EndNodeResult enr;
			if (endNodeNames.TryGetValue (name, out enr))
				return enr.PacketsSent;
			else
				throw new ArgumentException ("End node not found");
		}
		public int GetEndNodePacketsReceived(string name){
			EndNodeResult enr;
			if (endNodeNames.TryGetValue (name, out enr))
				return enr.PacketsReceived;
			else
				throw new ArgumentException ("End node not found");

		}
		public int GetEndNodeTimeWaited(string name){
			EndNodeResult enr;
			if (endNodeNames.TryGetValue (name, out enr))
				return enr.TimeWaited;
			else
				throw new ArgumentException ("Node not found");
		}
		public decimal GetEndNodeTimeIdle(string name){
			EndNodeResult enr;
			if (endNodeNames.TryGetValue (name, out enr))
				return enr.TimeIdle;
			else
				throw new ArgumentException ("End node not found");
		}
		public decimal GetEndNodeAvgWaitTime(string name){
			EndNodeResult enr;
			if (endNodeNames.TryGetValue (name, out enr))
				return enr.AvgWaitTime;
			else
				throw new ArgumentException ("End node not found");
		}

		public int GetServerNodeAddress(string name){
			ServerNodeResult snr;
			if (serverNodeNames.TryGetValue (name, out snr))
				return snr.Address;
			else
				throw new ArgumentException ("Server node not found");
		}
		public int GetServerNodePacketsProcessed(string name){
			ServerNodeResult snr;
			if (serverNodeNames.TryGetValue (name, out snr))
				return snr.PacketsProcessed;
			else
				throw new ArgumentException ("Server node not found");
		}
		public int GetServerNodeTimeWaited(string name){
			ServerNodeResult snr;
			if (serverNodeNames.TryGetValue (name, out snr))
				return snr.TimeWaited;
			else
				throw new ArgumentException ("Server node not found");
		}
		public decimal GetServerNodePercentTimeIdle(string name){
			ServerNodeResult snr;
			if (serverNodeNames.TryGetValue (name, out snr))
				return snr.TimeIdle;
			else
				throw new ArgumentException ("Server node not found");
		}
		public decimal GetServerNodeAverageWaitTime(string name){
			ServerNodeResult snr;
			if (serverNodeNames.TryGetValue (name, out snr))
				return snr.AvgWaitTime;
			else
				throw new ArgumentException ("Server node not found");
		}

		public int GetNetworkNodePacketsProcessed(string name){
			NetworkNodeResult nnr;
			if (networkNodeNames.TryGetValue (name, out nnr))
				return nnr.PacketsProcessed;
			else
				throw new ArgumentException ("Network node not found");
		}
		public int GetNetworkNodeTimeWaited(string name){
			NetworkNodeResult nnr;
			if (networkNodeNames.TryGetValue (name, out nnr))
				return nnr.TimeWaited;
			else
				throw new ArgumentException ("Network node not found");
		}
		public decimal GetNetworkNodeTimeIdle(string name){
			NetworkNodeResult nnr;
			if (networkNodeNames.TryGetValue (name, out nnr))
				return nnr.TimeIdle;
			else
				throw new ArgumentException ("Network node not found");
		}
		public decimal GetNetworkNodeAverageWaitTime(string name){
			NetworkNodeResult nnr;
			if (networkNodeNames.TryGetValue (name, out nnr))
				return nnr.AvgWaitTime;
			else
				throw new ArgumentException ("Network node not found");
		}

		public int GetLinkPacketsCarried(string name){
			LinkResult lr;
			if (linkNames.TryGetValue (name, out lr))
				return lr.PacketsCarried;
			else
				throw new ArgumentException ("Link not found");
		}
		public int GetLinkPacketsDropped(string name){
			LinkResult lr;
			if (linkNames.TryGetValue (name, out lr))
				return lr.PacketsDropped;
			else
				throw new ArgumentException ("Link not found");
		}
		public decimal GetLinkDropPercentage(string name){
			LinkResult lr;
			if (linkNames.TryGetValue (name, out lr))
				return lr.DropPercentage;
			else
				throw new ArgumentException ("Link not found");
		}
		public int GetLinkActiveTime(string name){
			LinkResult lr;
			if (linkNames.TryGetValue (name, out lr))
				return lr.ActiveTime;
			else
				throw new ArgumentException ("Link not found");
		}
		public int GetLinkPassiveTime(string name){
			LinkResult lr;
			if (linkNames.TryGetValue (name, out lr))
				return lr.PassiveTime;
			else
				throw new ArgumentException ("Link not found");
		}
		public decimal GetLinkIdleTimePercentage(string name){
			LinkResult lr;
			if (linkNames.TryGetValue (name, out lr))
				return lr.IdleTime;
			else
				throw new ArgumentException ("Link not found");
		}
	}
}