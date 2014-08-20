using System;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class LinkResultWidget : Gtk.Bin
	{
		public LinkResultWidget ()
		{
			this.Build ();
		}

		public void LoadParams(ResultModel rm,string name){
			label35.Text = rm.GetLinkPacketsCarried (name)+"";
			label36.Text = rm.GetLinkActiveTime (name)+"";
			label37.Text = rm.GetLinkPassiveTime (name)+"";
			label38.Text = Math.Round(rm.GetLinkIdleTimePercentage (name),Storer.decimals)+"";
			label39.Text = Math.Round(rm.GetLinkDataCarried (name),Storer.decimals)+"";
			label40.Text = Math.Round(rm.GetLinkDataPerTic (name),Storer.decimals)+"";
			label7.Text = Math.Round(rm.GetLinkUsage (name),Storer.decimals)+"";
			label8.Text = rm.GetLinkDataSent (name)+"";
			label9.Text = rm.GetLinkDataLost (name)+"";
			label10.Text = Math.Round(rm.GetLinkPercentageDataLost (name),Storer.decimals)+"";
			label11.Text = Math.Round(rm.GetLinkPercentageDataDelivered (name),Storer.decimals)+"";
			label12.Text = Math.Round(rm.GetLinkPercentageDataLostInCarry (name),Storer.decimals)+"";
		}
	}
}

