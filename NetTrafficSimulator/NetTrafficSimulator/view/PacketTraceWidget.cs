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
			LinkedList<LinkedList<KeyValuePair<Node,int>>> list_traces = rm.GetPacketTraces ();
			LinkedListNode<LinkedList<KeyValuePair<Node,int>>> list_trace_node = list_traces.First;
			while (list_trace_node!=null) {
				LinkedList<KeyValuePair<Node,int>> list_trace = list_trace_node.Value;
				LinkedListNode<KeyValuePair<Node,int>> node = list_trace.First;
				while (node!=null) {
					store.AppendValues (node.Value.Key.Name, node.Value.Value);
					node = node.Next;
				}
				list_trace_node = list_trace_node.Next;
			}
		}
	}
}

