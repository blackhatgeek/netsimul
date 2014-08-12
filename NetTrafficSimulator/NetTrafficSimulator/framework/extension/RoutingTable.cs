using System;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	* Routing table
	* List of addresses - destinations
	* List of expiry times
	* Allowing to get best route for given address in O(1)
	* Allowing to remove remove expired records in O(n)
	* Allowing to update record in 
	*/
	public class RoutingTable
	{
		HashSet<Record> records;
		Dictionary<int,Record> bestRoute;//addr -> record
		static readonly ILog log = LogManager.GetLogger (typeof(RoutingTable));
		readonly int flush_timer,expiry_timer,maxHop;
		readonly MFF_NPRG031.Model model;
		int activeRecs;

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

		public void SetRecord(int addr,Link l, int metric){
			log.Debug ("Set record");
			SetRecord (new Record (addr, l, metric, maxHop, this));
		}

		public void DeleteRecord(Record r){
			Record test;
			if (bestRoute.TryGetValue (r.Address, out test)) {
				if (test == r)
					bestRoute.Remove (r.Address);
			}
		}

		public RoutingTable(int flush,int expiry,int maxHop,MFF_NPRG031.Model m){
			if (m == null)
				throw new ArgumentNullException ("Model null");
			this.expiry_timer = expiry;

			this.flush_timer = flush;

			this.maxHop = maxHop;

			this.records = new HashSet<Record> ();
			this.bestRoute = new Dictionary<int, Record> ();
			this.model = m;
			this.activeRecs = 0;
			this.model = m;
		}

		public HashSet<Record> GetRecords(){
			return records;
		}

		public int RecordsCount{
			get{
				return bestRoute.Count;
			}
		}

		public int ActiveRecs{
			get{
				return activeRecs;
			}
		}

		public void DecActiveRecs(){
			activeRecs--;
		}
	}

	public class Record:MFF_NPRG031.Process{
		int address;
		Link route;
		int metric;
		int maxHop;
		bool expired;
		RoutingTable rt;

		public Record(int address,Link route,int metric,int maxHop,RoutingTable rt){
			this.address = address;
			this.route = route;
			this.metric = metric;
			this.expired = false;
			this.maxHop = maxHop;
			this.rt = rt;
		}

		public int Address{
			get{
				return address;
			}
		}

		public Link Route{
			get{
				return route;
			} set{
				route=value;
			}
		}

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

		public bool Expired{
			get{
				return this.expired;
			}
		}

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