using System;
using log4net;

namespace NetTrafficSimulator
{
	/**
	*/
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ServerNodeResultWidget : Gtk.Bin
	{
		static readonly ILog log = LogManager.GetLogger(typeof(ServerNodeResultWidget));
		/**
		 */
		public ServerNodeResultWidget ()
		{
			this.Build ();
		}

		/**
		 */
		public void LoadParams(ResultModel rm,string name){
			try{
				label6.Text = rm.GetServerNodePacketsProcessed(name)+"";
				label7.Text = rm.GetServerNodePacketsMalreceived (name)+"";
				label8.Text = rm.GetServerNodeTimeWaited (name)+"";
				label9.Text = Math.Round(rm.GetServerNodePercentTimeIdle (name),Storer.decimals)+"";
				label10.Text = Math.Round(rm.GetServerNodeAverageWaitTime (name),Storer.decimals)+"";
			}catch(ArgumentException ae){
				log.Debug (ae.Message);
			}
		}
	}
}

