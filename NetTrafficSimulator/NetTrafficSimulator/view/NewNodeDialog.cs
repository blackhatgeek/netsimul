using System;

namespace NetTrafficSimulator
{
	/**
	*/
	public partial class NewNodeDialog : Gtk.Dialog
	{
		NetTrafficSimulator.NetworkModel nm;
		/**
		 */
		public string node_name;
		int ntype;
		/**
		 */
		public NewNodeDialog (NetworkModel nm,int ntype)
		{
			this.Build ();
			this.nm = nm;
			if ((ntype == NetworkModel.END_NODE) || (ntype == NetworkModel.NETWORK_NODE) || (ntype == NetworkModel.SERVER_NODE)) {
				this.ntype = ntype;
				switch (ntype) {
				case NetworkModel.END_NODE:
					label1.Text = "Add new end node";
					break;
				case NetworkModel.NETWORK_NODE:
					label1.Text = "Add new network node";
					break;
				case NetworkModel.SERVER_NODE:
					label1.Text = "Add new server node";
					break;
				}
			}else
				throw new ArgumentException ("Invalid node type: " + ntype);
		}

		/**
		 */
		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			if (nm != null) {
				if (this.entry1.Text.Contains ("\r") || this.entry1.Text.Contains ("\n") || this.entry1.Text.Contains("\t") || this.entry1.Text.EndsWith (" ") || this.entry1.Text.StartsWith (" ") || this.entry1.Text.Contains ("  "))
					this.Respond (Gtk.ResponseType.No);
				else {
					if (nm.HaveNode (this.entry1.Text)) {
						this.Respond (Gtk.ResponseType.Reject);
					} else {
						nm.AddNode (this.entry1.Text, ntype);
						this.node_name = this.entry1.Text;
					}
				}
			} else
				throw new ArgumentNullException ("Network model null");
		}
	}
}

