using System;
using System.Collections.Generic;

namespace NetTrafficSimulator
{
	/**
	 * Simulation controller takes data from NetworkModel and SimulationModel and sets up simulation framework
	 */
	public class SimulationController
	{
		private NetworkModel network_model;
		private SimulationModel simulation_model;
		private MFF_NPRG031.Model framework_model;
		private ResultModel result_model;
		private int endNodeCounter,networkNodeCounter,serverNodeCounter,addressCounter,nodeCounter;
		private Node[] nodes;
		private ServerNode[] servers;
		private LinkedList<Link> links;

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
			//initialize processes
			initializeProcesses ();
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
					EndNode en = new EndNode (name, network_model.GetNodeAddr(i),network_model.GetEndNodeMaxPacketSize(name));
					nodes [nodeCounter] = en;
					endNodeCounter++;
					addressCounter++;
					nodeCounter++;
					break;
				case NetworkModel.NETWORK_NODE:
					int interfaces = network_model.GetConnectionCount (i);
					NetworkNode nn = new NetworkNode (network_model.GetNodeName(i), interfaces);
					nodes [nodeCounter] = nn;
					networkNodeCounter++;
					nodeCounter++;
					break;
				case NetworkModel.SERVER_NODE:
					ServerNode sn = new ServerNode (network_model.GetNodeName(i), network_model.GetNodeAddr(i));
					nodes [nodeCounter] = sn;
					serverNodeCounter++;
					servers [serverNodeCounter] = sn;
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
							Link l = new Link (network_model.GetLinkName(i,j), network_model.LinkCapacity(i,j), x, y);
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
		 * If Model is not null, invoke Run method on each Node and Link registered
		 * @throws InvalidOperationException if model is null (not created yet)
		 */
		private void initializeProcesses(){
			if (framework_model != null) {
				foreach (Node n in nodes)
					n.Run (framework_model);
				foreach (Link l in links)
					l.Run(framework_model);
			} else
				throw new InvalidOperationException ("[SimulationController.initializeProcesses] Framework model not created");
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
					result_model.SetNewNetworkNodeResult (nn.Name,nn.PacketsProcessed,nn.TimeWaited,nn.GetPercentageTimeIdle(framework_model),nn.AverageWaitTime);
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
			InitializeFramework ();
			framework_model.Simulate ();
			PopulateResultModel ();
		}
	}
}