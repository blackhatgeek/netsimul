using NUnit.Framework;
using System;

namespace NetTrafficSimulator
{
	/**
	 * Test of SimulationModel class
	 */
	[TestFixture()]
	public class SimulationModelTest
	{
		[Test()]
		/**
		 * Create a simulation model with random non-negatove number as time parameter
		 */
		public void SetUp ()
		{
			int t = new Random ().Next ();
			SimulationModel sm = new SimulationModel();
			sm.Time = t;
			Assert.AreEqual (t, sm.Time);
		}

		[Test()]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		/**
		 * Attempt to create a SimulationModel with negative time - expected exception to be thrown
		 */ 
		public void NegativeTime()
		{
			new SimulationModel ().Time=-1;

		}
	}
}

