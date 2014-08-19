using System;

namespace NetTrafficSimulator
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class EndNodeWidget : Gtk.Bin
	{
		public EndNodeWidget ()
		{
			this.Build ();

		}

		public EndNodeParamWidget ParamWidget{
			get{
				return this.endnodeparamwidget3;
			}
		}

		public EndNodeResultWidget ResultWidget{
			get{
				return this.endnoderesultwidget4;
			}
		}

		public EndNodeEventsWidget EventWidget{
			get{
				return this.endnodeeventswidget4;
			}
		}
	}
}

