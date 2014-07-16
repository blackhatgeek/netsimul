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

		public Hub (String name,int ports,int wait_time, int frames_send_per_tic, int frames_process_per_tic):base(name,ports,wait_time,frames_send_per_tic,frames_process_per_tic)
		{
		}

		public override int ProcessFrame (EtherFrame f, Model m)
		{
			SendFrame (f,m);
			return 1;
		}

		/*protected void Dispatch (EtherFrame f){
			foreach (EtherFrame ef in queue) {
				foreach (NetworkInterface ni in interfaces) {
					if (ni.InUse) ni.Linka.Doprav (ef);
				}
			}
		}*/

		public override void ZpracujUdalost (Stav u, Model m)
		{
			base.ZpracujUdalost (u, m);
		}

		protected override void sending (Model m)
		{
			//pro kazdy port je potreba vystupni fronta - je soucasti rozhrani
			//z q_out se ramec nakopiruje do jednotlivych vystupnich front
			//pro kazdy port bezi cyklus:
				//dokud je volno a jeho vystupni fronta neco obsahuje
					//vem etherframe z fronty a zkus ho poslat prislusnou linkou
				//kdyz neni volno je linka zaneprazdnena
			while (q_out.Count>0) {
				EtherFrame ef = q_out.Dequeue ();
				foreach(NetworkInterface ni in interfaces){
					ni.Out_q.Enqueue (ef);
				}
			}
			foreach (NetworkInterface ni in interfaces) {

			}


			//odeslani packetu - projdeme vystupni frontu a napiseme na vystup hlasku o zpracovani ramce
			//pokud ready=true, jinak cekame az prijemce bude pripraven a zatim ukladame ramce do vystupni fronty
			/*while ((this.Link.Volno)&&(q_out.Count>0))
				EtherFrame f = q_out.Dequeue ();
				Console.WriteLine ("Node " + name + " sent a frame to " + f.Destination + " at time " + m.Cas);
			try{
				this.net.Linka.Doprav (f);
			}catch(LinkException e){
				Console.WriteLine ("Oops! link trouble:" + e.Message);
			}
			if (!this.Link.Volno)
				Console.WriteLine ("Link busy");
			*/
		}
	}
}

