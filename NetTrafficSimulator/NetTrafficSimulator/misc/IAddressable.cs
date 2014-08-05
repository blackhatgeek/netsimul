using System;

namespace NetTrafficSimulator
{
	/**
	 * Addressable objects have Address property
	 */
	public interface IAddressable
	{
		/**
		 * The address of the addressable
		 */
		int Address {
			get;
		}
	}
}

