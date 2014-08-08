using System;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * Simulation controller takes data from NetworkModel and SimulationModel and sets up simulation framework
	 */
	public class SimulationController
	{
		private static readonly ILog log=LogManager.GetLogger(typeof(SimulationController));
		private NetworkModel network_model;
		private SimulationModel simulation_model;
		private MFF_NPRG031.Model framework_model;
		private ResultModel result_model;
		private int endNodeCounter,networkNodeCounter,serverNodeCounter,addressCounter,nodeCounter;
		private Node[] nodes;
		private ServerNode[] servers;
		private LinkedList<Link> links;
		private Dictionary<string,Node> node_names;

		/**
		 * Populated result model to pick-up
		 */ 
		public ResultModel Results{
			get{
				return result_model;
			}
		}

		/**
		 * Constructor stores Network and Simulation models for future reference
		 * @param nm Network Model
		 * @param sm Simulation Model
		 * @throws ArgumentException If any of models is null or the network model provided is not valid
		 */
		public SimulationController (NetworkModel nm,SimulationModel sm)
		{
			if ((nm == null) || (sm == null))
				throw new ArgumentNullException ("[SimulationController] No model provided should be null");
			if (nm.Valid)
				this.network_model = nm;
			else
				throw new ArgumentException ("[SimulationController] Network model not valid");
			this.simulation_model = sm;
			this.node_names = new Dictionary<string, Node> ();
		}

		/**
		 * Initializes simulation framework: create Model, create necessary Nodes, set them up and place into Calendar
		 */
		private void InitializeFramework(){
			//create nodes
			createNodes ();
			//create links
			createLinks ();
			//create model
			createModel ();
			//create events
			createEvents ();
		}

		/**
		 * For each node in Network Model create an appropriate Node instance and store it in nodes array. Nodes array and counters are initialized here.
		 * Address generated here for nodes requiring an address.
		 * @throws InvalidOperationException if found unidentified node type
		 */
		private void createNodes(){
			endNodeCounter = 0;
			networkNodeCounter = 0;
			serverNodeCounter = 0;
			addressCounter = 0;
			nodeCounter = 0;
			nodes = new Node[network_model.NodeCount];
			this.servers = new ServerNode[network_model.ServerNodeCount];
			for (int i=0; i<network_model.NodeCount; i++) {
				switch (network_model.GetNodeType (i)) {
				case NetworkModel.END_NODE:
					string name = network_model.GetNodeName (i);
					EndNode en = new EndNode (name, network_model.GetNodeAddr (i), network_model.GetEndNodeMaxPacketSize (name),simulation_model.IsRandomTalker (name));
					if (node_names.ContainsKey (name))
						throw new ArgumentException ("Duplicate node name");
					node_names.Add (name, en);
					nodes [nodeCounter] = en;
					endNodeCounter++;
					addressCounter++;
					nodeCounter++;
					break;
				case NetworkModel.NETWORK_NODE:
					int interfaces = network_model.GetConnectionCount (i);
					NetworkNode nn = new NetworkNode (network_model.GetNodeName(i), interfaces,simulation_model.MaxHop);
					nodes [nodeCounter] = nn;
					networkNodeCounter++;
					nodeCounter++;
					break;
				case NetworkModel.SERVER_NODE:
					ServerNode sn = new ServerNode (network_model.GetNodeName(i), network_model.GetNodeAddr(i));
					nodes [nodeCounter] = sn;
					servers [serverNodeCounter] = sn;
					serverNodeCounter++;
					nodeCounter++;
					addressCounter++;
					break;
				default:
					throw new InvalidOperationException ("[SimulationController.createNodes] Unidentified node type");
				}
			}
		}

		/**
		 * <p>For each pair of nodes where connection is marked, create a new Link instance and register the link to each node. New links are stored in LinkedList<Link> links 
		 * initialized here (as their count is unknown)</p>
		 * <p>Test is necessary to verify links are created properly according to the model</p>
		 * @throws InvalidOperationException if any node is of unidentified type, the network model is null, the nodes array is null or length of the nodes array don't match node 
		 * count in the network model
		 */
		private void createLinks(){
			links = new LinkedList<Link> ();
			if ((network_model != null) && (nodes != null) && (nodes.Length == network_model.NodeCount)) {
				for (int i = 0; i < network_model.NodeCount; i++) {
					Node x = nodes [i];
					int j = i+1;//[i,i] connection forbidden
					while (j<network_model.NodeCount) {
						Node y = nodes [j];
						if (network_model.AreConnected (i, j)){
							//TESTME links parsed correctly??
							Link l = new Link (network_model.GetLinkName(i,j), network_model.LinkCapacity(i,j), x, y,network_model.GetLinkToggleProbability(i,j));
							if (x is EndNode)
								(x as EndNode).Link = l;
							else if (x is NetworkNode)
								(x as NetworkNode).ConnectLink (l);
							else if (x is ServerNode)
								(x as ServerNode).Link = l;
							else
								throw new InvalidOperationException ("Node " + x + "is not EndNode nor NetworkNode nor ServerNode");
							if (y is EndNode)
								(y as EndNode).Link = l;
							else if (y is NetworkNode)
								(y as NetworkNode).ConnectLink (l);
							else if (y is ServerNode)
								(y as ServerNode).Link = l;
							else
								throw new InvalidOperationException ("Node " + y + " is not EndNode nor NetworkNode nor ServerNode");
							links.AddLast (l);
						}
						j++;
					}
				}
			} else
				throw new InvalidOperationException ("[SimulationController.createLinks] Network model null or nodes array null or length of nodes array don't match " +
					"node count in network model");
		}

		/**
		 * Unless simulation model is null initialize Model with appropriate Time To Run
		 * @throws InvalidOperationException if SimulationModel is null
		 */
		private void createModel(){
		if (simulation_model != null) {
				if (servers != null) {
					framework_model = new MFF_NPRG031.Model (simulation_model.Time, servers);
				}else
					throw new InvalidOperationException ("Servers array empty");
		} else
			throw new InvalidOperationException ("[SimulationController.createModel] SimulationModel null");
		}

		/**
		 * Create loaded events from SimulationModel
		 */
		private void createEvents(){
			if ((framework_model != null)&&(network_model!=null)) {
				foreach (SimulationModel.Event e in simulation_model.GetEvents()) {
					Node from_node; 
					int node2_num = network_model.GetNodeNum (e.node2);
					int from_addr = network_model.GetNodeAddr (network_model.GetNodeNum (e.node1));
					if (node_names.TryGetValue (e.node1, out from_node)) {
						if (from_node is EndNode && network_model.GetNodeType (node2_num).Equals (NetworkModel.SERVER_NODE)) {
							//schedule node e.node1 to state SEND (with packet from e.node1 to e.node2 of e.size as parameter) at time e.when
							MFF_NPRG031.State st = new MFF_NPRG031.State (MFF_NPRG031.State.state.SEND,new Packet (from_addr, network_model.GetNodeAddr (node2_num), e.size));
							MFF_NPRG031.Event ev = new MFF_NPRG031.Event (from_node, st, e.when);
							framework_model.K.Schedule (ev);
						} else
							throw new ArgumentException ("Must send from END NODE to SERVER NODE");
					} else
						throw new ArgumentException ("Node not found: " + e.node1);
				}
			}else throw new InvalidOperationException ("[SimulationController.createEvents] FrameworkModel or NetworkModel null");
		}

		/**
		 * Stores statistics into Result Model
		 */
		private void PopulateResultModel(){
			result_model = new ResultModel (endNodeCounter, serverNodeCounter, networkNodeCounter, links.Count);
			foreach (Node n in nodes) {
				if (n is EndNode) {
					EndNode en = n as EndNode;
					result_model.SetNewEndNodeResult (
						en.Name, en.Address, en.PacketsSent, en.PacketsReceived, en.PacketsMalreceived, en.TimeWaited, en.GetPercentageTimeIdle (framework_model), en.AverageWaitTime,
						en.AveragePacketSize);
				} else if (n is ServerNode) {
					ServerNode sn = n as ServerNode;
					result_model.SetNewServerNodeResult (sn.Name, sn.Address, sn.PacketsProcessed,sn.PacketsMalreceived, sn.TimeWaited, sn.GetPercentageTimeIdle (framework_model), sn.AverageWaitTime);
				} else if (n is NetworkNode) {
					NetworkNode nn = n as NetworkNode;
					result_model.SetNewNetworkNodeResult (nn.Name,nn.PacketsProcessed,nn.TimeWaited,nn.GetPercentageTimeIdle(framework_model),nn.AverageWaitTime,nn.PacketsDropped,nn.PercentagePacketsDropped,nn.RoutingMessagesSent,nn.RoutingMessagesReceived,nn.RoutingMessagesPercentageProcessed);
				}
			}
			foreach (Link l in links) {
				result_model.SetNewLinkResult (l.Name, l.PacketsCarried, l.PacketsDropped, l.DropPercentage, l.ActiveTime, l.PassiveTime, l.PercentageTimeIdle);
			}
		}

		/**
		 * Initialize framework, run simulation, populate result model
		 */
		public void Run(){
			log.Info ("Initializing framework");
			InitializeFramework ();
			log.Info ("Running simulation");
			framework_model.Simulate ();
			log.Info ("Populating result model");
			PopulateResultModel ();
		}
	}
}