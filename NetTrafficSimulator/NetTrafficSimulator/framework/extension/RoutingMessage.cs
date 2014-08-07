using System;

namespace NetTrafficSimulator
{
	public abstract class RoutingMessage:Packet{
		private Link link;
		public RoutingMessage(Link l,int size):base(int.MinValue,int.MinValue,size){
			this.link = l;
		}
		public Link Link{
			get{
				return this.link;
			}
		}
	}

	public class Request:RoutingMessage
	{
		public Request (Link l):base(l,1)
		{
		}
	}

	public class Response:RoutingMessage
	{
		RoutingTable route;
		public Response(Link l,RoutingTable rt):base(l,rt.RecordsCount){
			this.route = rt;
		}
		public RoutingTable Table{
			get{
				return route;
			}
		}
	}
}

