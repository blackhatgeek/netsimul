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
			log.Info("Entered XMLIO("+input+","+output+")");
			try{
				Loader l = new Loader (input);
				NetworkModel nm = l.LoadNM ();
				SimulationModel sm = l.LoadSM ();
				SimulationController sc = new SimulationController (nm, sm);
				log.Info("Loaded data, created models and controller, starting simulation");
				sc.Run ();
				log.Info("Storing results");
				ResultModel rm = sc.Results;
				Storer s = new Storer (output);
				s.StoreResultModel (rm);
				log.Info("Leaving XMLIO");
			}catch(IOException e ){
				log.Error (e.Message);
				log.Debug(e.StackTrace);
			}catch(XmlSchemaException e){
				log.Error ("Input file not valid");
				log.Debug("EXCEPTION: "+e.Message+"\n"+e.StackTrace);
			}
		}
	}
}

