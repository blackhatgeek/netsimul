using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * XMLIO Loader loads data from XML file, does necessary verifications and creates appropriate models
	 */
	public class Loader
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Loader));
		private XmlDocument xd;
		private HashSet<string> sn;
		private Dictionary<string,bool> en;
		private Dictionary<string,string> false_default_routes;
		const string schema = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:vc=\"http://www.w3.org/2007/XMLSchema-versioning\" elementFormDefault=\"qualified\" attributeFormDefault=\"unqualified\" vc:minVersion=\"1.1\"\ntargetNamespace=\"http://ms.mff.cuni.cz/~mansuroa/netsimul/model/v0\" xmlns:ns=\"http://ms.mff.cuni.cz/~mansuroa/netsimul/model/v0\">\n\t<xs:element name=\"simulation\">\n\t\t<xs:complexType>\n\t\t\t<xs:sequence>\n\t\t\t\t<xs:element name=\"model\" maxOccurs=\"1\" minOccurs=\"1\">\n\t\t\t\t\t<xs:complexType>\n\t\t\t\t\t\t<xs:sequence>\n\t\t\t\t\t\t\t<xs:element name=\"nodes\" type=\"ns:tNode\" minOccurs=\"0\" maxOccurs=\"1\"/>\n\t\t\t\t\t\t\t<xs:element name=\"links\" type=\"ns:tLink\" minOccurs=\"0\" maxOccurs=\"1\"/>\n\t\t\t\t\t\t</xs:sequence>\n\t\t\t\t\t</xs:complexType>\n\t\t\t\t</xs:element>\n\t\t\t\t<xs:element name=\"events\" maxOccurs=\"1\" minOccurs=\"0\">\n\t\t\t\t\t<xs:complexType>\n\t\t\t\t\t\t<xs:choice>\n\t\t\t\t\t\t\t<xs:element name=\"event\" type=\"ns:tEvent\" minOccurs=\"1\" maxOccurs=\"unbounded\"/>\n\t\t\t\t\t\t</xs:choice>\n\t\t\t\t\t</xs:complexType>\n\t\t\t\t</xs:element>\n\t\t\t</xs:sequence>\n\t\t\t<xs:attribute name=\"time_run\" use=\"required\" type=\"xs:nonNegativeInteger\"/>\n\t\t\t<xs:attribute name=\"max_hop\" use=\"optional\" default=\"30\" type=\"xs:positiveInteger\"/>\n\t\t\t<xs:attribute name=\"version\" use=\"required\" type=\"xs:decimal\" fixed=\"0.04\"/>\n\t\t</xs:complexType>\n\t\t<xs:unique name=\"NodeName\">\n\t\t\t<xs:selector xpath=\"child::network/nodes\" />\n\t\t\t<xs:field xpath=\"./@name\" />\n\t\t</xs:unique>\n\t\t<xs:unique name=\"LinkName\">\n\t\t\t<xs:selector xpath=\"network/links/link\" />\n\t\t\t<xs:field xpath=\"./@name\" />\n\t\t</xs:unique>\n\t\t<xs:keyref name=\"LinkNode1\" refer=\"ns:NodeName\">\n\t\t\t<xs:selector xpath=\"network/links/link\" />\n\t\t\t<xs:field xpath=\"./@node1\" />\n\t\t</xs:keyref>\n\t\t<xs:keyref name=\"LinkNode2\" refer=\"ns:NodeName\">\n\t\t\t<xs:selector xpath=\"network/links/link\" />\n\t\t\t<xs:field xpath=\"./@node2\" />\n\t\t</xs:keyref>\n\t\t<xs:keyref name=\"EventWho\" refer=\"ns:NodeName\">\n\t\t\t<xs:selector xpath=\"events/event\" />\n\t\t\t<xs:field xpath=\"./@who\" />\n\t\t</xs:keyref>\n\t\t<xs:keyref name=\"EventWhere\" refer=\"ns:NodeName\">\n\t\t\t<xs:selector xpath=\"events/event\" />\n\t\t\t<xs:field xpath=\"./@where\" />\n\t\t</xs:keyref>\n\t\t<xs:keyref name=\"DefaultRoute\" refer=\"ns:LinkName\">\n\t\t\t<xs:selector xpath=\"network/nodes/network\"/>\n\t\t\t<xs:field xpath=\"./@default\"/>\n\t\t</xs:keyref>\n\t</xs:element>\n\t<xs:complexType name=\"tNode\">\n\t\t<xs:choice minOccurs=\"0\" maxOccurs=\"unbounded\">\n\t\t\t<xs:element name=\"server\" type=\"ns:tEndpoint\" />\n\t\t\t<xs:element name=\"end\">\n\t\t\t\t<xs:complexType>\n\t\t\t\t\t<xs:complexContent>\n\t\t\t\t\t\t<xs:extension base=\"ns:tEndpoint\">\n\t\t\t\t\t\t\t<xs:attribute name=\"mps\" type=\"xs:nonNegativeInteger\" use=\"optional\" />\n\t\t\t\t\t\t\t<xs:attribute name=\"randomTalk\" type=\"xs:boolean\" use=\"optional\" default=\"false\" />\n\t\t\t\t\t\t</xs:extension>\n\t\t\t\t\t</xs:complexContent>\n\t\t\t\t</xs:complexType>\n\t\t\t</xs:element>\n\t\t\t<xs:element name=\"network\" type=\"ns:tNetwork\" />\n\t\t</xs:choice>\n\t</xs:complexType>\n\t<xs:complexType name=\"tEndpoint\">\n\t\t<xs:attribute name=\"name\" type=\"xs:token\" use=\"required\" />\n\t\t<xs:attribute name=\"address\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n\t</xs:complexType>\n\t<xs:complexType name=\"tNetwork\">\n\t\t<xs:attribute name=\"name\" type=\"xs:token\" use=\"required\" />\n\t\t<xs:attribute name=\"default\" type=\"xs:token\" use=\"required\" />\n\t</xs:complexType>\n\t<xs:complexType name=\"tLink\">\n\t\t<xs:choice minOccurs=\"0\" maxOccurs=\"unbounded\">\n\t\t\t<xs:element name=\"link\">\n\t\t\t\t<xs:complexType>\n\t\t\t\t\t<xs:attribute name=\"name\" type=\"xs:token\" use=\"required\" />\n\t\t\t\t\t<xs:attribute name=\"node1\" type=\"xs:token\" use=\"required\" />\n\t\t\t\t\t<xs:attribute name=\"node2\" type=\"xs:token\" use=\"required\" />\n\t\t\t\t\t<xs:attribute name=\"capacity\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n\t\t\t\t\t<xs:attribute name=\"toggle_probability\" type=\"xs:decimal\" use=\"required\" />\n\t\t\t\t</xs:complexType>\n\t\t\t</xs:element>\n\t\t</xs:choice>\n\t</xs:complexType>\n\t<xs:complexType name=\"tEvent\">\n\t\t<xs:attribute name=\"who\" type=\"xs:token\" use=\"required\" />\n\t\t<xs:attribute name=\"when\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n\t\t<xs:attribute name=\"where\" type=\"xs:token\" use=\"required\" />\n\t\t<xs:attribute name=\"size\" type=\"xs:decimal\" use=\"optional\" default=\"1\" />\n\t</xs:complexType>\n</xs:schema>";


		/**
		 * <p>Initialize Loader by opening model file, validating input model file and creating necessary objects</p> 
		 */ 
		public Loader(string fname){
			//Set-up validator
			XmlReaderSettings settings = new XmlReaderSettings ();
			//Hardcoded model schema
			StringReader sr = new StringReader (schema);
			XmlSchema model_schema = XmlSchema.Read (sr, modelSchemaValidationEventHandler);
			settings.Schemas.Add (model_schema);
			settings.ValidationType = ValidationType.Schema;
			settings.ValidationEventHandler += new ValidationEventHandler (modelSchemaValidationEventHandler);

			//Load model
			if(!File.Exists(fname))
			   throw new IOException("File doesn't exist ("+fname+")");
			FileStream fs = new FileStream (fname, FileMode.Open,FileAccess.Read);
			XmlReader model = XmlReader.Create (fs, settings);
			xd = new XmlDocument ();
			xd.Load (model);
			xd.Validate (modelSchemaValidationEventHandler);

			//HashSet pro EN a SN
			en = new Dictionary<string, bool> ();
			sn = new HashSet<string> ();
		}

		/**
		 * Generate NetworkModel by parsing input model file and doing necessary validations
		 * @return NetworkModel based on XML model
		 * @throws ArgumentOutOfRangeException wrong toggle probability for link
		 */ 
		public NetworkModel LoadNM(){
			XmlElement nodes = xd.GetElementsByTagName ("nodes").Item(0) as XmlElement;
			NetworkModel nm = new NetworkModel ();
			XmlNodeList nl = nodes.ChildNodes;
			false_default_routes = new Dictionary<string, string> ();
			for (int i=0; i<nl.Count; i++) {
				XmlElement node = nl.Item (i) as XmlElement;
				string name;
				switch (node.Name) {
				case "server":
					name = node.Attributes.GetNamedItem ("name").Value;
					nm.AddNode (name, NetworkModel.SERVER_NODE);
					nm.SetEndpointNodeAddr (name, Convert.ToInt32 (node.Attributes.GetNamedItem ("address").Value));
					sn.Add (name);
					break;
				case "end":
					name = node.Attributes.GetNamedItem ("name").Value;
					nm.AddNode (name, NetworkModel.END_NODE);
					nm.SetEndpointNodeAddr (name, Convert.ToInt32 (node.Attributes.GetNamedItem ("address").Value));

					if (node.HasAttribute ("mps"))
						nm.SetEndNodeMaxPacketSize (name, Convert.ToInt32 (node.Attributes.GetNamedItem ("mps").Value));
					bool random = false;
					if (node.HasAttribute ("randomTalk"))
						random = node.Attributes.GetNamedItem ("randomTalk").Value.ToLower().Equals ("true");
					en.Add (name,random);
					break;
				case "network":
					name = node.Attributes.GetNamedItem ("name").Value;
					nm.AddNode (name, NetworkModel.NETWORK_NODE);
					string lname = node.Attributes.GetNamedItem ("default").Value;
					if (false_default_routes.ContainsKey (lname))
						throw new ArgumentException ("Link " + lname + " default route on both ends");
					false_default_routes.Add (lname, name);
					log.Debug ("Marked: " + lname + ":" + name);
					break;
				default:
					break;
				}
			}
			XmlElement links = xd.GetElementsByTagName ("links").Item(0) as XmlElement;
			nl = links.ChildNodes;
			for (int i=0; i<nl.Count; i++) {
				log.Debug ("Link #" + i);
				XmlElement link = nl.Item (i) as XmlElement;
				if (!link.Name.Equals ("link")) {
					log.Error ("Parsing links: Element name not link (link "+i+") name:"+link.Name);
					break;
				}
				string name = link.Attributes.GetNamedItem ("name").Value;
				if (name == null)
					throw new ArgumentNullException ("Link name null (link:" + i + ")");
				string n1 = link.Attributes.GetNamedItem ("node1").Value;
				if (n1 == null)
					throw new ArgumentNullException ("Link node1 null (link:" + i + ")");
				string n2 = link.Attributes.GetNamedItem ("node2").Value;
				if (n2 == null)
					throw new ArgumentNullException ("Link node2 null (link:" + i + ")");
				int capa = Convert.ToInt32(link.Attributes.GetNamedItem ("capacity").Value);
				decimal toggle = Convert.ToDecimal (link.Attributes.GetNamedItem ("toggle_probability").Value);
				//verifikace
				if ((toggle >= 0.0m) && (toggle <= 1.0m)) {
					log.Debug ("Set link: " + n1 + "<-->" + n2 + " capacity " + capa + " toggle prob. " + toggle+" link name: "+name);
					nm.SetConnected (n1, n2, name, capa, toggle);
				} else {
					throw new ArgumentOutOfRangeException ("Wrong toggle_probability " + toggle+ " for link "+name);
				} //dalsi verifikace soucast model.xsd 0.03
				//verifikace default route
				string nnode_name;
				if (false_default_routes.TryGetValue (name, out nnode_name)) {
					if ((nnode_name != n1) && (nnode_name != n2))
						throw new ArgumentException ("Default route " + name + " is not connected to the network node " + nnode_name);
					else {
						nm.SetNetworkNodeDefaultRoute (nnode_name, name);
						false_default_routes.Remove (name);
						log.Debug ("Set default route: " + nnode_name + " via "+name);
						log.Debug ("Removed " + name);
					}
				}
			}
			if (false_default_routes.Count != 0) {
				log.Error ("Unsatisfied default routes:");
				foreach (KeyValuePair<string,string> kvp in false_default_routes) {
					log.Error ("Link name:" + kvp.Key + " Network node name:" + kvp.Value);
				}
				throw new ArgumentException ("False default route exist");
			}
			return nm;
		}

		/**
		 * Generate SimulationModel by parsing input file and doing necessary validations
		 * @return SimulationModel based on XML model
		 * @throws ArgumentOutOfRangeException event size negative, event where not ServerNode, wvent who not EndNode
		 */
		public SimulationModel LoadSM(){
			log.Debug ("Enter LoadSM");
			XmlElement ttr = xd.GetElementsByTagName ("simulation").Item(0) as XmlElement;
			int time = Convert.ToInt32 (ttr.GetAttribute ("time_run"));
			int max_hop = Convert.ToInt32 (ttr.GetAttribute ("max_hop"));
			log.Debug ("Time to run: " + time + " Max hop: " + max_hop);
			XmlElement events = xd.GetElementsByTagName ("events").Item(0) as XmlElement;

			SimulationModel sm;

			if (events != null) {
				XmlNodeList nl = events.ChildNodes;

				if ((nl != null) && (ttr != null)) {
					log.Debug ("Have <simulation>, have <events>");
					sm = new SimulationModel ();

					log.Debug ("Parsing events");
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
						if (en.ContainsKey (who)) {
							if (sn.Contains (loc)) {
								if (size >= 0.0m)
									sm.SetEvent (who, loc, when, size);
								else
									throw new ArgumentOutOfRangeException ("Error parsing event: size ("+size+") negative");
							} else throw new ArgumentOutOfRangeException("Error parsing event: where ("+loc+") not ServerNode");
						}
						else
							throw new ArgumentOutOfRangeException ("Error parsing event: who (" + who + ") not EndNode");
						//Wrong packet size (" + size + ") - must not be negative OR where (" + loc + ") not ServerNode
					}
				} else 
					throw new Exception ("Model corrupt");
			} else {
				log.Debug ("No events in model");
				sm = new SimulationModel ();
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

		/**
		 * Logs warning and Error with exception in response to validation events
		 * @param sender validation event sender
		 * @param e validation event arguments
		 * @throws Exception model validation failed
		 */
		private void modelSchemaValidationEventHandler(object sender, ValidationEventArgs e)
		{
			if (e.Severity == XmlSeverityType.Warning)
			{
				log.Warn (e.Message);
			}
			else if (e.Severity == XmlSeverityType.Error)
			{
				log.Error (e.Message);
				throw new System.Xml.Schema.XmlSchemaValidationException ("Model validation failed");
			}
		}

		public static string GetSchema(){
			return schema;
		}
	}
}