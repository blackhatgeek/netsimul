using System;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ServerNodeParamWidget : Gtk.Bin
	{
		MainWindow mw;
		NetworkModel nm;
		string name;

		public ServerNodeParamWidget ()
		{
			this.Build ();
		}

		public void LoadParams(NetworkModel nm,String nname,MainWindow mw){
			this.mw=mw;
			this.nm = nm;
			this.name = nname;
			this.entry3.Text = nname;
			this.spinbutton3.Value = nm.GetEndpointNodeAddr (nname);
			this.label3.Text = nm.GetEndpointNodeLink (nname);
		}

		protected void OnButton473Clicked (object sender, EventArgs e)
		{
			if ((nm != null) && (mw != null)) {
				if (!entry3.Text.Equals (name)) {
					try {
						nm.SetNodeName (name, entry3.Text);
						name = entry3.Text;
						mw.NodeNameChanged ();
					} catch (ArgumentException) {
						Gtk.MessageDialog md = new Gtk.MessageDialog (mw, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, "Name change failed");
						md.Run ();
						md.Destroy ();
						entry3.Text = name;
					}
				}
			}
			nm.SetEndpointNodeAddr (name, spinbutton3.ValueAsInt);
		}
	}
}

