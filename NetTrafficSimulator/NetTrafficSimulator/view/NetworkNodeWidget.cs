using System;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class NetworkNodeWidget : Gtk.Bin
	{
		public NetworkNodeWidget ()
		{
			this.Build ();
		}

		public NetworkNodeParamWidget ParamWidget{
			get{
				return this.networknodeparamwidget1;
			}
		}

		public NetworkNodeResultWidget ResultWidget{
			get{
				return this.networknoderesultwidget1;
			}
		}
	}
}

