using System;

namespace MFF_NPRG031
{
	/**
	 * Framework model holds Calendar and Time
	 * The simulation is started using it's Simulate() method
	 */
	public class Model
	{
		private NetTrafficSimulator.ServerNode[] servers;
		public NetTrafficSimulator.ServerNode[] Servers{
			get{
				return servers;
			}
		}

		/**
		 * Simulation calendar
		 */
		public Calendar K;
		/**
		 * End of simulation flag
		 */
		public bool Finish;
		/**
		 * Simulation time
		 */
		public int Time;
		/**
		 * User set time to run the simulation
		 */
		private int time_to_run;

		/**
		 * Creates a model
		 * @param time_to_run how long to run simulation
		 */
		public Model(int time_to_run,NetTrafficSimulator.ServerNode[] servers)
		{
			K = new Calendar ();
			Finish = false;
			this.time_to_run = time_to_run;
			this.servers = servers;
		}

		/**
		 * Start a simulation
		 */
		public int Simulate()
		{
			while (!Finish) {
				Event e = K.First ();
				if (e != null) {
					Time = e.when;
					if (Time >= time_to_run)
							Finish = true;
					e.who.ProcessEvent (e.what, this);
				} else
					Finish = true;
			}
			return Time;
		}
	}
}

