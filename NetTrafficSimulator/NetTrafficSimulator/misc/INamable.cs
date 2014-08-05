using System;

namespace NetTrafficSimulator
{
	/**
	 * Namable objects have Name property
	 */
	public interface INamable
	{
		/**
		 * Name of the Namable object instance
		 */ 
		string Name {
			get;
		}
	}
}

