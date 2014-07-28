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
		private int endNodeCounter,networkNodeCounter,serverNodeCounter,addressCounter,nodeCounter;//,linkCounter;
		private Node[] nodes;
		private LinkedList<Link> links;
		private MFF_NPRG031.Model model;

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
			for (int i=0; i<network_model.NodeCount; i++) {
				switch (network_model.GetNodeType (i)) {
				case NetworkModel.END_NODE:
					EndNode en = new EndNode ("End node " + endNodeCounter, addressCounter);
					nodes [nodeCounter] = en;
					endNodeCounter++;
					addressCounter++;
					nodeCounter++;
					break;
				case NetworkModel.NETWORK_NODE:
					int interfaces = network_model.GetConnectionCount (i);
					NetworkNode nn = new NetworkNode ("Network node " + networkNodeCounter, interfaces);
					nodes [nodeCounter] = nn;
					networkNodeCounter++;
					nodeCounter++;
					break;
				case NetworkModel.SERVER_NODE:
					ServerNode sn = new ServerNode ("Server node " + serverNodeCounter, addressCounter);
					nodes [nodeCounter] = sn;
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
		 * <p>As link capacities are not supported by NetworkModel yet, all are set for 0 at this point.</p>
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
							//TODO Link capacities!!
							//TESTME links parsed correctly??
							Link l = new Link (x + " - " + y + " link", 0, x, y);
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
			model = new MFF_NPRG031.Model (simulation_model.Time);
		} else
			throw new InvalidOperationException ("[SimulationController.createModel] SimulationModel null");
		}

		/**
		 * If Model is not null, invoke Run method on each Node and Link registered
		 * @throws InvalidOperationException if model is null (not created yet)
		 */
		private void initializeProcesses(){
			if (model != null) {
				foreach (Node n in nodes)
					n.Run (model);
				foreach (Link l in links)
					l.Run(model);
			} else
				throw new InvalidOperationException ("[SimulationController.initializeProcesses] Model not created");
		}

		/**
		 * Runs a simulation, stores statistics
		 */
		private void RunSimulation(){
			model.Simulate ();
		}

		/**
		 * Stores statistics into Result Model unless this is done by RunSimulation
		 */
		private void PopulateResultModel(){
			throw new NotImplementedException ();
		}

		/**
		 * Initialize framework, run simulation, populate result model
		 */
		public void Run(){
			InitializeFramework ();
			RunSimulation ();
			PopulateResultModel ();
		}
	}
}

