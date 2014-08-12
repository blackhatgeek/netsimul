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
			NetworkNode nn=new NetworkNode ("Network node " + 0, 0,0,new MFF_NPRG031.Model(1));
			Assert.AreEqual ("Network node 0", nn.Name);
			Assert.AreEqual (0, nn.Interfaces);
			nn.ConnectLink (null,null);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateNetworkNode1(){
			NetworkNode nn=new NetworkNode ("Network node " + 1, 1,0,new MFF_NPRG031.Model(1));
			Assert.AreEqual ("Network node 1", nn.Name);
			Assert.AreEqual (1, nn.Interfaces);
			nn.ConnectLink (null,null);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void CreateNetworkNode2()
		{
			new NetworkNode ("Network node " + 2, -1,0,new MFF_NPRG031.Model(1));
		}

		[Test()]
		public void CreateServerNode(){
			ServerNode sn = new ServerNode ("Server node 1", -1);
			Assert.AreEqual ("Server node 1", sn.Name);
			Assert.AreEqual (-1, sn.Address);
			Assert.Null (sn.Link);
		}

		private Node[] createNodes(NetworkModel network_model,MFF_NPRG031.Model model){
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
						NetworkNode nn = new NetworkNode ("Network node " + networkNodeCounter, interfaces,10,model);
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

			MFF_NPRG031.Model m = new MFF_NPRG031.Model (1);
			Node[] nodes = createNodes (network_model,m);
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

			network_model.SetConnected (0, 2, 3,0.0m);
			nodes = createNodes (network_model,m);
			Assert.AreEqual (1, (nodes [2] as NetworkNode).Interfaces);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void CreateNodesFromNetworkModel_InvalidModel(){
			createNodes(new NetworkModel (1),new MFF_NPRG031.Model(1));
		}

		[Test()]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CreateLink0(){
			new Link (null, -1, null, null,new MFF_NPRG031.Model(0));
		}

		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateLink1(){
			new Link (null, 1, null, new EndNode ("EN", 1),new MFF_NPRG031.Model(1));
		}

		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateLink2(){
			new Link (null, 1, new EndNode ("EN", 1),null,new MFF_NPRG031.Model(1));
		}

		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateLink3(){
			new Link (null, 1, null,null,new MFF_NPRG031.Model(1));
		}

		[Test()]
		public void CreateLink4(){
			new Link ("L", 0, new EndNode ("EN1", 0),new EndNode("EN2",1),new MFF_NPRG031.Model(1));
		}

		[Test()]
		public void NetworkNodeConnectLink0(){
			EndNode en=new EndNode("EN0",0);
			MFF_NPRG031.Model m = new MFF_NPRG031.Model (1);
			NetworkNode nn = new NetworkNode ("NN0", 1,0,m);
			Link l = new Link ("L0", 1, en, nn,m);
			nn.ConnectLink (l,m);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void NetworkNodeConnectLink1(){
			EndNode en0 = new EndNode ("EN0", 0);
			EndNode en1 = new EndNode ("EN!", 1);
			MFF_NPRG031.Model m = new MFF_NPRG031.Model (1);
			Link l = new Link ("L0", 1, en0, en1,m);
			new NetworkNode ("NN0", 1,0,m).ConnectLink (l,m);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void NetworkNodeConnectLink_unavailable(){
			MFF_NPRG031.Model m = new MFF_NPRG031.Model (1);
			new NetworkNode ("NN0", 0,0,m).ConnectLink (new Link ("L0", 0, new EndNode ("EN0", 0), new EndNode ("EN1", 1),m),m);
		}

		//packet null
		//a i b jsou z linku, plna fronta
		//a i b jsou z linku, misto ve fronte
		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void LinkCarry0(){
			Link l = new Link ("L0", 0, new EndNode ("EN0", 0),new EndNode("EN1",1),new MFF_NPRG031.Model(1));
			l.Carry(null,new ServerNode("SN0",2),new ServerNode("SN1",3));
		}
		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void LinkCarry1(){
			ServerNode sn=new ServerNode ("SN0", 0);
			MFF_NPRG031.Model m = new MFF_NPRG031.Model (1);
			Link l = new Link ("L0", 0, sn, new EndNode ("EN0", 1),m);
			l.Carry (null, new NetworkNode ("NN0", 0,1,m), sn);
		}
		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void LinkCarry2(){
			EndNode en = new EndNode ("EN0", int.MaxValue);
			MFF_NPRG031.Model m = new MFF_NPRG031.Model (1);
			Link l = new Link ("L0", 0, en, new NetworkNode ("NN0", 0,1,m),m);
			l.Carry (null, en, new NetworkNode ("NN1", 0,1,m));
		}

		[Test()]
		public void LinkCarry3(){
			EndNode en = new EndNode ("EN0", int.MinValue);
			MFF_NPRG031.Model m = new MFF_NPRG031.Model (1);
			NetworkNode nn = new NetworkNode ("NN0", 0,1,m);
			Link l = new Link ("L0", 10, nn, en,m);
			l.Active = false;
			Assert.AreEqual (0, l.PacketsCarried);
			Assert.AreEqual (0, l.PercentageDataLostInCarry);
			l.Carry (new Packet(int.MinValue,int.MaxValue,1), nn, en);
			Assert.AreEqual (1, l.PacketsCarried);
			Assert.AreEqual (100.0, l.PercentageDataLostInCarry);
		}

		[Test()]
		public void LinkCarry4(){
			EndNode en = new EndNode ("EN0", 0);
			MFF_NPRG031.Model m = new MFF_NPRG031.Model (1);
			NetworkNode nn = new NetworkNode ("NN", 0,1,m);
			Link l = new Link ("L0", 1, en, nn,m);
			Assert.AreEqual (0, l.PacketsCarried,"carried");
			//Assert.AreEqual (0, l.PacketsDropped,"dropped");
			l.Carry (new Packet(10,20,2), en, nn);
			Assert.AreEqual (1, l.PacketsCarried,"carried");
			//Assert.AreEqual (1, l.PacketsDropped,"dropped");
		}

		[Test()]
		public void LinkCarry5(){
			MFF_NPRG031.Model m = new MFF_NPRG031.Model (1);
			NetworkNode nn = new NetworkNode ("NN0", 1,1,m);
			ServerNode sn = new ServerNode ("SN0", 0);
			Link l = new Link ("L0", 1, nn, sn,m);
			Assert.AreEqual (0, l.PacketsCarried);
			//Assert.AreEqual (0, l.PacketsDropped);
			l.Carry (new Packet(10,0,1), nn, sn);
			Assert.AreEqual (1, l.PacketsCarried);
			//Assert.AreEqual (0, l.PacketsDropped);
		}

		/*[Test()]
		[ExpectedException(typeof(InvalidOperationException))]
		public void EndNodeSend0(){
			new EndNode ("EN", 0).ProcessEvent (
				new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND,new Packet(0,10,10)), new MFF_NPRG031.Model (1));
		}*/ //pokud k node nevede link, pouze napise varovani

		[Test()]
		public void EndNodeSend1(){
			EndNode en0 = new EndNode ("EN0", 0);
			EndNode en1 = new EndNode ("EN1", 1);
			MFF_NPRG031.Model m = new MFF_NPRG031.Model (1);
			Link l = new Link ("L0", 10, en0, en1,m);
			en0.Link = l;
			en1.Link = l;
			en0.ProcessEvent (new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND,new Packet(0,1,3)), m);
			Assert.AreEqual (1, l.PacketsCarried);
			//Assert.AreEqual (0, l.PacketsDropped);
			Assert.AreEqual (1, en0.PacketsSent);
		}

		[Test()]
		public void LinkProcessEvent(){
			EndNode en0 = new EndNode ("EN0", 0);
			EndNode en1 = new EndNode ("EN1", 1);
			MFF_NPRG031.Model m = new MFF_NPRG031.Model (2);
			Link l = new Link ("L0", 1, en0, en1,m);
			l.Carry (new Packet (1, 1,0), en0, en1);
			l.Schedule (m.K, new MFF_NPRG031.State (MFF_NPRG031.State.state.RECEIVE), 0);
			m.Time = 0;
			MFF_NPRG031.Event e = m.K.First ();
			if (e != null) {
				Assert.AreEqual (MFF_NPRG031.State.state.RECEIVE, e.what.Actual);
				Assert.AreEqual (0, e.when,"Link ev 1 time");
				Assert.AreEqual (l, e.who);
				l.ProcessEvent (e.what, m);
			} else
				Assert.True (false, "Link event 1");
			e = m.K.First ();
			if (e != null) {
				Assert.AreEqual (MFF_NPRG031.State.state.SEND, e.what.Actual);
				Assert.AreEqual (1, e.when,"Link ev 2 time");
				Assert.AreEqual (l, e.who);
				m.Time = 1;
				l.ProcessEvent (e.what, m);
			} else
				Assert.True (false,"Link event 2");
			e = m.K.First ();
			if (e != null) {
				Assert.AreEqual (MFF_NPRG031.State.state.RECEIVE, e.what.Actual);
				Assert.AreEqual (1, e.when,"Link ev 3 time");
				m.Time = 1;
				Assert.AreEqual (l, e.who);
			} else
				Assert.True (false,"Link event 3");
			e = m.K.First ();
			if (e != null) {
				Assert.AreEqual (MFF_NPRG031.State.state.RECEIVE, e.what.Actual);
				Assert.AreEqual (1, e.when, "EN rec time");
				Assert.AreEqual (en1, e.who);
			} else
				Assert.True (false, "EN event");
		}

		[Test()]
		public void NetworkNodeProcessEvent(){
			log4net.ILog log = log4net.LogManager.GetLogger (typeof(SimulationControllerTest));
			log.Debug ("** NetworkNodeProcessEvent: new model");
			MFF_NPRG031.Model m = new MFF_NPRG031.Model (4);
			log.Debug ("** NetworkNodeProcessEvent: new nn");
			NetworkNode nn = new NetworkNode ("NN0", 1,1,m);
			log.Debug ("** NetworkNodeProcessEvent: new en");
			EndNode en = new EndNode ("EN0", 0);
			log.Debug ("** NetworkNodeProcessEvent: new link");
			Link l = new Link ("L0", 1, nn, en,m);
			log.Debug ("** NetworkNodeProcessEvent: connect link");
			nn.ConnectLink (l,m);
			log.Debug ("** NetworkNodeProcessEvent: set link");
			en.Link = l;
			log.Debug ("** NetworkNodeProcessEvent: new state");
			MFF_NPRG031.State s = new MFF_NPRG031.State (MFF_NPRG031.State.state.RECEIVE,new Packet (1, 0,0));
			log.Debug ("** NetworkNodeProcessEvent: nn process ev");
			nn.ProcessEvent (s, m);
			log.Debug ("** NetworkNodeProcessEvent: get first event");
			MFF_NPRG031.Event e = m.K.First ();
			if (e != null) {
				Assert.AreEqual (MFF_NPRG031.State.state.SEND, e.what.Actual, "Actual state");
				Assert.AreEqual (0, e.what.Data.Destination, "Destination");
				Assert.AreEqual (1, e.what.Data.Source, "Source");
				Assert.AreEqual (1, e.when, "When");
				Assert.AreEqual (nn, e.who, "Whp");
			} else
				Assert.True (false, "Event");
			log.Debug ("** NetworkNodeProcessEvent: model time");
			m.Time = 1;
			log.Debug ("** NetworkNodeProcessEvent: nn process ev 1");
			nn.ProcessEvent (e.what, m);
			Assert.AreEqual (1, l.PacketsCarried,"Packets carried");
			Assert.AreEqual (0, l.DataCarried, "Data carried");
			Assert.AreEqual (0, l.DataLost,"Data lost");
			log.Debug ("** NetworkNodeProcessEvent: model time");
			m.Time++;
			log.Debug ("** NetworkNodeProcessEvent: nn process ev 2");
			l.ProcessEvent (new MFF_NPRG031.State (MFF_NPRG031.State.state.RECEIVE), m);
			log.Debug ("** NetworkNodeProcessEvent: get first event");
			e = m.K.First ();//invalid timer
			if (e != null) {
				/*	Assert.AreEqual (MFF_NPRG031.State.state.INVALID_TIMER, e.what.Actual, "INVALID TIMER");
			} else
				Assert.True (false,"Timer event");
			e = m.K.First ();*/
				Assert.AreEqual (MFF_NPRG031.State.state.SEND, e.what.Actual);
				Assert.AreEqual (l, e.who);
				l.ProcessEvent (e.what, m);
			} else
				Assert.True (false, "event");
			e = m.K.First ();
			if (e != null) {
				Assert.AreEqual (MFF_NPRG031.State.state.RECEIVE, e.what.Actual);
				Assert.AreEqual (2, e.when);
				Assert.AreEqual (en, e.who);
			} else
				Assert.True (false, "EN event");
		}
		[Test()]
		public void ServerNodeProcessEvent(){
			ServerNode sn = new ServerNode ("SN1", 1);
			EndNode en = new EndNode ("EN1", 0);
			MFF_NPRG031.Model m = new MFF_NPRG031.Model (3);
			Link l = new Link ("L1", 1, sn, en,m);
			sn.Link = l;
			en.Link = l;
			MFF_NPRG031.State s = new MFF_NPRG031.State (MFF_NPRG031.State.state.RECEIVE, new Packet (0, 1,0));

			sn.ProcessEvent (s, m);

			MFF_NPRG031.Event e = m.K.First ();
			if (e != null) {
				Assert.AreEqual (MFF_NPRG031.State.state.SEND, e.what.Actual);
				Assert.AreEqual (0, e.what.Data.Destination, "Desination");
				Assert.AreEqual (1, e.what.Data.Source, "Source");
				Assert.AreEqual (1, e.when, "When");
				Assert.AreEqual (sn, e.who);
			} else
				Assert.IsTrue (false, "SN event");
			m.Time = 1;
			sn.ProcessEvent (e.what, m);

			Assert.AreEqual (1, l.PacketsCarried);
			Assert.AreEqual (0, l.DataCarried);
			Assert.AreEqual (0, l.DataLost);

			m.Time++;
			l.ProcessEvent (new MFF_NPRG031.State (MFF_NPRG031.State.state.RECEIVE), m);
			e = m.K.First ();
			if (e != null) {
				Assert.AreEqual (MFF_NPRG031.State.state.SEND, e.what.Actual);
				Assert.AreEqual (3, e.when);
				Assert.AreEqual (l, e.who);
			} else
				Assert.IsTrue (false,"Link send event");
			m.Time++;
			l.ProcessEvent (e.what, m);

			e = m.K.First ();//link receive
			if (e != null)
				Assert.AreEqual (MFF_NPRG031.State.state.RECEIVE, e.what.Actual);
			else
				Assert.IsTrue(false,"Link receive event");
			e = m.K.First ();
			if (e != null) {
				Assert.AreEqual (MFF_NPRG031.State.state.RECEIVE, e.what.Actual);
				Assert.AreEqual (3, e.when);
				Assert.AreEqual (en, e.who);
			} else
				Assert.IsTrue (false, "EN event");
		}

		[Test()]
		public void CreateLinksFromNetworkModel(){
			NetworkModel nm = new NetworkModel (4);
			nm.SetNodeType (0,NetworkModel.END_NODE);
			nm.SetNodeType (1,NetworkModel.NETWORK_NODE);
			nm.SetNodeType (2,NetworkModel.SERVER_NODE);
			nm.SetNodeType (3,NetworkModel.END_NODE);
			nm.SetConnected (0, 1, 3,0.0m);
			nm.SetConnected (2, 1, 8,0.0m);
			nm.SetConnected (3, 1, 1,0.0m);
			Node[] nodes = createNodes (nm,new MFF_NPRG031.Model(1));
			LinkedList<Link> links = createLinks (nm,nodes,new MFF_NPRG031.Model(0));
			Assert.AreEqual (nodes [1], links.First.Value.GetPartner (nodes [0]));
			Assert.AreEqual (3, links.First.Value.Capacity);
			Assert.AreEqual (nodes [1], links.First.Next.Value.GetPartner (nodes [2]));
			Assert.AreEqual (8, links.First.Next.Value.Capacity);
			Assert.AreEqual (nodes [1], links.Last.Value.GetPartner (nodes [3]));
			Assert.AreEqual (1, links.Last.Value.Capacity);
		}

		private LinkedList<Link> createLinks(NetworkModel network_model,Node[] nodes,MFF_NPRG031.Model framework_model){
			LinkedList<Link> links = new LinkedList<Link> ();
			//Node[] nodes=new Node[network_model.NodeCount];
			if ((network_model != null) && (nodes != null) && (nodes.Length == network_model.NodeCount)) {
				for (int i = 0; i < network_model.NodeCount; i++) {
					Node x = nodes [i];
					int j = i+1;//[i,i] connection forbidden
					while (j<network_model.NodeCount) {
						Node y = nodes [j];
						if (network_model.AreConnected (i, j)){
							//TESTME links parsed correctly??
							Link l = new Link (x + " - " + y + " link", network_model.LinkCapacity(i,j), x, y,framework_model);
							if (x is EndNode)
								(x as EndNode).Link = l;
							else if (x is NetworkNode)
								(x as NetworkNode).ConnectLink (l,framework_model);
							else if (x is ServerNode)
								(x as ServerNode).Link = l;
							else
								throw new InvalidOperationException ("Node " + x + "is not EndNode nor NetworkNode nor ServerNode");
							if (y is EndNode)
								(y as EndNode).Link = l;
							else if (y is NetworkNode)
								(y as NetworkNode).ConnectLink (l,framework_model);
							else if (y is ServerNode)
								(y as ServerNode).Link = l;
							else
								throw new InvalidOperationException ("Node " + y + " is not EndNode nor NetworkNode nor ServerNode");
							links.AddLast (l);
						}
						j++;
					}
				}
				return links;
			} else
				throw new InvalidOperationException ("[SimulationController.createLinks] Network model null or nodes array null or length of nodes array don't match " +
				                                     "node count in network model");
		}

		[Test()]
		public void NetworkNodeEndpointDelivery(){
			EndNode en1 = new EndNode ("EN1", 1);
			EndNode en2 = new EndNode ("EN2", 2);
			ServerNode sn1 = new ServerNode ("SN1", 3);
			ServerNode sn2 = new ServerNode ("SN2", 4);
			MFF_NPRG031.Model m = new MFF_NPRG031.Model (4);
			NetworkNode nn = new NetworkNode ("NN0", 4,3,m);
			Link l1 = new Link ("L1", 10, en1, nn,m);
			Link l2 = new Link ("L2", 10, en2, nn,m);
			Link l3 = new Link ("L3", 10, sn1, nn,m);
			Link l4 = new Link ("L4", 10, sn2, nn,m);
			en1.Link = l1;
			en2.Link = l2;
			sn1.Link = l3;
			sn2.Link = l4;
			nn.ConnectLink (l1,m);
			nn.ConnectLink (l2,m);
			nn.ConnectLink (l3,m);
			nn.ConnectLink (l4,m);

			//EN1 -> NN
			en1.ProcessEvent (new MFF_NPRG031.State (MFF_NPRG031.State.state.SEND, new Packet (1, 3,1)), m);
			Assert.AreEqual (1, l1.PacketsCarried, "L1 packets carried");
			//Assert.AreEqual (0, l1.PacketsDropped);
			l1.ProcessEvent(new MFF_NPRG031.State(MFF_NPRG031.State.state.RECEIVE),m);
			MFF_NPRG031.Event e = m.K.First ();
			Assert.AreEqual(l1,e.who);
			m.Time = e.when;
			//sending
			l1.ProcessEvent (e.what, m);
			e = m.K.First ();
			Assert.AreEqual (l1, e.who);//link receive
			e = m.K.First ();
			Assert.AreEqual(nn,e.who);//nn receive
			Assert.AreEqual (1, e.when);
			//dorazilo na NN
			nn.ProcessEvent (e.what, m);
			Assert.AreEqual (1, nn.PacketsProcessed,"NN packets processed");
			e = m.K.First ();
			//nn send
			Assert.AreEqual (nn, e.who);
			Assert.AreEqual (2, e.when);
			Assert.AreEqual (MFF_NPRG031.State.state.SEND, e.what.Actual);
			m.Time = 2;
			nn.ProcessEvent (e.what, m);
			//forward
			Assert.AreEqual (1, l3.PacketsCarried,"L3 packets carried");
			//Assert.AreEqual (0, l3.PacketsDropped);
			//zbytek je doruceni stejne jako pres l1
		}

		public void InitializeProcesses(){
		}

		public void PopulateResultModel(){
		}
	}
}