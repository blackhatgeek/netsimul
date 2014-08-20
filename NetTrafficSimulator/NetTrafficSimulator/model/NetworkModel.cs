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
		static readonly ILog log = LogManager.GetLogger(typeof(NetworkModel));

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
		 * Amount of nodes in the model
		 */
		//private int node_count;
		/**
		 * Adjacency matrix of the network: links[x,y] is positive if there is a direct connection between nodes x and y
		 * and the number represents the link capacity
		 */
//		private link_rec[,] links;
		/**
		 * link_count[x] is amount of links connected to a node x
		 */
//		private int[] link_count;
		/**
		 * types[x] is type of a node x
		 */
//		private int[] types;
//		private int[] addr;//addr[x] is address of node x
//		private Dictionary<string,int> n_name;//node name
		//private Dictionary<string,int> en_mps;//end node max packet size
//		private Dictionary<int,int> nn_default;//default route for network node
//		private Dictionary<string,link_rec> l_name;//link name
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
			//if (node_count >= 0) {
			//	this.node_count = node_count;
			//	this.links = new link_rec[node_count, node_count];
			//	this.types = new int[node_count];
			//	this.link_count=new int[node_count];
			//	this.addr=new int[node_count];
			//	this.n_name = new Dictionary<string, int> ();
				//this.en_mps = new Dictionary<string, int> ();

			//	for (int i=0; i<node_count; i++) {
			//		for (int j=0; j<node_count; j++) {
			//			links [i, j].capacity = NO_CONNECTION;
			//		}
			//	}

			//	for (int i=0; i<node_count; i++) {
			//		types [i] = UNIDENTIFIED_NODE;
			//		link_count [i] = 0;
			//	}
				this.SNCount = 0;

			//	this.nn_default = new Dictionary<int, int> ();
			//	this.l_name = new Dictionary<string, link_rec> ();
			//} else
			//	throw new ArgumentOutOfRangeException ("[new NetworkModel("+node_count+"] " + ILLEGAL_PARAMETER);
		}

		//getters and setters
		//connections
		/**
		 * True if there is a direct link between nodes x and y
		 * @param x node
		 * @param y node
		 * @return if there's a link
		 * @throws ArgumentOutOfRangeException when x or y are incorrect
		 */
		//UPDATED
		/*public bool AreConnected(int x,int y){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count))
				return links [x, y].capacity!=NO_CONNECTION;
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.AreConnected("+x+","+y+")] "+ILLEGAL_PARAMETER);
		}*/

		/**
		 * Returns capacity of a link between two nodes
		 * @param x node
		 * @param y node
		 * @return link capacity (positive integer) or zero if there's no link between particular nodes
		 */
		//UPDATED
		/*public int LinkCapacity(int x,int y){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count))
				return links[x,y].capacity;
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.LinkCapacity("+x+","+y+")] "+ILLEGAL_PARAMETER);
		}*/

		/**
		 * <p>Marks a direct link between nodes x and y with given capacity.</p>
		 * <p>If there wasn't link yet, increments appropriate link_count counters</p>
		 * @param x node
		 * @param y node
		 * @param capacity positive integer for link capacity
		 * @param toggle_probability probability link toggles (switches from active to passive ... appears/disappears)
		 * @throws ArgumentOutOfRangeException if any of x or y or capacity are incorrect
		 */
		//UPDATED
		/*public void SetConnected(int x,int y,int capacity,decimal toggle_probability){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count)&&(capacity>0)&&(toggle_probability>=0.0m)&&(toggle_probability<=1.0m)) {
				if (links [x, y].capacity == NO_CONNECTION) {
					link_count [x]++;
					link_count [y]++;
				} else
					log.Warn ("Overwriting connection between " + GetNodeName (x) + " and " + GetNodeName (y));

				links [x, y].capacity = capacity;
				links [y, x].capacity = capacity;
				links [x, y].ToggleProb = toggle_probability;
				links [y, x].ToggleProb = toggle_probability;
				links [x, y].A = x;
				links [x, y].B = y;
				links [y, x].A = x;
				links [y, x].B = y;
			}
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.SetConnected("+x+","+y+","+capacity+","+toggle_probability+")] "+ILLEGAL_PARAMETER);
		}*/
		/**
		 * <p>Ensures there's no direct link between nodes x and y</p>
		 * <p>If there previously was a direct link, also decrements link_count counters</p>
		 * @param x node
		 * @param y node
		 * @throws ArgumentOutOfRangeException if x or y are incorrect
		 */
		//UPDATED
		/*public void SetDisconnected(int x,int y){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count)) {
				if (links [x, y].capacity!=NO_CONNECTION) {
					link_count [x]--;
					link_count [y]--;
				}
				links [x, y].capacity = NO_CONNECTION;
				links [y, x].capacity = NO_CONNECTION;
			} else
				throw new ArgumentOutOfRangeException ("[NetworkModel.SetDisconnected(" + x + "," + y + ")] "+ILLEGAL_PARAMETER);
		}*/
		/**
		 * For given node returns value of link_count counter
		 * @param node node of interest
		 * @return amount of direct links connected to that node
		 * @throws ArgumentOutOfRangeException if node is incorrect
		 */
		//UPDATE NOT NEEDED? -> DELETE
		/*public int GetConnectionCount(int node){
			if ((node >= 0) && (node < node_count))
				return link_count [node];
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.GetConnectionCount(" + node + ")] "+ILLEGAL_PARAMETER);
		}*/

		/**
		 * Amount of servers in model
		 */
		//UPDATE NOT NEEDED -> LEAVE AS IS
		public int ServerNodeCount{
			get{
				return SNCount;
			}
		}

		//types
		/**
		 * For given node returns a type of node
		 * @param node a node of interest
		 * @return one of following constants: END_NODE, SERVER_NODE, NETWORK_NODE, UNIDENTIFIED_NODE
		 * @throws ArgumentOutOfRangeException if given node is incorrect
		 */
		//UPDATED
		/*public int GetNodeType(int node){
			if ((node >= 0) && (node < node_count))
				return types [node];
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.GetNodeType(" + node + ")] "+ILLEGAL_PARAMETER);
		}*/

		/**
		 * For given node sets a type of node
		 * @param node a node of interest
		 * @param type one of following constants: END_NODE,SERVER_NODE,NETWORK_NODE
		 * @throws ArgumentOutOfRangeException if given node is incorrect or type is UNIDENTIFIED_NODE or is not any of declared constants
		 */
		//UPDATED
		/*public void SetNodeType(int node,int type){
			if ((node >= 0) && (node < node_count) && ((type == END_NODE) || (type == NETWORK_NODE)))
				types [node] = type;
			else if (type == SERVER_NODE) {
				SNCount++;
				types [node] = type;
			}
			else
				throw new ArgumentOutOfRangeException("[NetworkModel.SetNodeType("+node+","+type+")] "+ILLEGAL_PARAMETER);
		}*/

		//properties
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
		 * Adjacency matrix for the network
		 */
		//NO UPDATE NEEDED -> DELETE
		/*public link_rec[,] Link{
			get{
				return links;
			}
		}*/
		/**
		 * Types of nodes
		 */
		//NO UPDATE NEEDED -> DELETE
		/*public int[] Type{
			get{
				return types;
			}
		}*/
		/**
		 * Amount of links connected to nodes
		 */
		//NO UPDATE NEEDED -> DELETE
		/*public int[] LinkCount{
			get{
				return link_count;
			}
		}*/

		/**
		 * <p>Prints matrix of adjacency and node type for each node on Console</p>
		 * <p>END_NODE is marked as EN</p>
		 * <p>SERVER_NODE is marked as SN</p>
		 * <p>NETWORK_NODE is marked as NN</p>
		 * <p>otherwise type is marked as N/A</p>
		 */
		//NO UPDATE NEEDED -> DELETE
		/*public void Print(){
			//header
			Console.Write ("Node\tType\t");
			for (int i = 1; i < node_count; i++) {
				Console.Write (i + "\t");
			}
			Console.WriteLine (node_count);
			//contents
			for (int i=0; i<node_count; i++) {
				String ntype;
				switch (types [i]) {
				case END_NODE:
					ntype = "EN";
					break;
				case SERVER_NODE:
					ntype = "SN";
					break;
				case NETWORK_NODE:
					ntype = "NN";
					break;
				default:
					ntype = "N/A";
					break;
				}
				Console.Write ((i+1) + "\t\t" + ntype + "\t");
				for (int j=0; j<node_count-1; j++) {
					if (links [i, j].capacity!=NO_CONNECTION)
						Console.Write ("+");
					Console.Write ("\t");
				}
				if (links [i, node_count-1].capacity!=NO_CONNECTION)
					Console.Write ("+");
				Console.WriteLine ("");
			}
		}*/

		/**
		 * Validates the model
		 * Model is valid if all following constraints are met:
		 * - link count for each node is greater or equal to zero
		 * - link count for each node is less than amount of nodes in model
		 * - no node is connected to itself
		 * - only if node is NETWORK_NODE, it's link count is greater than 1
		 * - no node in model is UNIDENTIFIED_NODE
		 * - adjacency matrix is symmetric
		 * - each link capacity is non-negative
		 * @return if model is valid or not
		 * TODO update
		 */
		//NO UPDATE NEEDED -> DELETE
		/*public bool Valid{
			get{
				bool valid=true;
				int i = 0;
				while((i<node_count)&&valid){
					if((link_count[i]<0)||(link_count[i]>=node_count))//pokud link_count[i]==node_count pak je node spojen se sebou
					   valid=false;
					if (links [i, i].capacity!=NO_CONNECTION)
						valid = false;
					if ((types [i] != NETWORK_NODE) && (link_count [i] > 1))
						valid = false;
					if (types [i] == UNIDENTIFIED_NODE)
						valid = false;
					i++;
				}
				if(valid)
				for(i=0;i<node_count-1;i++){
						int j = i;
						while (j<node_count) {
							if (links [i, j].capacity != links [j, i].capacity)
								valid = false;
							if (links [i, j].capacity < 0)
								valid = false;
							j++;
						}
					}
				return valid;
			}
		}*/

		/**
		 * <p>Notes a name for a node</p>
		 * <p>Here we don't care if the node is namable or not</p>
		 * @param node node number
		 * @param name node name
		 * @throws ArgumentOutOfRangeException node number out of range
		 */
		//UPDATED
		/*public void SetNodeName(int node,string name){
			if ((node >= 0) && (node < node_count)) {
				if (n_name.ContainsKey (name))
					n_name.Remove (name);
				this.n_name.Add (name, node);
			}
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.SetNodeName(" + node + "," + name + ")] " + ILLEGAL_PARAMETER);
		}*/

		/**
		 * For given node number return node name
		 * @param node node number
		 * @return node name
		 * @throws ArgumentException node not found
		 */
		//NO UPDATE NEEDED -> DELETE
		/*public string GetNodeName(int node){
			foreach (string key in n_name.Keys) {
				int num=-255;
				n_name.TryGetValue (key, out num);
				if (num == node)
					return key;
			}
			throw new ArgumentException ("Not found");
		}*/

		/**
		 * For given node name get node number
		 * @param node node name
		 * @return node number
		 * @throws ArgumentException node not found
		 */
		//NO UPDATE NEEDED -> DELETE
		/*public int GetNodeNum(string node){
			int num;
			if (!n_name.TryGetValue (node, out num))
				throw new ArgumentException ("Node not found "+node);
			else
				return num;
		}*/

		/**
		 * <p>Set network address for given node number. Here we care if node is addressable or not</p>
		 * @param node node number
		 * @param addr new network address for node
		 * @throws ArgumentOutOfRangeException invalid node number, negative address, node not EndNode not ServerNode
		 */
		//UPDATED
		/*public void SetNodeAddr(int node,int addr){
			if ((node >= 0) && (node < node_count)&&(addr>=0)&&((types[node]==END_NODE)||(types[node]==SERVER_NODE)))
				this.addr[node]=addr;
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.SetNodeAddr(" + node + "," + addr + ")] " + ILLEGAL_PARAMETER);
		}*/

		/**
		 * Get network address for node number
		 * @param node node number
		 * @return network address of the node
		 * @throws ArgumentOutOfRangeException invalid node number or type of node is not END NODE or SERVER NODE
		 */
		//UPDATED
		/*public int GetNodeAddr(int node){
			if ((node >= 0) && (node < node_count)&&((types[node]==END_NODE)||(types[node]==SERVER_NODE)))
				return this.addr [node];
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.GetNodeAddr(" + node+")] " + ILLEGAL_PARAMETER);
		}*/

		/**
		 * Do we have a node with name provided in our model
		 * @param name node name
		 * @return exists node with name given in the model
		 */
		//UPDATED LOCALLY
		public bool HaveNode(string name){
			return this.node_records.ContainsKey(name);
		}

		/**
		 * Set name for link
		 * @param x a node number of a node connected to the link
		 * @param y a node number of a node connected to the link
		 * @param name new node name
		 * @throws ArgumentException invalid node number or no connection between x and y
		 */
		//UPDATED
		/*public void SetLinkName(int x,int y,string name){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count) && links [x, y].capacity != NO_CONNECTION) {
				if (name == null)
					throw new ArgumentNullException ("Link name null");
				log.Debug ("Link: " + x + "<--->" + y + " new name:" + name);
				links [x, y].name = name;
				links [y, x].name = name;
				if (l_name.ContainsKey (name)) {
					l_name.Remove (name);
				}
				l_name.Add (name,links [x, y]);
			}
			else
				throw new ArgumentException ("[NetworkModel.SetLinkName(" + x + "," + y + "," + name + ")] " + ILLEGAL_PARAMETER);
		}*/

		/**
		 * For given node numbers get the name of link between them
		 * @param x a node number of a node connected to the link
		 * @param y a node number of a node connected to the link
		 * @return node name
		 * @throws ArgumentException invalid node number or no connection between x and y
		 */ 
		//NO UPDATE NEEDED -> DELETE
		/*public string GetLinkName(int x,int y){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count) && links [x, y].capacity != NO_CONNECTION) {
				if (links [x, y].name != null)
					return links [x, y].name;
				else
					throw new ArgumentNullException ("Link name null: " + x + "<-->" + y);
			}
			else 
				throw new ArgumentException ("[NetworkModel.GetLinkName(" + x + "," + y + ")] " + ILLEGAL_PARAMETER);
		}*/

		/**
		 * Sets max packet size for EndNode when using random talker communication
		 * @param n end node name
		 * @param mps max packet size
		 * @throws ArgumentException mps negative, node name not found, node not END_NODE
		 */
		//UPDATED
		/*public void SetEndNodeMaxPacketSize(string n,int mps){
			int node;
			if (mps >= 0)
				if (n_name.TryGetValue (n, out node))//existuje takovy node
					if (types [node] == END_NODE){//je spravneho typu
						if (this.en_mps.ContainsKey (n))
							this.en_mps.Remove (n);
						this.en_mps.Add (n, mps);
					}
					else
						throw new ArgumentException ("[NetworkModel.SetEndNodeMaxPacketSize(" + n + "," + mps + ")] " + ILLEGAL_PARAMETER);
				else
					throw new ArgumentException ("[NetworkModel.SetEndNodeMaxPacketSize(" + n + "," + mps + ")] " + ILLEGAL_PARAMETER);
			else
				throw new ArgumentException ("[NetworkModel.SetEndNodeMaxPacketSize(" + n + "," + mps + ")] " + ILLEGAL_PARAMETER);
		}*/

		/**
		 * Gets max packet size for EndNode when using random talker communication
		 * @param n end node name 
		 * @return max packet size for an end node
		 * @throws ArgumentException no EndNode with such name
		 */
		//UPDATED
		/*public int GetEndNodeMaxPacketSize(string n){
			if (this.en_mps.ContainsKey (n)) {
				int mps;
				this.en_mps.TryGetValue (n, out mps);
				return mps;
			} else {
				if (n_name.ContainsKey (n)) {
					int node;
					this.n_name.TryGetValue (n, out node);
					if (types [node] == END_NODE) {
						this.en_mps.Add (n, DEFAULT_END_NODE_MPS);
						return DEFAULT_END_NODE_MPS;
					} else//neni to END_NODE
						throw new ArgumentException ("[NetworkModel.GetEndNodeMaxPacketSize(" + n + ")] " + ILLEGAL_PARAMETER);
				} else //neni to NODE
					throw new ArgumentException ("[NetworkModel.GetEndNodeMaxPacketSize(" + n + ")] " + ILLEGAL_PARAMETER);
			}
		}*/

		/**
		 * Return toggle probability of a link between two nodes
		 * @param x a node connected to the link
		 * @param y a node connected to the link
		 * @return link toggle probability of the link
		 */
		//UPDATED
		/*public decimal GetLinkToggleProbability(int x,int y){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count))
				return links[x,y].ToggleProb;
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.GetLinkToggleProbability("+x+","+y+")] "+ILLEGAL_PARAMETER);
		}*/

		//UPDATED
		/*public void SetDefaultRoute(int node, int gateway){
			if ((node >= 0) && (node < node_count) && (gateway >= 0) && (gateway < node_count) && (node != gateway)) {
				nn_default.Add (node, gateway);
			} else
				throw new ArgumentOutOfRangeException ("[NetworkModel.SetDefaultRoute(" + node + "," + gateway + ")] " + ILLEGAL_PARAMETER);
		}*/

		//UPDATED
		/*public string GetNetworkNodeDefaultRoute(int node){
			int route;
			if (nn_default.TryGetValue (node, out route))
				return links [node, route].name;
			else
				throw new ArgumentException ("[NetworkModel.GetDefaultRoute(" + node + ")] " + ILLEGAL_PARAMETER);
		}*/

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

