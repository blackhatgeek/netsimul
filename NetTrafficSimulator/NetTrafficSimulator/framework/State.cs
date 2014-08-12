using System;
using log4net;

namespace MFF_NPRG031
{
	/**
	 * State is SEND or RECEIVE and can contain additional data in form of Packet
	 */
	public class State
	{
		static readonly ILog log=LogManager.GetLogger(typeof(State));
		/**
		 * Actual state - enum. Available states are SEND or RECEIVE
		 */
		public enum state{
			/**
			 * Send packets
			 */
			SEND,
			/**
			 * Process received packets
			 */
			RECEIVE,
			/**
			 * Send link state from active to passive and vice versa
			 */
			TOGGLE,
			/**
			 * Network node send response
			 */
			UPDATE_TIMER,
			/**
			 * Mark record in routing table as unreachable
			 */
			INVALID_TIMER,
			/**
			 * Remove record from routing table
			 */
			FLUSH_TIMER
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
			if (data == null)
				log.Warn ("Data null, state " + s);
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

		/**
		 * @return Actual state string representation
		 */
		public override string ToString ()
		{
			return actual_state.ToString();
		}
	}
}

