using System;
using System.IO;
using System.Xml.Schema;
using Gtk;
using log4net;

/**
 * Main window of the application
 */
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
	int addrcounter;

	const string END = "END", SERVER = "SERVER", NETWORK = "NETWORK";

	/**
	 * Build the view, set up nodes tree view and links tree view
	*/
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

	/**
	 * File - load model menu
	 * Show FileChooserDialog, try to set up loader and load models, based on models load nodes box and links box
	 * on exception show message dialog
	 */
	protected void loadFromFileHandler (object sender, EventArgs ev)
	{
			FileChooserDialog fc = new Gtk.FileChooserDialog ("Model to load", this, FileChooserAction.Open, 
			                                                      "Cancel", ResponseType.Cancel, "Load", ResponseType.Accept);
			if (model_path != null)
				fc.SetCurrentFolder(model_path);
			if (fc.Run () == (int)ResponseType.Accept) {
				System.IO.FileInfo fi = new FileInfo (fc.Filename);
				model_path = fi.DirectoryName;
				log.Debug ("Model path:" + model_path);
				try{
					NetTrafficSimulator.Loader loader = new NetTrafficSimulator.Loader (fc.Filename);
					nm = loader.LoadNM ();
					sm = loader.LoadSM ();
					rm = null;

					node_names=nm.GetNodeNames();
					link_names=nm.GetLinkNames();

					addrcounter = nm.MaxAddr;

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
				}catch(Exception e){
					log.Fatal (e.Message);
					log.Debug (e.StackTrace);
					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error,
					                                      ButtonsType.Close, e.Message);
					md.Run ();
					md.Destroy ();
				}
			}

			fc.Destroy ();
			frame12.Visible = false;
			button18.Visible = false;
	}

	/**
	 * Simulation - parameters menu item
	 * Show SimulationParametersDialog - if OK button was clicked, set parameters in SimulationModel
	*/
	protected void OnParametersMenuClick (object sender, EventArgs ev){
		if (sm != null) {
			NetTrafficSimulator.SimulationParametersDialog spd = new NetTrafficSimulator.SimulationParametersDialog (sm);
			if (spd.Run () == (int)ResponseType.Ok) {
				sm.MaxHop = spd.maxHop;
				sm.Time = spd.time;
				sm.TraceRandom = spd.random;
			}
			spd.Destroy ();
		} else {
			MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Close, "Load or create model first!");
			md.Run ();
			md.Destroy ();
		}
	}

	/**
	 * Simulation - Run menu item
	 * Set up SimulationController, Run simulation, Get result model and load traces to PacketTraceWidget
	*/
	protected void OnRunMenuClick (object sender, EventArgs ev){
		if ((nm != null) && (sm != null)) {
			this.packettracewidget1.Clear ();
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

	/**
	 * File - Exit simulator
	*/
	protected void exitHandler (object sender, EventArgs ev){
		Application.Quit ();
	}

	/**
	 * File - save results as
	 * Show filechooser dialog, if file exists ask and delete file, set up storer and store results model
	*/
	protected void saveResultsHandler(object sender,EventArgs ev){
		if (rm != null) {
			FileChooserDialog fd = new Gtk.FileChooserDialog ("Save results as ", this, FileChooserAction.Save, "Cancel", ResponseType.Close, "Save", ResponseType.Accept);
			if (result_path != null)
				fd.SetCurrentFolder (result_path);
			else if (model_path != null)
				fd.SetCurrentFolder (model_path);

			if (fd.Run () == (int)ResponseType.Accept) {
				System.IO.FileInfo fi = new FileInfo (fd.Filename);
				result_path = fi.DirectoryName;
				log.Debug ("Result path: " + result_path);
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

	/**
	 * Load node names to nodes box
	*/
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

	/**
	 * Load link names into link box
	*/
	private void loadLinksBox(){
		linkListStore.Clear ();
		foreach (string link in link_names) {
			linkListStore.AppendValues (link, nm.GetLinkNode1 (link), nm.GetLinkNode2 (link));
			log.Debug ("Appending: " + link + " node: " + nm.GetLinkNode1(link) + " node:" + nm.GetLinkNode2(link));
		}
	}

	/**
	 * Changed cursor in nodes box - load and show appropriade widget
	*/
	private void nodeTreeCursorChanged(object sender,EventArgs e){
		log.Debug ("nodeTreeCursorChanged");
		TreeSelection selection = (sender as TreeView).Selection;
		TreeModel model;
		TreeIter iter;
		if (selection.GetSelected (out model, out iter)) {
			//node name ... model.GetValue(iter,0);
			//node type ... model.GetValue(iter,1);
			if(GtkAlignment2.Child!=null)
				GtkAlignment2.Child.Destroy();
			GtkLabel13.Text = "<b>"+model.GetValue (iter, 0).ToString()+"</b>";
			GtkLabel13.UseMarkup = true;
			object o = model.GetValue (iter, 1);
			if(o!=null){
				string s = o.ToString();
			if (s != null) {
				switch (s) {
					case SERVER:
						NetTrafficSimulator.ServerNodeWidget sw = new NetTrafficSimulator.ServerNodeWidget ();
						if (sw.ParamWidget == null) {
							log.Error ("Param widget null");
							break;
						}
						sw.ParamWidget.LoadParams (nm, model.GetValue (iter, 0).ToString (), this);
						if (rm != null)
							sw.ResultWidget.LoadParams (rm, model.GetValue (iter, 0).ToString ());
						else
							sw.ResultWidget.InitLabels ();
					GtkAlignment2.Child = sw;
					break;
					case END:
						NetTrafficSimulator.EndNodeWidget ew = new NetTrafficSimulator.EndNodeWidget ();
						if (ew.ParamWidget == null) {
							log.Error ("Param widget null");
							break;
						}
						ew.ParamWidget.LoadParams (nm, sm, model.GetValue (iter, 0).ToString (), this);
						ew.EventWidget.LoadParams (nm, sm, model.GetValue (iter, 0).ToString (), this);
						if (rm != null)
							ew.ResultWidget.LoadParams (rm, model.GetValue (iter, 0).ToString ());
						else
							ew.ResultWidget.InitLabels ();
					GtkAlignment2.Child = ew;
					break;
					case NETWORK:
						NetTrafficSimulator.NetworkNodeWidget nw = new NetTrafficSimulator.NetworkNodeWidget ();
						if (nw.ParamWidget == null) {
							log.Error ("Param widget null");
							break;
						}
						nw.ParamWidget.LoadParams (nm, model.GetValue (iter, 0).ToString (), this);
						if (rm != null)
							nw.ResultWidget.LoadParams (rm, model.GetValue (iter, 0).ToString ());
						else
							nw.ResultWidget.InitLabels ();
					GtkAlignment2.Child = nw;
					break;
				default:
					break;
				}

				frame12.Visible = true;
				GtkAlignment2.Child.Visible = true;
				button18.Visible = true;
			}
			}
		}
	}

	/**
	 * Changed cursor in links box - load and show widget
	*/
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
			else
				lw.ResultWidget.InitLabels ();
			frame12.Visible = true;
			button18.Visible = true;
			GtkAlignment2.Child = lw;
			GtkAlignment2.Child.Visible = true;
		}

	}

	/**
	 * File - New model
	 * Create new NM, SM clear node and link boxes
	*/
	protected void newModelHandler (object sender, EventArgs e)
	{
		nm = new NetTrafficSimulator.NetworkModel ();
		sm = new NetTrafficSimulator.SimulationModel ();
		rm = null;
		linkListStore.Clear ();
		nodeListStore.Clear ();
		packettracewidget1.Clear ();
		frame12.Visible = false;
		button18.Visible = false;
		addrcounter = 0;
	}

	/**
	 * Add new end node
	*/
	protected void onAddNewEndNode (object sender, EventArgs e)
	{
		if (addrcounter == int.MaxValue) {
			log.Warn ("Address overflow");
			addrcounter = -1;
		}
		addrcounter++;
		addNode (NetTrafficSimulator.NetworkModel.END_NODE);
	}

	/**
	 * Add new network node
	*/
	protected void onAddNetNode (object sender, EventArgs e)
	{
		addNode (NetTrafficSimulator.NetworkModel.NETWORK_NODE);
	}

	/**
	 * Add new server node
	*/
	protected void onAddServNode (object sender, EventArgs e)
	{
		if (addrcounter == int.MaxValue) {
			log.Warn ("Address overflow");
			addrcounter = -1;
		}
		addrcounter++;
		addNode (NetTrafficSimulator.NetworkModel.SERVER_NODE);
	}


	/**
	 * Set up and show NewNodeDialog, if all goes well update nodes box
	*/
	private void addNode(int ntype){
		if ((ntype == NetTrafficSimulator.NetworkModel.END_NODE) || (ntype == NetTrafficSimulator.NetworkModel.NETWORK_NODE) || (ntype == NetTrafficSimulator.NetworkModel.SERVER_NODE)) {
			if (nm != null) {
				NetTrafficSimulator.NewNodeDialog nend = new NetTrafficSimulator.NewNodeDialog (nm, ntype, addrcounter);
				switch (nend.Run ()) {
				case (int)ResponseType.Reject:
					nend.Destroy ();
					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close, "Duplicate node name! Node names must remain unique in the model. Node was not added.");
					md.Run ();
					md.Destroy ();
					addrcounter--;
					break;
				case (int)ResponseType.No:
					nend.Destroy ();
					MessageDialog md1 = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close, "Node name cannot contain: LF,CR,tab,spaces at the beginning or at the end, multiple spaces next to each other. Node name cannot be empty. Node was not added.");
					md1.Run ();
					md1.Destroy ();
					addrcounter--;
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
					addrcounter--;
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

	/**
	 * Trigger for node name change from a widget - get new node names and load nodes box
	*/
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

	/**
	 * Tigger for link name change
	 * load link names and load links box
	*/
	public void LinkChanged(){
		link_names = nm.GetLinkNames ();
		loadLinksBox ();
		if (GtkAlignment2.Child is NetTrafficSimulator.LinkWidget) {
			GtkLabel13.Text = "<b>" + (GtkAlignment2.Child as NetTrafficSimulator.LinkWidget).ParamWidget.GetName () + "</b>";
			GtkLabel13.UseMarkup = true;
		}
	}

	/**
	 * Model - Add link
	 * Show NewLinkDialog and if OK then update links box
	*/
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
				MessageDialog md1 = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close, "Link name cannot contain: LF,CR,tab,spaces at the beginning or at the end, multiple spaces next to each other. Link name cannot be empty. Link was not added.");
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

	/**
	 * Delete button clicked - remove node or link
	*/
	protected void OnDeleteButtonClicked (object sender, EventArgs e)
	{
		log.Debug ("Delete");
		if (nm != null) {
			try{
				if (GtkAlignment2.Child is NetTrafficSimulator.LinkWidget) {
					log.Debug("Delete link");
					string[] rel = nm.GetRelatedNodes(GtkLabel13.Text);
					string msg = "Remove link "+GtkLabel13.Text+"?\n\nAffected nodes:\n";
					if(rel!=null){
						foreach (string n in rel) {
							msg += "\t" + n + "\n";
						}
					}
					MessageDialog md  = new MessageDialog(this,DialogFlags.DestroyWithParent,MessageType.Question,ButtonsType.YesNo,msg);
					if (md.Run () == (int)ResponseType.Yes) {
						nm.RemoveLink (GtkLabel13.Text);
						link_names=nm.GetLinkNames();
						loadLinksBox ();
					}
					md.Destroy ();
				} else {
					log.Debug("Delete node");
					string[] rel = nm.GetRelatedLinks (GtkLabel13.Text);
					int t = nm.GetNodeType (GtkLabel13.Text);

					string msg = "Remove node: " + GtkLabel13.Text + "?\n";

					System.Collections.Generic.LinkedList<NetTrafficSimulator.SimulationModel.Event> events_to_remove = new System.Collections.Generic.LinkedList<NetTrafficSimulator.SimulationModel.Event> ();
					if ((t == NetTrafficSimulator.NetworkModel.END_NODE) || (t == NetTrafficSimulator.NetworkModel.SERVER_NODE)) {
						System.Collections.Generic.LinkedList<NetTrafficSimulator.SimulationModel.Event> events = sm.GetEvents ();
						if(events!=null){
							log.Debug("Related events");
							if(events.Count>0){
								msg+="\nRelated events to be removed:\n";
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
					}
					log.Debug("Related links");
					if(rel!=null){
						if (rel.Length != 0) {
							msg += "\nRelated links to be removed:\n";
							foreach (string l in rel) {
								msg += "\t" + l + "\n";
							}
						}
					}

					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.YesNo, msg);
					if (md.Run () == (int)ResponseType.Yes) {
						log.Debug("Dialog response Yes");
						if(rel!=null){
							log.Debug("Removing links");
							foreach (string l in rel) {
								nm.RemoveLink (l);
								try{
									rm.RemoveLinkResult(l);
								}catch(ArgumentException ex){
									log.Debug(ex.Message);
								}
							}
						}
						log.Debug("Removing events");
						foreach (NetTrafficSimulator.SimulationModel.Event ev in events_to_remove) {
							sm.GetEvents ().Remove (ev);
						}
						if(rm!=null){
							log.Debug("Removing results");
							try{
								switch(nm.GetNodeType(GtkLabel13.Text)){
								case NetTrafficSimulator.NetworkModel.END_NODE: rm.RemoveEndNodeResult(GtkLabel13.Text); break;
								case NetTrafficSimulator.NetworkModel.SERVER_NODE: rm.RemoveServerNodeResult(GtkLabel13.Text); break;
								case NetTrafficSimulator.NetworkModel.NETWORK_NODE: rm.RemoveNetworkNodeResult(GtkLabel13.Text);break;
								}
							}catch(ArgumentException ex){
								log.Debug(ex.Message);
							}
						}

						nm.RemoveNode (GtkLabel13.Text);
						node_names=nm.GetNodeNames();
						link_names=nm.GetLinkNames();
						loadLinksBox ();
						loadNodesBox ();
					}
					md.Destroy ();
				}
				this.frame12.Visible=false;
				this.button18.Visible=false;
			}catch(ArgumentException ae){
				log.Debug (ae.Message);
			}
		}
	}

	/**
	 * File - save model as
	 * Show dialog - create storer - store model
	*/
	protected void OnSaveModelAsActionActivated (object sender, EventArgs e)
	{
		if ((nm!=null)&&(sm != null)) {
			FileChooserDialog fd = new Gtk.FileChooserDialog ("Save model as ", this, FileChooserAction.Save, "Cancel", ResponseType.Close, "Save", ResponseType.Accept);
			if (model_path != null)
				fd.SetCurrentFolder (model_path);

			if (fd.Run () == (int)ResponseType.Accept) {
				System.IO.FileInfo fi = new FileInfo (fd.Filename);
				model_path = fi.DirectoryName;
				log.Debug ("Model path: "+model_path);
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

	/**
	 * Help - About
	 * Show about dialog
	*/
	protected void OnAboutActionActivated (object sender, EventArgs e)
	{
		NetTrafficSimulator.AboutDialog ab = new NetTrafficSimulator.AboutDialog ();
		ab.Run ();
		ab.Destroy ();
	}
	
}
