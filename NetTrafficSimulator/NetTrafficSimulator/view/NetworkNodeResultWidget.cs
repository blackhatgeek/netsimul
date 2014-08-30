using System;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * Widget with results of a NetworkNode
	*/
	[System.ComponentModel.ToolboxItem(true)]
	public partial class NetworkNodeResultWidget : Gtk.Bin
	{
		static readonly ILog log = LogManager.GetLogger(typeof(NetworkNodeResultWidget));
		Gtk.ListStore store;

		/**
		 * Build the widget
		 */
		public NetworkNodeResultWidget ()
		{
			this.Build ();

			Gtk.TreeViewColumn lname = new Gtk.TreeViewColumn ();
			lname.Title = "Link name";
			Gtk.TreeViewColumn psent = new Gtk.TreeViewColumn ();
			psent.Title = "Packets sent";
			Gtk.TreeViewColumn ppsent = new Gtk.TreeViewColumn ();
			ppsent.Title = "Percent packets sent";
			Gtk.TreeViewColumn rpsent = new Gtk.TreeViewColumn ();
			rpsent.Title = "Link rank (packets sent)";
			Gtk.TreeViewColumn dsent = new Gtk.TreeViewColumn ();
			dsent.Title = "Data sent";
			Gtk.TreeViewColumn pdsent = new Gtk.TreeViewColumn ();
			pdsent.Title = "Percent data sent";
			Gtk.TreeViewColumn rdsent = new Gtk.TreeViewColumn ();
			rdsent.Title = "Link rank (data sent)";
			nodeview1.AppendColumn (lname);
			nodeview1.AppendColumn (psent);
			nodeview1.AppendColumn (ppsent);
			nodeview1.AppendColumn (rpsent);
			nodeview1.AppendColumn (dsent);
			nodeview1.AppendColumn (pdsent);
			nodeview1.AppendColumn (rdsent);

			store = new Gtk.ListStore (typeof(string), typeof(int), typeof(string), typeof(int), typeof(string), typeof(string), typeof(int));
			nodeview1.Model = store;

			Gtk.CellRendererText lnameCell = new Gtk.CellRendererText ();
			lname.PackStart (lnameCell, true);
			Gtk.CellRendererText psentCell = new Gtk.CellRendererText ();
			psent.PackStart (psentCell, true);
			Gtk.CellRendererText ppsentCell = new Gtk.CellRendererText ();
			ppsent.PackStart (ppsentCell, true);
			Gtk.CellRendererText rpsentCell = new Gtk.CellRendererText ();
			rpsent.PackStart (rpsentCell, true);
			Gtk.CellRendererText dsentCell = new Gtk.CellRendererText ();
			dsent.PackStart (dsentCell, true);
			Gtk.CellRendererText pdsentCell = new Gtk.CellRendererText ();
			pdsent.PackStart (pdsentCell, true);
			Gtk.CellRendererText rdsentCell = new Gtk.CellRendererText ();
			rdsent.PackStart (rdsentCell, true);

			lname.AddAttribute (lnameCell, "text", 0);
			psent.AddAttribute (psentCell, "text", 1);
			ppsent.AddAttribute (ppsentCell, "text", 2);
			rpsent.AddAttribute (rpsentCell, "text", 3);
			dsent.AddAttribute (dsentCell, "text", 4);
			pdsent.AddAttribute (pdsentCell, "text", 5);
			rdsent.AddAttribute (rdsentCell, "text", 6);
		}

		/**
		 * Load parameters from ResultModel
		 */
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

				NetworkNode.IfaceUse[] iu = rm.GetNetworkNodeIfaceUse (name);
				foreach(NetworkNode.IfaceUse rec in iu){
					store.AppendValues(rec.lname,rec.psent,(double)Math.Round(rec.ppsent,Storer.decimals)+"",rec.rpsent,(double)Math.Round(rec.dsent,Storer.decimals)+"",(double)Math.Round(rec.pdsent,Storer.decimals)+"",rec.rdsent);
				}
			}catch(ArgumentException ae){
				log.Debug (ae.Message);
			}
		}

		public void InitLabels(){
			label20.Text = "N/A";
			label21.Text = "N/A";
			label22.Text = "N/A";
			label23.Text = "N/A";
			label24.Text = "N/A";
			label25.Text = "N/A";
			label26.Text = "N/A";
			label27.Text = "N/A";
			label28.Text = "N/A";
		}
	}
}

