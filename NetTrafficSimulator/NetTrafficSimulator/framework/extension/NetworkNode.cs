using System;

namespace NetTrafficSimulator
{
	public class NetworkNode:Node
	{
		Link[] interfaces;
		int interfaces_count,interfaces_used;
		int delay;

		/**
		 * Creates a new network node with given name and interfaces count
		 * The delay is set fixed as 1
		 * @param name Human readable node name
		 * @param interfaces_count How many ports does the network node have
		 */
		public NetworkNode (String name,int interfaces_count):base(name)
		{
			this.interfaces = new Link[interfaces_count];
			this.interfaces_count = interfaces_count;
			this.interfaces_used = 0;
			this.delay = 1;
		}

		/**
		 * If possible, note a new link in use on first interface available
		 * @param l Link to connect
		 * @throws ArgumentException if no port is available
		 */
		public void ConnectLink(Link l){
			if (interfaces_used < interfaces_count) {
				interfaces [interfaces_used] = l;
				interfaces_used++;
			} else
				throw new ArgumentException ("No port available");
		}

		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			switch (state.Actual) {
			case MFF_NPRG031.State.state.RECEIVE:
				scheduleForward (state.Data, selectDestination (state.Data), model);
				break;
			default:
				throw new ArgumentException ("[NetworkNode "+Name+"] Neplatny stav: "+state);
			}
		}

		/**
		 * Empty
		 */
		public override void Run (MFF_NPRG031.Model m)
		{
		}

		private Link selectDestination(Packet p){
			throw new NotImplementedException ();
		}

		/**
		 * Given the packet and the link to use, schedule SEND for the link with packet included at time T+delay
		 * @param p Packet to forward
		 * @param l Link to use (result of selectDestination)
		 * @param model the Model
		 */
		private void scheduleForward(Packet p,Link l,MFF_NPRG031.Model model){
			l.Schedule(model.K,new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND,p),model.Time+delay);
		}
	}
}

