using System;

namespace GuiLite
{
	public class EtherFrame{
		//TODO: omezeni velikosti pro size
		public const int UPPER_BOUND_SIZE=1;
		public const int LOWER_BOUND_SIZE=0;
		public const int CONSTANT_DATA_SIZE=0;


		private MACaddr source;
		private MACaddr destination;
		private object data;//TODO
		private int size;//octets .. bytes

		private bool crc;

		public EtherFrame(MACaddr source,MACaddr destination,object data,int size){
			this.source=source;
			this.destination = destination;
			this.data = data;
			if (size <= UPPER_BOUND_SIZE)
				this.size = size;
			else
				throw new ArgumentOutOfRangeException ("Max velikost ramce " + UPPER_BOUND_SIZE);
			if (size <= LOWER_BOUND_SIZE)
				this.size = LOWER_BOUND_SIZE;
			this.crc = true;
		}

		public MACaddr Source{
			get{return source;}
		}

		public MACaddr Destination{
			get{return destination;}
		}

		public object Data{
			get{return data;}
		}

		public int Size{
			get{return size;}
			set{ size = value;}//POZOR

		}

		public bool CRC{
			get{return crc;}
			set{ crc = value;}
		}
	}
}