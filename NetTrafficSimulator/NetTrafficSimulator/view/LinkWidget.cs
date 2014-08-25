using System;

namespace NetTrafficSimulator
{
	/**
	 * Link widget is composed of LinkParamWidget and LinkResultWidget
	*/
	[System.ComponentModel.ToolboxItem(true)]
	public partial class LinkWidget : Gtk.Bin
	{
		/**
		 * Build the widget
		 */
		public LinkWidget ()
		{
			this.Build ();
		}

		/**
		 * Get the param widget
		 */
		public LinkParamWidget ParamWidget{
			get{
				return this.linkparamwidget1;
			}
		}

		/**
		 * Get the result widget
		 */
		public LinkResultWidget ResultWidget{
			get{
				return this.linkresultwidget1;
			}
		}
	}
}

