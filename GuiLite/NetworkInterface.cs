using System;
using System.Collections.Generic;

namespace GuiLite
{
	public class InterfaceException:NullReferenceException{
		public InterfaceException(String name):base(name){
		}
	}
	public class NetworkInterface
	{
		private Link link;
		private MACaddr mac;
		private Queue<EtherFrame> out_q;
		private String name;

		public NetworkInterface(String name,MACaddr mac){
			this.out_q = new Queue<EtherFrame> ();
			this.name = name;
			this.mac = mac;
		}

		public NetworkInterface(String name):this(name,MACaddr.Factory.Instance.GetMAC()){//!! MACException
		}

		public NetworkInterface():this(null){//!! MACException
			this.name = this.mac.ToString ();
		}

		public Link Linka{
			set{this.link=value;}
			get{ return this.link;}
		}

		public MACaddr MAC{
			set {this.mac=value;}
			get { return this.mac;}
		}

		public bool InUse{
			get{
				return link != null;
			}
		}

		public void Dispatch(EtherFrame f,Model m){
			Console.WriteLine ("Network interface " + name + " sent a frame to " + f.Destination + " at time " + m.Cas);
			if (link != null)
				this.link.Doprav (f);
			else
				throw new InterfaceException ("Link not connected");
		}
	}
}

