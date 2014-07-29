using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NetTrafficSimulator
{
	[TestFixture()]
	public class SimulationControllerTest
	{
		[Test()]
		public void CreateEndNode ()
		{
			EndNode en = new EndNode ("End node " + 0, 0);
			Assert.AreEqual (0, en.Address);
			Assert.AreEqual ("End node 0", en.Name);
		}

		public void CreateNetworkNode(){
		}

		public void CreateServerNode(){
		}

		public void CreateNodesFromNetworkModel(){
			NetworkModel network_model=new NetworkModel(-1);


			int endNodeCounter = 0;
			int networkNodeCounter = 0;
			int serverNodeCounter = 0;
			int addressCounter = 0;
			int nodeCounter = 0;
			Node[] nodes = new Node[network_model.NodeCount];
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

		public void CreateNodesFromNetworkModel_InvalidModel(){
		}

		public void CreateLink(){
		}

		public void NetworkNodeConnectLink(){
		}

		public void NetworkNodeConnectLink_unavailable(){
		}

		public void LinkCarry(){
		}

		public void LinkCarryInvalid(){
		}

		public void EndNodeSend(){
		}

		public void LinkProcessEvent(){
		}

		public void NetworkNodeProcessEvent(){
		}

		public void ServerNodeProcessEvent(){
		}

		public void CreateLinksFromNetworkModel(){
			NetworkModel network_model = new NetworkModel (-1);
			Node[] nodes=new Node[0];
			LinkedList<Link> links = new LinkedList<Link> ();
			if ((network_model != null) && (nodes != null) && (nodes.Length == network_model.NodeCount)) {
				for (int i = 0; i < network_model.NodeCount; i++) {
					Node x = nodes [i];
					int j = i+1;//[i,i] connection forbidden
					while (j<network_model.NodeCount) {
						Node y = nodes [j];
						if (network_model.AreConnected (i, j)){
							//TESTME links parsed correctly??
							Link l = new Link (x + " - " + y + " link", network_model.LinkCapacity(i,j), x, y);
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

		public void CreateFrameworkModel(){
		}

		public void InitializeProcesses(){
		}
	}
}