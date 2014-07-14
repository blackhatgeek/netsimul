using System;
using System.Collections.Generic;

namespace GuiLite
{
	public class LinkException:Exception{
		public LinkException(String text):base(text){
		}
	}

	public class Link:Proces
	{
		private int OPT;//octets per tic
		private int carryOctets;//kolik prepravuji v dalsim kroku
		private double failureProb;
		private Queue<EtherFrame> queue;
		private EtherFrame last;
		private Node a, b;
		private int k;
		private bool k_lock;

		public Node A{
			set{ this.a = value;}
			get{ return this.a;}
		}

		public Node B{
			set{ this.b = value;}
			get{return this.b;}
		}

		public Link (int octets_per_tic,double drop_probability)
		{
			this.OPT = octets_per_tic;
			this.carryOctets = 0;
			this.queue = new Queue<EtherFrame> ();
			this.a = null;
			this.b = null;
			this.last = null;
			this.k = 1;
			this.k_lock = false;
			if ((drop_probability >= 0.0) && (drop_probability <= 1.0))
				this.failureProb = drop_probability;
			else
				throw new ArgumentOutOfRangeException ("drop_probability must be between 0.0 and 1.0");
		}
		public bool Volno{
			get{
				return (a!=null)&&(b!=null)&&(carryOctets < OPT)&&(!k_lock);
			}
		}

		//prijima data kdyz je volno
		public void Doprav(EtherFrame f){
			if (this.Volno) {//mj. carryOctets<OPT - lze vkladat
				if (new Random().NextDouble () > failureProb) {//pravdepodobnost zahozeni
					queue.Enqueue (f);
					last=f;
					carryOctets += f.Size;
				} else {
					Console.WriteLine ("Zahazuji ramec");
				}
			} else
				throw new LinkException ("Plna linka");
		}

		public override void ZpracujUdalost (Stav u, Model m)
		{
			if (u.Equals (Stav.SENDING)) {
				if (carryOctets <= OPT) {
					this.deliver (queue.Count, m);
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
					EtherFrame ef = queue.Dequeue ();
					ef.Size -= (d + k * OPT);
					queue.Enqueue (ef);
					//ramec ma zbytkovou velikost naplanuji se do stavu sending v case m.Cas+k+1
					this.Naplanuj (m.K, Stav.UNLOCK, m.Cas + k);
				}
			} else if (u.Equals (Stav.UNLOCK)) {
				k_lock = false;
				this.Naplanuj (m.K, Stav.SENDING, m.Cas + 1);
			}
		}

		//doruci sadu ramcu
		private void deliver(int frames,Model m){
			if (frames <= queue.Count) {
				Console.WriteLine ("Dorucuji " + frames + " ramcu");
				for (int i=0; i<frames; i++) {
					EtherFrame ef = queue.Dequeue();
					if (ef.Destination.Equals (a.MAC)) {
						a.ReceiveFrame (ef, m);
					} else if (last.Destination.Equals (b.MAC)) {
						b.ReceiveFrame (ef, m);
					}
					carryOctets -= ef.Size;
				}
				this.Naplanuj (m.K, Stav.SENDING, m.Cas + 1);
			}
		}
	}
}