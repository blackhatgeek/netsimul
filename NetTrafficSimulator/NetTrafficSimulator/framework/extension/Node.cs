using System;

namespace NetTrafficSimulator
{
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
		 * String representation of a node is it's name
		 * @return the node name
		 */
		public override string ToString ()
		{
			return Name;
		}
	}
}

