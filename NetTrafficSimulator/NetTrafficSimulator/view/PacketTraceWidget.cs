using System;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * Packet trace widget shows packet traces from the latest simulation
	*/
	[System.ComponentModel.ToolboxItem(true)]
	public partial class PacketTraceWidget : Gtk.Bin
	{
		Gtk.ListStore store;
		static readonly ILog log = LogManager.GetLogger(typeof(PacketTraceWidget));
		/**
		 * Build the widget - set up treeview
		 */
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

		/**
		 * Load traces from ResultModel
		 */
		public void Load(ResultModel rm){
			if (rm != null) {
				log.Debug ("Loading packet traces");
				LinkedList<KeyValuePair<string,int>>[] traces = rm.GetPacketTraces ();
				foreach (LinkedList<KeyValuePair<string,int>> ll in traces) {
					foreach (KeyValuePair<string,int> kvp in ll) {
						store.AppendValues (kvp.Key, kvp.Value);
					}
				}
			} else
				log.Warn ("Load traces from ResultModel to PacketTraceWidget - Result model null");
		}

		/**
		 * Clear widget before new run
		 */
		public void Clear(){
			this.store.Clear ();
		}
	}
}

