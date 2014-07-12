using System;
using System.Collections.Generic;

namespace GuiLite
{
	public class Node:Proces
	{
		private int packsPerTic;
		private Node linkedTo;
		private String name;
		private Queue<Packet> q_in;//,q_out;

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
			q_in = new Queue<Packet> ();
			//q_out = new Queue<Packet> ();
		}

		public void ReceivePacket(Packet p,Model m){		
			//byl dorucen packet,vlozime do fronty
			q_in.Enqueue (p);
			this.Naplanuj (m.K, Stav.RECEIVING, m.Cas + 1);
			Console.WriteLine ("Node " + name + " received a packet at time "+m.Cas);
		}

		public override void ZpracujUdalost(Stav u,Model m){
			if (u == Stav.RECEIVING) {
				//prijem packetu - projdeme vstupni frontu a napiseme na vystup hlasku o zpracovani packetu
				while (q_in.Count>0) {
					//Packet p = 
						q_in.Dequeue ();
					Console.WriteLine ("Node " + name + " processed a packet at time " + m.Cas);
				}
				//naplanovani
				int kdy = m.Cas + 1;//zatim neresime cas nutny na zpracovani packetu
				this.Naplanuj (m.K, Stav.SENDING, kdy);
			} else if (u == Stav.SENDING) {
				//odeslani packetu - projdeme vystupni frontu a napiseme na vystup hlasku o zpracovani packetu
				for(int i=0;i<packsPerTic;i++){
					Packet p = new Packet();
					linkedTo.ReceivePacket (p, m);
					Console.WriteLine ("Node " + name + " sent a packet to " + linkedTo.Name + " at time " + m.Cas);
				}
				//naplanovani
				int kdy = m.Cas + 1;
				this.Naplanuj (m.K, Stav.RECEIVING, kdy);
			}
		}

		public void Init(Model m){
			this.Naplanuj (m.K, Stav.SENDING, 0);
		}
	}
}

