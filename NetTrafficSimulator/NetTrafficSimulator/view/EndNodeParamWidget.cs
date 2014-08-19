using System;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class EndNodeParamWidget : Gtk.Bin
	{
		public EndNodeParamWidget ()
		{
			this.Build ();
		}

		public void LoadParams(NetworkModel nm,SimulationModel sm,String nname){
			this.entry1.Text = nname;
			this.spinbutton2.Value = nm.GetEndNodeMaxPacketSize (nname);
			int nnum = nm.GetNodeNum (nname);
			this.spinbutton1.Value = nm.GetNodeAddr (nnum);
			if (sm.IsRandomTalker (nname)) {
				this.radiobutton1.Active = true;
				this.radiobutton3.Active = false;
			} else {
				this.radiobutton3.Active = true;
				this.radiobutton1.Active = false;
			}
		}
	}
}

