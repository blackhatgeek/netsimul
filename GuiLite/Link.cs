using System;
using System.Collections.Generic;

namespace GuiLite
{
	public class LinkException:Exception{
		public LinkException(String text):base(text){
		}
	}

	//Simplex: Link smeruje k konkretnimu uzlu
	public class Link:Proces
	{
		private readonly string NAME;
		private int OPT;//octets per tic
		private int carryOctets;//kolik prepravuji v dalsim kroku
		private double failureProb,errProb;
		private Queue<EtherFrame> queue,pool;
		private EtherFrame last;
		private Node destination;
		private int k;
		private bool k_lock;

		public Node Destination{
			set{ this.destination = value;}
			get{ return this.destination;}
		}

		public Link (String name,int octets_per_tic,double drop_probability,double damage_probability)
		{
			this.NAME = name;
			this.OPT = octets_per_tic;
			this.carryOctets = 0;
			this.queue = new Queue<EtherFrame> ();
			this.destination = null;
			this.last = null;
			this.k = 1;
			this.k_lock = false;
			if ((drop_probability >= 0.0) && (drop_probability <= 1.0))
				this.failureProb = drop_probability;
			else
				throw new ArgumentOutOfRangeException ("drop_probability must be between 0.0 and 1.0");
		if ((damage_probability >= 0.0) && (damage_probability <= 1.0))
			this.errProb = damage_probability;
		else
			throw new ArgumentOutOfRangeException ("damage_probability must be between 0.0 and 1.0");
		}
		public bool Volno{
			get{
				return (destination != null) && (carryOctets < OPT) && (!k_lock);
			}
		}
		public string Name{
			get{
				return this.NAME;
			}
		}

		//prijima data 
		//kdyz je volno rovnou do fronty
		//jinak do poolu
		public void Doprav(EtherFrame f){
			//mozne poskozeni ramce
			if (new Random ().NextDouble () <= errProb) {
				f.CRC = false;
				Console.WriteLine ("Ramec se cestou poskodil");
			}

			//mozne zahozeni
			if (new Random ().NextDouble () > failureProb) {
				if (this.Volno) {
					queue.Enqueue (f);
					last = f;
					carryOctets += f.Size;
				} else {
					//vezme do bufferu a az bude volno prepise do spravne fronty
					pool.Enqueue (f);
				}
			} else {
				Console.WriteLine ("Zahazuji ramec");
			}
		}

		//pokud je pripojen destination, doplnim queue z poolu az do OPT
		private void pool2Queue(){
			if (destination!=null)
				while ((carryOctets<OPT)&&(pool.Count>0)) {
					EtherFrame f = pool.Dequeue ();
					queue.Enqueue (f);
					last = f;
					carryOctets += f.Size;
				}
		}

		public override void ZpracujUdalost (Stav u, Model m)
		{
			if (u.Equals (Stav.SENDING)) {
				if (carryOctets <= OPT) {
					this.deliver (queue.Count, m);
					pool2Queue ();//pokud se dostaly packety do poolu, premistime do fronty
					this.Naplanuj (m.K, Stav.SENDING, m.Cas + 1);
				} else {
					//linka se zavre, kdyz je plna
					//posledni ramec muze byt "precuhujici"
					//dorucim vsechno-1 (to muze byt 0 ramcu, pokud je tam jeden tlusty)
					int a = carryOctets;
					this.deliver (queue.Count - 1, m);
					//a - carryOctets ... kolik jsem ted poslal
					int d = OPT - (a - carryOctets);//kolik jeste muzu poslat v tomto kroku
					carryOctets -= d;//v tomto kroku poslu, co muzu ... pozdeji snizim, velikost ramce
					//spocitam kolik kol budu dorucovat posledni ramec
					k = (int)Math.Floor ((decimal)(carryOctets / OPT));//kolik casu "trva doruceni"
					Console.WriteLine ("Cekam dalsich " + k + " kroku");
					k_lock = true;
					//snizim ramec o d, ktere jsem "poslal ted" a k*OPT
					last.Size -= (d + k * OPT);
					//ramec ma zbytkovou velikost naplanuji se do stavu sending v case m.Cas+k+1
					this.Naplanuj (m.K, Stav.UNLOCK, m.Cas + k);
				}
			} else if (u.Equals (Stav.UNLOCK)) {
				k_lock = false;
				pool2Queue ();//pokud se mezi tim dostaly packety do poolu, premistime do fronty
				this.Naplanuj (m.K, Stav.SENDING, m.Cas + 1);
			}
		}

		//doruci sadu ramcu
		private void deliver(int frames,Model m){
			if (frames <= queue.Count) {
				Console.WriteLine ("Dorucuji " + frames + " ramcu");
				for (int i=0; i<frames; i++) {
					EtherFrame ef = queue.Dequeue();
					destination.ReceiveFrame (ef, m);
					carryOctets -= ef.Size;
				}
				this.Naplanuj (m.K, Stav.SENDING, m.Cas + 1);
			}
		}
	}
}