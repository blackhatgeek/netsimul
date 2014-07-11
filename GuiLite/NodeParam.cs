using System;

namespace GuiLite
{
	public partial class NodeParam : Gtk.Dialog
	{
		public NodeParam (String name)
		{
			this.Title = name;
			this.Build ();
			this.label2.Text = name;
		}

		protected void btn_cancel_click (object sender, EventArgs e)
		{
			this.Destroy();
		}

		protected void btn_ok_click (object sender, EventArgs e)
		{
			this.Destroy ();
		}
	}
}

