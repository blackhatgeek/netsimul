using System;

namespace GuiLite
{
	public class EtherFrame{
		//TODO: omezeni velikosti pro size

		private MACaddr source;
		private MACaddr destination;
		private object data;//TODO
		private int size;//octets .. bytes

		private bool crc;

		public EtherFrame(MACaddr source,MACaddr destination,object data,int size){
			this.source=source;
			this.destination = destination;
			this.data = data;
			this.size = size;
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

		public object Size{
			get{return size;}
		}

		public bool CRC{
			get{return crc;}
			set{ crc = value;}
		}
	}
}