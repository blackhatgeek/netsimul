using System;
using System.Collections.Generic;

namespace NetTrafficSimulator
{
	public class NetworkNode:Node,IResultProvider
	{
		Link[] interfaces;
		int[] interface_use_count;
		int interfaces_count,interfaces_used,processed,time_wait,last_process;
		int delay;

		/**
		 * Creates a new network node with given name and interfaces count
		 * The delay is set fixed as 1
		 * @param name Human readable node name
		 * @param interfaces_count How many ports does the network node have
		 * @throws ArgumentException on negative interfaces_count
		 */
		public NetworkNode (String name,int interfaces_count):base(name)
		{
			if (interfaces_count >= 0) {
				this.interfaces = new Link[interfaces_count];
				this.interface_use_count = new int[interfaces_count];
				this.interfaces_count = interfaces_count;
				this.interfaces_used = 0;
				this.delay = 1;
				this.processed = 0;
				this.time_wait = 0;
				this.last_process = 0;
			} else
				throw new ArgumentException ("[NetworkNode] Negative interface count");
		}

		public int Interfaces{
			get{
				return interfaces.Length;
			}
		}

		/**
		 * If possible, note a new link in use on first interface available
		 * @param l Link to connect
		 * @throws ArgumentException if no port is available
		 * @throws ArgumentNullException on l null
		 */
		public void ConnectLink(Link l){
			if (interfaces_used < interfaces_count) {
				if (l != null) {
					interfaces [interfaces_used] = l;
					interfaces_used++;
				} else
					throw new ArgumentNullException ("Link null");
			} else
				throw new ArgumentException ("No port available");
		}

		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			this.time_wait += model.Time - last_process;
			this.last_process = model.Time;
			switch (state.Actual) {
			case MFF_NPRG031.State.state.RECEIVE:
				processed++;
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

		public Dictionary<string,object> GetResults(MFF_NPRG031.Model model){
			Dictionary<string,object> result = new Dictionary<string, object> ();
			//vybrat nekolik nejcasteji vybranych interfacu
			//delay je konstantni - bude-li random, tak prumerny
			result.Add ("Packets processed", processed);
			result.Add ("Time waited", time_wait);
			result.Add("Time idle",time_wait/model.Time);
			result.Add ("Average wait time", time_wait / processed);
			return result;
		}
	}
}

