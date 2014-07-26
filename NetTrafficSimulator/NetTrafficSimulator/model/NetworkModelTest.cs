using NUnit.Framework;
using System;

namespace NetTrafficSimulator
{
	[TestFixture()]
	/**
	 * Test of NetworkModel class
	 */ 
	public class NetworkModelTest
	{
		[Test()]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		/**
		 * Try to create model with -1 nodes
		 */
		public void ConstructorInvalid ()
		{
			new NetworkModel (-1);
		}

		[Test()]
		/**
		 * Create constructor with 0 nodes. Assert node count and validity
		 */ 
		public void ConstructorZero(){
			NetworkModel nm=new NetworkModel (0);
			Assert.AreEqual (nm.NodeCount, 0);
			Assert.True (nm.Valid);
		}

		[Test()]
		/**
		 * Create model with one node. Assert node is initially unidentified and model invalid, assert node count and connection count. Set node to END_NODE type, verify GetNodeType and Type[0]. Assert model is valid.
		 * Print model onto Console. Assert link count. Assert no loop connection [0,0]. Create connection [0,0] and assert model is invalid. Print model onto Console.
		 */
		public void ConstructorOne(){
			NetworkModel nm = new NetworkModel (1);
			Assert.AreEqual (nm.GetNodeType (0), NetworkModel.UNIDENTIFIED_NODE);
			Assert.False (nm.Valid);
			Assert.AreEqual (nm.NodeCount, 1);
			Assert.AreEqual (nm.GetConnectionCount (0), 0);
			nm.SetNodeType (0, NetworkModel.END_NODE);
			Assert.AreEqual (NetworkModel.END_NODE, nm.GetNodeType (0));
			Assert.AreEqual (NetworkModel.END_NODE, nm.Type[0]);
			Assert.True (nm.Valid);
			nm.Print ();
			Assert.AreEqual (nm.LinkCount[0], 0);
			Assert.False (nm.AreConnected (0, 0));
			nm.SetConnected (0, 0);
			Assert.False (nm.Valid);
			nm.Print ();
		}

		[Test()]
		/**
		 * Create model with 3 elements. Assert all of them are unidentified and model is invalid. Assert node count and all connection counts. Set node types and assert validity. Create connections, print onto Console, 
		 * assert validity, create invalid connection - assert model not valid, fix and create different invalid connection, assert model not valid
		 */
		public void ConstructorThree(){
			NetworkModel nm = new NetworkModel (3);
			Assert.AreEqual (nm.GetNodeType (0), NetworkModel.UNIDENTIFIED_NODE);
			Assert.AreEqual (nm.GetNodeType (1), NetworkModel.UNIDENTIFIED_NODE);
			Assert.AreEqual (nm.GetNodeType (2), NetworkModel.UNIDENTIFIED_NODE);
			Assert.False (nm.Valid);
			Assert.AreEqual (nm.NodeCount, 3);
			Assert.AreEqual (nm.GetConnectionCount (0), 0);
			Assert.AreEqual (nm.GetConnectionCount (1), 0);
			Assert.AreEqual (nm.GetConnectionCount (2), 0);
			nm.SetNodeType (0, NetworkModel.END_NODE);
			nm.SetNodeType (1, NetworkModel.NETWORK_NODE);
			nm.SetNodeType (2, NetworkModel.SERVER_NODE);
			Assert.True (nm.Valid);
			nm.SetConnected (0, 1);
			nm.SetConnected (1, 2);
			nm.Print ();
			Assert.True (nm.Valid);
			nm.SetConnected (0, 2);
			Assert.False (nm.Valid);
			nm.SetDisconnected (0, 2);
			nm.SetConnected (2, 2);
			Assert.False (nm.Valid);
		}
	}
}

