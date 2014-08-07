using System;

namespace NetTrafficSimulator
{
	public abstract class RoutingMessage:Packet{
		public RoutingMessage(int size):base(int.MinValue,int.MinValue,(decimal)size){
		}
	}

	public class Request:RoutingMessage
	{
		private Link link;
		public Request (Link l):base(1)
		{
			this.link = l;
		}
		public Link Link{
			get{
				return this.link;
			}
		}
	}

	public class Response:RoutingMessage
	{
		RoutingTable route;
		public Response(RoutingTable rt):base(rt.RecordsCount){
			this.route = rt;
		}
		public RoutingTable Table{
			get{
				return route;
			}
		}
		//zaznamy
	}
}

