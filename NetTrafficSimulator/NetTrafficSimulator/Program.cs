using System;
using Gtk;
using log4net;
using log4net.Config;

namespace NetTrafficSimulator
{
	class MainClass
	{
		static readonly ILog log = LogManager.GetLogger(typeof(MainClass));
		const string USAGE="Usage:\nXML IO:\tapp xmlio <input> <output>\nGUI:\tno parameters";
		public static void Main (string[] args)
		{
			//nacist konfiguraci loggeru z XML
			XmlConfigurator.Configure (new System.IO.FileInfo("log4net.xml"));
			//pokud je konzolovy parametr XMLIO, nacist data z XML
			//jinak pustit GUI
			log.Info ("Entering application");
			try{
				if (args.Length == 3) {
					if (args [0].Equals ("xmlio")) {
						XMLIO.Simulate (args [1], args [2]);
					} else
						Console.WriteLine (USAGE);
				} else if (args.Length == 0) {
					Application.Init ();
					MainWindow win = new MainWindow ();
					win.Show ();
					Application.Run ();
				} else
					Console.WriteLine (USAGE);
			}catch(Exception e){
				log.Error("EXCEPTION: "+e.Message+"\n"+e.StackTrace);
			}
			log.Info ("Leaving application");
		}
	}
}
