using System;

namespace NetTrafficSimulator
{
	public partial class NewEventDialog : Gtk.Dialog
	{
		SimulationModel sm;
		public SimulationModel.Event generated_event;
		public NewEventDialog (string node,NetworkModel nm,SimulationModel sm)
		{
			this.Build ();

			this.sm = sm;

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

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			sm.SetEvent (label9.Text, combobox3.ActiveText, spinbutton1.ValueAsInt, (decimal)spinbutton2.Value);
			generated_event = new SimulationModel.Event(label9.Text,combobox3.ActiveText,spinbutton1.ValueAsInt,(decimal)spinbutton2.Value);
		}
	}
}

