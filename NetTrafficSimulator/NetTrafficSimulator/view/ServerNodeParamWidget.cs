using System;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ServerNodeParamWidget : Gtk.Bin
	{
		public ServerNodeParamWidget ()
		{
			this.Build ();
		}

		public void LoadParams(NetworkModel nm,String nname){
			this.entry3.Text = nname;
			this.spinbutton3.Value = nm.GetNodeAddr (nm.GetNodeNum (nname));
			//for (int i=0; i<nm.L; i++) {
				//this.combobox2.AppendText(nm.Lin`
			//}
		}
	}
}

