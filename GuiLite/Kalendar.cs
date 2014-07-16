using System;
using System.Collections.Generic;

namespace GuiLite
{
	public class Kalendar
	{
		List<Udalost> kalendar = new List<Udalost>();
		const int MAX = 0x7FFFFFFF;
		public Kalendar()
		{
			kalendar.Clear();
		}
		public bool Prazdny()     //kalendar je prazdny
		{
			return kalendar.Count == 0;
		}
		public Udalost Prvni()     //vrati prvni udalost a vyradi ji z kalendare
		{
			if (Prazdny()) return null;
			int cas = MAX;
			Udalost prvni = null;
			foreach (Udalost u in kalendar)
				if (u.kdy < cas) { prvni = u; cas = u.kdy; }
			kalendar.Remove(prvni);
			return prvni;
		}
		public void Zarad(Udalost udalost)  //zaradi udalost do kalendare
		{
			kalendar.Add(udalost);
		}
		public void Vyrad(Proces p)  //vyradi proces z kalendare
		{
			foreach (Udalost u in kalendar)
				if (u.kdo == p) { kalendar.Remove(u); break; }
		}
	}
}