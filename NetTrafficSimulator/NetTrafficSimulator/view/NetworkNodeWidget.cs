using System;

namespace NetTrafficSimulator
{
	/**
	 * NetworkNodeWidget consists of NetworkNodeParamWidget and NetworkNodeResultWidget
	*/
	[System.ComponentModel.ToolboxItem(true)]
	public partial class NetworkNodeWidget : Gtk.Bin
	{
		/**
		 * Build the widget
		 */
		public NetworkNodeWidget ()
		{
			this.Build ();
		}

		/**
		 * The param widget
		 */
		public NetworkNodeParamWidget ParamWidget{
			get{
				return this.networknodeparamwidget1;
			}
		}

		/**
		 * The result widget
		 */
		public NetworkNodeResultWidget ResultWidget{
			get{
				return this.networknoderesultwidget1;
			}
		}
	}
}

