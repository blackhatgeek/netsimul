using System;

namespace GuiLite
{
	public class Node
	{
		private int packsPerTic;

		public int PacksPerTic
		{
			get { return this.packsPerTic; }
			set { this.packsPerTic = value; }
		}


		public Node (int ppt)
		{
			this.packsPerTic = ppt;
		}

		public Node()
		{
			this.packsPerTic = 0;
		}
	}
}

