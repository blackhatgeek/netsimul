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
			//nacist konfiguraci loggeru z XML pokud existuje
			if (System.IO.File.Exists ("log4net.xml"))
				XmlConfigurator.Configure (new System.IO.FileInfo ("log4net.xml"));
			else {
				log4net.Filter.LevelRangeFilter lrf = new log4net.Filter.LevelRangeFilter ();
				lrf.LevelMin = log4net.Core.Level.Info;
				log4net.Appender.ConsoleAppender ca = new log4net.Appender.ConsoleAppender ();
				ca.AddFilter (lrf);
				log4net.Appender.RollingFileAppender rfa = new log4net.Appender.RollingFileAppender ();
				rfa.AppendToFile = true;
				rfa.File = "trasim.log";
				rfa.MaximumFileSize = "100KB";
				rfa.MaxSizeRollBackups = 2;
				log4net.Repository.Hierarchy.Hierarchy h = new log4net.Repository.Hierarchy.Hierarchy ();
				BasicConfigurator.Configure (h,ca,rfa);
			}
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
				log.Debug("EXCEPTION: "+e.Message+"\n"+e.StackTrace);
			}
			log.Info ("Leaving application");
		}
	}
}
