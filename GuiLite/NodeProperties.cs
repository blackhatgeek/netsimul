using System;

namespace GuiLite
{
	public class NodeProperties
	{
		private String name;
		private int wait,fppt,fspt;

		public String Name{
			get{return this.name;}
			set{ this.name = value;}
		}

		public int FPPT{
			get{return this.fppt;}
			set{ this.fppt = value;}
		}

		public NodeProperties (String name)
		{
			this.name = name;
		}
	}
}

