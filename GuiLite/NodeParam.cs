using System;

namespace GuiLite
{
	public partial class NodeParam : Gtk.Dialog
	{
		private Node node;
		public NodeParam (Node n)
		{
			this.node = n;
			this.Title = n.Name;
			this.Build ();
			this.label2.Text = n.Name;
			//this.spinbutton1.Value = n.WaitTime;
			this.spinbutton2.Value = n.FramesProcessPerTic;
			//this.spinbutton3.Value = n.FramesSentPerTic;
		}

		protected void btn_cancel_click (object sender, EventArgs e)
		{
			this.Destroy();
		}

		protected void btn_ok_click (object sender, EventArgs e)
		{
			//node.WaitTime = this.spinbutton1.ValueAsInt;
			node.FramesProcessPerTic = this.spinbutton2.ValueAsInt;
			//node.FramesSentPerTic = this.spinbutton3.ValueAsInt;
			this.Destroy ();
		}
	}
}