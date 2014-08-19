using System;
using System.IO;
using System.Xml.Schema;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * Given input and output XML files, loads and runs a simulation
	 */ 
	public class XMLIO
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(XMLIO));

		/**
		 * Set up Loader, get NetworkModel and SimulationModel, pass them to SimulationController, run SimulationController, get ResultModel, set up Storer and store ResultModel
		 */
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
			}catch(ArgumentOutOfRangeException e){
				log.Error ("Attribute value out of range: "+e.Message);
				log.Debug("EXCEPTION: "+e.Message+"\n"+e.StackTrace);
			}catch(ArgumentNullException e){
				log.Error ("Argument null: " + e.Message);
				log.Debug ("EXCEPTION: " + e.Message + "\n" + e.StackTrace);
			}catch(ArgumentException e){
				log.Error ("Error: " + e.Message);
				log.Debug ("EXCEPTION: " + e.Message + "\n" + e.StackTrace);
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

