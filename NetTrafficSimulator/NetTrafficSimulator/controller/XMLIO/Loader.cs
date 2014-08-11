using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * XMLIO Loader loads data from XML file and creates appropriate models
	 */
	public class Loader
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Loader));
		private XmlDocument xd;
		private HashSet<string> sn;
		private Dictionary<string,bool> en;

		public Loader(string fname){
			//Set-up validator
			XmlReaderSettings settings = new XmlReaderSettings ();
			settings.Schemas.Add ("http://ms.mff.cuni.cz/~mansuroa/netsimul/model/v0", "model.xsd");
			settings.ValidationType = ValidationType.Schema;
			settings.ValidationEventHandler += new ValidationEventHandler (modelSchemaValidationEventHandler);

			//Load model
			if(!File.Exists(fname))
			   throw new IOException("File doesn't exist ("+fname+")");
			FileStream fs = new FileStream (fname, FileMode.Open,FileAccess.Read);
			XmlReader model = XmlReader.Create (fs, settings);
			xd = new XmlDocument ();
			xd.Load (model);

			//HashSet pro EN a SN
			en = new Dictionary<string, bool> ();
			sn = new HashSet<string> ();
		}

		public NetworkModel LoadNM(){
			XmlElement nodes = xd.GetElementsByTagName ("nodes").Item(0) as XmlElement;
			NetworkModel nm = new NetworkModel (nodes.ChildNodes.Count);
			XmlNodeList nl = nodes.ChildNodes;
			for (int i=0; i<nl.Count; i++) {
				XmlElement node = nl.Item (i) as XmlElement;
				string name;
				switch (node.Name) {
				case "server":
					name = node.Attributes.GetNamedItem ("name").Value;
					nm.SetNodeType (i, NetworkModel.SERVER_NODE);
					nm.SetNodeName (i, name);
					nm.SetNodeAddr (i, Convert.ToInt32 (node.Attributes.GetNamedItem ("address").Value));
					sn.Add (name);
					break;
				case "end":
					nm.SetNodeType (i, NetworkModel.END_NODE);
					name = node.Attributes.GetNamedItem ("name").Value;
					nm.SetNodeName (i, name);
					nm.SetNodeAddr (i, Convert.ToInt32 (node.Attributes.GetNamedItem ("address").Value));
					if (node.HasAttribute ("mps"))
						nm.SetEndNodeMaxPacketSize (name, Convert.ToInt32 (node.Attributes.GetNamedItem ("mps").Value));
					bool random = false;
					if (node.HasAttribute ("randomTalk"))
						random = node.Attributes.GetNamedItem ("randomTalk").Value.ToLower().Equals ("true");
					en.Add (name,random);
					break;
				case "network":
					nm.SetNodeType (i, NetworkModel.NETWORK_NODE);
					nm.SetNodeName (i, node.Attributes.GetNamedItem ("name").Value);
					break;
				default:
					break;
				}
			}
			XmlElement links = xd.GetElementsByTagName ("links").Item(0) as XmlElement;
			nl = links.ChildNodes;
			for (int i=0; i<nl.Count; i++) {
				XmlElement link = nl.Item (i) as XmlElement;
				if (!link.Name.Equals ("link")) {
					log.Error ("Parsing links: Element name not link (link "+i+") name:"+link.Name);
					break;
				}
				string name = link.Attributes.GetNamedItem ("name").Value;
				string n1 = link.Attributes.GetNamedItem ("node1").Value;
				string n2 = link.Attributes.GetNamedItem ("node2").Value;
				int capa = Convert.ToInt32(link.Attributes.GetNamedItem ("capacity").Value);
				decimal toggle = Convert.ToDecimal (link.Attributes.GetNamedItem ("toggle_probability").Value);
				//verifikace
				if ((toggle >= 0.0m) && (toggle <= 1.0m)) {
					nm.SetConnected (nm.GetNodeNum (n1), nm.GetNodeNum (n2), capa, toggle);
					nm.SetLinkName (nm.GetNodeNum (n1), nm.GetNodeNum (n2), name);
				} else {
					throw new ArgumentOutOfRangeException ("Wrong toggle_probability " + toggle+ " for link "+name);
				} //dalsi verifikace soucast model.xsd 0.03
			}
			return nm;
		}

		public SimulationModel LoadSM(){
			log.Debug ("Enter LoadSM");
			XmlElement ttr = xd.GetElementsByTagName ("simulation").Item(0) as XmlElement;
			int time = Convert.ToInt32 (ttr.GetAttribute ("time_run"));
			int max_hop = Convert.ToInt32 (ttr.GetAttribute ("max_hop"));
			XmlElement events = xd.GetElementsByTagName ("events").Item(0) as XmlElement;

			SimulationModel sm;

			if (events != null) {
				XmlNodeList nl = events.ChildNodes;

				if ((nl != null) && (ttr != null)) {
					log.Debug ("Passed if");
					sm = new SimulationModel (nl.Count);

					log.Debug ("Enter for");
					for (int i=0; i<nl.Count; i++) {
						XmlElement ev = nl.Item (i) as XmlElement;
						if (!ev.Name.Equals ("event")) {
							log.Error ("Parsing events: Element name not event (event " + i + ") name:" + ev.Name);
							break;
						}
						string who = ev.Attributes.GetNamedItem ("who").Value;
						int when = Convert.ToInt32 (ev.Attributes.GetNamedItem ("when").Value);
						string loc = ev.Attributes.GetNamedItem ("where").Value;
						decimal size = Convert.ToDecimal (ev.Attributes.GetNamedItem ("size").Value);
						//verifikace: who je EN, loc je SN, velikost je nezap.
						if (en.ContainsKey (who) && sn.Contains (loc) && (size >= 0.0m))
							sm.SetEvent (who, loc, when, size);
						else
							throw new ArgumentOutOfRangeException ("Wrong packet size (" + size + ") - must not be negative OR who (" + who + ") not EndNode OR loc (" + loc + ") not ServerNode");
					}
				} else 
					throw new Exception ("Model corrupt");
			} else {
				log.Debug ("No events in model");
				sm = new SimulationModel (0);
			}
			sm.Time = time;
			sm.MaxHop = max_hop;
			log.Debug ("Parsing random talkers");
			foreach (KeyValuePair<string,bool> kvp in en) {
				if (kvp.Value)
					sm.SetRandomTalker (kvp.Key);
			}
			return sm;
		}

		private void modelSchemaValidationEventHandler(object sender, ValidationEventArgs e)
		{
			if (e.Severity == XmlSeverityType.Warning)
			{
				log.Warn (e.Message);
			}
			else if (e.Severity == XmlSeverityType.Error)
			{
				log.Error (e.Message);
				throw new Exception ("Model validation failed");
			}
		}
	}
}