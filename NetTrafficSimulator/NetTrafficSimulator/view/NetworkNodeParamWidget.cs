using System;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class NetworkNodeParamWidget : Gtk.Bin
	{
		public NetworkNodeParamWidget ()
		{
			this.Build ();
		}

		public void LoadParams(NetworkModel nm,String nname){
			this.entry4.Text = nname;
		}
	}
}

