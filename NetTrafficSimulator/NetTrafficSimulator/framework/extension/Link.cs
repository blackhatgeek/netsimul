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
		private MFF_NPRG031.Model model;
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
			int timeAccepted;

			/**
			 * Create a DataEnvelope given the packet, source and target
			 * DataEnvelope specifies direction of data flow since link is full duplex
			 * @param p Packet to deliver
			 * @param source Packet sender
			 * @param target Packet receiver
			 */
			public DataEnvelope(Packet p,Node source,Node target,int time):base(p.Source,p.Destination,p.Size){
				if((source==null)||(target==null))
					throw new ArgumentNullException("[new DataEnvelope] Argument null");
				if(timeAccepted<0)
					throw new ArgumentOutOfRangeException("[new DataEnvelope] Negative time");
				this.p=p;
				this.source=source;
				this.destination=target;
				this.size_remainder=p.Size;
				this.steps=0;
				this.next=null;
				this.timeAccepted=time;
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

			public int TimeAccepted{
				get{
					return timeAccepted;
				}
			}
		}

		//QUEUE with possibility to "return" DataEnvelope back in front
		private DataEnvelope queue_head, queue_tail;

		//LINK
		static readonly ILog log=LogManager.GetLogger(typeof(Link));
		int active_time,inactive_time,env_carry;
		int last_toggle,wait_time,env_sent;

		private decimal capacity,data_carry,data_sent,lost_in_carry;
		private Node a, b;
		private bool active;
		private string name;

		public decimal Capacity{
			get{
				return this.capacity;
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
		public Link (String name,decimal capacity, Node a,Node b,MFF_NPRG031.Model model)
		{
			if (capacity <0) throw new ArgumentOutOfRangeException ("Link capacity cannot be negative");
			if (a == null || b == null)
				throw new ArgumentNullException ("Node cannot be null");

			this.name = name;
			this.capacity = capacity;
			//this.queue = new Queue<DataEnvelope> ();
			this.data_carry = 0.0m;
			this.a = a;
			this.b = b;
			this.active = true;
			this.last_toggle = -1;
			this.active_time = 0;
			this.inactive_time = 0;
			this.wait_time = 0;
			this.env_carry = 0;
			this.env_sent = 0;
			this.data_sent = 0.0m;
			this.model = model;
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
						//carried++;
						log.Debug ("(" + name + ") Link active, carried");
						DataEnvelope de = new DataEnvelope (p, origin, destination,model.Time);
						//queue.Enqueue (de);
						de.Next = null;
						if(queue_tail!=null)
							queue_tail.Next = de;
						queue_tail = de;
						if (queue_head == null)
							queue_head = de;
						data_carry += p.Size;
						env_carry++;
						log.Debug ("(" + name + ") Enqueued, carry "+env_carry+" envelopes containing " + data_carry + " of data; capacity " + capacity);
					} else {
						lost_in_carry += p.Size;
						data_carry += p.Size;
						env_carry++;
						log.Warn ("Not carried!");
					}
				} else
					throw new ArgumentException ("This link is capable of delivering data between nodes " + a + " and  " + b + ", though requested to deliver from " + origin + " to " + destination);
			} else
				throw new ArgumentException ("Packet null");
		}

		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			if(model==null)
				throw new ArgumentException("Model null");
			if (state != null) {
				log.Debug ("State " + state + ", time " + model.Time);
				switch (state.Actual) {
				case MFF_NPRG031.State.state.RECEIVE:
					if (state.Data != null)
						throw new ArgumentException ("Link state should not bear data for RECEIVE");
					decimal chunk = 0;
					while ((chunk<capacity)&&queue_head!=null) {
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
						wait_time += model.Time - daen.TimeAccepted;
						env_sent++;
						data_sent += daen.Data.Size;
					} else
						log.Warn ("Link " + name + " not active, but planned SEND triggered, dropping packet");
					break;
				case MFF_NPRG031.State.state.TOGGLE:
					log.Debug ("(" + name + ") Switching link state");
					active = !active;
					if (!active) {
						active_time += model.Time - last_toggle;
						this.ZrusPlan (model.K);
						log.Debug ("Link " + name + " removed from Calendar, active time " + active_time);
					} else {
						inactive_time += model.Time - last_toggle;
						this.Schedule (model.K, new MFF_NPRG031.State (MFF_NPRG031.State.state.RECEIVE), model.Time + 1);
						log.Debug ("Link " + name + " now active, passive time " + inactive_time);
					}
					last_toggle=model.Time;
					break;
				default: 
					throw new ArgumentException ("[Link " + name + "] Invalid state: " + state);
				}
			} else
				throw new ArgumentException ("Link " + name + " state null");
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
		public int GetActiveTime(MFF_NPRG031.Model m){
			return (last_toggle == -1) ? m.Time : active_time;
		}
		/**
		 * How much time was the link passive
		 */
		public int PassiveTime{
			get{
				return (last_toggle == -1) ? 0 : inactive_time;
			}
		}

		/**
		 * How much time was the link passive compared to time simulating
		 */
		public decimal GetPercentageTimeIdle(MFF_NPRG031.Model m) {
			if (m.Time == 0) {
				if (last_toggle == -1)
					return 0.0m;
				else
					return 100.0m;
			} else {
				return PassiveTime/ m.Time * 100.0m;
			}
		}

		/**
		 * How much actual data was carried
		 */
		public decimal DataCarried{
			get{
				return data_carry;
			}
		}

		/**
		 * How much actual data was sent
		 */
		public decimal DataSent{
			get{
				return data_sent;
			}
		}

		/**
		 * How much data was lost
		 */
		public decimal DataLost{
			get{
				return data_carry - data_sent;
			}
		}

		/**
		 * How much data was not accepted because link was not active when carry was invoked
		 */
		public decimal PercentageDataLostInCarry{
			get{
				if (data_carry > 0)
					return lost_in_carry / data_carry * 100.0m;
				else
					return 0.0m;
			}
		}

		/**
		 * How much data was lost in percentage
		 */
		public decimal PercentageDataLost{
			get{
				return (data_carry > 0) ? (data_carry - data_sent) / data_carry *100.0m: 0.0m;
			}
		}

		/**
		 * How much data was delivered in percentage
		 */
		public decimal PercentageDataDelivered{
			get{
				return (data_carry > 0) ? data_sent / data_carry * 100.0m: 100.0m;
			}
		}

		/**
		 * Average amount of data carried per tic
		 */
		public decimal GetAvgDataCarriedPerTic(MFF_NPRG031.Model m){
			if ((m.Time == 0) && (last_toggle == -1))
				return data_carry + 0.0m;
			else if (m.Time == 0)
				return 0.0m;
			else
				return data_carry / GetActiveTime (m);
		}

		/**
		 * Average percentage of data carried to link capacity
		 */
		public decimal GetAvgLinkUsage(MFF_NPRG031.Model m){
			if (capacity == 0) {
				return 100.0m;
			} else {
				return GetAvgDataCarriedPerTic (m) / capacity;
			}
		}

		/**
		 * Average time packet waited for delivery
		 */
		public decimal AvgWaitTime{
			get{
				if (env_carry != 0)
					return wait_time / env_carry;
				else
					return 0;
			}
		}

		public override string ToString ()
		{
			return name;
		}

		public bool ConnectedTo(Node n){
			return (a == n) || (b == n);
		}
	}
}

