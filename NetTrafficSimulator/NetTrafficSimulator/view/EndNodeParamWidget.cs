using System;
using log4net;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class EndNodeParamWidget : Gtk.Bin
	{
		static readonly ILog log = LogManager.GetLogger (typeof(EndNodeParamWidget));
		NetworkModel nm;
		SimulationModel sm;
		public string name;
		MainWindow mw;

		public EndNodeParamWidget ()
		{
			this.Build ();
		}

		public void LoadParams(NetworkModel nm,SimulationModel sm,String nname, MainWindow mw){
			this.nm = nm;
			this.name = nname;
			this.sm = sm;
			this.mw = mw;
			this.label1.Text = nm.GetEndpointNodeLink (nname);
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

		protected void OnButton277Clicked (object sender, EventArgs e)
		{
			if ((sm != null) && (nm != null) && (mw != null)) {
				if (!entry1.Text.Equals (name)) {
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
	