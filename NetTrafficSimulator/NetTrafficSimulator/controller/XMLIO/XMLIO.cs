using System;
using System.IO;
using System.Xml.Schema;
using log4net;

namespace NetTrafficSimulator
{
	public class XMLIO
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(XMLIO));
		public static void Simulate(string input,string output){
			log.Info("Entering simulate("+input+","+output+")");
			try{
				Loader l = new Loader (input);
				NetworkModel nm = l.LoadNM ();
				SimulationModel sm = l.LoadSM ();
				SimulationController sc = new SimulationController (nm, sm);
				log.Info("Loaded data, created models and controller, starting simulation");
				sc.Run ();
				log.Info("Simulation finished, storing results");
				ResultModel rm = sc.Results;
				Storer s = new Storer (output);
				s.StoreResultModel (rm);
				log.Info("Leaving simulate");
			}catch(IOException e ){
				log.Error("EXCEPTION: "+e.Message+"\n"+e.StackTrace);
				Console.WriteLine ("ERROR: "+e.Message);
			}catch(XmlSchemaException e){
				log.Error("EXCEPTION: "+e.Message+"\n"+e.StackTrace);
				Console.WriteLine ("ERROR: input file not valid");
				Console.WriteLine (e.Message); 
			}
		}
	}
}

