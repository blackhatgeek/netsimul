using System;
using log4net;

namespace MFF_NPRG031
{
	/**
	 * Framework model holds Calendar and Time
	 * The simulation is started using it's Simulate() method
	 */
	public class Model
	{
		/**
		 * Logger
		 */
		private static readonly ILog log=LogManager.GetLogger(typeof(Model));
		/**
		 * Servers available
		 */
		private NetTrafficSimulator.ServerNode[] servers;
		/**
		 * Servers available
		 */
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
		 * @param servers servers available for random choice
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
			log.Info ("Simulation started");
			while (!Finish) {
				Event e = K.First ();
				if (e != null) {
					log.Debug ("<Event> WHO:" + e.who + " WHAT:" + e.what + " WHEN:" + e.when);
					Time = e.when;
					if (Time > time_to_run) {
						Finish = true;
						log.Debug ("<Skipped event> WHO:" + e.who + " WHAT:" + e.what + " WHEN:" + e.when);
					} else {
						e.who.ProcessEvent (e.what, this);
					}
				} else
					Finish = true;
			}
			foreach (Event e in K.GetK())
				log.Debug ("<Remainder> WHO:" + e.who + " WHAT:" + e.what + " WHEN:" + e.when);
			log.Info ("Simulation finished at time " + Time);
			return Time;
		}
	}
}

