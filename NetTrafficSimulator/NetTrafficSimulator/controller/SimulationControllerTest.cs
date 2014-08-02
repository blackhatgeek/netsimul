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
			Assert.Null (en.Link);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void CreateNetworkNode0(){
			NetworkNode nn=new NetworkNode ("Network node " + 0, 0);
			Assert.AreEqual ("Network node 0", nn.Name);
			Assert.AreEqual (0, nn.Interfaces);
			nn.ConnectLink (null);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateNetworkNode1(){
			NetworkNode nn=new NetworkNode ("Network node " + 1, 1);
			Assert.AreEqual ("Network node 1", nn.Name);
			Assert.AreEqual (1, nn.Interfaces);
			nn.ConnectLink (null);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void CreateNetworkNode2()
		{
			new NetworkNode ("Network node " + 2, -1);
		}

		[Test()]
		public void CreateServerNode(){
			ServerNode sn = new ServerNode ("Server node 1", -1);
			Assert.AreEqual ("Server node 1", sn.Name);
			Assert.AreEqual (-1, sn.Address);
			Assert.Null (sn.Link);
		}

		private Node[] createNodes(NetworkModel network_model){
			Node[] nodes;
			if (network_model == null)
				throw new ArgumentNullException ("[SimulationController] No model provided should be null");
			if (network_model.Valid) {
				int endNodeCounter = 0;
				int networkNodeCounter = 0;
				int serverNodeCounter = 0;
				int addressCounter = 0;
				int nodeCounter = 0;
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
			else
				throw new ArgumentException ("[SimulationController] Network model not valid");
			return nodes;
		}

		[Test()]
		public void CreateNodesFromNetworkModel(){
			NetworkModel network_model=new NetworkModel(4);
			network_model.SetNodeType (0, NetworkModel.END_NODE);
			network_model.SetNodeType (1, NetworkModel.END_NODE);
			network_model.SetNodeType (2, NetworkModel.NETWORK_NODE);
			network_model.SetNodeType (3, NetworkModel.SERVER_NODE);

			Node[] nodes = createNodes (network_model);
			Assert.AreEqual (4, nodes.Length);

			Assert.True (nodes [0] is EndNode);
			Assert.AreEqual ("End node 0", nodes [0].Name);
			Assert.AreEqual (0, (nodes [0] as EndNode).Address);

			Assert.True (nodes [1] is EndNode);
			Assert.AreEqual ("End node 1", nodes [1].Name);
			Assert.AreEqual (1, (nodes [1] as EndNode).Address);

			Assert.True (nodes [2] is NetworkNode);
			Assert.AreEqual ("Network node 0", nodes [2].Name);
			Assert.AreEqual (0, (nodes [2] as NetworkNode).Interfaces);
	
			Assert.True (nodes [3] is ServerNode);
			Assert.AreEqual ("Server node 0", nodes [3].Name);
			Assert.AreEqual (2, (nodes [3] as ServerNode).Address);

			network_model.SetConnected (0, 2, 3);
			nodes = createNodes (network_model);
			Assert.AreEqual (1, (nodes [2] as NetworkNode).Interfaces);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void CreateNodesFromNetworkModel_InvalidModel(){
			createNodes(new NetworkModel (1));
		}

		[Test()]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CreateLink0(){
			new Link (null, -1, null, null);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateLink1(){
			new Link (null, 1, null, new EndNode ("EN", 1));
		}

		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateLink2(){
			new Link (null, 1, new EndNode ("EN", 1),null);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateLink3(){
			new Link (null, 1, null,null);
		}

		[Test()]
		public void CreateLink4(){
			new Link ("L", 0, new EndNode ("EN1", 0),new EndNode("EN2",1));
		}

		[Test()]
		public void NetworkNodeConnectLink0(){
			EndNode en=new EndNode("EN0",0);
			NetworkNode nn = new NetworkNode ("NN0", 1);
			Link l = new Link ("L0", 1, en, nn);
			nn.ConnectLink (l);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void NetworkNodeConnectLink1(){
			EndNode en0 = new EndNode ("EN0", 0);
			EndNode en1 = new EndNode ("EN!", 1);
			Link l = new Link ("L0", 1, en0, en1);
			new NetworkNode ("NN0", 1).ConnectLink (l);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void NetworkNodeConnectLink_unavailable(){
			new NetworkNode ("NN0", 0).ConnectLink (new Link ("L0", 0, new EndNode ("EN0", 0), new EndNode ("EN1", 1)));
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
		}

		private void createLinks(NetworkModel network_model){
			LinkedList<Link> links = new LinkedList<Link> ();
			Node[] nodes=new Node[network_model.NodeCount];
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