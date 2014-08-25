using System;

namespace NetTrafficSimulator
{
	/**
	 * ServerNodeWidget consists of ServerNodeParamWidget and ServerNodeResultWidget
	*/
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ServerNodeWidget : Gtk.Bin
	{
		/**
		 * Build the widget
		 */
		public ServerNodeWidget ()
		{
			this.Build ();
		}

		/**
		 * Param widget
		 */
		public ServerNodeParamWidget ParamWidget{
			get{
				return this.servernodeparamwidget1;
			}
		}

		/**
		 * Result widget
		 */
		public ServerNodeResultWidget ResultWidget{
			get{
				return this.servernoderesultwidget1;
			}
		}
	}
}

