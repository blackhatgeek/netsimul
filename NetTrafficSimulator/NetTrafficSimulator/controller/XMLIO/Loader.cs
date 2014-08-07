using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace NetTrafficSimulator
{
	/**
	 * XMLIO Loader loads data from XML file and creates appropriate models
	 */
	public class Loader
	{
		private XmlDocument xd;

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
		}

		public NetworkModel LoadNM(){
			XmlElement nodes = xd.GetElementsByTagName ("nodes").Item(0) as XmlElement;
			NetworkModel nm = new NetworkModel (nodes.ChildNodes.Count);
			XmlNodeList nl = nodes.ChildNodes;
			for (int i=0; i<nl.Count; i++) {
				XmlElement node = nl.Item (i) as XmlElement;
				switch (node.Name) {
				case "server":
					nm.SetNodeType (i, NetworkModel.SERVER_NODE);
					nm.SetNodeName (i, node.Attributes.GetNamedItem ("name").Value);
					nm.SetNodeAddr (i, Convert.ToInt32(node.Attributes.GetNamedItem ("address").Value));
					break;
				case "end":
					nm.SetNodeType (i, NetworkModel.END_NODE);
					string name = node.Attributes.GetNamedItem ("name").Value;
					nm.SetNodeName (i, name);
					nm.SetNodeAddr (i, Convert.ToInt32 (node.Attributes.GetNamedItem ("address").Value));
					if(node.HasAttribute("mps"))
						nm.SetEndNodeMaxPacketSize(name,Convert.ToInt32(node.Attributes.GetNamedItem("mps").Value));
					break;
				case "network":
					nm.SetNodeType (i, NetworkModel.NETWORK_NODE);
					nm.SetNodeName (i, node.Attributes.GetNamedItem ("name").Value);
					//nm.SetNodeAddr (i, Convert.ToInt32(node.Attributes.GetNamedItem ("address").Value));
					break;
				default:
					break;
				}
			}
			XmlElement links = xd.GetElementsByTagName ("links").Item(0) as XmlElement;
			nl = links.ChildNodes;
			for (int i=0; i<nl.Count; i++) {
				XmlElement link = nl.Item (i) as XmlElement;
				if (!link.Name.Equals ("link"))
					break;
				string name = link.Attributes.GetNamedItem ("name").Value;
				string n1 = link.Attributes.GetNamedItem ("node1").Value;
				string n2 = link.Attributes.GetNamedItem ("node2").Value;
				int capa = Convert.ToInt32(link.Attributes.GetNamedItem ("capacity").Value);
				decimal toggle = Convert.ToDecimal (link.Attributes.GetNamedItem ("toggle_probability"));
				//verifikace
				if (nm.HaveNode (n1) && nm.HaveNode (n2) && (toggle >= 0.0m) && (toggle <= 1.0m)) {
					nm.SetConnected (nm.GetNodeNum (n1), nm.GetNodeNum (n2), capa, toggle);
					nm.SetLinkName (nm.GetNodeNum (n1), nm.GetNodeNum (n2), name);
				} else {
					throw new ArgumentOutOfRangeException ("Wrong link " + n1 + " & " + n2 + " with toggle_probability " + toggle);
				}
			}
			return nm;
		}

		public SimulationModel LoadSM(){
			SimulationModel sm = new SimulationModel ();
			XmlElement ttr = xd.GetElementsByTagName ("simulation").Item(0) as XmlElement;
			sm.Time = Convert.ToInt32 (ttr.GetAttribute ("time_run"));
			sm.MaxHop = Convert.ToInt32 (ttr.GetAttribute ("max_hop"));
			return sm;
		}

		private void modelSchemaValidationEventHandler(object sender, ValidationEventArgs e)
		{
			if (e.Severity == XmlSeverityType.Warning)
			{
				Console.Write("WARNING: ");
				Console.WriteLine(e.Message);
			}
			else if (e.Severity == XmlSeverityType.Error)
			{
				Console.Write("ERROR: ");
				Console.WriteLine(e.Message);
			}
		}
	}
}