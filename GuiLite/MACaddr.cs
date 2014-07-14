using System;

namespace GuiLite
{
	public class MACaddr:IEquatable<MACaddr>
	{
		private int ADDR;

		public int MAC{
			get{ return this.ADDR;}
		}

		public MACaddr(int addr){
			this.ADDR = addr;
		}
		public override string ToString ()
		{
			return string.Format ("[MACaddr]"+ADDR);
		}

		public bool Equals(MACaddr adr){
			return adr.MAC == this.ADDR;
		}
	}

	public class MACaddrFactory
	{
		//cil je zajistit unicitu MAC adres
		private int n;
		private MACaddrFactory(){
			n=0;
		}
		private static MACaddrFactory factory = new MACaddrFactory();
		public static MACaddrFactory GetInstance(){
			return factory;
		}

		public MACaddr GetMAC(){
			MACaddr gen = new MACaddr(n);
			n++;
			return gen;
		}
	}
}

