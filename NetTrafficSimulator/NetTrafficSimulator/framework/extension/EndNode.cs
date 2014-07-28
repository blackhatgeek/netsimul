using System;

namespace NetTrafficSimulator
{
	public class EndNode:Node,IAddressable
	{
		private readonly int address;
		private int counter;
		private Link link;
		private Random r;

		/**
		 * Creates an EndNode with given name and address
		 */
		public EndNode(String n,int address):base(n){
			this.counter = 0;
			this.address = address;
			this.r = new Random ();
		}
		
		/**
		 * Link connecting the EndNode to the rest of the network
		 */
		public Link Link{
			get{
				return this.link;
			}set{
				this.link = value;
			}
		}

		/**
		 * Network address of the node
		 */
		public int Address{
			get{
				return this.address;
			}
		}

		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			switch (state.Actual) {
			case MFF_NPRG031.State.state.SEND:
				send ();
				this.Schedule (model.K, new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND), model.Time + wait_time());
				break;
			/*case MFF_NPRG031.State.state.WAIT:
				this.Schedule (model.K, new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND), model.Time + wait_time ());
				break;*/
			case MFF_NPRG031.State.state.RECEIVE:
				counter--;
				break;
			default:
				throw new ArgumentException ("[EndNode "+Name+"] Neplatny stav: "+state);
			}
		}

		public override void Run (MFF_NPRG031.Model m)
		{
			this.Schedule (m.K, new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND), m.Time);
		}

		/**
		 * Attept to post a new packet to the link, if such exist
		 * @throws InvalidOperationException if link is not connected
		 */
		private void send(){
			if (link != null) {
				this.link.Carry (new Packet (address,r.Next()), this, this.link.GetPartner (this));
				counter++;
			} else
				throw new InvalidOperationException ("[Node " + Name + "] Link neni pripojen");
		}

		/**
		 * Generates a wait time after sending
		 */
		private int wait_time(){
			throw new NotImplementedException ();
		}
	}
}

