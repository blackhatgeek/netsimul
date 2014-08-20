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
			for(int i=0;i<nm.NodeCount;i++){
			//	this.combobox4.AppendText (nm.GetNodeName (i));
			//	this.combobox5.AppendText (nm.GetNodeName (i));
			}
			//this.combobox4.Active=
			this.entry5.Text = lname;
			//this.spinbutton4.ValueAsInt=nm.LinkCapacity(
		}
	}
}

