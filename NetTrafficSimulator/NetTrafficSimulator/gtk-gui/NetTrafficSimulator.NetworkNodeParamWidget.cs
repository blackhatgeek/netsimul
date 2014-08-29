
// This file has been generated by the GUI designer. Do not modify.
namespace NetTrafficSimulator
{
	public partial class NetworkNodeParamWidget
	{
		private global::Gtk.VBox vbox5;
		private global::Gtk.HBox hbox9;
		private global::Gtk.Label label9;
		private global::Gtk.Entry entry4;
		private global::Gtk.HBox hbox10;
		private global::Gtk.Label label10;
		private global::Gtk.Fixed fixed5;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.NodeView nodeview1;
		private global::Gtk.HBox hbox11;
		private global::Gtk.Label label2;
		private global::Gtk.ComboBox combobox2;
		private global::Gtk.HBox hbox1;
		private global::Gtk.Label label1;
		private global::Gtk.SpinButton spinbutton1;
		private global::Gtk.HBox hbox2;
		private global::Gtk.Label label3;
		private global::Gtk.SpinButton spinbutton2;
		private global::Gtk.HBox hbox3;
		private global::Gtk.Label label4;
		private global::Gtk.SpinButton spinbutton3;
		private global::Gtk.Button button472;
		private global::Gtk.Fixed fixed2;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget NetTrafficSimulator.NetworkNodeParamWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "NetTrafficSimulator.NetworkNodeParamWidget";
			// Container child NetTrafficSimulator.NetworkNodeParamWidget.Gtk.Container+ContainerChild
			this.vbox5 = new global::Gtk.VBox ();
			this.vbox5.Name = "vbox5";
			this.vbox5.Spacing = 6;
			// Container child vbox5.Gtk.Box+BoxChild
			this.hbox9 = new global::Gtk.HBox ();
			this.hbox9.Name = "hbox9";
			this.hbox9.Spacing = 6;
			// Container child hbox9.Gtk.Box+BoxChild
			this.label9 = new global::Gtk.Label ();
			this.label9.Name = "label9";
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("Name");
			this.hbox9.Add (this.label9);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox9 [this.label9]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child hbox9.Gtk.Box+BoxChild
			this.entry4 = new global::Gtk.Entry ();
			this.entry4.CanFocus = true;
			this.entry4.Name = "entry4";
			this.entry4.IsEditable = true;
			this.entry4.InvisibleChar = ' ';
			this.hbox9.Add (this.entry4);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox9 [this.entry4]));
			w2.Position = 1;
			this.vbox5.Add (this.hbox9);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.hbox9]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.hbox10 = new global::Gtk.HBox ();
			this.hbox10.Name = "hbox10";
			this.hbox10.Spacing = 6;
			// Container child hbox10.Gtk.Box+BoxChild
			this.label10 = new global::Gtk.Label ();
			this.label10.Name = "label10";
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("Connected links");
			this.hbox10.Add (this.label10);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox10 [this.label10]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child hbox10.Gtk.Box+BoxChild
			this.fixed5 = new global::Gtk.Fixed ();
			this.fixed5.Name = "fixed5";
			this.fixed5.HasWindow = false;
			this.hbox10.Add (this.fixed5);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox10 [this.fixed5]));
			w5.Position = 1;
			this.vbox5.Add (this.hbox10);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.hbox10]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.nodeview1 = new global::Gtk.NodeView ();
			this.nodeview1.CanFocus = true;
			this.nodeview1.Name = "nodeview1";
			this.GtkScrolledWindow.Add (this.nodeview1);
			this.vbox5.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.GtkScrolledWindow]));
			w8.Position = 2;
			// Container child vbox5.Gtk.Box+BoxChild
			this.hbox11 = new global::Gtk.HBox ();
			this.hbox11.Name = "hbox11";
			this.hbox11.Spacing = 6;
			// Container child hbox11.Gtk.Box+BoxChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Default route");
			this.hbox11.Add (this.label2);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox11 [this.label2]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			// Container child hbox11.Gtk.Box+BoxChild
			this.combobox2 = global::Gtk.ComboBox.NewText ();
			this.combobox2.Name = "combobox2";
			this.hbox11.Add (this.combobox2);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox11 [this.combobox2]));
			w10.Position = 1;
			w10.Expand = false;
			w10.Fill = false;
			this.vbox5.Add (this.hbox11);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.hbox11]));
			w11.Position = 3;
			w11.Expand = false;
			w11.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Update timer");
			this.hbox1.Add (this.label1);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.label1]));
			w12.Position = 0;
			w12.Expand = false;
			w12.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.spinbutton1 = new global::Gtk.SpinButton (0, 100, 1);
			this.spinbutton1.CanFocus = true;
			this.spinbutton1.Name = "spinbutton1";
			this.spinbutton1.Adjustment.PageIncrement = 10;
			this.spinbutton1.ClimbRate = 1;
			this.spinbutton1.Numeric = true;
			this.hbox1.Add (this.spinbutton1);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.spinbutton1]));
			w13.Position = 1;
			w13.Expand = false;
			w13.Fill = false;
			this.vbox5.Add (this.hbox1);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.hbox1]));
			w14.Position = 4;
			w14.Expand = false;
			w14.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Expiry timer");
			this.hbox2.Add (this.label3);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.label3]));
			w15.Position = 0;
			w15.Expand = false;
			w15.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.spinbutton2 = new global::Gtk.SpinButton (0, 100, 1);
			this.spinbutton2.CanFocus = true;
			this.spinbutton2.Name = "spinbutton2";
			this.spinbutton2.Adjustment.PageIncrement = 10;
			this.spinbutton2.ClimbRate = 1;
			this.spinbutton2.Numeric = true;
			this.hbox2.Add (this.spinbutton2);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.spinbutton2]));
			w16.Position = 1;
			w16.Expand = false;
			w16.Fill = false;
			this.vbox5.Add (this.hbox2);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.hbox2]));
			w17.Position = 5;
			w17.Expand = false;
			w17.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Flush timer");
			this.hbox3.Add (this.label4);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.label4]));
			w18.Position = 0;
			w18.Expand = false;
			w18.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.spinbutton3 = new global::Gtk.SpinButton (0, 100, 1);
			this.spinbutton3.CanFocus = true;
			this.spinbutton3.Name = "spinbutton3";
			this.spinbutton3.Adjustment.PageIncrement = 10;
			this.spinbutton3.ClimbRate = 1;
			this.spinbutton3.Numeric = true;
			this.hbox3.Add (this.spinbutton3);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.spinbutton3]));
			w19.Position = 1;
			w19.Expand = false;
			w19.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.button472 = new global::Gtk.Button ();
			this.button472.CanFocus = true;
			this.button472.Name = "button472";
			this.button472.UseUnderline = true;
			this.button472.Label = global::Mono.Unix.Catalog.GetString ("Change");
			this.hbox3.Add (this.button472);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.button472]));
			w20.PackType = ((global::Gtk.PackType)(1));
			w20.Position = 2;
			w20.Expand = false;
			w20.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.fixed2 = new global::Gtk.Fixed ();
			this.fixed2.Name = "fixed2";
			this.fixed2.HasWindow = false;
			this.hbox3.Add (this.fixed2);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.fixed2]));
			w21.PackType = ((global::Gtk.PackType)(1));
			w21.Position = 3;
			this.vbox5.Add (this.hbox3);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.hbox3]));
			w22.Position = 6;
			w22.Expand = false;
			w22.Fill = false;
			this.Add (this.vbox5);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.button472.Clicked += new global::System.EventHandler (this.OnButton422Clicked);
		}
	}
}
