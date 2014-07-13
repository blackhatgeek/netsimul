using System;

namespace GuiLite
{
	public class Frame
	{
		private int id;
		public int ID{
			get{ return id;}
		}
		//toto ponese informace ohledne prenasenych dat, na zaklade kterych se node bude rozhodovat, co posle
		public Frame (int id)
		{
			this.id = id;
		}
	}

	public class ServiceFrame:Frame
	{
		public ServiceFrame(int id):base(id){
			this.t=Type.CONFIRMATION;
		}

		public ServiceFrame(int id,Type t):base(id){
			this.t=t;
		}

		public enum Type{
			CONFIRMATION, STOP_SENDING, READY
			//confirmation ma stejne frame id jako prichozi packet
		}

		private Type t;
		public Type type{
			get{return t;}
		}
	}
}