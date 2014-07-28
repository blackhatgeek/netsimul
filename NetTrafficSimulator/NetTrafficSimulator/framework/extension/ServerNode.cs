using System;

namespace NetTrafficSimulator
{
	public class ServerNode:Node,IAddressable
	{
		private readonly int address;
		private Link link;
		public ServerNode (String name,int address):base(name)
		{
			this.link=null;
			this.address = address;
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
			if (state.Actual == MFF_NPRG031.State.state.RECEIVE)
				sendResponse(generateResponse (state.Data),model.Time+wait_time());
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
	}
}

