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

		private const int DEFAULT_MAX_HOP = 30;
		private int maxHop;

		public SimulationModel ()
		{
			maxHop = DEFAULT_MAX_HOP;
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

		public int MaxHop{
			get{
				return maxHop;
			}
			set{
				if (value > 0)
					this.maxHop = value;
				else
					throw new ArgumentException ("Max hop must be positive");
			}
		}
	}
}

