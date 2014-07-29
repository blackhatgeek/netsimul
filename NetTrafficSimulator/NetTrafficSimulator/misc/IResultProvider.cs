using System;
using System.Collections.Generic;

namespace NetTrafficSimulator
{
	/**
	 * ResultProvider is capable of providing results to result model
	 */
	public interface IResultProvider
	{
		/**
		 * Provide results measured during simulation for populating a result model
		 * @return Dictionary, where string is a key representing a human-readable description of measured data and object is value measured
		 */ 
		Dictionary<string,object> GetResults (MFF_NPRG031.Model model);
	}
}

