using System;
using System.Collections.Generic;

namespace GuiLite
{
	public class Kalendar
	{
		//usporadany obousmerny cyklicky seznam Procesu s hlavou
		Proces k;                 //hlava kalendare
		public Kalendar()
		{
			k = new Proces();     //hlava prazdne fronty
			k.kpred = k; k.kza = k;
			k.plan = 0;
		}
		public bool Prazdny()     //kalendar je prazdny
		{
			return k.kza == k;
		}
		public Proces Prvni()     //vrati prvni Proces a vyradi ho z kalendare
		{
			if (Prazdny()) return null;
			Proces p = k.kza;
			k.kza = p.kza;
			p.kza.kpred = k;
			p.kza = null; p.kpred = null;
			return p;
		}
		public void Zarad(Proces p)  //zaradi podle casu planu (aktivuje)
		{
			Proces x = k.kza;
			while (x != k && p.plan >= x.plan) x = x.kza;
			//proces "p" zaradime do kalendare pred proces "x"
			p.kza = x; p.kpred = x.kpred;
			x.kpred = p; p.kpred.kza = p;
		}
		public void Vyrad(Proces p)  //vyradi z kalendare (pasivuje)
		{
			if (p.kza != null)
			{
				p.kza.kpred = p.kpred; p.kpred.kza = p.kza;
				p.kza = null; p.kpred = null;
			}
		}
	}
}

