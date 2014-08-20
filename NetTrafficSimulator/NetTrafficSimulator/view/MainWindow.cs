using System;
using System.IO;
using System.Xml.Schema;
using Gtk;
using log4net;

public partial class MainWindow: Gtk.Window
{	
	NetTrafficSimulator.NetworkModel nm;
	NetTrafficSimulator.SimulationModel sm;
	NetTrafficSimulator.ResultModel rm;
	private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
	ListStore linkListStore,nodeListStore;
	TreeViewColumn nodeNameColumn,nodeTypeColumn,linkNameColumn,linkNodeAColumn,linkNodeBColumn;
	string[] node_names,link_names;

	const string END = "END", SERVER = "SERVER", NETWORK = "NETWORK";

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		//nodes tree view init
		nodeNameColumn = new TreeViewColumn ();
		nodeNameColumn.Title = "Name";
		nodeTypeColumn = new Gtk.TreeViewColumn ();
		nodeTypeColumn.Title = "Type";
		treeview3.AppendColumn (nodeNameColumn);
		treeview3.AppendColumn (nodeTypeColumn);
		nodeListStore = new Gtk.ListStore (typeof(string), typeof(string));
		treeview3.Model = nodeListStore;

		CellRendererText nodeNameCell = new CellRendererText ();
		nodeNameColumn.PackStart (nodeNameCell, true);
		CellRendererText nodeTypeCell = new CellRendererText ();
		nodeTypeColumn.PackStart (nodeTypeCell, true);

		nodeNameColumn.AddAttribute (nodeNameCell, "text", 0);
		nodeTypeColumn.AddAttribute (nodeTypeCell, "text", 1);

		treeview3.CursorChanged += new EventHandler (nodeTreeCursorChanged);

		//links tree view init
		linkNameColumn = new Gtk.TreeViewColumn ();
		linkNameColumn.Title = "Name";
		linkNodeAColumn = new TreeViewColumn ();
		linkNodeAColumn.Title = "Node A";
		linkNodeBColumn = new TreeViewColumn ();
		linkNodeBColumn.Title = "Node B";
		treeview4.AppendColumn (linkNameColumn);
		treeview4.AppendColumn (linkNodeAColumn);
		treeview4.AppendColumn (linkNodeBColumn);
		linkListStore = new Gtk.ListStore (typeof(string),typeof(string),typeof(string));
		treeview4.Model = linkListStore;

		CellRendererText linkNameCell = new CellRendererText ();
		linkNameColumn.PackStart (linkNameCell, true);
		CellRendererText linkNodeACell = new CellRendererText ();
		linkNodeAColumn.PackStart (linkNodeACell, true);
		CellRendererText linkNodeBCell = new CellRendererText ();
		linkNodeBColumn.PackStart (linkNodeBCell, true);

		linkNameColumn.AddAttribute (linkNameCell, "text", 0);
		linkNodeAColumn.AddAttribute (linkNodeACell, "text", 1);
		linkNodeBColumn.AddAttribute (linkNodeBCell, "text", 2);
		treeview4.CursorChanged += new EventHandler (linkTreeCursorChanged);
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	protected void loadFromFileHandler (object sender, EventArgs ev)
	{
			FileChooserDialog fc = new Gtk.FileChooserDialog ("Model to load", this, FileChooserAction.Open, 
			                                                      "Cancel", ResponseType.Cancel, "Load", ResponseType.Accept);
			if (fc.Run () == (int)ResponseType.Accept) {
				NetTrafficSimulator.Loader loader = new NetTrafficSimulator.Loader (fc.Filename);
				try{
					nm = loader.LoadNM ();
					sm = loader.LoadSM ();

					node_names=nm.GetNodeNames();
					link_names=nm.GetLinkNames();

					//naplnit nodes
					loadNodesBox();
					//naplnit links
					loadLinksBox();

				}catch(ArgumentOutOfRangeException e){
					log.Error ("Attribute value out of range: "+e.Message);
					log.Debug("EXCEPTION: "+e.Message+"\n"+e.StackTrace);
					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error, 
				                                          ButtonsType.Close, "Attribute value out of range: " + e.Message);
					md.Run ();
					md.Destroy ();
				}catch(ArgumentNullException e){
					log.Error ("Argument null: " + e.Message);
					log.Debug ("EXCEPTION: " + e.Message + "\n" + e.StackTrace);
					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error,
				                                          ButtonsType.Close, "Argument null: " + e.Message);
					md.Run ();
					md.Destroy ();
				}catch(ArgumentException e){
					log.Error ("Error: " + e.Message);
					log.Debug ("EXCEPTION: " + e.Message + "\n" + e.StackTrace);
					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error,
				                                          ButtonsType.Close, "Error: " + e.Message);
					md.Run ();
					md.Destroy ();
				}catch(IOException e ){
					log.Error (e.Message);
					log.Debug(e.StackTrace);
					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error,
				                                          ButtonsType.Close, e.Message);
					md.Run ();
					md.Destroy ();
				}catch(XmlSchemaException e){
					log.Error ("Input file not valid");
					log.Debug("EXCEPTION: "+e.Message+"\n"+e.StackTrace);
					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error,
					                                          ButtonsType.Close, "Input file not valid");
					md.Run ();
					md.Destroy ();
				}
			}

			fc.Destroy ();
	}

	protected void OnParametersMenuClick (object sender, EventArgs ev){
		NetTrafficSimulator.SimulationParametersDialog spd = new NetTrafficSimulator.SimulationParametersDialog (sm);
		if (spd.Run () == (int)ResponseType.Ok) {
			sm.MaxHop = spd.maxHop;
			sm.Time = spd.time;
		}
		spd.Destroy ();
	}

	protected void OnRunMenuClick (object sender, EventArgs ev){
		NetTrafficSimulator.SimulationController sc = new NetTrafficSimulator.SimulationController (nm, sm);
		log.Info("Loaded data, created models and controller, starting simulation");
		sc.Run ();
		log.Info("Storing results");
		rm = sc.Results;
		packettracewidget1.Load (rm);
	}

	protected void exitHandler (object sender, EventArgs ev){
		Application.Quit ();
	}

	private void loadNodesBox(){
		foreach (string node in node_names) {
			string type="";
			switch (nm.GetNodeType (node)) {
			case NetTrafficSimulator.NetworkModel.END_NODE:
				type = END;
				break;
			case NetTrafficSimulator.NetworkModel.NETWORK_NODE:
				type = NETWORK;
				break;
			case NetTrafficSimulator.NetworkModel.SERVER_NODE:
				type = SERVER;
				break;
			default:
				MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close,
				                                      "Unidentified node type");
				md.Run ();
				md.Destroy ();
				break;
			}
			log.Debug ("Appending: " + node + " (" + type + ")");
			nodeListStore.AppendValues (node, type);
		}
	}

	private void loadLinksBox(){
		foreach (string link in link_names) {
			linkListStore.AppendValues (link, nm.GetLinkNode1 (link), nm.GetLinkNode2 (link));
			log.Debug ("Appending: " + link + " node: " + nm.GetLinkNode1(link) + " node:" + nm.GetLinkNode2(link));
		}
	}

	private void nodeTreeCursorChanged(object sender,EventArgs e){
		TreeSelection selection = (sender as TreeView).Selection;
		TreeModel model;
		TreeIter iter;
		if (selection.GetSelected (out model, out iter)) {
			//node name ... model.GetValue(iter,0);
			//node type ... model.GetValue(iter,1);
			GtkAlignment2.Child.Destroy();
			GtkLabel12.Text = "<b>"+model.GetValue (iter, 0).ToString()+"</b>";
			GtkLabel12.UseMarkup = true;
			switch (model.GetValue (iter, 1).ToString()) {
			case SERVER:
				NetTrafficSimulator.ServerNodeWidget sw = new NetTrafficSimulator.ServerNodeWidget ();
				sw.ParamWidget.LoadParams (nm, model.GetValue (iter, 0).ToString ());
				GtkAlignment2.Child = sw;
				break;
			case END:
				NetTrafficSimulator.EndNodeWidget ew = new NetTrafficSimulator.EndNodeWidget ();
				ew.ParamWidget.LoadParams (nm, sm, model.GetValue (iter, 0).ToString ());
				ew.EventWidget.LoadParams (sm, model.GetValue (iter, 0).ToString ());
				GtkAlignment2.Child = ew;
				break;
			case NETWORK:
				log.Debug ("Cursor on NN");
				NetTrafficSimulator.NetworkNodeWidget nw = new NetTrafficSimulator.NetworkNodeWidget ();
				log.Debug ("New NN widget");
				nw.ParamWidget.LoadParams (nm, model.GetValue (iter, 0).ToString ());
				log.Debug ("Loaded params");
				GtkAlignment2.Child = nw;
				log.Debug ("Done here");
				break;
			default:
				break;
			}

			GtkAlignment2.Child.Visible = true;
		}
	}

	private void linkTreeCursorChanged(object sender,EventArgs e){
		TreeSelection selection = (sender as TreeView).Selection;
		TreeModel model;
		TreeIter iter;
		if (selection.GetSelected (out model, out iter)) {
			//link name ... model.GetValue(iter,0);
			//node A    ... model.GetValue(iter,1);
			//node B    ... model.GetValue(iter,2);
			GtkAlignment2.Child.Destroy();
			GtkLabel12.Text = "<b>"+model.GetValue (iter, 0).ToString()+"</b>";
			GtkLabel12.UseMarkup = true;
			NetTrafficSimulator.LinkWidget lw = new NetTrafficSimulator.LinkWidget ();
			lw.ParamWidget.LoadParams (nm, model.GetValue (iter, 0).ToString ());
			GtkAlignment2.Child = lw;
			GtkAlignment2.Child.Visible = true;
		}
	}
}
