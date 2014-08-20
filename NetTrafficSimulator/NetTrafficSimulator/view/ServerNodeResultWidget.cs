using System;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ServerNodeResultWidget : Gtk.Bin
	{
		public ServerNodeResultWidget ()
		{
			this.Build ();
		}

		public void LoadParams(ResultModel rm,string name){
			label6.Text = rm.GetServerNodePacketsProcessed(name)+"";
			label7.Text = rm.GetServerNodePacketsMalreceived (name)+"";
			label8.Text = rm.GetServerNodeTimeWaited (name)+"";
			label9.Text = Math.Round(rm.GetServerNodePercentTimeIdle (name),Storer.decimals)+"";
			label10.Text = Math.Round(rm.GetServerNodeAverageWaitTime (name),Storer.decimals)+"";
		}
	}
}

