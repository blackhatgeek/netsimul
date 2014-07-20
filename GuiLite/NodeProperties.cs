using System;

namespace GuiLite
{
	public class NodeProperties
	{
		private String name;
		private int power;

		public String Name{
			get{return this.name;}
			set{ this.name = value;}
		}

		public int POWER{
			get{return this.power;}
			set{ this.power = value;}
		}

		public NodeProperties (String name)
		{
			this.name = name;
		}
	}
}

