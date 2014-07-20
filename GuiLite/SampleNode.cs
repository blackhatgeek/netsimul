using System;

namespace GuiLite
{
	public class SampleNode:Node
	{
		private const int N=5;
		private const int M=7;

		private int respond_to_each_n;
		private int send_each_m_tics;
		private int requests=0;

		private SampleNode buddy;

		public SampleNode Buddy{
			set{this.buddy=value;}
			get{ return this.buddy;}
		}
		public SampleNode (String name):base(name)
		{
			this.send_each_m_tics = M;
			this.respond_to_each_n = N;
			this.buddy = null;
		}

		public SampleNode(String name,int frames_process_per_tic,int respond_to_each_n,int send_each_m_tics):
			base(name,frames_process_per_tic){
			this.respond_to_each_n = respond_to_each_n;
			this.send_each_m_tics = send_each_m_tics;
			this.buddy = null;
		}

		public override int ProcessFrame (EtherFrame f, Model m)
		{
			if (requests % respond_to_each_n == 0) {
				this.SendFrame(new EtherFrame(this.MAC,f.Source,null,1),m);
				return 1;
			} else
				return 0;
		}

		public override void ZpracujUdalost (Stav u, Model m)
		{
			requests++;
			if(buddy!=null)
			if (requests % send_each_m_tics == 0) {
				this.SendFrame (new EtherFrame (this.MAC, buddy.MAC, null, 1),m);
				this.Naplanuj (m.K, Stav.SENDING, m.Cas + send_each_m_tics);
			}
			base.ZpracujUdalost (u, m);
		}
	}
}