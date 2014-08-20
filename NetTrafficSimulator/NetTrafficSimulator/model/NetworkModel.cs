using System;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * <p>NetworkModel generates network map based on data provided by user.</p>
	 * <p>Network map is a graph represented as adjacency matrix.</p>
	 * <p>Node can be of type END_NODE, SERVER_NODE, NETWORK_NODE or UNIDENTIFIED_NODE</p>
	 */
	public class NetworkModel
	{
		/**
		 * END_NODE is a end-user node, which initiates connection to server-node
		 */
		public const int END_NODE = 0;
		/**
		 * SERVER_NODE is server, which responds to received packets
		 */
		public const int SERVER_NODE = 1;
		/**
		 * NETWORK_NODE is router / switch on the network
		 */
		public const int NETWORK_NODE = 2;
		/**
		 * UNIDENTIFIED_NODE indicates an error in model
		 */
		public const int UNIDENTIFIED_NODE = -1;
		/**
		 * Message string added to any illegal argument exception thrown
		 */
		private const string ILLEGAL_PARAMETER = "Neplatny parametr";
		/**
		 * Mark there is no link between two nodes
		 * Link capacity zero serves well for this purpose as it makes sure without any additional verification one cannot send data through the link
		 */
		private const int NO_CONNECTION = 0;

		private const int DEFAULT_END_NODE_MPS = 10;

		/**
		 * Server node count
		 */
		private int SNCount;

		/**
		 * Creates a NetworkModel with given amount of nodes
		 * @param node_count amount of nodes in model
		 * @throws ArgumentOutOfRangeException if given node_count is less than 0
		 */
		public NetworkModel ()
		{
				this.SNCount = 0;
		}

		/**
		 * Amount of servers in model
		 */
		//UPDATE NOT NEEDED -> LEAVE AS IS
		public int ServerNodeCount{
			get{
				return SNCount;
			}
		}

		/**
		 * Amount of nodes in the model
		 */
		//UPDATED LOCALLY
		public int NodeCount{
			get{
				return node_records.Count;
			}
		}

		/**
		 * Do we have a node with name provided in our model
		 * @param name node name
		 * @return exists node with name given in the model
		 */
		//UPDATED LOCALLY
		public bool HaveNode(string name){
			return this.node_records.ContainsKey(name);
		}

		//"seznam nodes" - dictionary
		//"seznam links"
		private class LinkRecord:IEquatable<LinkRecord>{
			public string node1,node2,name;
			public int capacity;
			public decimal toggle_probability;
			public NodeRecord nodeA, nodeB;
			public LinkRecord(string name,string n1,string n2,int cap,decimal tp){
				this.name = name;
				this.node1 = n1;
				this.node2 = n2;
				this.capacity = cap;
				this.toggle_probability=tp;
			}
			public bool Equals (LinkRecord obj)
			{
				if (obj is LinkRecord) {
					return (obj as LinkRecord).name.Equals (name);
				} else
					return false;
			}
		}

		Dictionary<string,LinkRecord> link_records = new Dictionary<string, LinkRecord>();

		private class NodeRecord{
			public string name;
			public int type;
			public NodeRecord(string name){
				this.name=name;
			}
		}
		private class EndpointNodeRecord:NodeRecord{
			public int addr;
			public LinkRecord link;
			public EndpointNodeRecord(string name):base(name){
				type = NetworkModel.SERVER_NODE;
			}
		}
		private class EndNodeRecord:EndpointNodeRecord{
			public int maxPacketSize;
			public EndNodeRecord(string name):base(name){
				type = NetworkModel.END_NODE;
			}
		}
		private class NetworkNodeRecord:NodeRecord{
			public HashSet<LinkRecord> links = new HashSet<LinkRecord>();
			public LinkRecord default_route;
			public NetworkNodeRecord(string name):base(name){
				type = NetworkModel.NETWORK_NODE;
			}
		}

		Dictionary<string,NodeRecord> node_records = new Dictionary<string, NodeRecord>();

		/**
		 * True if there is a direct link between nodes x and y
		 * @param x node
		 * @param y node
		 * @return if there's a link
		 * @throws ArgumentOutOfRangeException when x or y are incorrect
		 */
		public bool AreConnected(string x,string y){
			NodeRecord X, Y;
			if (node_records.TryGetValue (x, out X)) {
				if (node_records.TryGetValue (y, out Y)) {
					if (X is EndpointNodeRecord) {
						if ((X as EndpointNodeRecord).link.node1.Equals (y) || (X as EndpointNodeRecord).link.node2.Equals (y))
							return true;
						else
							return false;
					} else if (Y is EndpointNodeRecord) {
						if ((Y as EndpointNodeRecord).link.node1.Equals (x) || (Y as EndpointNodeRecord).link.node2.Equals (x))
							return true;
						else
							return false;
					} else {
						bool ret = false;
						if ((X as NetworkNodeRecord).links.Count > (Y as NetworkNodeRecord).links.Count) {
							foreach (LinkRecord lr in (Y as NetworkNodeRecord).links) {
								if (lr.node1.Equals (x) || lr.node2.Equals (x)) {
									ret = true;
									break;
								}
							}
						} else {
							foreach (LinkRecord lr in (X as NetworkNodeRecord).links) {
								if (lr.node1.Equals (y) || lr.node2.Equals (y)) {
									ret = true;
									break;
								}
							}
						}
						return ret;
					}
				} else
					throw new ArgumentException ("[NetworkModel.AreConnected(" + x + "," + y + ")] Node not found:"+y);
			} else
				throw new ArgumentException ("[NetworkModel.AreConnected(" + x + "," + y + ")] Node not found:"+x);
		}
		public int LinkCapacity(string link){
			LinkRecord lr;
			if (link_records.TryGetValue (link, out lr)) {
				return lr.capacity;
			} else
				throw new ArgumentException ("[NetworkModel.LinkCapacity(" + link + ") Link not found");
		}
		/**
		 * <p>Marks a direct link between nodes x and y with given capacity.</p>
		 * <p>If there wasn't link yet, increments appropriate link_count counters</p>
		 * @param x node
		 * @param y node
		 * @param capacity positive integer for link capacity
		 * @param toggle_probability probability link toggles (switches from active to passive ... appears/disappears)
		 * @throws ArgumentOutOfRangeException if any of x or y or capacity are incorrect
		 */
		public void SetConnected(string x,string y,string lname,int capacity, decimal toggle_probability){
			NodeRecord X, Y;
			if (node_records.TryGetValue (x, out X)) {
				if (node_records.TryGetValue (y, out Y)) {
					if ((capacity > 0) && (toggle_probability >= 0.0m) && (toggle_probability <= 1.0m)) {
						LinkRecord lr = new LinkRecord (lname,x, y, capacity, toggle_probability);
						if (X is EndpointNodeRecord) {
							(X as EndpointNodeRecord).link = lr;
						} else {
							(X as NetworkNodeRecord).links.Add (lr);
						}
						link_records.Add (lname, lr);
					} else
						throw new ArgumentOutOfRangeException ("[NetworkModel.SetConnected(" + x + "," + y + "," + capacity + "," + toggle_probability + ")] " + ILLEGAL_PARAMETER);
				} else
					throw new ArgumentException ("[NetworkModel.SetConnected(" + x + "," + y + "," + capacity + "," + toggle_probability + ")] Node not found: " + y);
			} else
				throw new ArgumentException ("[NetworkModel.SetConnected(" + x + "," + y + "," + capacity + "," + toggle_probability + ")] Node not found: " + x);
		}
		/**
		 * <p>Ensures there's no direct link between nodes x and y</p>
		 * <p>If there previously was a direct link, also decrements link_count counters</p>
		 * @param x node
		 * @param y node
		 * @throws ArgumentOutOfRangeException if x or y are incorrect
		 */
		public void SetDisconnected(string link){
			LinkRecord lr;
			if (link_records.TryGetValue (link,out lr)) {
				if (lr.nodeA is EndpointNodeRecord)
					(lr.nodeA as EndpointNodeRecord).link = null;
				else {
					(lr.nodeA as NetworkNodeRecord).links.Remove (lr);
					if ((lr.nodeA as NetworkNodeRecord).default_route.Equals (lr))
						(lr.nodeA as NetworkNodeRecord).default_route = null;
				}

				if (lr.nodeB is EndpointNodeRecord)
					(lr.nodeB as EndpointNodeRecord).link = null;
				else {
					(lr.nodeB as NetworkNodeRecord).links.Remove (lr);
					if ((lr.nodeB as NetworkNodeRecord).default_route.Equals (lr))
						(lr.nodeB as NetworkNodeRecord).default_route = null;
				}

				link_records.Remove (link);
			}
		}
		/**
		 * For given node returns a type of node
		 * @param node a node of interest
		 * @return one of following constants: END_NODE, SERVER_NODE, NETWORK_NODE, UNIDENTIFIED_NODE
		 * @throws ArgumentOutOfRangeException if given node is incorrect
		 */
		public int GetNodeType(string node){
			NodeRecord nr;
			if (node_records.TryGetValue (node, out nr)) {
				return nr.type;
			} else throw new ArgumentException("[NetworkModel.GetNodeType("+node+")] Node not found");
		}
		public void AddNode(string name,int type){
			switch (type) {
			case END_NODE:
				EndNodeRecord enr = new EndNodeRecord (name);
				enr.maxPacketSize = DEFAULT_END_NODE_MPS;
				node_records.Add (name, enr);
				break;
			case SERVER_NODE:
				EndpointNodeRecord epnr = new EndpointNodeRecord (name);
				node_records.Add (name, epnr);
				SNCount++;
				break;
			case NETWORK_NODE:
				NetworkNodeRecord nnr = new NetworkNodeRecord (name);
				node_records.Add (name, nnr);
				break;
			default:
				throw new ArgumentException ("Invalid type: " + type);
			}
		}
		/**
		 * <p>Notes a name for a node</p>
		 * <p>Here we don't care if the node is namable or not</p>
		 * @param node node number
		 * @param name node name
		 * @throws ArgumentOutOfRangeException node number out of range
		 */
		public void SetNodeName(string oldname,string newname){
			NodeRecord nr;
			if (node_records.TryGetValue (oldname, out nr)) {
				if (!oldname.Equals (newname)) {
					if (node_records.ContainsKey (newname)) {
						throw new ArgumentException ("New name " + newname + " already taken. Names must be unique for nodes.");
					} else {
						nr.name = newname;
						node_records.Remove (oldname);
						node_records.Add (newname, nr);
					}
				}
			} throw new ArgumentException ("Node not found: " + oldname);
		}
		/**
		 * <p>Set network address for given node number. Here we care if node is addressable or not</p>
		 * @param node node number
		 * @param addr new network address for node
		 * @throws ArgumentOutOfRangeException invalid node number, negative address, node not EndNode not ServerNode
		 */
		public void SetEndpointNodeAddr(string name,int addr){
			NodeRecord nr;
			if (node_records.TryGetValue (name, out nr)) {
				if (nr is EndpointNodeRecord) {
					(nr as EndpointNodeRecord).addr = addr;
				} else
					throw new ArgumentException ("Node not endpoint node: " + name);
			} else
				throw new ArgumentException ("Node not found: " + name);
		}
		/**
		 * Get network address for node number
		 * @param node node number
		 * @return network address of the node
		 * @throws ArgumentOutOfRangeException invalid node number or type of node is not END NODE or SERVER NODE
		 */
		public int GetEndpointNodeAddr(string name){
			NodeRecord nr;
			if (node_records.TryGetValue (name, out nr)) {
				if (nr is EndpointNodeRecord) {
					return (nr as EndpointNodeRecord).addr;
				} else
					throw new ArgumentException ("Node not endpoint node: " + name);
			} else
				throw new ArgumentException ("Node not found: " + name);
		}
		/**
		 * Set name for link
		 * @param x a node number of a node connected to the link
		 * @param y a node number of a node connected to the link
		 * @param name new node name
		 * @throws ArgumentException invalid node number or no connection between x and y
		 */
		public void SetLinkName(string oldname,string newname){
			LinkRecord lr;
			if(link_records.TryGetValue(oldname,out lr)){
				if (!oldname.Equals (newname)) {
					if (link_records.ContainsKey (newname)) {
						throw new ArgumentException ("New name " + newname + " already taken. Names must be unique for links.");
					} else {
						lr.name = newname;
						link_records.Remove (oldname);
						link_records.Add (newname, lr);
					}
				}
			}else throw new ArgumentException("Link not found: "+oldname);
		}
		/**
		 * Sets max packet size for EndNode when using random talker communication
		 * @param n end node name
		 * @param mps max packet size
		 * @throws ArgumentException mps negative, node name not found, node not END_NODE
		 */
		public void SetEndNodeMaxPacketSize(string name,int mps){
			NodeRecord nr;
			if(node_records.TryGetValue(name,out nr)){
				if (nr is EndNodeRecord) {
					if (mps >= 0) {
						(nr as EndNodeRecord).maxPacketSize = mps;
					} else
						throw new ArgumentOutOfRangeException ("Max packet size must not be negative");
				} else
					throw new ArgumentException ("Node not end node: " + name);
			}else throw new ArgumentException("Node not found: "+name);
		}
		/**
		 * Gets max packet size for EndNode when using random talker communication
		 * @param n end node name 
		 * @return max packet size for an end node
		 * @throws ArgumentException no EndNode with such name
		 */
		public int GetEndNodeMaxPacketSize(string name){
			NodeRecord nr;
			if (node_records.TryGetValue (name, out nr)) {
				if (nr is EndNodeRecord) {
					return (nr as EndNodeRecord).maxPacketSize;
				} else
					throw new ArgumentException ("Node not end node: " + name);
			} else
				throw new ArgumentException ("Node not found: " + name);
		}
		/**
		 * Return toggle probability of a link between two nodes
		 * @param x a node connected to the link
		 * @param y a node connected to the link
		 * @return link toggle probability of the link
		 */
		public decimal GetLinkToggleProbability(string name){
			LinkRecord lr;
			if (link_records.TryGetValue (name, out lr)) {
				return lr.toggle_probability;
			} else
				throw new ArgumentException ("Link not found: " + name);
		}
		public void SetLinkToggleProbability(string name,decimal tp){
			LinkRecord lr;
			if (link_records.TryGetValue (name, out lr)) {
				if ((tp >= 0.0m) && (tp <= 1.0m)) {
					lr.toggle_probability = tp;
				} else
					throw new ArgumentOutOfRangeException ("Toggle probability must be between 0.0 and 1.0, was " + tp);
			} else
				throw new ArgumentException ("Link not found: " + name);
		}
		public void SetNetworkNodeDefaultRoute(string node,string link){
			NodeRecord nr;
			if (node_records.TryGetValue (node, out nr)) {
				if (nr is NetworkNodeRecord) {
					LinkRecord lr;
					if (link_records.TryGetValue(link,out lr)) {
						if ((nr as NetworkNodeRecord).links.Contains (lr)) {
							(nr as NetworkNodeRecord).default_route = lr;
						} else
							throw new ArgumentException ("Link (" + link + ") not connected to the network node " + node);
					} else
						throw new ArgumentException ("Link not found: " + link);
				} else
					throw new ArgumentException ("Node not network node: " + node);
			} else
				throw new ArgumentException ("Node not found: " + node);
		}
		public string GetNetworkNodeDefaultRoute(string node){
			NodeRecord nr;
			if (node_records.TryGetValue (node,out nr)) {
				if (nr is NetworkNodeRecord) {
					return (nr as NetworkNodeRecord).default_route.name;
				} else
					throw new ArgumentException ("Node not network node: " + node);
			} else
				throw new ArgumentException ("Node not found: " + node);
		}
		public int GetNetworkNodeInterfacesCount(string node){
			NodeRecord nr;
			if(node_records.TryGetValue(node,out nr)){
				if (nr is NetworkNodeRecord) {
					return (nr as NetworkNodeRecord).links.Count;
				} else
					throw new ArgumentException ("Node not network node:" + node);
			}else throw new ArgumentException("Node not found:"+node);
		}
		public string GetLinkNode1(string link){
			LinkRecord lr;
			if (link_records.TryGetValue (link, out lr)) {
				return lr.node1;
			} else
				throw new ArgumentException ("Link not found: " + link);
		}
		public string GetLinkNode2(string link){
			LinkRecord lr;
			if (link_records.TryGetValue (link, out lr)) {
				return lr.node2;
			} else
				throw new ArgumentException ("Link not found: " + link);
		}
		/**
		 * Returns capacity of a link between two nodes
		 * @param x node
		 * @param y node
		 * @return link capacity (positive integer) or zero if there's no link between particular nodes
		 */
		public int GetLinkCapacity(string link){
			LinkRecord lr;
			if (link_records.TryGetValue (link, out lr)) {
				return lr.capacity;
			} else
				throw new ArgumentException ("Link not found: " + link);
		}
		public bool IsLinkDefaultRouteForNetworkNode(string node,string link){
			NodeRecord nr;
			if (node_records.TryGetValue (node, out nr)) {
				if (nr is NetworkNodeRecord) {
					return (nr as NetworkNodeRecord).default_route.name.Equals (link);
				}else throw new ArgumentException("Node not network node: "+node);
			} else
				throw new ArgumentException ("Node not found: " + node);
		}

		public string[] GetNodeNames(){
			string[] names = new string[node_records.Count];
			node_records.Keys.CopyTo (names,0);
			return names;
		}
		public string[] GetLinkNames(){
			string[] names = new string[link_records.Count];
			link_records.Keys.CopyTo (names, 0);
			return names;
		}
		public int GetLinkCount(){
			return this.link_records.Count;
		}
	}
}

