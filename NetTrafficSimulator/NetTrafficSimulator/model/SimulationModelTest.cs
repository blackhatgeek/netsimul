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
			SimulationModel sm = new SimulationModel(0);
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
			new SimulationModel (0).Time=-1;

		}
	}
}

