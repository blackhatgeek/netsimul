using System;

namespace MFF_NPRG031
{
	/**
	 * State is SEND or RECEIVE and can contain additional data in form of Packet
	 */
	public class State
	{
		/**
		 * Actual state - enum. Available states are SEND or RECEIVE
		 */
		public enum state{
			SEND,RECEIVE
		}

		private state actual_state;
		private NetTrafficSimulator.Packet additionalData;

		/**
		 * Create a new state with no additional data
		 */
		public State(state s){
			this.actual_state = s;
			additionalData = null;
		}

		/**
		 * Create a new state holding additional data
		 */
		public State(state s,NetTrafficSimulator.Packet data):this(s){
			this.additionalData = data;
		}

		/**
		 * Actual state: SEND or RECEIVE
		 */
		public state Actual{
			get{
				return this.actual_state;
			}
		}

		/**
		 * Additional data
		 */
		public NetTrafficSimulator.Packet Data{
			get{
				return additionalData;
			}
		}
	}
}

