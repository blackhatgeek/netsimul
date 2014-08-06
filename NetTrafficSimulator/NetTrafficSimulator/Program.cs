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
			BasicConfigurator.Configure ();
			//pokud je konzolovy parametr XMLIO, nacist data z XML
			//jinak pustit GUI
			log.Info ("Entering application");
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
			log.Info ("Leaving application");
		}
	}
}
