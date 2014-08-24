using System;
using log4net;

namespace NetTrafficSimulator
{
	/**
	*/
	public partial class NewLinkDialog : Gtk.Dialog
	{
		static readonly ILog log = LogManager.GetLogger (typeof(NewLinkDialog));
		NetworkModel nm;
		/**
		 */
		public string link_name,node1,node2;
		/**
		 */
		public NewLinkDialog (NetworkModel nm,MainWindow mw)
		{
			this.Build ();
			this.nm = nm;
			foreach (String node in nm.GetNodeNames()) {
				combobox4.AppendText (node);
				combobox5.AppendText (node);
			}
			combobox4.Active = 1;
			combobox5.Active = 1;
		}

		/**
		 */
		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			if (nm != null) {
				if (this.entry1.Text.Contains ("\r") || this.entry1.Text.Contains ("\n") || this.entry1.Text.Contains ("\t") || this.entry1.Text.EndsWith (" ") || this.entry1.Text.StartsWith (" ") || this.entry1.Text.Contains ("  "))
					this.Respond (Gtk.ResponseType.No);
				else {
					try {
						link_name = entry1.Text;
						node1 = combobox4.ActiveText;
						node2 = combobox5.ActiveText;
						int capacity = spinbutton3.ValueAsInt;
						decimal tp = (decimal)spinbutton4.Value;
						if (nm.CanSetConnected (node1, node2, link_name, capacity, tp)) {
							nm.SetConnected (node1, node2, link_name, capacity, tp);

						} else
							this.Respond (Gtk.ResponseType.Reject);
					} catch (ArgumentException ae) {
						log.Debug ("Caught exception: " + ae.Message);
						this.Respond (Gtk.ResponseType.Reject);
					}
				}
			} else
				throw new ArgumentException ("Network model null");
		}
	}
}

