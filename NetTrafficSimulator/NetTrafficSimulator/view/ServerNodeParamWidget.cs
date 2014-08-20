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
			this.spinbutton3.Value = nm.GetEndpointNodeAddr (nname);
			int active = 0, i = 0;
			foreach (string link in nm.GetLinkNames()) {
				combobox2.AppendText (link);
				if (link.Equals (nm.GetEndpointNodeLink (nname)))
					active = i;
				i++;
			}
			combobox2.Active = active;
		}
	}
}

