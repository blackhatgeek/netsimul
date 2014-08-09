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
		//DATA ENVELOPE
		/**
		 * Information container about data, especially direction of flow as source and target nodes are distinguished
		 */
		private class DataEnvelope:Packet{
			private Packet p;
			private decimal size_remainder;
			private int steps;
			private Node source,destination;
			private DataEnvelope next;

			/**
			 * Create a DataEnvelope given the packet, source and target
			 * DataEnvelope specifies direction of data flow since link is full duplex
			 * @param p Packet to deliver
			 * @param source Packet sender
			 * @param target Packet receiver
			 */
			public DataEnvelope(Packet p,Node source,Node target):base(p.Source,p.Destination,p.Size){
				if((source==null)||(target==null))
					throw new ArgumentNullException("[new DataEnvelope] Argument null");
				this.p=p;
				this.source=source;
				this.destination=target;
				this.size_remainder=p.Size;
				this.steps=0;
				this.next=null;
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
			public Node SourceNode{
				get{
					return source;
				}
			}
			/**
			 * The receiver
			 */
			public Node DestinationNode{
				get{
					return destination;
				}
			}

			public override string ToString ()
			{
				return string.Format ("[DataEnvelope: Data={0}, Source={1}, Destination={2}, Steps={3}]", Data, Source, Destination,Steps);
			}

			/**
			 * Can we deliver the remainder of the packet during next tic
			 */ 
			public bool Multistage(decimal available){
				size_remainder = (available > size_remainder) ? 0 : (size_remainder - available);
				steps++;
				return size_remainder > 0;
			}

			public int Steps{
				get{
					return steps;
				}
			}

			public DataEnvelope Next{
				set{
					if(value!=this)
						this.next = value;
				}
				get{
					return this.next;
				}
			}
		}

		//QUEUE with possibility to "return" DataEnvelope back in front
		private DataEnvelope queue_head, queue_tail;

		//LINK
		static readonly ILog log=LogManager.GetLogger(typeof(Link));
		int active_time,inactive_time,carried,env_carry;
		//int last_process;

		private decimal capacity,data_carry;
		private Node a, b;
		private bool active;
		private string name;
		private decimal toggle_probability;
		//private Random r;

		public decimal Capacity{
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
		public Link (String name,decimal capacity, Node a,Node b,decimal toggle_probability)
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
			//this.queue = new Queue<DataEnvelope> ();
			this.data_carry = 0;
			this.a = a;
			this.b = b;
			this.active = true;
			//this.last_process = 0;
			this.active_time = 0;
			this.inactive_time = 0;
			this.carried = 0;
			this.env_carry = 0;
			//this.r = new Random ();
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
			if (p != null) {
				if (((origin == a) && (destination == b)) || ((origin == b) && (destination == a))) {
					if (active) {
						carried++;
						log.Debug ("(" + name + ") Link active, carried");
						DataEnvelope de = new DataEnvelope (p, origin, destination);
						//queue.Enqueue (de);
						de.Next = null;
						if(queue_tail!=null)
							queue_tail.Next = de;
						queue_tail = de;
						if (queue_head == null)
							queue_head = de;
						log.Debug ("Packet size " + p.Size);
						data_carry += p.Size;
						env_carry++;
						log.Debug ("(" + name + ") Enqueued, carry "+env_carry+" envelopes containing " + data_carry + " of data; capacity " + capacity);
					} else {
						log.Warn ("Not carried!");
					}
				} else
					throw new ArgumentException ("This link is capable of delivering data between nodes " + a + " and  " + b + ", though requested to deliver from " + origin + " to " + destination);
			} else
				throw new ArgumentException ("Packet null");
		}

		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			log.Debug ("Link " + name + " process event");
			if(model==null)
				throw new ArgumentException("Model null");
			if (state != null) {
				log.Debug ("State " + state + ", time " + model.Time);
				switch (state.Actual) {
				case MFF_NPRG031.State.state.RECEIVE:
					if (state.Data != null)
						throw new ArgumentException ("Link state should not bear data for RECEIVE");
					decimal chunk = 0;
					log.Debug ("Capacity: " + capacity + " queue empty: " + (queue_head == null));
					while ((chunk<capacity)&&queue_head!=null) {
						log.Debug ("Processing envelope");
						DataEnvelope de = queue_head;//dequeue
						if (de == null)
							throw new ArgumentException ("DE null");
						else {
							queue_head = de.Next;//dequeue
							if (de.Multistage (capacity - chunk)) {
								//vlozit zpet na zacatek queue
								log.Debug ("Multistage delivery");
								queue_head = de;
								chunk = capacity;
								log.Debug ("Scheduling next RECEIVE at " + (model.Time + 1));
								this.Schedule (model.K, state, model.Time + 1);
							} else {//dopravime cely packet (zbytek packetu) v tomto kroku
								log.Debug ("Will deliver in one step");
								if (de.Data.Size > 0)
									chunk += de.Data.Size;
								else if (de.Data.Size == 0)
									chunk += 1;
								else
									throw new ArgumentOutOfRangeException ("Negative packet size");
								MFF_NPRG031.State s = new MFF_NPRG031.State (MFF_NPRG031.State.state.SEND, de);
								if (s.Data == null)
									log.Error ("State data null");
								log.Debug ("Scheduling SEND at "+(model.Time + de.Steps));
								this.Schedule (model.K, s, model.Time + de.Steps);
							}
						}
					}
					log.Debug ("Link " + name + " processed " + chunk + " of data from queue, scheduling RECEIVE at "+(model.Time+1));
					this.Schedule (model.K, new MFF_NPRG031.State (MFF_NPRG031.State.state.RECEIVE), model.Time + 1);;
					break;
				case MFF_NPRG031.State.state.SEND:
					if (active) {
						if (state.Data == null)
							throw new ArgumentException ("Link state should bear data for SEND");
						if (!(state.Data is DataEnvelope))
							throw new ArgumentException ("Link state data should be DataEnvelope for SEND");
						DataEnvelope daen = state.Data as DataEnvelope;
						if (daen.Data == null)
							throw new ArgumentNullException ("Packet null");
						daen.DestinationNode.Schedule (model.K, new MFF_NPRG031.State(MFF_NPRG031.State.state.RECEIVE, daen.Data), model.Time);
						log.Debug ("(" + Name + ") Delivery to " + daen.DestinationNode.Name + " at " + model.Time);
					} else
						log.Warn ("Link " + name + " not active, but planned SEND triggered, dropping packet");
					break;
				case MFF_NPRG031.State.state.TOGGLE:
					log.Debug ("(" + name + ") Switching link state");
					active = !active;
					if (a is NetworkNode) {
						log.Debug ("(" + name + ") Triggering switch on " + a.Name);
						(a as NetworkNode).LinkSwitchTrigger (this, model);
					}
					if (b is NetworkNode) {
						log.Debug ("(" + Name + ") Triggering switch on " + b.Name);
						(b as NetworkNode).LinkSwitchTrigger (this, model);
					}
					if (!active) {
						this.ZrusPlan (model.K);
						log.Debug ("Link " + name + " removed from Calendar");
					}
					break;
				default: 
					throw new ArgumentException ("[Link " + name + "] Invalid state: " + state);
				}
			} else
				throw new ArgumentException ("Link " + name + " state null");
		}

		public override void Run (MFF_NPRG031.Model m)
		{
			this.Schedule (m.K, new MFF_NPRG031.State(MFF_NPRG031.State.state.RECEIVE), m.Time);
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
				return env_carry;
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

