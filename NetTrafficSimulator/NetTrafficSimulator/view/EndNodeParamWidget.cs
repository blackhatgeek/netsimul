using System;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * Widget with parameters of an EndNode
	 */
	[System.ComponentModel.ToolboxItem(true)]
	public partial class EndNodeParamWidget : Gtk.Bin
	{
		static readonly ILog log = LogManager.GetLogger (typeof(EndNodeParamWidget));
		NetworkModel nm;
		SimulationModel sm;
		/**
		 * EndNode name
		 */
		public string name;
		MainWindow mw;

		/**
		 * Build EndNodeWidget
		 */
		public EndNodeParamWidget ()
		{
			this.Build ();
		}

		/**
		 * Load parameters from models
		 */
		public void LoadParams(NetworkModel nm,SimulationModel sm,String nname, MainWindow mw){
			this.nm = nm;
			this.name = nname;
			this.sm = sm;
			this.mw = mw;
			this.label6.Text = nm.GetEndpointNodeLink (nname);
			this.entry1.Text = nname;
			this.spinbutton2.Value = nm.GetEndNodeMaxPacketSize (nname);
			this.spinbutton1.Value = nm.GetEndpointNodeAddr (nname);
			if (sm.IsRandomTalker (nname)) {
				this.radiobutton1.Active = true;
				this.radiobutton3.Active = false;
			} else {
				this.radiobutton3.Active = true;
				this.radiobutton1.Active = false;
			}
		}

		/**
		 * Change button
		 * Set parameters to the current values in widget fields
		 * Special care is exercised for name change as it serves as ID
		 */
		protected void OnButton277Clicked (object sender, EventArgs e)
		{
			if ((sm != null) && (nm != null) && (mw != null)) {
				if (!entry1.Text.Equals (name)) {
					if (this.entry1.Equals("")||this.entry1.Text.Contains ("\r") || this.entry1.Text.Contains ("\n") || this.entry1.Text.Contains ("\t") || this.entry1.Text.EndsWith (" ") || this.entry1.Text.StartsWith (" ") || this.entry1.Text.Contains ("  ")) {
						Gtk.MessageDialog md1 = new Gtk.MessageDialog (mw, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Close, "Node name cannot contain: LF,CR,tab,spaces at the beginning or at the end, multiple spaces next to each other. Node name cannot be empty. Name was not changed.");
						md1.Run ();
						md1.Destroy ();
						entry1.Text = name;
					} else {
						try {
							nm.SetNodeName (name, entry1.Text);
							log.Debug ("Name changed");
							name = entry1.Text;
							mw.NodeNameChanged ();
						} catch (ArgumentException) {
							Gtk.MessageDialog md = new Gtk.MessageDialog (mw, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, "Name change failed");
							md.Run ();
							md.Destroy ();
							entry1.Text = name;
						}
					}
				}

				nm.SetEndpointNodeAddr (name, spinbutton1.ValueAsInt);
				if (this.radiobutton1.Active) {
					sm.SetRandomTalker (name);
				} else {
					sm.UnsetRandomTalker (name);
				}
				nm.SetEndNodeMaxPacketSize (name, this.spinbutton2.ValueAsInt);
			}
		}
	}
}
	