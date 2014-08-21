using System;
using log4net;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class NetworkNodeParamWidget : Gtk.Bin
	{
		static readonly ILog log = LogManager.GetLogger(typeof(NetTrafficSimulator.NetworkNodeParamWidget));
		Gtk.ListStore store;
		MainWindow mw;
		NetworkModel nm;
		public string name;
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

		protected void OnButton422Clicked (object sender, EventArgs e)
		{
			if ((nm != null) && (mw != null)) {
				if (!entry4.Text.Equals (name)) {
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

				nm.SetNetworkNodeDefaultRoute (name, combobox2.ActiveText);
			}
		}
	}
}

