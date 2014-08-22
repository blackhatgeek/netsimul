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
	string model_path,result_path;

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
			if (model_path != null)
				fc.SetCurrentFolder(model_path);
			if (fc.Run () == (int)ResponseType.Accept) {
				model_path = fc.CurrentFolder;
				try{
					NetTrafficSimulator.Loader loader = new NetTrafficSimulator.Loader (fc.Filename);
					nm = loader.LoadNM ();
					sm = loader.LoadSM ();

					node_names=nm.GetNodeNames();
					link_names=nm.GetLinkNames();

					//naplnit nodes
					loadNodesBox();
					//naplnit links
					loadLinksBox();

				}catch(System.Xml.Schema.XmlSchemaException e){
					log.Error ("Input file not valid");
					log.Debug("EXCEPTION: "+e.Message+"\n"+e.StackTrace);
					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error,
					                                      ButtonsType.Close, "Input file not valid");
					md.Run ();
					md.Destroy ();
				
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
				}
			}

			fc.Destroy ();
	}

	protected void OnParametersMenuClick (object sender, EventArgs ev){
		if (sm != null) {
			NetTrafficSimulator.SimulationParametersDialog spd = new NetTrafficSimulator.SimulationParametersDialog (sm);
			if (spd.Run () == (int)ResponseType.Ok) {
				sm.MaxHop = spd.maxHop;
				sm.Time = spd.time;
			}
			spd.Destroy ();
		} else {
			MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Close, "Load or create model first!");
			md.Run ();
			md.Destroy ();
		}
	}

	protected void OnRunMenuClick (object sender, EventArgs ev){
		if ((nm != null) && (sm != null)) {
			NetTrafficSimulator.SimulationController sc = new NetTrafficSimulator.SimulationController (nm, sm);
			log.Info ("Loaded data, created models and controller, starting simulation");
			sc.Run ();
			log.Info ("Storing results");
			rm = sc.Results;
			packettracewidget1.Load (rm);
		} else {
			MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Close, "Load or create model first!");
			md.Run ();
			md.Destroy ();
		}
	}

	protected void exitHandler (object sender, EventArgs ev){
		Application.Quit ();
	}

	protected void saveResultsHandler(object sender,EventArgs ev){
		if (rm != null) {
			FileChooserDialog fd = new Gtk.FileChooserDialog ("Save results as ", this, FileChooserAction.Save, "Cancel", ResponseType.Close, "Save", ResponseType.Accept);
			if (result_path != null)
				fd.SetCurrentFolder (result_path);
			else if (model_path != null)
				fd.SetCurrentFolder (model_path);

			if (fd.Run () == (int)ResponseType.Accept) {
				result_path = fd.CurrentFolder;
				bool store = true;
				if(File.Exists(fd.Filename)){
					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.YesNo, "File exists! Overwrite?");
					if (md.Run() == (int)ResponseType.Yes) {
						File.Delete (fd.Filename);
					} else
						store = false;
					md.Destroy ();
				}
				if (store) {
					NetTrafficSimulator.Storer storer = new NetTrafficSimulator.Storer (fd.Filename);
					storer.StoreResultModel (rm);
				}
			}
			fd.Destroy ();
		} else {
			MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Close, "Run simulation first!");
			md.Run ();
			md.Destroy ();
		}
	}

	private void loadNodesBox(){
		nodeListStore.Clear ();
		if (node_names != null) {
			foreach (string node in node_names) {
				string type = "";
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
	}

	private void loadLinksBox(){
		linkListStore.Clear ();
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
			GtkLabel13.Text = "<b>"+model.GetValue (iter, 0).ToString()+"</b>";
			GtkLabel13.UseMarkup = true;
			switch (model.GetValue (iter, 1).ToString()) {
			case SERVER:
				NetTrafficSimulator.ServerNodeWidget sw = new NetTrafficSimulator.ServerNodeWidget ();
				sw.ParamWidget.LoadParams (nm, model.GetValue (iter, 0).ToString (),this);
				if (rm != null)
					sw.ResultWidget.LoadParams (rm, model.GetValue (iter, 0).ToString ());
				GtkAlignment2.Child = sw;
				break;
			case END:
				NetTrafficSimulator.EndNodeWidget ew = new NetTrafficSimulator.EndNodeWidget ();
				ew.ParamWidget.LoadParams (nm, sm, model.GetValue (iter, 0).ToString (),this);
				ew.EventWidget.LoadParams (nm, sm, model.GetValue (iter, 0).ToString (),this);
				if (rm != null)
					ew.ResultWidget.LoadParams (rm, model.GetValue (iter, 0).ToString ());
				GtkAlignment2.Child = ew;
				break;
			case NETWORK:
				NetTrafficSimulator.NetworkNodeWidget nw = new NetTrafficSimulator.NetworkNodeWidget ();
				nw.ParamWidget.LoadParams (nm, model.GetValue (iter, 0).ToString (),this);
				if (rm != null)
					nw.ResultWidget.LoadParams (rm, model.GetValue (iter, 0).ToString ());
				GtkAlignment2.Child = nw;
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
			GtkAlignment2.Child.Destroy ();
			GtkLabel13.Text = "<b>" + model.GetValue (iter, 0).ToString () + "</b>";
			GtkLabel13.UseMarkup = true;
			NetTrafficSimulator.LinkWidget lw = new NetTrafficSimulator.LinkWidget ();
			lw.ParamWidget.LoadParams (nm, model.GetValue (iter, 0).ToString (),this);
			if (rm != null)
				lw.ResultWidget.LoadParams (rm, model.GetValue (iter, 0).ToString ());
			GtkAlignment2.Child = lw;
			GtkAlignment2.Child.Visible = true;
		}

	}

	protected void newModelHandler (object sender, EventArgs e)
	{
		nm = new NetTrafficSimulator.NetworkModel ();
		sm = new NetTrafficSimulator.SimulationModel ();
		linkListStore.Clear ();
		nodeListStore.Clear ();
	}
	
	protected void onAddNewEndNode (object sender, EventArgs e)
	{
		addNode (NetTrafficSimulator.NetworkModel.END_NODE);
	}
	protected void onAddNetNode (object sender, EventArgs e)
	{
		addNode (NetTrafficSimulator.NetworkModel.NETWORK_NODE);
	}
	protected void onAddServNode (object sender, EventArgs e)
	{
		addNode (NetTrafficSimulator.NetworkModel.SERVER_NODE);
	}


	private void addNode(int ntype){
		if ((ntype == NetTrafficSimulator.NetworkModel.END_NODE) || (ntype == NetTrafficSimulator.NetworkModel.NETWORK_NODE) || (ntype == NetTrafficSimulator.NetworkModel.SERVER_NODE)) {
			if (nm != null) {
				NetTrafficSimulator.NewNodeDialog nend = new NetTrafficSimulator.NewNodeDialog (nm, ntype);
				switch (nend.Run ()) {
				case (int)ResponseType.Reject:
					nend.Destroy ();
					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close, "Duplicate node name! Node names must remain unique in the model. Node was not added.");
					md.Run ();
					md.Destroy ();
					break;
				case (int)ResponseType.No:
					nend.Destroy ();
					MessageDialog md1 = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close, "Node name cannot contain: LF,CR,tab,spaces at the beginning or at the end, multiple spaces next to each other. Node was not added.");
					md1.Run ();
					md1.Destroy ();
					break;
				case (int)ResponseType.Ok:
					string dsc = "";
					switch (ntype) {
					case NetTrafficSimulator.NetworkModel.END_NODE:
						dsc = END;
						break;
					case NetTrafficSimulator.NetworkModel.SERVER_NODE:
						dsc = SERVER;
						break;
					case NetTrafficSimulator.NetworkModel.NETWORK_NODE:
						dsc = NETWORK;
						break;
					}
					node_names = nm.GetNodeNames ();
					TreeIter ti = nodeListStore.AppendValues (nend.node_name, dsc);
					treeview3.Selection.UnselectAll ();
					treeview3.Selection.SelectIter (ti);
					TreePath tp = treeview3.Selection.GetSelectedRows () [0];
					treeview3.SetCursor (tp, nodeNameColumn, false);
					nend.Destroy ();
					break;
				default:
					nend.Destroy ();
					break;
				}
			} else {
				MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Close, "Load or create model first!");
				md.Run ();
				md.Destroy ();
			}
		} else
			throw new ArgumentException ("Invalid node type: " + ntype);
	}

	public void NodeNameChanged(){
		log.Debug ("Triggered node name change");
		node_names = nm.GetNodeNames ();
		loadNodesBox ();
		if (GtkAlignment2.Child is NetTrafficSimulator.EndNodeWidget) {
			GtkLabel13.Text = "<b>" + (GtkAlignment2.Child as NetTrafficSimulator.EndNodeWidget).ParamWidget.name + "</b>";
			GtkLabel13.UseMarkup = true;
			(GtkAlignment2.Child as NetTrafficSimulator.EndNodeWidget).EventWidget.name = (GtkAlignment2.Child as NetTrafficSimulator.EndNodeWidget).ParamWidget.name;
		} else if (GtkAlignment2.Child is NetTrafficSimulator.ServerNodeWidget) {
			GtkLabel13.Text = "<b>" + (GtkAlignment2.Child as NetTrafficSimulator.ServerNodeWidget).ParamWidget.name + "</b>";
			GtkLabel13.UseMarkup = true;
		} else if (GtkAlignment2.Child is NetTrafficSimulator.NetworkNodeWidget) {
			GtkLabel13.Text = "<b>" + (GtkAlignment2.Child as NetTrafficSimulator.NetworkNodeWidget).ParamWidget.name + "</b>";
			GtkLabel13.UseMarkup = true;
		}
	}

	public void LinkChanged(){
		link_names = nm.GetLinkNames ();
		loadLinksBox ();
		if (GtkAlignment2.Child is NetTrafficSimulator.LinkWidget) {
			GtkLabel13.Text = "<b>" + (GtkAlignment2.Child as NetTrafficSimulator.LinkWidget).ParamWidget.GetName () + "</b>";
			GtkLabel13.UseMarkup = true;
		}
	}

	protected void OnAddLinkActionActivated (object sender, EventArgs e)
	{
		if (nm != null) {
			NetTrafficSimulator.NewLinkDialog nld = new NetTrafficSimulator.NewLinkDialog (nm, this);
			switch (nld.Run ()) {
			case (int)ResponseType.Reject:
				nld.Destroy ();
				MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close, "Error occured, link was not added!");
				md.Run ();
				md.Destroy ();
				break;
			case (int)ResponseType.No:
				nld.Destroy ();
				MessageDialog md1 = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close, "Link name cannot contain: LF,CR,tab,spaces at the beginning or at the end, multiple spaces next to each other. Link was not added.");
				md1.Run ();
				md1.Destroy ();
				break;
			case (int)ResponseType.Ok:
				linkListStore.AppendValues (nld.link_name, nld.node1, nld.node2);
				link_names = nm.GetLinkNames ();
				nld.Destroy ();
				break;
			default:
				nld.Destroy ();
				break;
			}
		} else {
			MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Close, "Load or create model first!");
			md.Run ();
			md.Destroy ();
		}
	}

	protected void OnDeleteButtonClicked (object sender, EventArgs e)
	{
		if (nm != null) {
			try{
				if (GtkAlignment2.Child is NetTrafficSimulator.LinkWidget) {
					string[] rel = nm.GetRelatedNodes(GtkLabel13.Text);
					string msg = "Remove link "+GtkLabel13.Text+"?\n\nAffected nodes:\n";
					foreach (string n in rel) {
						msg += "\t" + n + "\n";
					}
					MessageDialog md  = new MessageDialog(this,DialogFlags.DestroyWithParent,MessageType.Question,ButtonsType.YesNo,msg);
					if (md.Run () == (int)ResponseType.Yes) {
						nm.RemoveLink (GtkLabel13.Text);
						link_names=nm.GetLinkNames();
						loadLinksBox ();
					}
					md.Destroy ();
				} else {
					string[] rel = nm.GetRelatedLinks (GtkLabel13.Text);
					int t = nm.GetNodeType (GtkLabel13.Text);

					string msg = "Remove node: " + GtkLabel13.Text + "?\n";

					System.Collections.Generic.LinkedList<NetTrafficSimulator.SimulationModel.Event> events_to_remove = new System.Collections.Generic.LinkedList<NetTrafficSimulator.SimulationModel.Event> ();
					if ((t == NetTrafficSimulator.NetworkModel.END_NODE) || (t == NetTrafficSimulator.NetworkModel.SERVER_NODE)) {
						msg+="\nRelated events to be removed:\n";
						System.Collections.Generic.LinkedList<NetTrafficSimulator.SimulationModel.Event> events = sm.GetEvents ();
						if(events.Count>0){
							System.Collections.Generic.LinkedListNode<NetTrafficSimulator.SimulationModel.Event> node = events.First;
							while (node.Next!=null) {
								if (node.Value.node1.Equals (GtkLabel13.Text)||node.Value.node2.Equals(GtkLabel13.Text)) {
									msg += "\t" + node.Value.node1 + " -> " + node.Value.node2 + " at " + node.Value.when + " of size " + node.Value.size+"\n";
									events_to_remove.AddLast (node.Value);
								}
								node = node.Next;
							}
						}
					}
					if (rel.Length != 0) {
						msg += "\nRelated links to be removed:\n";
						foreach (string l in rel) {
							msg += "\t" + l + "\n";
						}
					}

					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.YesNo, msg);
					if (md.Run () == (int)ResponseType.Yes) {
						foreach (string l in rel) {
							nm.RemoveLink (l);
						}
						foreach (NetTrafficSimulator.SimulationModel.Event ev in events_to_remove) {
							sm.GetEvents ().Remove (ev);
						}
						nm.RemoveNode (GtkLabel13.Text);
						node_names=nm.GetNodeNames();
						link_names=nm.GetLinkNames();
						loadLinksBox ();
						loadNodesBox ();
					}
					md.Destroy ();
				}
			}catch(ArgumentException ae){
				log.Debug (ae.Message);
			}
		}
	}

	protected void OnSaveModelAsActionActivated (object sender, EventArgs e)
	{
		if ((nm!=null)&&(sm != null)) {
			FileChooserDialog fd = new Gtk.FileChooserDialog ("Save results as ", this, FileChooserAction.Save, "Cancel", ResponseType.Close, "Save", ResponseType.Accept);
			if (model_path != null)
				fd.SetCurrentFolder (model_path);

			if (fd.Run () == (int)ResponseType.Accept) {
				model_path = fd.CurrentFolder;
				bool store = true;
				if(File.Exists(fd.Filename)){
					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.YesNo, "File exists! Overwrite?");
					if (md.Run() == (int)ResponseType.Yes) {
						File.Delete (fd.Filename);
					} else
						store = false;
					md.Destroy ();
				}
				if (store) {
					NetTrafficSimulator.Storer storer = new NetTrafficSimulator.Storer (fd.Filename);
					storer.StoreModel (nm, sm);
				}
			}
			fd.Destroy ();
		} else {
			MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Close, "Model not loaded! Load or create model first.");
			md.Run ();
			md.Destroy ();
		}
	}

	protected void OnAboutActionActivated (object sender, EventArgs e)
	{
		NetTrafficSimulator.AboutDialog ab = new NetTrafficSimulator.AboutDialog ();
		ab.Run ();
		ab.Destroy ();
	}
	
}
