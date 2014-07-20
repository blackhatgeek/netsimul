using System;
using System.Collections.Generic;

namespace GuiLite
{
	public class NodeException:Exception{
		public NodeException (String s):base(s){
		}
	}

	public abstract class Node:Proces{
		protected const int DEFAULT_POWER=16;

		private int power;
		protected String name;
		private Queue<EtherFrame> q_in;
		private NetworkInterface net;
		private bool locked;
		private EtherFrame last;
		private int last_remaining;

		public int Power
		{
			get{ return this.power;}
			set{ 
				if (value >= 0)
					this.power = value;
				else
					Console.WriteLine ("Attempt to change power to negative value! Not changed");
			}
		}

		public Link Link
		{
			get { return this.net.Linka; }
			set { this.net.Linka = value; }
		}

		public NetworkInterface Network{
			set{ this.net = value;}
		}

		public String Name{
			get { return this.name; }

			set { this.name = value;}
		}

		public MACaddr MAC{
			get{ return this.net.MAC;}
		}

		public Node(String name): this (name,DEFAULT_POWER){
		}

		public Node(String name,int power)
		{
			if (power < 0)
				throw new ArgumentOutOfRangeException ("Attempting to create node " + name + " with negative frames to process per tic");
			this.name = name;
			this.power = power;
			this.q_in = new Queue<EtherFrame> ();
			this.locked = false;
		}

		//vnejsi rozhrani - model
		//link pouziva Node.ReceiveFrame pro predani ramce
		public void ReceiveFrame(EtherFrame f,Model m){		
			//byl dorucen ramec,vlozime do fronty
			q_in.Enqueue (f);
			Console.WriteLine ("Node " + name + " received a frame at time " + m.Cas);
		}

		//vnitrni funkcionalita
		//potomek Node, bude v dusledku simulovat protokol
		//zde ma preddefinovany zpusob, jak odeslat ramec
		//potomci mohou prepsat (vice rozhrani apod.)
		protected virtual void SendFrame(EtherFrame f,Model m){
		if (net != null) {
			Console.WriteLine ("Node " + name + " sent a frame for sending to " + f.Destination + " at time " + m.Cas);
			this.net.Dispatch (f, m);
		} else
			throw new NodeException ("Linka nepripojena");
		}

		//vnitrni funkcionalita
		//rozbali ramec, vynda packet, bude zpracovavat packet
		//vraci jak dlouho bude zpracovavat packet
		public abstract int ProcessFrame (EtherFrame f, Model m);

		//vnejsi rozhrani - simulace
		public override void ZpracujUdalost (Stav u, Model m)
		{
			if (u == Stav.PROCESSING) {
				int remaining = this.power;
				if(locked){
					if(last_remaining>=remaining){
						remaining -= last_remaining;
						last_remaining=0;
						this.ProcessFrame (last, m);
						locked = false;
					}
					else{
						last_remaining -= remaining;
						remaining = 0;
						locked = true;
					}
				}
				while (remaining>0) {
					last = q_in.Dequeue ();
					if (remaining >= last.ProcessingCost) {
						remaining -= last.ProcessingCost;
						this.ProcessFrame (last, m);
						locked=false;
					}else {
						last_remaining = last.ProcessingCost - remaining;
						remaining = 0;
						locked = true;
					}
				}
				this.Naplanuj (m.K, Stav.PROCESSING, m.Cas + 1);
			}
		}

		//vnejsi rozhrani - simulace / program
		public void Fin(){
			//Console.WriteLine ("Node " + name + ": incoming queue has " + q_in.Count + " frames in, outcoming queue has " + q_out.Count + " frames in");
			locked = false;
			q_in.Clear();
		}

		//vnejsi rozhrani - program
		public NodeProperties ExportProperties(){
			NodeProperties n = new NodeProperties(this.name);
			n.POWER = this.power;
			return n;
		}

		//vnejsi rozhrani - program
		public void ImportProperties(NodeProperties n){
			this.name = n.Name;
			this.power = n.POWER;
		}
	}

	public abstract class MultiportNode:Node{
		protected NetworkInterface[] interfaces;
		protected int ports, in_use;

		public MultiportNode (String name,int ports):this(name,ports,DEFAULT_POWER){
		}

		public MultiportNode (String name,int ports,int frames_process_per_tic):base(name,frames_process_per_tic)
		{
			interfaces = new NetworkInterface[ports];
			this.ports = ports;
			for (int i=0; i<ports; i++) {
					interfaces [i] = new NetworkInterface ();//!! MACException
			}
		}

		public bool PortAvailable{
			get{
				return in_use < (ports-1);
			}
		}

		public void Connect(Link l){
			int i = 0;
			bool search = true;
			while((i<ports)&search){
				if (interfaces [i].InUse)
					i++;
				else {
					search = false;
					if (interfaces [i].MAC != l.Destination.MAC)
						interfaces [i].Linka = l;
					else
						throw new LinkException ("Smycka na jednom portu");
				}
			}
		}

		//jakym zpusobem se rozdeli packety na jednotlive vystupni rozhrani
		protected override void SendFrame (EtherFrame f, Model m)
		{
			this.multiport_sending (f,m);
		}

		protected abstract void multiport_sending (EtherFrame f, Model m);
	}

}