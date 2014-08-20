using System;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * Simulation controller takes data from NetworkModel and SimulationModel and sets up simulation framework, runs a simulation, collects results and populates ResultModel
	 */
	public class SimulationController
	{
		private static readonly ILog log=LogManager.GetLogger(typeof(SimulationController));
		private NetworkModel network_model;
		private SimulationModel simulation_model;
		private MFF_NPRG031.Model framework_model;
		private ResultModel result_model;
		private int endNodeCounter,networkNodeCounter,serverNodeCounter,nodeCounter,linkCounter;
		private Node[] nodes;
		private ServerNode[] servers;
		private LinkedList<EndNode> randomTalkers;//TODO: count counts and make an array
		private LinkedList<NetworkNode> routers;//TODO: count counts and make an array
		private Packet[] tracedPackets;
		private Dictionary<string,Node> node_names;
		private int traced;
		private string[] node_names_array;
		private Link[] links;

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
			//if (nm.Valid)
			this.network_model = nm;
			this.node_names_array = nm.GetNodeNames ();
			this.links = new Link[network_model.GetLinkCount()];
			//else
			//	throw new ArgumentException ("[SimulationController] Network model not valid");
			this.simulation_model = sm;
		}

		/**
		 * Initializes simulation framework: create Model, create necessary Nodes, set them up and place into Calendar
		 */
		private void InitializeFramework(){
			//create endpoint nodes
			createEndpointNodes ();
			//create model
			createModel ();
			//create network nodes
			createNetworkNodes ();
			//create links
			createLinks ();
			//initialize processes
			initializeProcesses ();
		}


		private void createNetworkNodes(){
			log.Debug ("Create network nodes");
			networkNodeCounter = 0;
			this.routers = new LinkedList<NetworkNode> ();

			foreach (string name in node_names_array) {
				switch (network_model.GetNodeType (name)) {
				case NetworkModel.NETWORK_NODE:
					log.Debug ("NN:" + name);
					int interfaces = network_model.GetNetworkNodeInterfacesCount (name);
					NetworkNode nn = new NetworkNode (name, interfaces, simulation_model.MaxHop, framework_model);
					nodes [nodeCounter] = nn;
					node_names.Add (name, nn);
					networkNodeCounter++;
					routers.AddLast (nn);
					nodeCounter++;
					break;
				}
			}
		}
		/**
		 * For each node in Network Model create an appropriate Node instance and store it in nodes array. Nodes array and counters are initialized here. Address set here for nodes requiring an address.
		 * @throws InvalidOperationException if found unidentified node type
		 */
		private void createEndpointNodes(){
			log.Debug ("Create endpoint nodes");
			endNodeCounter = 0;
			serverNodeCounter = 0;
			nodeCounter = 0;
			nodes = new Node[network_model.NodeCount];
			this.servers = new ServerNode[network_model.ServerNodeCount];
			this.node_names = new Dictionary<string, Node> ();
			this.randomTalkers = new LinkedList<EndNode> ();

			foreach (string name in node_names_array) {
				switch (network_model.GetNodeType (name)) {
				case NetworkModel.END_NODE:
					log.Debug ("EN:" + name);
					bool rt = simulation_model.IsRandomTalker (name);
					EndNode en = new EndNode (name, network_model.GetEndpointNodeAddr (name));
					if (rt)
						randomTalkers.AddLast (en);
					node_names.Add (name, en);
					nodes [nodeCounter] = en;
					endNodeCounter++;
					nodeCounter++;
					break;
				case NetworkModel.NETWORK_NODE:
					break;
				case NetworkModel.SERVER_NODE:
					log.Debug ("SN:" + name);
					ServerNode sn = new ServerNode (name, network_model.GetEndpointNodeAddr (name));
					node_names.Add (name, sn);
					nodes [nodeCounter] = sn;
					serverNodeCounter++;
					nodeCounter++;
					break;
				default:
					throw new InvalidOperationException ("[SimulationController.createNodes] Unidentified node type");
				}
			}
		}

		/**
		 * <p>For each pair of nodes where connection is marked, create a new Link instance and register the link to each node. New links are stored in LinkedList<Link> links 
		 * initialized here (as their count is unknown)</p>
		 * @throws InvalidOperationException if any node is of unidentified type, the network model is null, the nodes array is null or length of the nodes array don't match node 
		 * count in the network model
		 */
		private void createLinks(){
			log.Debug ("Create links");
			if (framework_model == null)
				throw new InvalidOperationException ("[SimulationController.createLinks] Framework model not initialized");
			this.linkCounter = 0;
			foreach(string link in network_model.GetLinkNames ()){
				if (link == null)
					log.Error ("Link null");
				log.Debug ("Link: " + link);
				Node a;
				string n1 = network_model.GetLinkNode1 (link);
				log.Debug ("N1:" + n1);
				if (node_names.TryGetValue (n1, out a)) {
					Node b;
					string n2 = network_model.GetLinkNode2 (link);
					log.Debug ("N2:" + n2);
					if (node_names.TryGetValue (n2, out b)) {
						decimal cap = network_model.GetLinkCapacity (link);
						log.Debug ("Capacity:" + cap);
						Link l = new Link (link, cap, a, b, framework_model);
						links [linkCounter] = l;
						linkCounter++;
						if (a is EndpointNode) {
							(a as EndpointNode).Link = l;
						} else {
							log.Debug ("NN connect link");
							(a as NetworkNode).ConnectLink (l, framework_model, network_model.IsLinkDefaultRouteForNetworkNode (a.Name, link));
						}

						if (b is EndpointNode) {
							(b as EndpointNode).Link = l;
						} else {
							log.Debug ("NN connect link 2");
							(b as NetworkNode).ConnectLink (l, framework_model, network_model.IsLinkDefaultRouteForNetworkNode (b.Name, link));
						}
					} else {
						throw new ArgumentException ("Node not found: " + n2);
					}
				} else {
					throw new ArgumentException ("Node not found: " + n1);
				}
				log.Debug ("FOR iteration done");
			}
		}

		/**
		 * Unless simulation model is null initialize Framework Model with appropriate Time To Run
		 * @throws InvalidOperationException if SimulationModel is null
		 */
		private void createModel(){
			log.Debug ("Create model");
			if (simulation_model != null) {
				framework_model = new MFF_NPRG031.Model (simulation_model.Time);
			} else
				throw new InvalidOperationException ("[SimulationController.createModel] SimulationModel null");
		}

		/**
		 * <p>Create loaded events from SimulationModel</p>
		 * <p>Create random SEND events for RandomTalkers (EndNodes)</p>
		 * <p>Create random TOGGLE events for links</p>
		 */
		private void createEvents(){
			log.Debug ("Create events");
			if ((framework_model != null)&&(network_model!=null)) {
				log.Debug ("User defined simulation events");
				traced = simulation_model.GetEvents ().Length;
				this.tracedPackets = new Packet[traced];
				for (int i = 0; i < traced; i++) {
					SimulationModel.Event e = simulation_model.GetEvents () [i];
					Node from_node; 
					int from_addr = network_model.GetEndpointNodeAddr (e.node1);
					if (node_names.TryGetValue (e.node1, out from_node)) {
						if (from_node is EndNode && network_model.GetNodeType (e.node2).Equals (NetworkModel.SERVER_NODE)) {
							//schedule node e.node1 to state SEND (with packet from e.node1 to e.node2 of e.size as parameter) at time e.when
							Packet p = new Packet (from_addr, network_model.GetEndpointNodeAddr (e.node2), e.size, true);
							tracedPackets[i] = p;
							MFF_NPRG031.State st = new MFF_NPRG031.State (MFF_NPRG031.State.state.SEND,p);
							MFF_NPRG031.Event ev = new MFF_NPRG031.Event (from_node, st, e.when);
							framework_model.K.Schedule (ev);
						} else
							throw new ArgumentException ("Must send from END NODE to SERVER NODE");
					} else
						throw new ArgumentException ("Node not found: " + e.node1);
				}
				//random talkers
				log.Debug ("Random talkers events");
				foreach (EndNode en in randomTalkers) {
					Random r = new Random ();
					//kolik eventu
					int events = r.Next (simulation_model.Time);
					for (int i=0; i<=events; i++) {
						//cas
						int time = r.Next (simulation_model.Time);
						//server
						int server = r.Next (serverNodeCounter);
						//size
						int sizeUpper = network_model.GetEndNodeMaxPacketSize (en.Name);
						decimal size = r.Next (sizeUpper) + (decimal)r.NextDouble ();
						MFF_NPRG031.State st = new MFF_NPRG031.State (MFF_NPRG031.State.state.SEND, new Packet (en.Address, servers [server].Address, size));
						framework_model.K.Schedule(new MFF_NPRG031.Event(en,st,time));
						log.Debug("Scheduled random talker: "+en.Name+" at "+time+" to "+servers[server].Name+" size "+size);
					}
						
				}
				//link toggles
				log.Debug ("Link toggles");
				foreach (Link l in links) {
					decimal tp = network_model.GetLinkToggleProbability (l.Name);
					log.Debug ("Link name " + l.Name+" toggle_probability "+tp);
					Random r = new Random ();
					//kolik togglu
					int toggles = r.Next (framework_model.Time);
					for (int i=0; i<=toggles; i++) {
						//cas
						int time = r.Next (framework_model.Time);
						decimal random = (decimal)r.NextDouble ();
						log.Debug ("Random: " + random);
						if (random < tp) {
							framework_model.K.Schedule (new MFF_NPRG031.Event (l, new MFF_NPRG031.State (MFF_NPRG031.State.state.TOGGLE), time));
							log.Debug ("Scheduled link toggle: " + l.Name + " at " + time);
						}
					}

				}
			}else throw new InvalidOperationException ("[SimulationController.createEvents] FrameworkModel or NetworkModel null");
		}

		/**
		 * Stores statistics into Result Model
		 */
		private void PopulateResultModel(){
			result_model = new ResultModel (endNodeCounter, serverNodeCounter, networkNodeCounter, linkCounter,traced);
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
		if (framework_model != null)
			foreach (Link l in links) {
				result_model.SetNewLinkResult (l.Name, l.PacketsCarried, l.GetActiveTime (framework_model), l.PassiveTime, l.GetPercentageTimeIdle (framework_model), l.DataCarried, l.GetAvgDataCarriedPerTic (framework_model),
				                               l.GetAvgLinkUsage (framework_model), l.DataSent, l.DataLost, l.PercentageDataLost, l.PercentageDataDelivered, l.PercentageDataLostInCarry);
			}
		else
			throw new InvalidOperationException ("[SimulationController.PopulateResultModel] Framework model not created");
			if (tracedPackets != null)
				foreach (Packet p in tracedPackets)
					result_model.SetPacketTrace (p);
			else
				throw new InvalidOperationException ("[SimulationController.PopulateResultModel] Traced packets not initialized");
		}

		/**
		 * If Model is not null, createEvents and schedule links to receive at 0
		 * @throws InvalidOperationException if framework_model is null (not created yet)
		 */
		private void initializeProcesses(){
			log.Debug ("Initialize processes");
			if (framework_model != null) {
				createEvents ();
				//link receive
 				foreach (Link l in links)
					l.Schedule (framework_model.K, new MFF_NPRG031.State(MFF_NPRG031.State.state.RECEIVE), 0);
				//network node timers
				foreach (NetworkNode n in routers) {
					n.Schedule (framework_model.K, new MFF_NPRG031.State (MFF_NPRG031.State.state.UPDATE_TIMER), 0);
					n.SendRequest (framework_model);
				}
 			} else
 				throw new InvalidOperationException ("[SimulationController.initializeProcesses] Framework model not created");
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