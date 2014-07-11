using System;
using Gtk;
using GuiLite;

public partial class MainWindow: Gtk.Window
{	
	private GuiLite.Node a, b;
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		a = new Node ();
		b = new Node ();
		Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void btnAclicked (object sender, EventArgs e)
	{
		GuiLite.NodeParam apar = new GuiLite.NodeParam ("A",a);
		apar.Show ();
	}

	protected void btnBclicked (object sender, EventArgs e)
	{
		GuiLite.NodeParam bpar = new GuiLite.NodeParam ("B",b);
		bpar.Show ();
	}
}
