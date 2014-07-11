using System;

namespace GuiLite
{
	public class Node
	{
		private int packsPerTic;
		private Node linkedTo;
		private String name;

		public int PacksPerTic
		{
			get { return this.packsPerTic; }
			set { this.packsPerTic = value; }
		}

		public Node LinkedTo
		{
			get { return this.linkedTo; }
			set { this.linkedTo = value; }
		}

		public String Name{
			get { return this.name; }
		}

		public Node(String name)
		{
			this.name = name;
			this.packsPerTic = 0;
		}

		public void ReceivePacket(int time){
			Console.WriteLine ("Node " + name + " received a packet at time "+time);
		}

		public void SendPacket(int time){
			Console.WriteLine ("Node " + name + " sent a packet to " + linkedTo.Name + " at time " + time);
			linkedTo.ReceivePacket (time+1);
		}
	}
}

