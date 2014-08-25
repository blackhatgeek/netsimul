using System;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * Dialog to set up a new event
	*/
	public partial class NewEventDialog : Gtk.Dialog
	{
		static readonly ILog log = LogManager.GetLogger(typeof(NewEventDialog));
		SimulationModel sm;
		NetworkModel nm;
		MainWindow mw;
		/**
		 * The generated event
		 */
		public SimulationModel.Event generated_event;
		/**
		 * Build the dialog - populate combobox with servers, show node name, set max time as simulation time to run
		 */
		public NewEventDialog (string node,NetworkModel nm,SimulationModel sm,MainWindow mw)
		{
			this.Build ();

			this.sm = sm;
			this.nm = nm;
			this.mw = mw;

			this.spinbutton1.Adjustment.Upper = sm.Time;
			this.label9.Text = node;

			string[] nodes = nm.GetNodeNames ();
			foreach(string n in nodes){
				if (nm.GetNodeType(n).Equals(NetworkModel.SERVER_NODE)) {
					combobox3.AppendText (n);
				}
			}
			combobox3.Active = 0;
		}

		/**
		 * Button OK - Set event in SimulationModel if possible
		 */
		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			log.Debug ("OK clicked");
			if (mw == null)
				log.Error ("Main window null");
			if (nm.HaveNode (combobox3.ActiveText)) {
				log.Debug ("Have node");
				try{
					sm.SetEvent (label9.Text, combobox3.ActiveText, spinbutton1.ValueAsInt, (decimal)spinbutton2.Value);
					generated_event = new SimulationModel.Event (label9.Text, combobox3.ActiveText, spinbutton1.ValueAsInt, (decimal)spinbutton2.Value);
					log.Debug ("Generated ev:" + generated_event.node1 + "," + generated_event.node2 + "," + generated_event.size + "," + generated_event.when);
				}catch(ArgumentException){
					this.Respond (Gtk.ResponseType.Reject);
				}
			} else {
				this.Respond (Gtk.ResponseType.Reject);
			}
		}
	}
}

