using System;

namespace NetTrafficSimulator
{
	/**
	 * Dialog to create a new node - only set the name, other parameters in widget
	 */
	public partial class NewNodeDialog : Gtk.Dialog
	{
		NetTrafficSimulator.NetworkModel nm;
		/**
		 * Node name
		 */
		public string node_name;
		int ntype;
		/**
		 * Build the dialog - set label based on node type
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
		 * OK clicked - add node to NetworkModel
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

