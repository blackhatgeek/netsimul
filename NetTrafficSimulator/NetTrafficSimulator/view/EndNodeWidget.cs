using System;

namespace NetTrafficSimulator
{
	/**
	 * EndNodeWidget is composed of EndNodeParamWidget, EndNodeEventsWidget and EndNodeResultWidget
	*/
	[System.ComponentModel.ToolboxItem(true)]
	public partial class EndNodeWidget : Gtk.Bin
	{
		/**
		 * Build the widget
		 */
		public EndNodeWidget ()
		{
			this.Build ();

		}

		/**
		 * Get param widget
		 */
		public EndNodeParamWidget ParamWidget{
			get{
				return this.endnodeparamwidget3;
			}
		}

		/**
		 * Get result widget
		 */
		public EndNodeResultWidget ResultWidget{
			get{
				return this.endnoderesultwidget4;
			}
		}

		/**
		 * Get events widget
		 */
		public EndNodeEventsWidget EventWidget{
			get{
				return this.endnodeeventswidget4;
			}
		}
	}
}

