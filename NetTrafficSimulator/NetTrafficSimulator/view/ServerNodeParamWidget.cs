using System;

namespace NetTrafficSimulator
{
	/**
	*/
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ServerNodeParamWidget : Gtk.Bin
	{
		MainWindow mw;
		NetworkModel nm;
		/**
		 */
		public string name;

		/**
		 */
		public ServerNodeParamWidget ()
		{
			this.Build ();
		}

		/**
		 */
		public void LoadParams(NetworkModel nm,String nname,MainWindow mw){
			this.mw=mw;
			this.nm = nm;
			this.name = nname;
			this.entry3.Text = nname;
			this.spinbutton3.Value = nm.GetEndpointNodeAddr (nname);
			this.label3.Text = nm.GetEndpointNodeLink (nname);
		}

		/**
		 */
		protected void OnButton473Clicked (object sender, EventArgs e)
		{
			if ((nm != null) && (mw != null)) {
				if (!entry3.Text.Equals (name)) {
					if (this.entry3.Text.Contains ("\r") || this.entry3.Text.Contains ("\n") || this.entry3.Text.Contains ("\t") || this.entry3.Text.EndsWith (" ") || this.entry3.Text.StartsWith (" ") || this.entry3.Text.Contains ("  ")) {
						Gtk.MessageDialog md1 = new Gtk.MessageDialog (mw, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Close, "Node name cannot contain: LF,CR,tab,spaces at the beginning or at the end, multiple spaces next to each other. Name was not changed.");
						md1.Run ();
						md1.Destroy ();
					} else {
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
			}
			nm.SetEndpointNodeAddr (name, spinbutton3.ValueAsInt);
		}
	}
}

