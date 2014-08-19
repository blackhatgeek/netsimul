using System;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ServerNodeWidget : Gtk.Bin
	{
		public ServerNodeWidget ()
		{
			this.Build ();
		}

		public ServerNodeParamWidget ParamWidget{
			get{
				return this.servernodeparamwidget1;
			}
		}

		public ServerNodeResultWidget ResultWidget{
			get{
				return this.servernoderesultwidget1;
			}
		}
	}
}

