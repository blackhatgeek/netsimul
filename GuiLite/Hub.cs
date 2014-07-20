using System;
using System.Collections.Generic;

namespace GuiLite
{
	public class Hub:MultiportNode
	{
		//Node.ReceiveFrame -> q_in -> Node.ZpracujUdalost -> ProcessFrame -> SendFrame -> q_out -> ZpracujUdalost -> Node.ZpracujUdalost -> sending -> out_q -> link

		public Hub (String name,int ports):base(name,ports)
		{
		}

		public Hub (String name,int ports, int frames_process_per_tic):base(name,ports,frames_process_per_tic)
		{
		}

		public override int ProcessFrame (EtherFrame f, Model m)
		{
			SendFrame (f,m);
			return 1;
		}

		public override void ZpracujUdalost (Stav u, Model m)
		{
			base.ZpracujUdalost (u, m);
		}

		protected override void multiport_sending (EtherFrame f,Model m)
		{
			int processed = 0;
			foreach (NetworkInterface ni in interfaces) {
				ni.Dispatch (f, m);
			}
			processed++;
		}
	}
}

