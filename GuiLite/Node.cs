using System;
using System.Collections.Generic;

namespace GuiLite
{
	public abstract class Node:Proces
	{
		protected const int DEFAULT_WAIT_TIME=3;
		protected const int DEFAULT_FSPT=4;
		protected const int DEFAULT_FPPT=10;

		private int framesSentPerTic, framesProcessPerTic;
		protected String name;
		private Queue<EtherFrame> q_in;
		protected Queue<EtherFrame> q_out;
		private int wait;//kolik kol se ceka na potvrzeni prijeti ramce
		private int scheduled;//kolik packetu se bude zpracovavat v pristim kole
		private bool req_flood_stop;
		private int nearest_sending_scheduled=-1;
		private int nearest_receiving_scheduled=-1;
		private NetworkInterface net;

		public int FramesSentPerTic
		{
			get { return this.framesSentPerTic; }
			set { this.framesSentPerTic = value; }
		}
		public int WaitTime
		{
			get{ return this.wait;}
			set{ this.wait = value;}
		}
		public int FramesProcessPerTic
		{
			get{ return this.framesProcessPerTic;}
			set{ this.framesProcessPerTic = value;}
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
		}

		public MACaddr MAC{
			get{ return this.net.MAC;}
		}

		public Node(String name): this (name, DEFAULT_WAIT_TIME,DEFAULT_FSPT,DEFAULT_FPPT){
		}

		public Node(String name,int wait_time, int frames_send_per_tic, int frames_process_per_tic)
		{
			if (wait_time <= 0)	throw new ArgumentOutOfRangeException ("Attempting to create node "+name+" with wait_time <=0");
			if (frames_process_per_tic < 0)
				throw new ArgumentOutOfRangeException ("Attempting to create node " + name + " with negative frames to process per tic");
			if (frames_send_per_tic < 0)
				throw new ArgumentOutOfRangeException ("Attempting to create node " + name + " sending negative number of frames per tic");
			this.name = name;
			this.framesSentPerTic = frames_send_per_tic;
			this.framesProcessPerTic = frames_process_per_tic;
			this.wait = wait_time;
			this.q_in = new Queue<EtherFrame> ();
			this.q_out = new Queue<EtherFrame> ();
		}

		//vnejsi rozhrani - model
		//link pouziva Node.ReceiveFrame pro predani ramce
		public void ReceiveFrame(EtherFrame f,Model m){		
			//byl dorucen ramec,vlozime do fronty
			q_in.Enqueue (f);
			if ((nearest_receiving_scheduled < m.Cas) || (nearest_receiving_scheduled > m.Cas + 1)) {
				this.Naplanuj (m.K, Stav.RECEIVING, m.Cas + 1);
				nearest_receiving_scheduled = m.Cas + 1;
				scheduled++;
			}
			if ((scheduled == framesProcessPerTic)&&(!req_flood_stop)) {
				Console.WriteLine ("Node " + this.name + " stopped receiving frames due to flood at time " + m.Cas);
				req_flood_stop = true;
			}
			Console.WriteLine ("Node " + name + " received a frame at time " + m.Cas);
		}

		//vnitrni funkcionalita
		//potomek Node, bude v dusledku simulovat protokol
		protected void SendFrame(EtherFrame f,Model m){
			Console.WriteLine ("Node " + name + " prepared a frame for sending to " + f.Destination + " at time " + m.Cas);
			q_out.Enqueue (f);
		}

		//vnitrni funkcionalita
		//rozbali ramec, vynda packet, bude zpracovavat packet
		//vraci jak dlouho bude zpracovavat packet
		public abstract int ProcessFrame (EtherFrame f, Model m);

		//vnejsi rozhrani - simulace
		//TODO:
		//predelat stavy Node: processing a communicating
		//communicating: 
			//prijme a posle packety ve vstupni/vystupni fronte
				//vystupni fronta je soucast rozhrani a odeslani se provede pomoci Dispatch
				//vstupni fronta je soucast node a prijem se provede pomoci ReceiveFrame
				//protoze mi je jedno, ktery Link zpravu dorucil, ale neni mi povetsinou jedno, ktery Link zpravu posle
			//najednou
			//link se bude starat o to, kdy dorazi (predevsim half-duplex deleni komunikace)
		//processing: zpracovava vnitrni pracovni frontu
		public override void ZpracujUdalost(Stav u,Model m){
			Console.WriteLine ("Node " + this.name + " now " + u);
			int processed = 0;int kdy = -255;
			if (u == Stav.RECEIVING) {
				//prijem ramce - projdeme vstupni frontu a napiseme na vystup hlasku o zpracovani ramce a zavolame ProcessFrame
				kdy = m.Cas + 1;
				if (q_in.Count == 0)
					Console.WriteLine ("Node " + name + " didn't receive anything at time " + m.Cas);
				while ((q_in.Count>0)&&(processed<framesProcessPerTic)) {
					EtherFrame f = q_in.Dequeue ();
					Console.WriteLine ("Node " + name + " processed a frame at time " + m.Cas);
					kdy += ProcessFrame (f, m);//process frame vraci "casovou slozitost" zpracovani
				}
				//uz jsme schopni prijimat dalsi ramce
				if (req_flood_stop) {
					Console.WriteLine ("Node " + name + " now ready to receive frames");
					req_flood_stop = false;
				}
				//naplanovani
				if ((nearest_sending_scheduled < m.Cas) || (nearest_sending_scheduled > kdy)) {
					this.Naplanuj (m.K, Stav.SENDING, kdy);
					nearest_sending_scheduled = kdy;
				}
			} else if (u == Stav.SENDING) {
				//odeslani
				int t = 1;
				if (q_out.Count == 0)
					Console.WriteLine ("Node " + name + " has nothing to send at time " + m.Cas);
				else
					t = this.sending (m);
				//naplanovani
				kdy = m.Cas + t;
				if ((nearest_receiving_scheduled < m.Cas) || (nearest_receiving_scheduled > kdy)) {
					this.Naplanuj (m.K, Stav.RECEIVING, kdy);
					nearest_receiving_scheduled = kdy;
				}
			}
		}

		//vnejsi rozhrani - simulace / program
		public void Init(Model m){
			scheduled = 0;
			req_flood_stop = false;
			if (nearest_sending_scheduled != 0) {
				this.Naplanuj (m.K, Stav.SENDING, 0);
				nearest_sending_scheduled = 0;
			}
		}

		//vnejsi rozhrani - simulace / program
		public void Fin(){
			Console.WriteLine ("Node " + name + ": incoming queue has " + q_in.Count + " frames in, outcoming queue has " + q_out.Count + " frames in");
			q_in.Clear();
			q_out.Clear();
		}

		//vnejsi rozhrani - program
		public NodeProperties ExportProperties(){
			NodeProperties n = new NodeProperties(this.name);
			n.Wait = this.WaitTime;
			n.FPPT = this.FramesProcessPerTic;
			n.FSPT = this.FramesSentPerTic;
			return n;
		}

		//vnejsi rozhrani - program
		public void ImportProperties(NodeProperties n){
			this.name = n.Name;
			this.WaitTime = n.Wait;
			this.framesProcessPerTic = n.FPPT;
			this.framesSentPerTic = n.FSPT;
		}

		//vnitrni funkcionalita
		protected virtual int sending(Model m){
			this.net.Dispatch (q_out, this.name, m);
			return 1;
		}
	}

	public abstract class MultiportNode:Node{
		protected NetworkInterface[] interfaces;
		protected int ports, in_use;

		public MultiportNode (String name,int ports):this(name,ports,DEFAULT_WAIT_TIME,DEFAULT_FSPT,DEFAULT_FPPT){
		}

		public MultiportNode (String name,int ports,int wait_time, int frames_send_per_tic, int frames_process_per_tic):base(name,wait_time,frames_send_per_tic,frames_process_per_tic)
		{
			interfaces = new NetworkInterface[ports];
			this.ports = ports;
			for (int i=0; i<ports; i++) {
				interfaces [i] = new NetworkInterface ();
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
		protected abstract void sending (Model m);
	}

}