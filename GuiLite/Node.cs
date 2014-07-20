using System;
using System.Collections.Generic;

namespace GuiLite
{
	public abstract class Node:Proces
	{
		protected const int DEFAULT_FPPT=10;

		private int framesProcessPerTic;
		protected String name;
		private Queue<EtherFrame> q_in;
		protected Queue<EtherFrame> q_out;
		private int nearest_sending_scheduled=-1;
		private int nearest_receiving_scheduled=-1;
		private NetworkInterface net;
		private bool send_lock;

		public int FramesProcessPerTic
		{
			get{ return this.framesProcessPerTic;}
			set{ 
				if (value >= 0)
					this.framesProcessPerTic = value;
				else
					Console.WriteLine ("Attempt to change FPPT to negative value! Not changed");
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

		public Node(String name): this (name,DEFAULT_FPPT){
		}

		public Node(String name,int frames_process_per_tic)
		{
			if (frames_process_per_tic < 0)
				throw new ArgumentOutOfRangeException ("Attempting to create node " + name + " with negative frames to process per tic");
			this.name = name;
			this.framesProcessPerTic = frames_process_per_tic;
			this.q_in = new Queue<EtherFrame> ();
			this.q_out = new Queue<EtherFrame> ();
		}

		//vnejsi rozhrani - model
		//link pouziva Node.ReceiveFrame pro predani ramce
		public void ReceiveFrame(EtherFrame f,Model m){		
			//byl dorucen ramec,vlozime do fronty
			q_in.Enqueue (f);
			//PROBLEM, pokud bezi delsi zpracovani a prijde ramec (aby se nezkratila doba zpracovani predchoziho)- zpracuj udalost stavi lock a zpracuji az bude odemceno
			if (!send_lock&&((nearest_receiving_scheduled < m.Cas) || (nearest_receiving_scheduled > m.Cas + 1))) {
				this.Naplanuj (m.K, Stav.PROCESSING, m.Cas + 1);//v dalsim kroku urcite prectu
				nearest_receiving_scheduled = m.Cas + 1;
			}
			Console.WriteLine ("Node " + name + " received a frame at time " + m.Cas);
		}

		//vnitrni funkcionalita
		//potomek Node, bude v dusledku simulovat protokol
		//zde ma preddefinovany zpusob, jak odeslat ramec
		protected void SendFrame(EtherFrame f,Model m){
			Console.WriteLine ("Node " + name + " prepared a frame for sending to " + f.Destination + " at time " + m.Cas);
			q_out.Enqueue (f);
			//NSS=Cas ... za chvili zpracuji
			//NSS=Cas+1...zpracuji v pristim kroku (to bych nastavoval)
			//jinak .. naplanuj NSS=Cas+1
			/*if ((nearest_sending_scheduled < m.Cas) || (nearest_sending_scheduled > m.Cas + 1)) {
				this.Naplanuj (m.K, Stav.SENDING, m.Cas + 1);
				nearest_sending_scheduled = m.Cas + 1;
			}*/
			//send_frame je volano process_frame, ktere je volano ZpracujUdalost, ktere vhodne naplanuje
		}

		//vnitrni funkcionalita
		//rozbali ramec, vynda packet, bude zpracovavat packet
		//vraci jak dlouho bude zpracovavat packet
		public abstract int ProcessFrame (EtherFrame f, Model m);

		//vnejsi rozhrani - simulace
		public override void ZpracujUdalost(Stav u,Model m){
			Console.WriteLine ("Node " + this.name + " now " + u);
			int processed = 0;	int kdy = -255;
			if (u == Stav.PROCESSING) {
				//zpracovani ramce - projdeme vstupni frontu a napiseme na vystup hlasku o zpracovani ramce a zavolame ProcessFrame
				kdy = m.Cas + 1;
				if (q_in.Count == 0)
					Console.WriteLine ("Node " + name + " has no frames to process at time " + m.Cas);
				while ((q_in.Count>0)&&(processed<framesProcessPerTic)) { //FPPT=1 1.krok processed=0 -> processed=1 cyklus se znova nespusti
					EtherFrame f = q_in.Dequeue ();
					Console.WriteLine ("Node " + name + " processed a frame at time " + m.Cas);
					kdy += ProcessFrame (f, m);//process frame vraci "casovou slozitost" zpracovani
					processed++;
					//Console.WriteLine (kdy);
				}

				//naplanovani
				if (nearest_sending_scheduled<kdy) {
					send_lock = true;
					this.Naplanuj (m.K, Stav.SENDING, kdy);
					nearest_sending_scheduled = kdy;
				}
			} else if (u == Stav.SENDING) {
				send_lock = false;
				//odeslani
				int t = 1;
				if (q_out.Count == 0)
					Console.WriteLine ("Node " + name + " has nothing to send at time " + m.Cas);
				else
					t = this.sending (m);
				//naplanovani
				kdy = m.Cas + t;
				if ((nearest_receiving_scheduled < kdy) || (nearest_receiving_scheduled > kdy)) {
					this.Naplanuj (m.K, Stav.PROCESSING, kdy);
					nearest_receiving_scheduled = kdy;
				}
			}
		}

		//vnejsi rozhrani - simulace / program
		public void Init(Model m){
			//scheduled = 0;
			//req_flood_stop = false;
			send_lock = false;
			//Console.WriteLine (name+": aloha, NSS: "+nearest_sending_scheduled+", NRS: "+nearest_receiving_scheduled);
			if (nearest_sending_scheduled != 0) {
				//Console.WriteLine ("!!@$");
				this.Naplanuj (m.K, Stav.SENDING, 0);
				nearest_sending_scheduled = 0;
			}
			if (nearest_receiving_scheduled != 0) {
				this.Naplanuj (m.K, Stav.PROCESSING, 0);
				nearest_receiving_scheduled = 0;
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
			n.FPPT = this.FramesProcessPerTic;
			return n;
		}

		//vnejsi rozhrani - program
		public void ImportProperties(NodeProperties n){
			this.name = n.Name;
			this.framesProcessPerTic = n.FPPT;
		}

		//vnitrni funkcionalita
		//pokud ma Node vice rozhrani, tato metoda se stara o jejich rozdeleni mezi jednotliva rozhrani
		protected virtual int sending(Model m){
			int t = 0;
			foreach (EtherFrame ef in q_out){
				this.net.Dispatch (ef, m);
				t++;
			}
			return t!=0?t:1;
		}
	}

	public abstract class MultiportNode:Node{
		protected NetworkInterface[] interfaces;
		protected int ports, in_use;

		public MultiportNode (String name,int ports):this(name,ports,DEFAULT_FPPT){
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
		protected override int sending (Model m){
			return this.multiport_sending (m);
		}

		protected abstract int multiport_sending (Model m);
	}

}