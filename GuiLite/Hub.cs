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

		/*protected override int sending (Model m)
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
			return 0;
		}*/
		protected override int multiport_sending (Model m)
		{
			int processed = 0;
			while ((q_out.Count>0)&&processed<FramesProcessPerTic) {
				EtherFrame f = q_out.Dequeue ();
				foreach (NetworkInterface ni in interfaces) {
					ni.Dispatch (f, m);
				}
				processed++;
			}
			return 1;
		}
	}
}

