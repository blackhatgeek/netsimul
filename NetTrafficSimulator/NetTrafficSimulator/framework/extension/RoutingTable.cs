using System;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	* Routing table holds best routes for particular network address
	*/
	public class RoutingTable
	{
		HashSet<Record> records;
		Dictionary<int,Record> bestRoute;//addr -> record
		static readonly ILog log = LogManager.GetLogger (typeof(RoutingTable));
		readonly int flush_timer,expiry_timer,maxHop;
		readonly MFF_NPRG031.Model model;
		int activeRecs;
	
		/**
		 * Create new RoutingTable
		 * @param expiry expiry timer - after ET record is no longer send to surrounding nodes
		 * @param flush flush timer - after FT record is removed from table
		 * @param maxHop Maximum value of hop counter
		 * @param m Framework model
		 * @throws ArgumentNullException model null
		 */
		public RoutingTable(int flush,int expiry,int maxHop,MFF_NPRG031.Model m){
			if (m == null)
				throw new ArgumentNullException ("Model null");
			this.expiry_timer = expiry;

			this.flush_timer = flush;
			if (flush <= expiry)
				log.Warn ("Flush timer is supposed to be higher then expiry timer");

			this.maxHop = maxHop;

			this.records = new HashSet<Record> ();
			this.bestRoute = new Dictionary<int, Record> ();
			this.model = m;
			this.activeRecs = 0;
			this.model = m;
		}

		/**
		 * For given network address return best link known to deliver packet
		 * @param addr Network address
		 * @return best direction or null when route is expired or no route is available
		 * @throws ArgumentNullException record retrieved from routing table is null
		 */
		public Link GetLinkForAddr(int addr){
			Record r;
			if (bestRoute.TryGetValue (addr, out r)) {
				if (r != null) {
					if (!r.Expired)
						return r.Route;
					else {
						log.Warn ("Route via " + r.Route + " is expired");
						return null;
					}
				}
				else {
					throw new ArgumentNullException ("Routing record null for bestRoute to " + addr);
				}
			} else {
				log.Warn ("No route to " + addr);
				return null;
			}
		}
		/**
		 * Enters or updates a record in the routing table
		 * @param r Record to enter/update
		 * @throws ArgumentNullException record null, model null, bestRoute (actual routing table) null 
		 */
		public void SetRecord(Record r){
			if (r == null)
				throw new ArgumentNullException ("Record null");
			Record rec;
			if (model == null)
				throw new ArgumentNullException ("Model null");
			int expiry = model.Time + expiry_timer;
			int flush = model.Time + flush_timer;
			if (bestRoute == null)
				throw new ArgumentNullException ("bestRoute null");
			if (bestRoute.TryGetValue (r.Address, out rec)) {
				if (rec == null) {
					log.Warn("Got null as record - setting new");
					setRecord (r);
					log.Debug ("New record: TO " + r.Address + " VIA " + r.Route + " METRIC " + r.Metric + " EXPIRY " + expiry+" FLUSH "+flush);
				}
				if ((rec.Expired)|| (rec.Metric >= r.Metric)) {
					//update
					rec.ZrusPlan (model.K);
					records.Remove (rec);
					bestRoute.Remove (r.Address);

					setRecord (r);
					log.Debug ("Updated expired record: TO " + r.Address + " VIA " + r.Route + " METRIC " + r.Metric + " EXPIRY " + expiry+" FLUSH "+flush);
				} else {
					//no modify
					log.Debug ("Worse record: TO " + r.Address + " VIA " + r.Route + " METRIC " + r.Metric + " EXPIRY " + expiry + " FLUSH "+flush);
				}
			} else {
				//new
				setRecord (r);
				log.Debug ("New record: TO " + r.Address + " VIA " + r.Route + " METRIC " + r.Metric + " EXPIRY " + expiry+" FLUSH "+flush);
			}
		}

		/**
		 * Lists new record into Routing table and sets timers, if record is null warning is logged
		 * @param r new Record
		 * @throws ArgumentNullException framework model null
		 */
		private void setRecord(Record r){
			//r.Expired = false;
			if (model == null)
				throw new ArgumentNullException ("Framework model null");
			if (r != null) {
				if (!((r.Route.A is EndpointNode) || (r.Route.B is EndpointNode))) {
					r.Schedule (model.K, new MFF_NPRG031.State (MFF_NPRG031.State.state.INVALID_TIMER), model.Time + expiry_timer);
					r.Schedule (model.K, new MFF_NPRG031.State (MFF_NPRG031.State.state.FLUSH_TIMER), model.Time + flush_timer);
				}

				bestRoute.Add (r.Address, r);
				records.Add (r);
				activeRecs++;
			} else
				log.Warn ("Record null - not set");
		}

		/**
		 * Create new Record based on values provided and invokes SetRecord on it
		 * @param addr network address of new route
		 * @param l link to use
		 * @param metric hop count metric
		 */
		public void SetRecord(int addr,Link l, int metric){
			log.Debug ("Set record");
			SetRecord (new Record (addr, l, metric, maxHop, this));
		}

		/**
		 * Removes given record from the routing table
		 * @param r Record to remove
		 */
		public void DeleteRecord(Record r){
			Record test;
			if (bestRoute.TryGetValue (r.Address, out test)) {
				if (test == r) {
					if (!test.Expired)
						activeRecs--;
					bestRoute.Remove (r.Address);
				}
			}
		}

		/**
		 * Get records in a HashSet
		 * @return records
		 */
		public HashSet<Record> GetRecords(){
			return records;
		}

		/**
		 * Get amount of records in routing table
		 */
		public int RecordsCount{
			get{
				return bestRoute.Count;
			}
		}

		/**
		 * Get amount of active records in the routing table
		 */
		public int ActiveRecs{
			get{
				return activeRecs;
			}
		}

		/**
		 * Decrease amount of active records in the routing table
		 */
		public void DecActiveRecs(){
			activeRecs--;
		}
	}

	/**
	 * Record in the routing table
	 */
	public class Record:MFF_NPRG031.Process{
		int address;
		Link route;
		int metric;
		int maxHop;
		bool expired;
		RoutingTable rt;

		/**
		 * Create new record
		 * @param address network address of the destination
		 * @param route direction to the destination
		 * @param metric metric
		 * @param maxHop if updated metric is to be higher than maxHop it will be the maxHop value
		 * @param rt routing table the record is located in
		 */
		public Record(int address,Link route,int metric,int maxHop,RoutingTable rt){
			this.address = address;
			this.route = route;
			this.expired = false;
			this.maxHop = maxHop;
		 	this.Metric=metric;
			this.rt = rt;
		}

		/**
		 * Network address of the destination
		 */
		public int Address{
			get{
				return address;
			}
		}

		/**
		 * Direction to the destination
		 */
		public Link Route{
			get{
				return route;
			} set{
				route=value;
			}
		}

		/**
		 * Metric of the route
		 */
		public int Metric{
			get{
				return metric;
			}
			set{
				if (metric > maxHop)
					metric = maxHop;
				else
					metric = value;
			}
		}

		/**
		 * If record is expired
		 */
		public bool Expired{
			get{
				return this.expired;
			}
		}

		/**
		 * Set record as expired (it will decrease activeRecord counter in the routing table
		 */
		private void setExpired(){
			this.expired = false;
			rt.DecActiveRecs ();
		}

		public override void ProcessEvent (MFF_NPRG031.State state, MFF_NPRG031.Model model)
		{
			switch (state.Actual) {
			case MFF_NPRG031.State.state.INVALID_TIMER:
				this.setExpired ();
				break;
			case MFF_NPRG031.State.state.FLUSH_TIMER:
				this.rt.DeleteRecord (this);
				break;
			default:
				break;
			}
		}
	}
}