
// This file has been generated by the GUI designer. Do not modify.
namespace NetTrafficSimulator
{
	public partial class EndNodeResultWidget
	{
		private global::Gtk.VBox vbox8;
		private global::Gtk.HBox hbox18;
		private global::Gtk.Label label17;
		private global::Gtk.Label label23;
		private global::Gtk.HBox hbox19;
		private global::Gtk.Label label18;
		private global::Gtk.Label label24;
		private global::Gtk.HBox hbox20;
		private global::Gtk.Label label19;
		private global::Gtk.Label label25;
		private global::Gtk.HBox hbox21;
		private global::Gtk.Label label20;
		private global::Gtk.Label label26;
		private global::Gtk.HBox hbox22;
		private global::Gtk.Label label21;
		private global::Gtk.Label label27;
		private global::Gtk.HBox hbox23;
		private global::Gtk.Label label22;
		private global::Gtk.Label label28;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget NetTrafficSimulator.EndNodeResultWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "NetTrafficSimulator.EndNodeResultWidget";
			// Container child NetTrafficSimulator.EndNodeResultWidget.Gtk.Container+ContainerChild
			this.vbox8 = new global::Gtk.VBox ();
			this.vbox8.Name = "vbox8";
			this.vbox8.Spacing = 6;
			// Container child vbox8.Gtk.Box+BoxChild
			this.hbox18 = new global::Gtk.HBox ();
			this.hbox18.Name = "hbox18";
			this.hbox18.Spacing = 6;
			// Container child hbox18.Gtk.Box+BoxChild
			this.label17 = new global::Gtk.Label ();
			this.label17.Name = "label17";
			this.label17.LabelProp = global::Mono.Unix.Catalog.GetString ("Packets sent");
			this.hbox18.Add (this.label17);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox18 [this.label17]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child hbox18.Gtk.Box+BoxChild
			this.label23 = new global::Gtk.Label ();
			this.label23.Name = "label23";
			this.label23.LabelProp = global::Mono.Unix.Catalog.GetString ("N/A");
			this.hbox18.Add (this.label23);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox18 [this.label23]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			this.vbox8.Add (this.hbox18);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.hbox18]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vbox8.Gtk.Box+BoxChild
			this.hbox19 = new global::Gtk.HBox ();
			this.hbox19.Name = "hbox19";
			this.hbox19.Spacing = 6;
			// Container child hbox19.Gtk.Box+BoxChild
			this.label18 = new global::Gtk.Label ();
			this.label18.Name = "label18";
			this.label18.LabelProp = global::Mono.Unix.Catalog.GetString ("Packets received");
			this.hbox19.Add (this.label18);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox19 [this.label18]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child hbox19.Gtk.Box+BoxChild
			this.label24 = new global::Gtk.Label ();
			this.label24.Name = "label24";
			this.label24.LabelProp = global::Mono.Unix.Catalog.GetString ("label24");
			this.hbox19.Add (this.label24);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox19 [this.label24]));
			w5.Position = 1;
			w5.Expand = false;
			w5.Fill = false;
			this.vbox8.Add (this.hbox19);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.hbox19]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vbox8.Gtk.Box+BoxChild
			this.hbox20 = new global::Gtk.HBox ();
			this.hbox20.Name = "hbox20";
			this.hbox20.Spacing = 6;
			// Container child hbox20.Gtk.Box+BoxChild
			this.label19 = new global::Gtk.Label ();
			this.label19.Name = "label19";
			this.label19.LabelProp = global::Mono.Unix.Catalog.GetString ("Packets malreceived");
			this.hbox20.Add (this.label19);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox20 [this.label19]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			// Container child hbox20.Gtk.Box+BoxChild
			this.label25 = new global::Gtk.Label ();
			this.label25.Name = "label25";
			this.label25.LabelProp = global::Mono.Unix.Catalog.GetString ("label25");
			this.hbox20.Add (this.label25);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox20 [this.label25]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			this.vbox8.Add (this.hbox20);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.hbox20]));
			w9.Position = 2;
			w9.Expand = false;
			w9.Fill = false;
			// Container child vbox8.Gtk.Box+BoxChild
			this.hbox21 = new global::Gtk.HBox ();
			this.hbox21.Name = "hbox21";
			this.hbox21.Spacing = 6;
			// Container child hbox21.Gtk.Box+BoxChild
			this.label20 = new global::Gtk.Label ();
			this.label20.Name = "label20";
			this.label20.LabelProp = global::Mono.Unix.Catalog.GetString ("Percent time idle");
			this.hbox21.Add (this.label20);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox21 [this.label20]));
			w10.Position = 0;
			w10.Expand = false;
			w10.Fill = false;
			// Container child hbox21.Gtk.Box+BoxChild
			this.label26 = new global::Gtk.Label ();
			this.label26.Name = "label26";
			this.label26.LabelProp = global::Mono.Unix.Catalog.GetString ("label26");
			this.hbox21.Add (this.label26);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox21 [this.label26]));
			w11.Position = 1;
			w11.Expand = false;
			w11.Fill = false;
			this.vbox8.Add (this.hbox21);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.hbox21]));
			w12.Position = 3;
			w12.Expand = false;
			w12.Fill = false;
			// Container child vbox8.Gtk.Box+BoxChild
			this.hbox22 = new global::Gtk.HBox ();
			this.hbox22.Name = "hbox22";
			this.hbox22.Spacing = 6;
			// Container child hbox22.Gtk.Box+BoxChild
			this.label21 = new global::Gtk.Label ();
			this.label21.Name = "label21";
			this.label21.LabelProp = global::Mono.Unix.Catalog.GetString ("Average wait time");
			this.hbox22.Add (this.label21);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox22 [this.label21]));
			w13.Position = 0;
			w13.Expand = false;
			w13.Fill = false;
			// Container child hbox22.Gtk.Box+BoxChild
			this.label27 = new global::Gtk.Label ();
			this.label27.Name = "label27";
			this.label27.LabelProp = global::Mono.Unix.Catalog.GetString ("label27");
			this.hbox22.Add (this.label27);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.hbox22 [this.label27]));
			w14.Position = 1;
			w14.Expand = false;
			w14.Fill = false;
			this.vbox8.Add (this.hbox22);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.hbox22]));
			w15.Position = 4;
			w15.Expand = false;
			w15.Fill = false;
			// Container child vbox8.Gtk.Box+BoxChild
			this.hbox23 = new global::Gtk.HBox ();
			this.hbox23.Name = "hbox23";
			this.hbox23.Spacing = 6;
			// Container child hbox23.Gtk.Box+BoxChild
			this.label22 = new global::Gtk.Label ();
			this.label22.Name = "label22";
			this.label22.LabelProp = global::Mono.Unix.Catalog.GetString ("Average packet size");
			this.hbox23.Add (this.label22);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hbox23 [this.label22]));
			w16.Position = 0;
			w16.Expand = false;
			w16.Fill = false;
			// Container child hbox23.Gtk.Box+BoxChild
			this.label28 = new global::Gtk.Label ();
			this.label28.Name = "label28";
			this.label28.LabelProp = global::Mono.Unix.Catalog.GetString ("label28");
			this.hbox23.Add (this.label28);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.hbox23 [this.label28]));
			w17.Position = 1;
			w17.Expand = false;
			w17.Fill = false;
			this.vbox8.Add (this.hbox23);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.hbox23]));
			w18.Position = 5;
			w18.Expand = false;
			w18.Fill = false;
			this.Add (this.vbox8);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
		}
	}
}
