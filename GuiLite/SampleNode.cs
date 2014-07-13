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

		public SampleNode (String name):base(name)
		{
			this.send_each_m_tics = M;
			this.respond_to_each_n = N;
		}

		public SampleNode(String name,int wait_time,int frames_per_tic,int frames_process_per_tic,int respond_to_each_n,int send_each_m_tics):
			base(name,wait_time,frames_per_tic,frames_process_per_tic){
			this.respond_to_each_n = respond_to_each_n;
			this.send_each_m_tics = send_each_m_tics;
		}

		public override int ProcessFrame (Frame f, Model m)
		{
			if (requests % N == 0) {
				this.SendFrame (new Frame ((f.ID * N)%M),m);
				return 1;
			} else
				return 0;
		}

		public override void ZpracujUdalost (Stav u, Model m)
		{
			requests++;
			if (requests % M == 0) {
				this.SendFrame (new Frame (requests),m);
				this.Naplanuj (m.K, Stav.SENDING, m.Cas + M);
			}
			base.ZpracujUdalost (u, m);
		}
	}
}