using NUnit.Framework;
using System;

namespace NetTrafficSimulator
{
	[TestFixture()]
	public class RoutingTableTest
	{
		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void Empty ()
		{
			new RoutingTable ().GetLinkForAddress (0);
		}

		[Test()]
		public void Single(){
			RoutingTable rt = new RoutingTable ();
			Link l = new Link ("L0", 2, new EndNode ("E0", 1, 3), new EndNode ("E1", 0, 3));
			rt.SetRecord (0, l, 1);
			Assert.AreEqual(l,rt.GetLinkForAddress (0));
		}

		[Test()]
		public void TwoRoutes(){
			RoutingTable rt = new RoutingTable ();
			//Link l1=new Link(
		}
	}
}

