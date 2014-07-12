using System;

namespace GuiLite
{
	public class Model
	{
		public Kalendar K = new Kalendar();     //simulacni kalendar
		public bool Konec = false;              //priznak ukonceni simulace
		public int Cas;         				//simulacni cas
		private int doba_behu;

		public Model(Node[] uzly,int beh)
		{
			this.doba_behu = beh;
			//node jiz maji nactena data, potreba dopravit node do modelu
			foreach(Node n in uzly) n.Init (this);
		}

		public int Simulace()
		{
			Console.WriteLine ("Simulation started");
			while (!Konec)
			{
				Udalost u = K.Prvni();
				Cas = u.kdy;
				if (Cas == doba_behu)
					Konec = true;
					u.kdo.ZpracujUdalost(u.co, this);
			}
			Console.WriteLine ("Simulation finished");
			return Cas;
		}

	}
}

