using System;

namespace GuiLite
{
	public class Model
	{
		public Kalendar K;     //simulacni kalendar
		public bool Konec;              //priznak ukonceni simulace
		public int Cas;         				//simulacni cas
		private int doba_behu;
		private Node[] uzly;

		public Model(Node[] uzly,int beh)
		{
			K = new Kalendar ();
			Konec = false;
			this.uzly=uzly;
			this.doba_behu = beh;
			//node jiz maji nactena data, potreba dopravit node do modelu
			if(uzly!=null)
				foreach (Node n in uzly) n.Init (this);
		}

		public int Simulace()
		{
			if (uzly != null) {
				foreach (Node n in uzly)
					Console.WriteLine (n.Name + "\t" + n.FramesProcessPerTic);
				Console.WriteLine ("Simulation started");
				while (!Konec) {
					Udalost u = K.Prvni ();
					if (u != null) {
						Cas = u.kdy;
						if (Cas >= doba_behu)
							Konec = true;
						u.kdo.ZpracujUdalost (u.co, this);
					} else {
						Konec = true;
						Console.WriteLine ("Prazdny kalendar!");
					}
				}
				foreach (Node n in uzly)
					n.Fin ();
				Console.WriteLine ("Simulation finished");
			}
			return Cas;
		}
	}
}