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
		Gtk.TreeStore store;

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

			store = new Gtk.TreeStore (typeof(string), typeof(int));
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
				int i = 0;
				if (traces.Length != 0) {
					foreach (LinkedList<KeyValuePair<string,int>> ll in traces) {
						Gtk.TreeIter node = store.AppendValues (ll.First.Value.Key+"->"+ll.Last.Value.Key,ll.First.Value.Value);
						foreach (KeyValuePair<string,int> kvp in ll) {
							store.AppendValues (node, kvp.Key, kvp.Value);
						}
						i++;
					}
				}
			} else
				log.Warn ("Load traces from ResultModel to PacketTraceWidget - Result model null");
		}

		/**
		 * Clear widget before new run
		 */
		public void Clear(){
			log.Debug ("Clear trace");
			this.store.Clear ();
		}
	}
}

