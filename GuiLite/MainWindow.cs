using System;
using Gtk;
using GuiLite;

public partial class MainWindow: Gtk.Window
{	
	private object zamek=new object();

	private GuiLite.Node a, b;
	private GuiLite.Link link;
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		a = new SampleNode ("A");
		b = new SampleNode ("B");
		link = new Link ("L1",10, 0.0, 0.0);
		//link.A = a;
		//link.B = b;
		Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void btnAclicked (object sender, EventArgs e)
	{
		GuiLite.NodeParam apar = new GuiLite.NodeParam (a);
		apar.Show ();
	}

	protected void btnBclicked (object sender, EventArgs e)
	{
		GuiLite.NodeParam bpar = new GuiLite.NodeParam (b);
		bpar.Show ();
	}

	protected void OnButton7Clicked (object sender, EventArgs e)
	{
		lock (zamek) {
			NodeProperties bak_a = a.ExportProperties (), bak_b = b.ExportProperties ();
			Model m = new Model (new Node[] { a, b }, spinbutton1.ValueAsInt);
			m.Simulace ();
			a = new SampleNode ("A");
			b = new SampleNode ("B");
			a.ImportProperties (bak_a);
			b.ImportProperties(bak_b);
			//link.A = a;
			//link.B = b;
			m = null;
		}
	}

	protected void OnButton69Clicked (object sender, EventArgs e)
	{
		Application.Quit ();
	}
}