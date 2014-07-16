using System;
using System.Collections.Generic;

namespace GuiLite
{
	public class NetworkInterface
	{
		private Link link;
		private MACaddr mac;
		private Queue<EtherFrame> out_q;

		public Link Linka{
			set{this.link=value;}
			get{ return this.link;}
		}

		public MACaddr MAC{
			set {this.mac=value;}
			get { return this.mac;}
		}

		public void GenerateMac(){
			MACaddrFactory f = MACaddrFactory.Instance;
			this.mac = f.GetMAC ();
		}

		public bool InUse{
			get{
				return link != null;
			}
		}

		public Queue<EtherFrame> Out_q{
			get{
				return out_q;
			}
		}

		public NetworkInterface ()
		{
			mac = MACaddrFactory.Instance.GetMAC ();
		}

		public NetworkInterface (MACaddr a){
			mac = a;
		}

		public void Dispatch(Queue<EtherFrame> q_out,String name,Model m){
			//prapuvodni komentar:
			//odeslani packetu - projdeme vystupni frontu a napiseme na vystup hlasku o zpracovani ramce
			//pokud ready=true, jinak cekame az prijemce bude pripraven a zatim ukladame ramce do vystupni fronty
			while (this.link.Volno&&(this.out_q.Count>0)) {
				EtherFrame f = q_out.Dequeue ();
				Console.WriteLine ("Node " + name + " sent a frame to " + f.Destination + " at time " + m.Cas);
				try{
					this.link.Doprav (f);
				}catch(LinkException e){
					Console.WriteLine ("Oops! link trouble:" + e.Message);
				}
			}
			if (!this.link.Volno)
				Console.WriteLine ("Link busy");
		}
	}
}

