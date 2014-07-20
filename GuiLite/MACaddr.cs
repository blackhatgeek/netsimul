using System;

namespace GuiLite
{
	public class MACException:Exception{
		public MACException(String s):base(s){
		}
	}

	public class MACaddr//:IEquatable<MACaddr>
	{
		//Equatable nepotrebne kvuli factory .. nelze vytvorit dve instance se stejnou adresou
		private int ADDR;

		public int MAC{
			get{ return this.ADDR;}
		}

		private MACaddr(int addr){
			this.ADDR = addr;
		}
		public override string ToString ()
		{
			return string.Format ("[MACaddr]"+ADDR);
		}

		public sealed class Factory
		{
			private const int UPPER_BOUND=10;//pocet MAC adres k dispozici

			//cil je zajistit unicitu MAC adres
			private int n;
			private Factory(){
				n=0;
				Console.WriteLine (n);
			}
			private static readonly Factory instance=new Factory();
			public static Factory Instance{
				get{
					return instance;
				}
			}

			public MACaddr GetMAC(){
				if ((n>=0) && (n <= UPPER_BOUND)) {
					Console.WriteLine ("Creating MAC " + n);
					MACaddr gen = new MACaddr (n);
					Console.WriteLine ("Created " + gen);
					n++;
					return gen;
				} else
					throw new MACException ("No more MAC address left");
			}
		}
	}
}

