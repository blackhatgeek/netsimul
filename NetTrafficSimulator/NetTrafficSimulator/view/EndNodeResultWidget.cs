using System;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * EndNodeResultWidget shows results for an endnode
	*/
	[System.ComponentModel.ToolboxItem(true)]
	public partial class EndNodeResultWidget : Gtk.Bin
	{
		static readonly ILog log = LogManager.GetLogger(typeof(EndNodeResultWidget));
		/**
		 * Build the widget
		 */
		public EndNodeResultWidget ()
		{
			this.Build ();
		}

		/**
		 * Load results from ResultModel
		 */
		public void LoadParams(ResultModel rm,string nodename){
			try{
				label23.Text = rm.GetEndNodePacketsSent (nodename)+"";
				label24.Text = rm.GetEndNodePacketsReceived (nodename)+"";
				label25.Text = rm.GetEndNodePacketsMalreceived (nodename)+"";
				label26.Text = Math.Round(rm.GetEndNodePercentTimeIdle (nodename),Storer.decimals)+"";
				label27.Text = Math.Round(rm.GetEndNodeAverageWaitTime (nodename),Storer.decimals)+"";
				label28.Text = Math.Round(rm.GetEndNodeAveragePacketSize (nodename),Storer.decimals)+"";
			}catch(ArgumentException ae){
				log.Debug (ae.Message);
			}
		}

		/**
		 * Set all labels as "N/A"
		 */
		public void InitLabels(){
			label23.Text = "N/A";
			label24.Text = "N/A";
			label25.Text = "N/A";
			label26.Text = "N/A";
			label27.Text = "N/A";
			label28.Text = "N/A";
		}
	}
}

