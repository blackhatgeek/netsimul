using System;

namespace NetTrafficSimulator
{
	/**
	 * Simulation model holds parameters of a simulation
	 */
	public class SimulationModel
	{
		/**
		 * Time to run simulation
		 */
		private int time;

		public SimulationModel ()
		{
		}
	
		/**
		 * Time to run simulation, can't be negative
		 * @throws ArgumentOutOfRangeException negative time on set
		 */ 
		public int Time{
			get{
				return this.time;
			}
			set{
				if (value >= 0)
					this.time = value;
				else
					throw new ArgumentOutOfRangeException ("[SimulationModel] Negative time");
			}
		}


	}
}

