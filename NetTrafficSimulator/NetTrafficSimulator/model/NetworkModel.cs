using System;
using System.Collections.Generic;

namespace NetTrafficSimulator
{
	/**
	 * <p>NetworkModel generates network map based on data provided by user.</p>
	 * <p>Network map is a graph represented as adjacency matrix.</p>
	 * <p>Node can be of type END_NODE, SERVER_NODE, NETWORK_NODE or UNIDENTIFIED_NODE</p>
	 */
	public class NetworkModel
	{
		//graf site
		//uzly a spoje

		//end node - spojen nejvyse s jednim
		//network node - mnoho spojeni
		//server node - spojen nejvyse s jednim

		/**
		 * Information about link
		 */
		public struct link_rec{
			private int cap;
			/**
			 * Link capacity
			 */
			public int capacity{
				get{
					return this.cap;
				}
				set{
					if (value >= 0)
						this.cap = value;
					else
						throw new ArgumentException ("Negative link capacity "+value+" ("+name+")");
				}
			}
			/**
			 * Link name
			 */
			public string name;

			/**
			 * Link drop probability
			 */
			private decimal toggle_probability;
			public decimal ToggleProb{
				get{
					return this.toggle_probability;
				}
				set{
					if ((value >= 0.0m) && (value <= 1.0m))
						this.toggle_probability = value;
					else
						throw new ArgumentException ("Toggle probability must be between 0.0 and 1.0, was "+value);
				}
			}
		}

		//linky - node x spojen s node y
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
		private int node_count;
		/**
		 * Adjacency matrix of the network: links[x,y] is positive if there is a direct connection between nodes x and y
		 * and the number represents the link capacity
		 */
		private link_rec[,] links;
		/**
		 * link_count[x] is amount of links connected to a node x
		 */
		private int[] link_count;
		/**
		 * types[x] is type of a node x
		 */
		private int[] types;
		private int[] addr;//addr[x] is address of node x
		private Dictionary<string,int> n_name;//node name
		private Dictionary<string,int> en_mps;//end node max packet size
		/**
		 * Server node count
		 */
		private int SNCount;

		/**
		 * Creates a NetworkModel with given amount of nodes
		 * @param node_count amount of nodes in model
		 * @throws ArgumentOutOfRangeException if given node_count is less than 0
		 */
		public NetworkModel (int node_count)
		{
			if (node_count >= 0) {
				this.node_count = node_count;
				this.links = new link_rec[node_count, node_count];
				this.types = new int[node_count];
				this.link_count=new int[node_count];
				this.addr=new int[node_count];
				this.n_name = new Dictionary<string, int> ();
				this.en_mps = new Dictionary<string, int> ();

				for (int i=0; i<node_count; i++) {
					for (int j=0; j<node_count; j++) {
						links [i, j].capacity = NO_CONNECTION;
					}
				}

				for (int i=0; i<node_count; i++) {
					types [i] = UNIDENTIFIED_NODE;
					link_count [i] = 0;
				}
				this.SNCount = 0;
			} else
				throw new ArgumentOutOfRangeException ("[new NetworkModel("+node_count+"] " + ILLEGAL_PARAMETER);
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
		public bool AreConnected(int x,int y){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count))
				return links [x, y].capacity!=NO_CONNECTION;
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.AreConnected("+x+","+y+")] "+ILLEGAL_PARAMETER);
		}

		/**
		 * Returns capacity of a link between two nodes
		 * @param x node
		 * @param y node
		 * @return link capacity (positive integer) or zero if there's no link between particular nodes
		 */
		public int LinkCapacity(int x,int y){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count))
				return links[x,y].capacity;
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.LinkCapacity("+x+","+y+")] "+ILLEGAL_PARAMETER);
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
		public void SetConnected(int x,int y,int capacity,decimal toggle_probability){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count)&&(capacity>0)&&(toggle_probability>=0.0m)&&(toggle_probability<=1.0m)) {
				if (links [x, y].capacity == NO_CONNECTION) {
					link_count [x]++;
					link_count [y]++;
				}
				links [x, y].capacity = capacity;
				links [y, x].capacity = capacity;
				links [x, y].ToggleProb = toggle_probability;
			}
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.SetConnected("+x+","+y+","+capacity+","+toggle_probability+")] "+ILLEGAL_PARAMETER);
		}
		/**
		 * <p>Ensures there's no direct link between nodes x and y</p>
		 * <p>If there previously was a direct link, also decrements link_count counters</p>
		 * @param x node
		 * @param y node
		 * @throws ArgumentOutOfRangeException if x or y are incorrect
		 */
		public void SetDisconnected(int x,int y){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count)) {
				if (links [x, y].capacity!=NO_CONNECTION) {
					link_count [x]--;
					link_count [y]--;
				}
				links [x, y].capacity = NO_CONNECTION;
				links [y, x].capacity = NO_CONNECTION;
			} else
				throw new ArgumentOutOfRangeException ("[NetworkModel.SetDisconnected(" + x + "," + y + ")] "+ILLEGAL_PARAMETER);
		}
		/**
		 * For given node returns value of link_count counter
		 * @param node node of interest
		 * @return amount of direct links connected to that node
		 * @throws ArgumentOutOfRangeException if node is incorrect
		 */
		public int GetConnectionCount(int node){
			if ((node >= 0) && (node < node_count))
				return link_count [node];
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.GetConnectionCount(" + node + ")] "+ILLEGAL_PARAMETER);
		}

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
		public int GetNodeType(int node){
			if ((node >= 0) && (node < node_count))
				return types [node];
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.GetNodeType(" + node + ")] "+ILLEGAL_PARAMETER);
		}

		/**
		 * For given node sets a type of node
		 * @param node a node of interest
		 * @param type one of following constants: END_NODE,SERVER_NODE,NETWORK_NODE
		 * @throws ArgumentOutOfRangeException if given node is incorrect or type is UNIDENTIFIED_NODE or is not any of declared constants
		 */
		public void SetNodeType(int node,int type){
			if ((node >= 0) && (node < node_count) && ((type == END_NODE) || (type == NETWORK_NODE)))
				types [node] = type;
			else if (type == SERVER_NODE) {
				SNCount++;
				types [node] = type;
			}
			else
				throw new ArgumentOutOfRangeException("[NetworkModel.SetNodeType("+node+","+type+")] "+ILLEGAL_PARAMETER);
		}

		//properties
		/**
		 * Amount of nodes in the model
		 */
		public int NodeCount{
			get{
				return node_count;
			}
		}
		/**
		 * Adjacency matrix for the network
		 */
		public link_rec[,] Link{
			get{
				return links;
			}
		}
		/**
		 * Types of nodes
		 */
		public int[] Type{
			get{
				return types;
			}
		}
		/**
		 * Amount of links connected to nodes
		 */
		public int[] LinkCount{
			get{
				return link_count;
			}
		}

		/**
		 * <p>Prints matrix of adjacency and node type for each node on Console</p>
		 * <p>END_NODE is marked as EN</p>
		 * <p>SERVER_NODE is marked as SN</p>
		 * <p>NETWORK_NODE is marked as NN</p>
		 * <p>otherwise type is marked as N/A</p>
		 */
		public void Print(){
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
		}

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
		 */
		public bool Valid{
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
		}

		public void SetNodeName(int node,string name){
			if ((node >= 0) && (node < node_count)) {
				if (n_name.ContainsKey (name))
					n_name.Remove (name);
				this.n_name.Add (name, node);
			}
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.SetNodeName(" + node + "," + name + ")] " + ILLEGAL_PARAMETER);
		}

		public string GetNodeName(int node){
			foreach (string key in n_name.Keys) {
				int num=-255;
				n_name.TryGetValue (key, out num);
				if (num == node)
					return key;
			}
			throw new ArgumentException ("Not found");
		}

		public int GetNodeNum(string node){
			int num;
			if (!n_name.TryGetValue (node, out num))
				throw new ArgumentException ("Node not found "+node);
			else
				return num;
		}

		public void SetNodeAddr(int node,int addr){
			if ((node >= 0) && (node < node_count)&&(addr>=0)&&((types[node]==END_NODE)||(types[node]==SERVER_NODE)))
				this.addr[node]=addr;
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.SetNodeAddr(" + node + "," + addr + ")] " + ILLEGAL_PARAMETER);
		}

		public int GetNodeAddr(int node){
			if ((node >= 0) && (node < node_count)&&((types[node]==END_NODE)||(types[node]==SERVER_NODE)))
				return this.addr [node];
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.GetNodeAddr(" + node+")] " + ILLEGAL_PARAMETER);
		}

		public bool HaveNode(string name){
			return this.n_name.ContainsKey (name);
		}

		public void SetLinkName(int x,int y,string name){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count)&&links[x,y].capacity!=NO_CONNECTION) 
				links [x, y].name = name;
			else
				throw new ArgumentException ("[NetworkModel.SetLinkName(" + x + "," + y + "," + name + ")] " + ILLEGAL_PARAMETER);
		}

		public string GetLinkName(int x,int y){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count))
				return links [x, y].name;
			else 
				throw new ArgumentException ("[NetworkModel.GetLinkName(" + x + "," + y + ")] " + ILLEGAL_PARAMETER);
		}

		/**
		 * Sets max packet size for EndNode
		 */
		public void SetEndNodeMaxPacketSize(string n,int mps){
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
		}

		public int GetEndNodeMaxPacketSize(string n){
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
		}

		public decimal GetLinkToggleProbability(int x,int y){
			if ((x >= 0) && (x < node_count) && (y >= 0) && (y < node_count))
				return links[x,y].ToggleProb;
			else
				throw new ArgumentOutOfRangeException ("[NetworkModel.GetLinkToggleProbability("+x+","+y+")] "+ILLEGAL_PARAMETER);
		}
	}
}

