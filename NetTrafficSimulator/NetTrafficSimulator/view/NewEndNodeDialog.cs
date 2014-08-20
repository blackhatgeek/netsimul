using System;

namespace NetTrafficSimulator
{
	public partial class NewEndNodeDialog : Gtk.Dialog
	{
		NetTrafficSimulator.NetworkModel nm;
		public string node_name;
		public NewEndNodeDialog (NetworkModel nm)
		{
			this.Build ();
			this.nm = nm;
		}

		protected void btnOKClicked (object sender, EventArgs e)
		{
			if (nm != null) {
				if(nm.HaveNode(this.entry1.Text)){
					this.Respond (Gtk.ResponseType.Reject);
				}
				else{
					nm.AddNode(this.entry1.Text,NetworkModel.END_NODE);
					this.node_name = this.entry1.Text;
				}
			}
		}
	}
}

