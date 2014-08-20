using System;
using log4net;

namespace NetTrafficSimulator
{
	public partial class NewLinkDialog : Gtk.Dialog
	{
		static readonly ILog log = LogManager.GetLogger (typeof(NewLinkDialog));
		NetworkModel nm;
		public string link_name,node1,node2;
		public NewLinkDialog (NetworkModel nm)
		{
			this.Build ();
			this.nm = nm;
			this.linkparamwidget2.LoadParams (nm, "");
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			if (nm != null) {
				try {
					nm.SetConnected (linkparamwidget2.GetN1Name (), linkparamwidget2.GetN2Name (), linkparamwidget2.GetName (), linkparamwidget2.GetCapacity (), linkparamwidget2.GetToggleProbability ());     
					this.link_name = linkparamwidget2.GetName ();
					this.node1 = linkparamwidget2.GetN1Name ();
					this.node2 = linkparamwidget2.GetN2Name ();
				} catch (ArgumentException ae) {
					log.Debug ("Caught exception: " + ae.Message);
					this.Respond (Gtk.ResponseType.Reject);
				}
			} else
				throw new ArgumentException ("Network model null");
		}
	}
}

