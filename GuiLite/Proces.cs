using System;

namespace GuiLite
{
	public class Proces
	{
		public Proces fpred, fza;  // razeni ve fronte
		public Proces kpred, kza;  // razeni v kalendari
		public int plan;           // casovy plan
		public Proces()
		{
			plan = 0;
			fpred = null; fza = null; kpred = null; kza = null;
		}
		public void DoFronty(Fronta f)  //zaradit do zvolene fronty
		{
			ZFronty();
			f.Zarad(this);
		}
		public void ZFronty()           //vyradit z libovolne fronty
		{
			if (fza != null)
			{
				fza.fpred = fpred; fpred.fza = fza;
				fza = null; fpred = null;
			}
		}
		public void Naplanuj(Kalendar kalendar, int plan)
		{
			kalendar.Vyrad(this);
			this.plan = plan;
			kalendar.Zarad(this);
		}
		public void ZrusPlan(Kalendar kalendar)
		{
			kalendar.Vyrad(this);
		}
		virtual public void ZpracujUdalost() { }
	}
}