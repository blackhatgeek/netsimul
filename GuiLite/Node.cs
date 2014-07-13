using System;
using System.Collections.Generic;

namespace GuiLite
{
	public class Node:Proces
	{
		private const int DEFAULT_WAIT_TIME=3;
		private const int DEFAULT_FSPT=4;
		private const int DEFAULT_FPPT=10;
		private const int SECURITY_BUFFER=10;

		private int framesSentPerTic, framesProcessPerTic;
		private Node linkedTo;
		private String name;
		private Queue<Frame> q_in,q_out;
		private Dictionary<int,Frame> sent_frames_not_confirmed;//fid,frame
		private Dictionary<int,int> sent_frames_time;//fid,m.Cas
		private int fid;//frame id;
		private int wait;//kolik kol se ceka na potvrzeni prijeti ramce
		private int scheduled;//kolik packetu se bude zpracovavat v pristim kole
		private bool ready;//soused
		private bool req_flood_stop;
		private int nearest_sending_scheduled=-1;
		private int nearest_receiving_scheduled=-1;

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

		public Node LinkedTo
		{
			get { return this.linkedTo; }
			set { this.linkedTo = value; }
		}

		public String Name{
			get { return this.name; }
		}

		public Node(String name): this (name, DEFAULT_WAIT_TIME,DEFAULT_FSPT,DEFAULT_FPPT){
		}

		public Node(String name,int wait_time, int frames_send_per_tic, int frames_process_per_tic)
		{
			if (wait_time <= 0)	throw new ArgumentException ("Attempting to create node "+name+" with wait_time <=0");
			if (frames_process_per_tic < 0)
				throw new ArgumentException ("Attempting to create node " + name + " with negative frames to process per tic");
			this.name = name;
			this.framesSentPerTic = frames_send_per_tic;
			this.framesProcessPerTic = frames_process_per_tic;
			this.wait = wait_time;
			ready = false;
			this.q_in = new Queue<Frame> ();
			this.q_out = new Queue<Frame> ();
			this.sent_frames_not_confirmed = new Dictionary<int, Frame> ();
			this.sent_frames_time = new Dictionary<int, int> ();
		}

		public void ReceiveFrame(Frame f,Model m){		
			//byl dorucen ramec,vlozime do fronty
				//pokud neslo o servisni ramec
				//confirmation - vyhodime ze seznamu, pokud tam nebyl, nereagujeme
				//na stop_sending/ready treba reagovat upravou parametru ready
			if (f is ServiceFrame) {
				ServiceFrame sf = f as ServiceFrame;
				switch (sf.type) {
				case ServiceFrame.Type.STOP_SENDING:
					this.ready = false;
					Console.WriteLine ("Node " + this.name + " received request to stop sending packets from " + linkedTo.Name + " at time " + m.Cas);
					break;
				case ServiceFrame.Type.READY:
					this.ready = true;
					Console.WriteLine ("Node " + this.Name + " received information " + linkedTo.name + " is now ready to receive packets at time " + m.Cas);
					break;
				case ServiceFrame.Type.CONFIRMATION:
					if (sent_frames_not_confirmed.ContainsKey (sf.ID)) {
						sent_frames_not_confirmed.Remove (sf.ID);
						sent_frames_time.Remove (sf.ID);
					}
					Console.WriteLine ("Node " + this.name + " received confirmation " + linkedTo.Name + " received packet #" + sf.ID + " (Time: " + m.Cas + ")");
					break;
				}
			} else {
				q_in.Enqueue (f);
				if ((nearest_receiving_scheduled < m.Cas) || (nearest_receiving_scheduled > m.Cas + 1)) {
					this.Naplanuj (m.K, Stav.RECEIVING, m.Cas + 1);
					nearest_receiving_scheduled = m.Cas + 1;
					scheduled++;
				}
				if ((scheduled == framesProcessPerTic - SECURITY_BUFFER)&&(!req_flood_stop)) {
					q_out.Enqueue (new ServiceFrame (f.ID, ServiceFrame.Type.STOP_SENDING));
					Console.WriteLine ("Node " + this.name + " prepared request to " + linkedTo.name + " stop sending packets at time " + m.Cas);
					req_flood_stop = true;
				}
				Console.WriteLine ("Node " + name + " received a frame at time " + m.Cas);
			}
		}

		public override void ZpracujUdalost(Stav u,Model m){
			Console.WriteLine ("Node " + this.name + " now " + u);
			int processed = 0;
			if (u == Stav.RECEIVING) {
				//prijem ramce - projdeme vstupni frontu a napiseme na vystup hlasku o zpracovani ramce
				//pro kazdy ramec pripravime Confirmation
				while ((q_in.Count>0)&&(processed<framesProcessPerTic)) {
					Frame f=q_in.Dequeue ();
					q_out.Enqueue(new ServiceFrame (f.ID,ServiceFrame.Type.CONFIRMATION));
					Console.WriteLine ("Node " + name + " processed a frame at time " + m.Cas+" and prepared confirmation frame #"+f.ID);
				}
				//naplanovani
				int kdy = m.Cas + 1;//zatim neresime cas nutny na zpracovani packetu
				if ((nearest_sending_scheduled < m.Cas) || (nearest_sending_scheduled > kdy)) {
					this.Naplanuj (m.K, Stav.SENDING, kdy);
					nearest_sending_scheduled = kdy;
				}
			} else if (u == Stav.SENDING) {
				//odeslani packetu - projdeme vystupni frontu a napiseme na vystup hlasku o zpracovani ramce
				//pokud ready=true, jinak cekame az prijemce bude pripraven a zatim ukladame ramce do vystupni fronty
				Console.WriteLine ("Node " + linkedTo.Name + " ready " + ready);
				if (ready) {
					int j = 0;
					while ((j<framesSentPerTic)&&(q_out.Count>0)) {
						Frame f = q_out.Dequeue ();
						if (f is ServiceFrame) {
							Console.WriteLine ("Node " + name + " sent a service frame #" + f.ID + " to " + linkedTo.Name + " at time " + m.Cas);
							linkedTo.ReceiveFrame (f, m);
						} else {
							if (!sent_frames_not_confirmed.ContainsKey (f.ID)) {
								sent_frames_not_confirmed.Add (f.ID, f);
								sent_frames_time.Add (f.ID, m.Cas);
							}
							Console.WriteLine ("Node " + name + " sent a frame #" + f.ID + " to " + linkedTo.Name + " at time " + m.Cas);
							linkedTo.ReceiveFrame (f, m);
						}
					}
					//pozdeji bude jen while cyklus a for cyklus bude jinde a metoda SendFrame(Frame,Model) bude naplnovat frontu podle algoritmu pro dany node
					//v modelu, kdy se v kazdem kroku pouze posle urcity pocet packetu se v pripade, ze node neni dostupny jen zdrzi odesilani
					for (int i=j; i<framesSentPerTic; i++) {
						Frame f = new Frame (fid);
						sent_frames_not_confirmed.Add (fid, f);
						sent_frames_time.Add (fid, m.Cas);
						fid++;
						Console.WriteLine ("Node " + name + " sent a frame #" + fid + " to " + linkedTo.Name + " at time " + m.Cas);
						linkedTo.ReceiveFrame (f, m);
					}
				} else {
					for(int i=0;i<framesSentPerTic;i++){
						q_out.Enqueue (new Frame (fid));
						fid++;
					}
				}
				//prokud nebylo potvrzeno prijeti ramce, poslat znova
				//chci vsechny ramce z sent_frames_time s time=m.Cas-wait-1, ktere jsou not_confirmed
				Queue<int> frames_to_remove = new Queue<int> ();
				foreach (KeyValuePair<int,Frame> kvp in sent_frames_not_confirmed) {
					int t,T=m.Cas-wait-1;
					if(sent_frames_time.TryGetValue(kvp.Key,out t)){
						if (t == T) {
							Console.WriteLine ("Frame " + kvp.Key + " was not confirmed since time " + t + ", scheduling resend in next round");
							q_out.Enqueue (kvp.Value);
							sent_frames_time.Remove (kvp.Key);
							//int k = kvp.Key;
							//sent_frames_not_confirmed.Remove (k);
							//.... out of sync
							frames_to_remove.Enqueue (kvp.Key);
						}
					}
				}
				while (frames_to_remove.Count>0)
					sent_frames_not_confirmed.Remove (frames_to_remove.Dequeue ());

				//naplanovani
				int kdy = m.Cas + 1;
				if ((nearest_receiving_scheduled < m.Cas) || (nearest_receiving_scheduled > kdy)) {
					this.Naplanuj (m.K, Stav.RECEIVING, kdy);
					nearest_receiving_scheduled = kdy;
				}
			}
		}

		public void Init(Model m){
			fid = 1;
			scheduled = 0;
			req_flood_stop = false;
			if (framesProcessPerTic == 0) {
				q_out.Enqueue (new ServiceFrame (0, ServiceFrame.Type.STOP_SENDING));
				Console.WriteLine ("Node " + name + " asked " + linkedTo.Name + " to stop sending packets at time " + m.Cas);
			} else {
				q_out.Enqueue (new ServiceFrame (0, ServiceFrame.Type.READY));
				Console.WriteLine ("Node " + name + " will inform " + linkedTo.Name + " it's ready at time " + m.Cas);
			}
			while (q_out.Count>0) {
				Frame f = q_out.Dequeue ();
				Console.WriteLine ("Node " + name + " sent a frame #" + fid + " to " + linkedTo.Name + " at time " + m.Cas);
				linkedTo.ReceiveFrame (f, m);
			}
			if (nearest_sending_scheduled != 0) {
				this.Naplanuj (m.K, Stav.SENDING, 0);
				nearest_sending_scheduled = 0;
			}
		}

		public void Fin(){
			Console.WriteLine ("Node " + name + ": incoming queue has " + q_in.Count + " frames in, outcoming queue has " + q_out.Count + " frames in");
			ready = false;
			q_in.Clear();
			q_out.Clear();
			sent_frames_time.Clear();
			sent_frames_not_confirmed.Clear ();

		}

		public Node Clone(){
			Node n = new Node (this.name);
			n.WaitTime = this.WaitTime;
			n.FramesProcessPerTic = this.FramesProcessPerTic;
			n.FramesSentPerTic = this.FramesSentPerTic;
			return n;
		}
	}
}

