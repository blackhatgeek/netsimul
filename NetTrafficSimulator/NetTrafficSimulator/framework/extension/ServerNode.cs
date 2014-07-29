using System;
using System.Collections.Generic;

namespace NetTrafficSimulator
{
	public class ServerNode:Node,IAddressable,IResultProvider
	{
		private readonly int address;
		private int time_waited,process;
		private Link link;
		public ServerNode (String name,int address):base(name)
		{
			this.link=null;
			this.address = address;
			this.time_waited = 0;
			this.process = 0;
		}

		public Link Link{
			get{
				return this.link;
			}set{
				this.link = value;
			}
		}

		public int Address{
			get{
				return this.address;
			}
		}

		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			if (state.Actual == MFF_NPRG031.State.state.RECEIVE) {
				int t = wait_time ();
				this.time_waited += t;
				this.process++;
				sendResponse (generateResponse (state.Data), model.Time+t);
			}
			else
				throw new ArgumentException ("[ServerNode "+Name+"] Neplatny stav: "+state);
		}

		private Packet generateResponse(Packet p){
			throw new NotImplementedException ();
		}

		private int wait_time(){
			throw new NotImplementedException ();
		}

		private void sendResponse(Packet p,int time){
			if (link != null)
				this.link.Carry (p, this, this.link.GetPartner (this));
			else
				throw new InvalidOperationException ("[Node " + Name + "] Link neni pripojen");
		}

		public override void Run (MFF_NPRG031.Model m)
		{
		}

		public Dictionary<string,object> GetResults(MFF_NPRG031.Model model){
			Dictionary<string,object> results = new Dictionary<string, object> ();
			results.Add ("Time waited", time_waited);
			results.Add ("Time idle (%)", time_waited / model.Time*100);
			results.Add ("Average wait time", time_waited / process);
			results.Add ("Packets processed", process);
			return results;
		}
	}
}

