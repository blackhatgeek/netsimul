using System;
using log4net;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class NetworkNodeParamWidget : Gtk.Bin
	{
		static readonly ILog log = LogManager.GetLogger(typeof(NetTrafficSimulator.NetworkNodeParamWidget));
		Gtk.ListStore store;
		public NetworkNodeParamWidget ()
		{
			this.Build ();

			Gtk.TreeViewColumn lnamecol = new Gtk.TreeViewColumn ();
			lnamecol.Title = "Link name";
			Gtk.TreeViewColumn ltocol = new Gtk.TreeViewColumn ();
			ltocol.Title = "Connected to";
			Gtk.TreeViewColumn defcol = new Gtk.TreeViewColumn ();
			defcol.Title = "Default";

			nodeview1.AppendColumn (lnamecol);
			nodeview1.AppendColumn (ltocol);
			nodeview1.AppendColumn (defcol);

			store = new Gtk.ListStore (typeof(string), typeof(string), typeof(bool));
			nodeview1.Model = store;

			Gtk.CellRendererText lnamecell = new Gtk.CellRendererText ();
			lnamecol.PackStart (lnamecell, true);
			Gtk.CellRendererText ltocell = new Gtk.CellRendererText ();
			ltocol.PackStart (ltocell, true);
			Gtk.CellRendererText defcell = new Gtk.CellRendererText ();
			defcol.PackStart (defcell, true);

			lnamecol.AddAttribute (lnamecell, "text", 0);
			ltocol.AddAttribute (ltocell, "text", 1);
			defcol.AddAttribute (defcell, "text", 2);

		}

		public void LoadParams(NetworkModel nm,String nname){
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
				foreach (string link in links) {
					log.Debug ("Link: " + link);
					string n1 = nm.GetLinkNode1 (link);
					log.Debug ("N1: " + n1);
					string to = n1.Equals (nname) ? nm.GetLinkNode2 (link) : n1;
					log.Debug ("TO: " + to);
					string default_link = nm.GetNetworkNodeDefaultRoute (nname);
					log.Debug ("Default link:" + default_link);
					bool is_default = link.Equals (default_link);
					store.AppendValues (link, to, is_default);
					log.Debug ("Appended");
				}
			}
			foreach (string link in nm.GetLinkNames()) {
				if(!(nm.GetLinkNode1(link).Equals(nname)||nm.GetLinkNode2(link).Equals(nname)))
					combobox3.AppendText (link);
			}
			combobox3.Active = 0;
		}
	}
}

