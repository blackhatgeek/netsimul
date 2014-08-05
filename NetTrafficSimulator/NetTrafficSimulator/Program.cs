using System;
using Gtk;

namespace NetTrafficSimulator
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//pokud je konzolovy parametr XMLIO, nacist data z XML
			//jinak pustit GUI

		}

		public static void RunGUI(){
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
		}

		public static void RunXMLIO(){

		}
	}
}
