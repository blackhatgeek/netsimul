using System;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	public class RoutingTable
	{
		static readonly ILog log=LogManager.GetLogger(typeof(RoutingTable));
		/**
		 * Routing table
		 * List of links - paths
		 * List of addresses - destinations
		 * Allowing to get best route for given address in O(1)
		 * Allowing inserting records and removing links in linear time
		 */
		public RoutingTable ()
		{
			addrList = new Dictionary<int, Record> ();
			linkList = new Dictionary<Link, Record> ();
		}

		/**
		 * Dual linked list: linkList for addresses reachable using given link (left - right) and addrList for routes for given address (up - down) 
		 * sorted by metric from best (smallest) to worst (biggest)
		 */
		class Record{
			private Link link;
			private int metric;
			private Record addrLeft,addrRight,linkLeft,linkRight;
			public Record(Link l,int metric){
				this.link=l;
				this.metric=metric;
			}
			public Link Link{
				get{
					return link;
				}
			}
			public int Metric{
				get{
					return metric;
				}
			}
			public Record AddrUp{
				get{
					return addrLeft;
				}
				set{
					addrLeft = value;
				}
			}

			public Record AddrDown{
				get{
					return addrRight;
				}
				set{
					addrRight = value;
				}
			}

			public Record LinkLeft{
				get{
					return linkLeft;
				}
				set{
					linkLeft=value;
				}
			}

			public Record LinkRight{
				get{
					return linkRight;
				}
				set{
					linkRight = value;
				}
			}
		}

		Dictionary<int,Record> addrList;
		Dictionary<Link,Record> linkList;

		/**
		 * Get best route for give address
		 * @param addr destination
		 * @return route
		 * @throws ArgumentException address not found
		 */
		public Link GetLinkForAddress(int addr){
			Record addrRec;
			if (addrList.TryGetValue (addr, out addrRec)) {
				if (addrRec != null)
					return addrRec.Link;
				else {
					log.Warn ("No link available for " + addr);
					return null;
				}
			} else {
				log.Debug (addrList.Count + "");
				log.Debug (addrList.ContainsKey (addr));
				throw new ArgumentException ("Address not found "+addr);
			}
		}

		/**
		 * Set new record to given address using given link, weighted by metric
		 * @param addr destination address
		 * @param l route
		 * @param metric metric
		 */ 
		public void SetRecord(int addr,Link l,int metric){
			log.Debug ("New record: " + addr + " via " + l.Name + " (" + metric+")");
			Record newRec, addrRec;
			newRec = new Record (l, metric);
			if (addrList.TryGetValue (addr, out addrRec)) {
				addrList.Remove (addr);
				if (addrRec != null) {
					if (addrRec.Metric >= metric) {
						//moje metrika je nejmensi
						newRec.AddrDown = addrRec;
						addrRec.AddrUp = newRec;
						addrList.Add (addr, newRec);
						log.Debug ("Best route, following " + addrRec.Link.Name);
					} else {
						//najit misto
						Record head = addrRec;
						while (addrRec.Metric<metric) {
							//klesame
							if (addrRec.AddrDown != null)
								addrRec = addrRec.AddrDown;
							else //konec
								break;
						}
						//vlozit
						if (addrRec.AddrDown == null) {
							addrRec.AddrDown = newRec;
							newRec.AddrUp = addrRec;
							log.Debug ("Worst route, after " + newRec.AddrUp.Link.Name);
						} else {
							newRec.AddrUp = addrRec.AddrUp;
							newRec.AddrUp.AddrDown = newRec;
							newRec.AddrDown = addrRec;
							addrRec.AddrUp = newRec;
							log.Debug ("Between " + newRec.AddrUp.Link.Name + " and " + newRec.AddrDown.Link.Name);
						}
						addrList.Add (addr, head);
					}
				} else {//nova adresa
					addrList.Add (addr, newRec);
					log.Debug ("First route");
				}
			} else {//nova adresa
				log.Debug ("Add to addrlist " + addr + " " + newRec.Link.Name + " " + newRec.Metric);
				addrList.Add (addr, newRec);
				log.Debug ("New destination "+addr);
			}
			//links
			Record linkR;
			if (linkList.TryGetValue (l, out linkR)) {
				linkList.Remove (l);
				if (linkR != null) {
					newRec.LinkRight = linkR;
					linkR.LinkLeft = newRec;
				}
				linkList.Add (l, newRec);
			} else
				linkList.Add (l, newRec);
			log.Debug ("Addr list count: " + addrList.Count + "\t Link list count: " + linkList.Count);
		}

		/**
		 * Remove all destinations reachable through particular link
		 * @param link Link no longer available
		 */
		public void RemoveLink(Link l){
			log.Debug ("Removing link " + l.Name + " from routing table");
			Record linkR;
			if (linkList.TryGetValue (l, out linkR)) {
				//projit seznamem vpravo do konce a popremostovat addr
				while (linkR!=null) {
					if (linkR.AddrUp != null)
						linkR.AddrUp.AddrDown = linkR.AddrDown;
					if (linkR.AddrDown != null)
						linkR.AddrDown.AddrUp = linkR.AddrUp;
					linkR.AddrDown = null;
					linkR.AddrUp = null;
					linkR = linkR.LinkRight;
					if (linkR != null)
						linkR.LinkLeft = null;
				}
			} else
				log.Warn ("Link doesn't exist in our table");
				//throw new Exception ("Link doesn't exist in our table");
		}
	}
	
}

