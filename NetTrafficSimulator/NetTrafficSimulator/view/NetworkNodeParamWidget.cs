using System;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * Widget with parameters of a network node
	*/
	[System.ComponentModel.ToolboxItem(true)]
	public partial class NetworkNodeParamWidget : Gtk.Bin
	{
		static readonly ILog log = LogManager.GetLogger(typeof(NetTrafficSimulator.NetworkNodeParamWidget));
		Gtk.ListStore store;
		MainWindow mw;
		NetworkModel nm;
		/**
		 * Network node's name
		 */
		public string name;

		/**
		 * Built the widget,  initialize nodeview for connected links
		 */
		public NetworkNodeParamWidget ()
		{
			this.Build ();

			Gtk.TreeViewColumn lnamecol = new Gtk.TreeViewColumn ();
			lnamecol.Title = "Link name";
			Gtk.TreeViewColumn ltocol = new Gtk.TreeViewColumn ();
			ltocol.Title = "Connected to";

			nodeview1.AppendColumn (lnamecol);
			nodeview1.AppendColumn (ltocol);

			store = new Gtk.ListStore (typeof(string), typeof(string));
			nodeview1.Model = store;

			Gtk.CellRendererText lnamecell = new Gtk.CellRendererText ();
			lnamecol.PackStart (lnamecell, true);
			Gtk.CellRendererText ltocell = new Gtk.CellRendererText ();
			ltocol.PackStart (ltocell, true);

			lnamecol.AddAttribute (lnamecell, "text", 0);
			ltocol.AddAttribute (ltocell, "text", 1);

		}

		/**
		 * Load parameters from models to GUI
		 */
		public void LoadParams(NetworkModel nm,String nname,MainWindow mw){
			this.nm=nm;
			this.mw = mw;
			this.name = nname;
			log.Debug ("Load NN params");
			if (nm == null)
				log.Error ("NM null");
			if (nname == null)
				log.Error ("node name null");
			else
				log.Debug ("node: " + nname);
			this.entry4.Text = nname;

			string[] links = nm.GetNetworkNodeLinks (nname);
			if (links != null) {
				log.Debug ("Processing links");
				int i = 0,def=0;
				foreach (string link in links) {
					log.Debug ("Link: " + link);
					string n1 = nm.GetLinkNode1 (link);
					log.Debug ("N1: " + n1);
					string to = n1.Equals (nname) ? nm.GetLinkNode2 (link) : n1;
					log.Debug ("TO: " + to);
					string default_link = nm.GetNetworkNodeDefaultRoute (nname);
					log.Debug ("Default link:" + default_link);
					store.AppendValues (link, to);

					if (link.Equals (default_link)) {
						def = i;
					}
					combobox2.AppendText (link);
					i++;
					log.Debug ("Appended");
				}
				combobox2.Active = def;
			}
		}

		/**
		 * Change button - carefully save current name and save default route. 
		 */
		protected void OnButton422Clicked (object sender, EventArgs e)
		{
			if ((nm != null) && (mw != null)) {
				if (!entry4.Text.Equals (name)) {
					if (this.entry4.Text.Equals("")||this.entry4.Text.Contains ("\r") || this.entry4.Text.Contains ("\n") || this.entry4.Text.Contains ("\t") || this.entry4.Text.EndsWith (" ") || this.entry4.Text.StartsWith (" ") || this.entry4.Text.Contains ("  ")) {
						Gtk.MessageDialog md1 = new Gtk.MessageDialog (mw, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Close, "Node name cannot contain: LF,CR,tab,spaces at the beginning or at the end, multiple spaces next to each other. Node name cannot be empty. Name was not changed.");
						md1.Run ();
						md1.Destroy ();
					} else {
						try {
							nm.SetNodeName (name, entry4.Text);
							name = entry4.Text;
							mw.NodeNameChanged ();
						} catch (ArgumentException) {
							Gtk.MessageDialog md = new Gtk.MessageDialog (mw, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, "Name change failed");
							md.Run ();
							md.Destroy ();
							entry4.Text = name;
						}
					}
				}

				if(nm.HaveLink(combobox2.ActiveText))
					nm.SetNetworkNodeDefaultRoute (name, combobox2.ActiveText);
			}
		}
	}
}

