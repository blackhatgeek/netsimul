
// This file has been generated by the GUI designer. Do not modify.
namespace NetTrafficSimulator
{
	public partial class ServerNodeWidget
	{
		private global::Gtk.VBox vbox4;
		private global::Gtk.Frame frame8;
		private global::Gtk.Alignment GtkAlignment;
		private global::NetTrafficSimulator.ServerNodeParamWidget servernodeparamwidget1;
		private global::Gtk.Label GtkLabel1;
		private global::Gtk.Frame frame9;
		private global::Gtk.Alignment GtkAlignment1;
		private global::NetTrafficSimulator.ServerNodeResultWidget servernoderesultwidget1;
		private global::Gtk.Label GtkLabel2;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget NetTrafficSimulator.ServerNodeWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "NetTrafficSimulator.ServerNodeWidget";
			// Container child NetTrafficSimulator.ServerNodeWidget.Gtk.Container+ContainerChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.frame8 = new global::Gtk.Frame ();
			this.frame8.Name = "frame8";
			this.frame8.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame8.Gtk.Container+ContainerChild
			this.GtkAlignment = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment.Name = "GtkAlignment";
			this.GtkAlignment.LeftPadding = ((uint)(12));
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			this.servernodeparamwidget1 = new global::NetTrafficSimulator.ServerNodeParamWidget ();
			this.servernodeparamwidget1.Events = ((global::Gdk.EventMask)(256));
			this.servernodeparamwidget1.Name = "servernodeparamwidget1";
			this.GtkAlignment.Add (this.servernodeparamwidget1);
			this.frame8.Add (this.GtkAlignment);
			this.GtkLabel1 = new global::Gtk.Label ();
			this.GtkLabel1.Name = "GtkLabel1";
			this.GtkLabel1.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Parameters</b>");
			this.GtkLabel1.UseMarkup = true;
			this.frame8.LabelWidget = this.GtkLabel1;
			this.vbox4.Add (this.frame8);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.frame8]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.frame9 = new global::Gtk.Frame ();
			this.frame9.Name = "frame9";
			this.frame9.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame9.Gtk.Container+ContainerChild
			this.GtkAlignment1 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment1.Name = "GtkAlignment1";
			this.GtkAlignment1.LeftPadding = ((uint)(12));
			// Container child GtkAlignment1.Gtk.Container+ContainerChild
			this.servernoderesultwidget1 = new global::NetTrafficSimulator.ServerNodeResultWidget ();
			this.servernoderesultwidget1.Events = ((global::Gdk.EventMask)(256));
			this.servernoderesultwidget1.Name = "servernoderesultwidget1";
			this.GtkAlignment1.Add (this.servernoderesultwidget1);
			this.frame9.Add (this.GtkAlignment1);
			this.GtkLabel2 = new global::Gtk.Label ();
			this.GtkLabel2.Name = "GtkLabel2";
			this.GtkLabel2.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Results</b>");
			this.GtkLabel2.UseMarkup = true;
			this.frame9.LabelWidget = this.GtkLabel2;
			this.vbox4.Add (this.frame9);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.frame9]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			this.Add (this.vbox4);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
		}
	}
}
