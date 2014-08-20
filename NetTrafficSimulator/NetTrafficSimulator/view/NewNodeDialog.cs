using System;
using log4net;

namespace NetTrafficSimulator
{
	public partial class NewNodeDialog : Gtk.Dialog
	{
		static readonly ILog log = LogManager.GetLogger(typeof(NewNodeDialog));
		NetTrafficSimulator.NetworkModel nm;
		public string node_name;
		int ntype;
		public NewNodeDialog (NetworkModel nm,int ntype)
		{
			this.Build ();
			this.nm = nm;
			if ((ntype == NetworkModel.END_NODE) || (ntype == NetworkModel.NETWORK_NODE) || (ntype == NetworkModel.SERVER_NODE))
				this.ntype = ntype;
			else
				throw new ArgumentException ("Invalid node type: " + ntype);
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			if (nm != null) {
				if (nm.HaveNode (this.entry1.Text)) {
					this.Respond (Gtk.ResponseType.Reject);
				} else {
					nm.AddNode (this.entry1.Text, ntype);
					this.node_name = this.entry1.Text;
				}
			} else
				throw new ArgumentNullException ("Network model null");
		}
	}
}

