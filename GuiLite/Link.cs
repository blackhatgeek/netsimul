using System;

namespace GuiLite
{
	public class LinkException:Exception{
		public LinkException(String text):base(text){
		}
	}

	public class Link:Proces
	{
		private int OPT;//octets per tic
		private int carry;//kolik prepravuji v dalsim kroku
		private EtherFrame[] stack; 
		private Node a, b;

		public void setA(Node a){
			this.a = a;
		}

		public void setB(Node b){
			this.b = b;
		}

		public Link (int octets_per_tic,Node a,Node b)
		{
			this.OPT = octets_per_tic;
			this.carry = 0;
			this.stack = new EtherFrame[OPT];
			this.a = a;
			this.b = b;
		}

		public bool Volno{
			get{
				return carry < OPT;
			}
		}
		public void Doprav(EtherFrame f){
			if (this.Volno) {
				this.stack [carry] = f;
				carry++;
			} else
				throw new LinkException ("Plna linka");
		}

		//planovat vzdy stav sending
		public override void ZpracujUdalost (Stav u, Model m)
		{
			foreach (EtherFrame f in stack) {
				if (f.Destination.Equals (a.MAC)) {
					//a.ReceiveFrame (f);
				} else if (f.Destination.Equals (b.MAC)) {
					//b.ReceiveFrame(f);
				}
			}
		}
	}
}

