using System;

namespace GuiLite
{
	public partial class NodeParam : Gtk.Dialog
	{
		private Node node;
		public NodeParam (String name,Node n)
		{
			this.node = n;
			this.Title = name;
			this.Build ();
			this.label2.Text = name;
			this.spinbutton3.Value = n.PacksPerTic;
		}

		protected void btn_cancel_click (object sender, EventArgs e)
		{
			this.Destroy();
		}

		protected void btn_ok_click (object sender, EventArgs e)
		{
			node.PacksPerTic = this.spinbutton3.ValueAsInt;
			this.Destroy ();
		}
	}
}

