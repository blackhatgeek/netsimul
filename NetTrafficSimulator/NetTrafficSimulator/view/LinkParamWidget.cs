using System;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class LinkParamWidget : Gtk.Bin
	{
		public LinkParamWidget ()
		{
			this.Build ();
		}

		public void LoadParams(NetworkModel nm,String lname){
			int active1=0,active2=0,i=0;
			foreach (String node in nm.GetNodeNames()) {
				combobox4.AppendText (node);
				combobox5.AppendText (node);
				if (node.Equals (nm.GetLinkNode1 (lname)))
					active1 = i;
				if (node.Equals (nm.GetLinkNode2 (lname)))
					active2 = i;
				i++;
			}
			this.combobox4.Active = active1;
			this.combobox5.Active = active2;
			this.entry5.Text = lname;
			this.spinbutton4.Value = nm.LinkCapacity (lname);
			this.spinbutton5.Value = (double)nm.GetLinkToggleProbability (lname);
		}
	}
}

