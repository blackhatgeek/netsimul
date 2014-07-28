using System;

namespace MFF_NPRG031
{
	public class State
	{
		//SEND,WAIT,RECEIVE,TOGGLE
		public enum state{
			SEND,RECEIVE
		}

		private state actual_state;
		private NetTrafficSimulator.Packet additionalData;
		public State(state s){
			this.actual_state = s;
			additionalData = null;
		}

		public State(state s,NetTrafficSimulator.Packet data):this(s){
			this.additionalData = data;
		}

		public state Actual{
			get{
				return this.actual_state;
			}
		}

		public NetTrafficSimulator.Packet Data{
			get{
				return additionalData;
			}
		}
	}
}

