using System;

namespace NetTrafficSimulator
{
	/**
	 * Abstract node
	 */
	public abstract class Node:MFF_NPRG031.Process,INamable
	{
		private string name;
		/**
		 * Create a new node with the human readable name specified
		 * @param name human readable name
		 */
		public Node (string name)
		{
			this.name = name;
		}

		/**
		 * Human readable name of the node
		 * @return the node name
		 */
		public string Name{
			get{
				return this.name;
			}
		}

		/**
		 */
		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			if (state != null) {
				if (state.Data != null) {
					if (state.Data.Traced)
						state.Data.SetNodePassedThrough (this, model.Time);
				}
			}
		}

		/**
		 * String representation of a node is it's name
		 * @return the node name
		 */
		public override string ToString ()
		{
			return Name;
		}
	}
}

