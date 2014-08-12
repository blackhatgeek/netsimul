using System;

namespace NetTrafficSimulator
{
	/**
	 * Special type of packet used by NetworkNodes to share information about network for routing purposes
	 */
	public abstract class RoutingMessage:Packet{
		private Link link;

		/**
		 * Create new RoutingMessage
		 * @param l Link to send the new RoutingMessage
		 * @param size size of the new RoutingMessage
		 */
		public RoutingMessage(Link l,int size):base(int.MinValue,int.MinValue,size){
			this.link = l;
		}

		/**
		 * Get the link used to send the RoutingMessage
		 */
		public Link Link{
			get{
				return this.link;
			}
		}
	}

	/**
	 * Subtype of RoutingMessage used by NetworkNodes to request information from surrounding nodes
	 */
	public class Request:RoutingMessage
	{
		/**
		 * Create new Request using link provided
		 * @param l Link to use
		 */
		public Request (Link l):base(l,1)
		{
		}
	}

	/**
	 * Subtype of RoutingMessage used by NetworkNodes to share their routing tables
	 */
	public class Response:RoutingMessage
	{
		RoutingTable route;
		/**
		 * Create new Response
		 * @param l Link to use
		 * @param rt RoutingTable to share
		 */
		public Response(Link l,RoutingTable rt):base(l,rt.RecordsCount){
			this.route = rt;
		}
		/**
		 * Shared routing table
		 */
		public RoutingTable Table{
			get{
				return route;
			}
		}
	}
}

