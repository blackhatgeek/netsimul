using System;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class EndNodeEventsWidget : Gtk.Bin
	{
		Gtk.ListStore store;
		public EndNodeEventsWidget ()
		{
			this.Build ();

			Gtk.TreeViewColumn toCol = new Gtk.TreeViewColumn ();
			toCol.Title = "To";
			Gtk.TreeViewColumn whenCol = new Gtk.TreeViewColumn ();
			whenCol.Title = "When";
			Gtk.TreeViewColumn sizeCol = new Gtk.TreeViewColumn ();
			sizeCol.Title = "Size";
			nodeview3.AppendColumn (toCol);
			nodeview3.AppendColumn (whenCol);
			nodeview3.AppendColumn (sizeCol);
			store = new Gtk.ListStore (typeof(string),typeof(int),typeof(int));
			nodeview3.Model = store;
			Gtk.CellRendererText toCell = new Gtk.CellRendererText ();
			toCol.PackStart (toCell, true);
			Gtk.CellRendererText whenCell = new Gtk.CellRendererText ();
			whenCol.PackStart (whenCell, true);
			Gtk.CellRendererText sizeCell = new Gtk.CellRendererText ();
			sizeCol.PackStart (sizeCell, true);
			toCol.AddAttribute (toCell, "text", 0);
			whenCol.AddAttribute (whenCell, "text", 1);
			sizeCol.AddAttribute (sizeCell, "text", 2);
		}

		public void LoadParams(SimulationModel sm,String nname){
			SimulationModel.Event[] evs = sm.GetEvents ();
			foreach (SimulationModel.Event e in evs) {
				if (e.node1.Equals (nname)) {
					store.AppendValues (e.node2, e.when, e.size);
				}
			}
		}
	}
}

