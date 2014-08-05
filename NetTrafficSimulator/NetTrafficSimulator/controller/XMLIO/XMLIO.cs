using System;

namespace NetTrafficSimulator
{
	public class XMLIO
	{
		public static void Simulate(string input,string output){
			Loader l = new Loader (input);
			NetworkModel nm = l.LoadNM ();
			SimulationModel sm = l.LoadSM ();
			SimulationController sc = new SimulationController (nm, sm);
			sc.Run ();
			ResultModel rm = sc.Results;
			Storer s = new Storer (output);
			s.StoreResultModel (rm);
		}
	}
}

