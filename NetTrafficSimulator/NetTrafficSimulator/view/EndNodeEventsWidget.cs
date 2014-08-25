using System;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * EndNodeEventsWidget shows events scheduled for an EndNode
	 */
	[System.ComponentModel.ToolboxItem(true)]
	public partial class EndNodeEventsWidget : Gtk.Bin
	{
		static readonly ILog log = LogManager.GetLogger(typeof(EndNodeEventsWidget));
		Gtk.ListStore store;
		/**
		 * EndNode name
		 */
		public string name;
		NetworkModel nm;
		SimulationModel sm;
		MainWindow mw;
		/**
		 * Create new EndNodeEventsWidget - initialize nodeview to show table of events
		 */
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
			store = new Gtk.ListStore (typeof(string),typeof(int),typeof(double));
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

		/**
		 * Load events to the EndNodeEventsWidget for the node name specified
		 */
		public void LoadParams(NetworkModel nm,SimulationModel sm,String nname,MainWindow mw){
			this.nm = nm;
			this.sm = sm;
			this.mw = mw;
			this.name = nname;
			System.Collections.Generic.LinkedList<SimulationModel.Event> evs = sm.GetEvents ();
			foreach (SimulationModel.Event e in evs) {
				if (e.node1.Equals (nname)) {
					store.AppendValues (e.node2, e.when, e.size);
				}
			}
		}

		/**
		 * Add button - show NewEventDialog and if OK was clicked there and addition was not rejected, add new event to the table
		 * If addition was rejected (as result of Exception from SimulationModel) the error message will be shown
		 */
		protected void OnButton2Clicked (object sender, EventArgs e)
		{
			if((nm!=null)&&(sm!=null)){
				NewEventDialog ned = new NewEventDialog (name, nm, sm,mw);
				int response = ned.Run ();
				if (response == (int)Gtk.ResponseType.Ok) {
					log.Debug (ned.generated_event.node2 + ", " + ned.generated_event.size + ", " + ned.generated_event.when);
					store.AppendValues (ned.generated_event.node2, ned.generated_event.when, (double)ned.generated_event.size);
					ned.Destroy ();
				} else if (response == (int)Gtk.ResponseType.Reject) {
					ned.Destroy ();
					Gtk.MessageDialog md = new Gtk.MessageDialog (mw, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, "Create event failed");
					md.Run ();
					md.Destroy ();
				} else {
					ned.Destroy ();
				}
			}
		}

		NetTrafficSimulator.SimulationModel.Event ev;
		Gtk.TreeIter ti;

		/**
		 * On cursor change note selected event (for deletion)
		 */
		protected void OnNodeview3CursorChanged (object sender, EventArgs e)
		{
			Gtk.TreeSelection selection = (sender as Gtk.NodeView).Selection;
			Gtk.TreeModel model;
			Gtk.TreeIter iter;
			if (selection.GetSelected (out model, out iter)) {
				string to = model.GetValue (iter, 0) as string;
				int when = (int)model.GetValue (iter, 1);
				decimal size = (decimal)(double)model.GetValue (iter, 2);

				ev = new SimulationModel.Event (name,to, when, size);
				ti = iter;

				button1.Sensitive = true;
			} else
				button1.Sensitive = false;
		}

		/**
		 * On delete button clicked remove sellected event
		 */
		protected void OnButton1Clicked (object sender, EventArgs e)
		{
			System.Collections.Generic.LinkedList<NetTrafficSimulator.SimulationModel.Event> events = sm.GetEvents ();
			NetTrafficSimulator.SimulationModel.Event toRemove = new SimulationModel.Event();
			bool init = false;
				System.Collections.Generic.LinkedListNode<NetTrafficSimulator.SimulationModel.Event> node = events.First;
				while (node.Next!=null) {
					if (node.Value.node1.Equals (ev.node1)&&node.Value.node2.Equals(ev.node2)&&
					    	(node.Value.size==ev.size)&&(node.Value.when==ev.when)) {
								toRemove = node.Value;
								init = true;
								break;
					}
					node = node.Next;
				}
			if(init)
				events.Remove (toRemove);
			store.Remove (ref ti);
		}
	}
}

