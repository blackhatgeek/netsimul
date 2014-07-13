using System;

namespace GuiLite
{
	public class Fronta
	{
		// obousmerny cyklicky seznam Procesu s hlavou
		Proces f;                 //hlava fronty
		public Fronta()
		{
			f = new Proces();     //hlava prazdne fronty
			f.fpred = f; f.fza = f;
		}
		public bool Prazdna()     //fronta je prazdna
		{
			return f.fza == f;
		}
		public Proces Prvni()     //vrati prvni Proces a vyradi ho z fronty
		{
			if (Prazdna()) return null;
			Proces p = f.fza;
			f.fza = p.fza;
			p.fza.fpred = f;
			p.fza = null; p.fpred = null;
			return p;
		}
		public void Zarad(Proces p) //na konec fronty
		{
			p.fpred = f.fpred; p.fza = f;
			f.fpred = p; p.fpred.fza = p;
		}
	}
}

