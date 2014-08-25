using System;

namespace NetTrafficSimulator
{
	/**
	 * Dialog to show parameters of a simulation
	*/
	public partial class SimulationParametersDialog : Gtk.Dialog
	{
		/**
		 * Max hop counter value
		 */
		public int maxHop;
		/**
		 * Time to run the simulation
		 */
		public int time;
		/**
		 * Build the dialog, load parameters from model
		 */
		public SimulationParametersDialog (SimulationModel sm)
		{
			this.maxHop = sm.MaxHop;
			this.time = sm.Time;
			this.Build ();
			this.spinbutton1.Value = sm.MaxHop;
			this.spinbutton2.Value = sm.Time;
		}

		/**
		 * Set public values with new modified ones
		 */
		protected void btnOKClicked (object sender, EventArgs e)
		{
			maxHop = spinbutton1.ValueAsInt;
			time = spinbutton2.ValueAsInt;
		}
	}
}

