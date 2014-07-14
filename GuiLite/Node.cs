using System;
using System.Collections.Generic;

namespace GuiLite
{
	public abstract class Node:Proces
	{
		private const int DEFAULT_WAIT_TIME=3;
		private const int DEFAULT_FSPT=4;
		private const int DEFAULT_FPPT=10;

		private int framesSentPerTic, framesProcessPerTic;
		private Link link;
		private String name;
		private Queue<EtherFrame> q_in,q_out;
		private int wait;//kolik kol se ceka na potvrzeni prijeti ramce
		private int scheduled;//kolik packetu se bude zpracovavat v pristim kole
		private bool req_flood_stop;
		private int nearest_sending_scheduled=-1;
		private int nearest_receiving_scheduled=-1;
		private MACaddr mac;

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
			get { return this.link; }
			set { this.link = value; }
		}

		public String Name{
			get { return this.name; }
		}

		public MACaddr MAC{
			get{ return this.mac;}
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
			this.mac = MACaddrFactory.GetInstance ().GetMAC ();
			this.name = name;
			this.framesSentPerTic = frames_send_per_tic;
			this.framesProcessPerTic = frames_process_per_tic;
			this.wait = wait_time;
			this.q_in = new Queue<EtherFrame> ();
			this.q_out = new Queue<EtherFrame> ();
		}

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

		public void SendFrame(EtherFrame f,Model m){
			Console.WriteLine ("Node " + name + " prepared a frame for sending to " + f.Destination + " at time " + m.Cas);
			q_out.Enqueue (f);
		}

		//rozbali ramec, vynda packet, bude zpracovavat packet
		public abstract int ProcessFrame (EtherFrame f, Model m);

		public override void ZpracujUdalost(Stav u,Model m){
			Console.WriteLine ("Node " + this.name + " now " + u);
			int processed = 0;int kdy = -255;
			if (u == Stav.RECEIVING) {
				//prijem ramce - projdeme vstupni frontu a napiseme na vystup hlasku o zpracovani ramce
				//pro kazdy ramec pripravime Confirmation
				kdy = m.Cas + 1;
				if (q_in.Count == 0)
					Console.WriteLine ("Node " + name + " didn't receive anything at time " + m.Cas);
				while ((q_in.Count>0)&&(processed<framesProcessPerTic)) {
					EtherFrame f = q_in.Dequeue ();
					Console.WriteLine ("Node " + name + " processed a frame at time " + m.Cas);
					kdy += ProcessFrame (f, m);//process frame vraci casovou slozitost zpracovani
				}
				//uz jsme schopni prijimat dalsi ramce, sdelit sousedovi
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
				//odeslani packetu - projdeme vystupni frontu a napiseme na vystup hlasku o zpracovani ramce
				//pokud ready=true, jinak cekame az prijemce bude pripraven a zatim ukladame ramce do vystupni fronty
				if (link.Volno) {
					int j = 0;
					if (q_out.Count == 0)
						Console.WriteLine ("Node " + name + " has nothing to send at time " + m.Cas);
					while ((j<framesSentPerTic)&&(q_out.Count>0)) {
						EtherFrame f = q_out.Dequeue ();
						Console.WriteLine ("Node " + name + " sent a frame to " + f.Destination + " at time " + m.Cas);
						link.Doprav (f);
					}
				}
				//naplanovani
				kdy = m.Cas + 1;
				if ((nearest_receiving_scheduled < m.Cas) || (nearest_receiving_scheduled > kdy)) {
					this.Naplanuj (m.K, Stav.RECEIVING, kdy);
					nearest_receiving_scheduled = kdy;
				}
			}
		}

		public void Init(Model m){
			scheduled = 0;
			req_flood_stop = false;
			if (nearest_sending_scheduled != 0) {
				this.Naplanuj (m.K, Stav.SENDING, 0);
				nearest_sending_scheduled = 0;
			}
		}

		public void Fin(){
			Console.WriteLine ("Node " + name + ": incoming queue has " + q_in.Count + " frames in, outcoming queue has " + q_out.Count + " frames in");
			q_in.Clear();
			q_out.Clear();
		}

		public NodeProperties ExportProperties(){
			NodeProperties n = new NodeProperties(this.name);
			n.Wait = this.WaitTime;
			n.FPPT = this.FramesProcessPerTic;
			n.FSPT = this.FramesSentPerTic;
			return n;
		}

		public void ImportProperties(NodeProperties n){
			this.name = n.Name;
			this.WaitTime = n.Wait;
			this.framesProcessPerTic = n.FPPT;
			this.framesSentPerTic = n.FSPT;
		}
	}
}