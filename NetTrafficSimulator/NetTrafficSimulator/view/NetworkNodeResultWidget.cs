using System;
using log4net;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class NetworkNodeResultWidget : Gtk.Bin
	{
		static readonly ILog log = LogManager.GetLogger(typeof(NetworkNodeResultWidget));
		public NetworkNodeResultWidget ()
		{
			this.Build ();
		}

		public void LoadParams(ResultModel rm,string name){
			try{
				label20.Text = rm.GetNetworkNodePacketsProcessed(name)+"";
				label21.Text = rm.GetNetworkNodeTimeWaited (name)+"";
				label22.Text = Math.Round(rm.GetNetworkNodePercentTimeIdle (name),Storer.decimals)+"";
				label23.Text = Math.Round(rm.GetNetworkNodeAverageWaitTime (name),Storer.decimals)+"";
				label24.Text = rm.GetNetworkNodePacketsDropped (name)+"";
				label25.Text = Math.Round(rm.GetNetworkNodePercentagePacketsDropped (name),Storer.decimals)+"";
				label26.Text = rm.GetNetworkNodeRoutingPacketsSent (name)+"";
				label27.Text = rm.GetNetworkNodeRoutingPacketsReceived (name)+"";
				label28.Text = Math.Round(rm.GetNetworkNodePercentageRoutingPackets (name),Storer.decimals)+"";
			}catch(ArgumentException ae){
				log.Debug (ae.Message);
			}
		}
	}
}

