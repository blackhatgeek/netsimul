using System;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * Connector between Nodes
	 */
	public class Link:MFF_NPRG031.Process,INamable
	{
		static readonly ILog log=LogManager.GetLogger(typeof(Link));
		int dropped,active_time,inactive_time,carried;
		int last_process;
		/**
		 * Information container about data, especially direction of flow as source and target nodes are distinguished
		 */
		private class DataEnvelope{
			private Packet p;
			private Node source,destination;
			/**
			 * Create a DataEnvelope given the packet, source and target
			 * DataEnvelope specifies direction of data flow since link is full duplex
			 * @param p Packet to deliver
			 * @param source Packet sender
			 * @param target Packet receiver
			 */
			public DataEnvelope(Packet p,Node source,Node target){
				if((source==null)||(target==null))
					throw new ArgumentNullException("[new DataEnvelope] Argument null");
				this.p=p;
				this.source=source;
				this.destination=target;
			}
			/**
			 * The packet
			 */
			public Packet Data{
				get{
					return p;
				}
			}
			/**
			 * The sender
			 */
			public Node Source{
				get{
					return source;
				}
			}
			/**
			 * The receiver
			 */
			public Node Destination{
				get{
					return destination;
				}
			}

			public override string ToString ()
			{
				return string.Format ("[DataEnvelope: Data={0}, Source={1}, Destination={2}]", Data, Source, Destination);
			}
		}

		private int capacity,next_queue_pos;
		private DataEnvelope[] queue;
		private Node a, b;
		private bool active;
		private string name;
		private decimal toggle_probability;
		private Random r;

		public int Capacity{
			get{
				return this.capacity;
			}
		}

		public decimal ToggleProbability{
			get{
				return this.toggle_probability;
			}
		}

		/**
		 * Creates a new link between two nodes with given capacity which specifies how many data the link will be able to deliver per time unit
		 * @param name		link name
		 * @param capacity	how many data a link can deliver per time unit
		 * @param a			node at one end
		 * @param b			node at other end
		 * @throws	ArgumentOutOfRangeException Negative link capacity
		 * @throws	ArgumentNullException any node null
		 */
		public Link (String name,int capacity, Node a,Node b,decimal toggle_probability)
		{
			if (capacity <0) throw new ArgumentOutOfRangeException ("Link capacity cannot be negative");
			if (a == null || b == null)
				throw new ArgumentNullException ("Node cannot be null");
			if ((toggle_probability >= 0.0m) && (toggle_probability <= 1.0m))
				this.toggle_probability = toggle_probability;
			else
				throw new ArgumentOutOfRangeException ("toggle_probability must be between 0.0 and 1.0");

			this.name = name;
			this.capacity = capacity;
			this.next_queue_pos = 0;
			this.queue = new DataEnvelope[capacity];
			this.a = a;
			this.b = b;
			this.active = true;
			this.last_process = -1;
			this.active_time = 0;
			this.inactive_time = 0;
			this.dropped = 0;
			this.carried = 0;
			this.r = new Random ();
		}

		/**
		 * Human readable link name
		 */ 
		public string Name{
			get{
				return this.name;
			}
		}

		/**
		 * Link is active if both nodes provided in constructor are not null and this state was not changed as result of toggle
		 */
		public bool Active{
			get{
				return this.active;
			}
			set{
				this.active = value;
			}
		}

		//potrebuji frontu na data
		//nezajima me, jesli je plna - data proste nedorazi
		/**
		 * Link is capable of carrying specified amount of packets each step of simulation
		 * This method verifies origin and destination match link definition and then, if there is availability to send the particular packet in the next step, will create a 
		 * data envelope object and store it in a queue
		 * If the link is unable to sent the packet in the next step of simulation, the packet will be lost
		 * @param p Packet to transfer
		 * @param origin Node sending the packet
		 * @param destination Node receiving the packet on the other end
		 * @throws ArgumentException if origin and destination are not nodes specified in the consructor regardless of order
		 */
		public void Carry(Packet p,Node origin,Node destination){
			if (((origin == a) && (destination == b)) || ((origin == b) && (destination == a))) {
				if (active) {
					carried++;
					log.Debug ("("+name+") Link active, carried");
					if (next_queue_pos < capacity) {
						DataEnvelope de = new DataEnvelope (p, origin, destination);
						queue [next_queue_pos] = de;
						next_queue_pos++;
						log.Debug ("("+name+") Enqueued");
					} else {
						dropped++;
						log.Debug ("("+name+") Dropped");
					}
				}
			} else
				throw new ArgumentException ("This link is capable of delivering data between nodes " + a + " and  " + b + ", though requested to deliver from " + origin + " to " + destination);
		}

		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			if (state.Data != null) throw new ArgumentException ("Link state should not bear data");
			if (active)
				active_time += model.Time - last_process;//kolik casu uplynulo od posledniho process - celou dobu byl link active
			else
				inactive_time += model.Time - last_process;//totez, ale celou dobu byl inactive
			log.Debug ("("+name+") <TIME> actual:" + model.Time + " active:" + active_time + " inactive:" + inactive_time + " last_process:" + last_process);
			last_process = model.Time;

			if (state.Actual.Equals (MFF_NPRG031.State.state.SEND)) {//vse v queue dorucit do cile a naplanovat se do send
				if (active) 
					send_queue (model);
				//vypadek linky?
				if (toggle ()) {
					log.Debug ("("+name+") Switching link state");
					active = !active;
					if (a is NetworkNode) {
						log.Debug ("(" + name + ") Triggering switch on " + a.Name);
						(a as NetworkNode).LinkSwitchTrigger (this,model);
					}
					if (b is NetworkNode) {
						log.Debug ("(" + Name + ") Triggering switch on " + b.Name);
						(b as NetworkNode).LinkSwitchTrigger (this,model);
					}
				}
				this.Schedule (model.K, new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND,state.Data), model.Time+1);
			} else
				throw new ArgumentException ("[Link " + name + "] Invalid state: " + state);
		}

		public override void Run (MFF_NPRG031.Model m)
		{
			this.Schedule (m.K, new MFF_NPRG031.State(MFF_NPRG031.State.state.SEND), m.Time);
		}

		//rozhodne o moznem vypadku linky
		/**
		 * Decide if toggle link status
		 * @return wheather to toggle link state
		 */
		private bool toggle(){
			decimal x = (decimal)r.NextDouble();
			if (toggle_probability>x) {
				log.Debug ("[Link  " + name + "] toggle(): random = " + x + ", toggle_probability=" + toggle_probability + ", toggle");
				return true;
			} else {
				log.Debug("[Link  "+name+"] toggle(): random = "+x+", toggle_probability="+toggle_probability+", do not toggle");
				return false;
			}
		}

		/**
		 * For each DataEnvelope enqueued will schedule destination to RECEIVE the data given at time T+1 and reset the queue for next step
		 * @param model the Model
		 */
		private void send_queue(MFF_NPRG031.Model model){
			if (model == null)
				throw new ArgumentNullException ("[Link.send_queue] Model null");
			//foreach (DataEnvelope de in queue) {
			for(int i=0;i<next_queue_pos;i++){
				DataEnvelope de = queue [i];
				if (de == null)
					throw new ArgumentNullException ("[Link.send_queue] DataEnvelope null");
				//de.Destination.Receive (de.Data);
				log.Debug ("(" + Name + ") Delivery to " + de.Destination + " at " + (model.Time + 1));
				de.Destination.Schedule (model.K, new MFF_NPRG031.State (MFF_NPRG031.State.state.RECEIVE, de.Data), model.Time + 1);
			}
			this.next_queue_pos = 0;
		}

		/**
		 * For given node return the second one of the pair
		 * @param x a node of the pair specified in the constructor
		 * @return the second node of the pair specified in the constructor
		 * @throws ArgumentException if x is not a node specified in the constructor
		 */
		public Node GetPartner(Node x){
			if (x == a)
				return b;
			else if (x == b)
				return a;
			else
				throw new ArgumentException ("[Link " + name + "] Can't tell a partner of the node not belonging to the link (" + x + ")");
		}

		//results
		/**
		 * Amount of packets carried by link (how many times carry() was called)
		 */
		public int PacketsCarried{
			get{
				return carried;
			}
		}
		/**
		 * How many times was a packet dropped as result of full link
		 */
		public int PacketsDropped {
			get {
				return dropped;
			}
		}
		/**
		 * Dropped to Carried ratio
		 */
		public decimal DropPercentage{
			get{
				if (carried != 0)
					return dropped / carried * 100;
				else
					return 0;
			}
		}
		/**
		 * How much time was the link active
		 */
		public int ActiveTime{
			get{
				return active_time;
			}
		}
		/**
		 * How much time was the link passive
		 */
		public int PassiveTime{
			get{
				return inactive_time;
			}
		}

		/**
		 * How much time was the link passive compared to time simulating
		 */
		public decimal PercentageTimeIdle {
			get {
				if ((active_time + inactive_time) != 0)
					return inactive_time / (active_time + inactive_time) * 100;
				else
					return 100;
			}
		}

		public override string ToString ()
		{
			return name;
		}
	}
}

