using System;

namespace GuiLite
{
	public class Packet:Proces
	{
		private Stav stav;
		public Stav State{
			get { return this.stav; }
			set { this.stav = value; }
		}
		public Packet ()
		{
			this.stav = Stav.WAITING_FOR_SEND;
		}
	}
}

