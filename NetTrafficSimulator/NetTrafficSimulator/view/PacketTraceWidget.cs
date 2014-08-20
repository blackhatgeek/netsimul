using System;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class PacketTraceWidget : Gtk.Bin
	{
		Gtk.ListStore store;
		static readonly ILog log = LogManager.GetLogger(typeof(PacketTraceWidget));
		public PacketTraceWidget ()
		{
			this.Build ();

			Gtk.TreeViewColumn nameCol = new Gtk.TreeViewColumn ();
			nameCol.Title = "Node name";
			Gtk.TreeViewColumn timeCol = new Gtk.TreeViewColumn ();
			timeCol.Title = "Time";
			treeview1.AppendColumn (nameCol);
			treeview1.AppendColumn (timeCol);

			store = new Gtk.ListStore (typeof(string), typeof(int));
			treeview1.Model = store;

			Gtk.CellRendererText nameCell = new Gtk.CellRendererText ();
			nameCol.PackStart (nameCell, true);

			Gtk.CellRendererText timeCell = new Gtk.CellRendererText ();
			timeCol.PackStart (timeCell, true);

			nameCol.AddAttribute (nameCell, "text", 0);
			timeCol.AddAttribute (timeCell, "text", 1);
		}

		public void Load(ResultModel rm){
			log.Debug ("Loading packet traces");
			LinkedList<KeyValuePair<string,int>>[] traces = rm.GetPacketTraces ();
			foreach (LinkedList<KeyValuePair<string,int>> ll in traces) {
				foreach (KeyValuePair<string,int> kvp in ll) {
					store.AppendValues (kvp.Key, kvp.Value);
				}
			}
		}
	}
}

