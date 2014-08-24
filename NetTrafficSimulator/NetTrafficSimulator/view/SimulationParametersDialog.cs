using System;

namespace NetTrafficSimulator
{
	/**
	*/
	public partial class SimulationParametersDialog : Gtk.Dialog
	{
		/**
		 */
		public int maxHop,time;
		/**
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
		 */
		protected void btnOKClicked (object sender, EventArgs e)
		{
			maxHop = spinbutton1.ValueAsInt;
			time = spinbutton2.ValueAsInt;
		}
	}
}

