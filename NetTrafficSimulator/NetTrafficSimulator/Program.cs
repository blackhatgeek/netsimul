using System;
using Gtk;

namespace NetTrafficSimulator
{
	class MainClass
	{
		const string USAGE="Usage:\nXML IO:\tapp xmlio <input> <output>\nGUI:\tno parameters";
		public static void Main (string[] args)
		{
			//pokud je konzolovy parametr XMLIO, nacist data z XML
			//jinak pustit GUI
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
		}
	}
}
