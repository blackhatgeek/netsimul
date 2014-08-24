using System;

namespace NetTrafficSimulator
{
	/**
	*/
	[System.ComponentModel.ToolboxItem(true)]
	public partial class LinkWidget : Gtk.Bin
	{
		/**
		 */
		public LinkWidget ()
		{
			this.Build ();
		}

		/**
		 */
		public LinkParamWidget ParamWidget{
			get{
				return this.linkparamwidget1;
			}
		}

		/**
		 */
		public LinkResultWidget ResultWidget{
			get{
				return this.linkresultwidget1;
			}
		}
	}
}

