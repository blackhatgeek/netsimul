using System;
using log4net;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class LinkParamWidget : Gtk.Bin
	{
		static readonly ILog log = LogManager.GetLogger(typeof(LinkParamWidget));
		public LinkParamWidget ()
		{
			this.Build ();
		}

		public void LoadParams(NetworkModel nm,String lname){
			int active1=0,active2=0,i=0;
			log.Debug ("Nodes: " + nm.GetNodeNames ().Length);
			foreach (String node in nm.GetNodeNames()) {
				log.Debug ("Node: " + node);
				combobox4.AppendText (node);
				combobox5.AppendText (node);
				try{
					if (node.Equals (nm.GetLinkNode1 (lname)))
						active1 = i;
					if (node.Equals (nm.GetLinkNode2 (lname)))
						active2 = i;
				}catch(ArgumentException ae){
					log.Debug ("Caught argument exception in link param widget - load params: "+ae.Message);
				}
				i++;
			}
			this.combobox4.Active = active1;
			this.combobox5.Active = active2;
			this.entry5.Text = lname;
			try{
				this.spinbutton4.Value = nm.LinkCapacity (lname);
				this.spinbutton5.Value = (double)nm.GetLinkToggleProbability (lname);
			}catch(ArgumentException ae){
				log.Debug ("Caught argument exception in link param widget - load params: " + ae.Message+ "(*)");
			}
		}

		public string GetName(){
			return this.entry5.Text;
		}

		public string GetN1Name(){
			return this.combobox4.ActiveText;
		}

		public string GetN2Name(){
			return this.combobox5.ActiveText;
		}

		public int GetCapacity(){
			return this.spinbutton4.ValueAsInt;
		}

		public decimal GetToggleProbability(){
			return (decimal)this.spinbutton5.Value;
		}

	}
}

