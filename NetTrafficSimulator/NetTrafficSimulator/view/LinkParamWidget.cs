using System;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * Widget with parameters of a link
	 */
	[System.ComponentModel.ToolboxItem(true)]
	public partial class LinkParamWidget : Gtk.Bin
	{
		MainWindow mw;
		string name,n1,n2;
		int active1=0,active2=0,lcap;
		decimal tp;
		NetworkModel nm;
		static readonly ILog log = LogManager.GetLogger(typeof(LinkParamWidget));

		/**
		 * Build th link param widget
		 */
		public LinkParamWidget ()
		{
			this.Build ();
		}

		/**
		 * Load params from models
		 */
		public void LoadParams(NetworkModel nm,String lname,MainWindow mw){
			this.nm = nm;
			this.name = lname;
			this.mw = mw;
			int i=0;
			log.Debug ("Nodes: " + nm.GetNodeNames ().Length);
			foreach (String node in nm.GetNodeNames()) {
				log.Debug ("Node: " + node);
				combobox4.AppendText (node);
				combobox5.AppendText (node);
				try{
					if (node.Equals (nm.GetLinkNode1 (lname))){
						active1 = i;
						n1=node;
					}
					if (node.Equals (nm.GetLinkNode2 (lname))){
						active2 = i;
						n2=node;
					}
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
				lcap = nm.LinkCapacity(lname);
				this.spinbutton5.Value = (double)nm.GetLinkToggleProbability (lname);
				tp = nm.GetLinkToggleProbability(lname);
			}catch(ArgumentException ae){
				log.Debug ("Caught argument exception in link param widget - load params: " + ae.Message+ "(*)");
			}
		}

		/**
		 * Get link name
		 */
		public string GetName(){
			return this.entry5.Text;
		}

		/**
		 * Change button
		 * Try to set parameters to the values currently in fields or show dialogs and recover from exceptions
		 */
		protected void OnButton618Clicked (object sender, EventArgs e)
		{
			if (nm != null) {
				if (mw != null) {
					bool trigmwc = false;
					if (!entry5.Text.Equals (name)) {
						log.Debug ("Name change to " + entry5.Text + "?");
						if (this.entry5.Text.Equals ("") || this.entry5.Text.Contains ("\r") || this.entry5.Text.Contains ("\n") || this.entry5.Text.Contains ("\t") || this.entry5.Text.EndsWith (" ") || this.entry5.Text.StartsWith (" ") || this.entry5.Text.Contains ("  ")) {
							Gtk.MessageDialog md1 = new Gtk.MessageDialog (mw, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Close, "Link name cannot contain: LF,CR,tab,spaces at the beginning or at the end, multiple spaces next to each other. Link name cannot be empty. Name was not changed.");
							md1.Run ();
							md1.Destroy ();
							entry5.Text = name;
						} else {
							try {
								nm.SetLinkName (name, entry5.Text);
								name = entry5.Text;
								trigmwc = true;
							} catch (ArgumentException) {
								Gtk.MessageDialog md = new Gtk.MessageDialog (mw, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, "Name change failed");
								md.Run ();
								md.Destroy ();
								entry5.Text = name;
							}
						}
					}
					if (!combobox4.ActiveText.Equals (n1) || !combobox5.ActiveText.Equals (n2) || spinbutton4.ValueAsInt != lcap || (decimal)spinbutton5.Value != tp) {
						log.Debug ("Parameters changed");
						try {
							nm.RemoveLink (name);
							log.Debug("Link removed");

							if (nm.CanSetConnected (combobox4.ActiveText, combobox5.ActiveText, name, spinbutton4.ValueAsInt, (decimal)spinbutton5.Value)){

								nm.SetConnected (combobox4.ActiveText, combobox5.ActiveText, name, spinbutton4.ValueAsInt,
							                 (decimal)spinbutton5.Value);
								if (combobox4.Active != active1) {
									trigmwc = true;
									active1 = combobox4.Active;
									n1 = combobox4.ActiveText;
								}
								if (combobox5.Active != active2) {
									trigmwc = true;
									active2 = combobox5.Active;
									n2 = combobox5.ActiveText;
								}
								lcap = spinbutton4.ValueAsInt;
								tp = (decimal)spinbutton5.Value;
							}else{
								Gtk.MessageDialog md = new Gtk.MessageDialog (mw, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, "Parameters change failed");
								md.Run ();
								md.Destroy ();
								combobox4.Active = active1;
								combobox5.Active = active2;
								spinbutton4.Value = lcap;
								spinbutton5.Value = (double)tp;
								try{
									nm.SetConnected(n1,n2,name,lcap,tp);
								}catch(Exception ex){
									log.Error ("Exception on recovery: "+ex.Message);
									Gtk.MessageDialog md1 = new Gtk.MessageDialog (mw, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, "Failed to recover");
									md1.Run ();
									md1.Destroy ();
								}
							}

						} catch (Exception) {
							Gtk.MessageDialog md = new Gtk.MessageDialog (mw, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, "Parameters change failed");
							md.Run ();
							md.Destroy ();
							try{
								nm.SetConnected (n1, n2, name, lcap, tp);
							}catch(Exception ex){
								log.Error ("Exception on recovery: "+ex.Message);
								Gtk.MessageDialog md1 = new Gtk.MessageDialog (mw, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, "Failed to recover");
								md1.Run ();
								md1.Destroy ();
							}
							combobox4.Active = active1;
							combobox5.Active = active2;
							spinbutton4.Value = lcap;
							spinbutton5.Value = (double)tp;
						}
					}
					if (trigmwc)
						mw.LinkChanged ();
				} 
			}					
		}
	}
}

