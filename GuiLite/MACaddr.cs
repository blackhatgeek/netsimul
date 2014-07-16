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

	public sealed class MACaddrFactory
	{
		//cil je zajistit unicitu MAC adres
		private int n;
		private MACaddrFactory(){
			n=0;
			Console.WriteLine (n);
		}
		private static readonly MACaddrFactory instance=new MACaddrFactory();
		public static MACaddrFactory Instance{
			get{
				return instance;
			}
		}

		public MACaddr GetMAC(){
			Console.WriteLine("Creating MAC "+n);
			MACaddr gen = new MACaddr(n);
			Console.WriteLine ("Created " + gen);
			n++;
			return gen;
		}
	}
}

