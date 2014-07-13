using System;

namespace GuiLite
{
	public abstract class Proces
	{
		public void Naplanuj(Kalendar kalendar, Stav co, int kdy)
		{
			kalendar.Zarad(new Udalost(this, co, kdy));
		}
		public void ZrusPlan(Kalendar kalendar)
		{
			kalendar.Vyrad(this);
		}
		abstract public void ZpracujUdalost(Stav u, Model m);

	}
}