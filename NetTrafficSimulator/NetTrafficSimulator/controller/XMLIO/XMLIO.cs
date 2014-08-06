using System;
using System.IO;
using System.Xml.Schema;

namespace NetTrafficSimulator
{
	public class XMLIO
	{
		public static void Simulate(string input,string output){
			try{
				Loader l = new Loader (input);
				NetworkModel nm = l.LoadNM ();
				SimulationModel sm = l.LoadSM ();
				SimulationController sc = new SimulationController (nm, sm);
				sc.Run ();
				ResultModel rm = sc.Results;
				Storer s = new Storer (output);
				s.StoreResultModel (rm);
			}catch(IOException e ){
				Console.WriteLine ("ERROR: "+e.Message);
			}catch(XmlSchemaException e){
				Console.WriteLine ("ERROR: input file not valid");
				Console.WriteLine (e.Message);
			}
		}
	}
}

