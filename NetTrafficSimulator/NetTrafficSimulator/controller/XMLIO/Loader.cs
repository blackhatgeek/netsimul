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
			   throw new IOException("File doesn't exist");
			FileStream fs = new FileStream (fname, FileMode.Open,FileAccess.Read);
			XmlReader model = XmlReader.Create (fs, settings);
			XmlDocument xd = new XmlDocument ();
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
					nm.SetNodeName (i, node.Attributes.GetNamedItem ("name").Value);
					nm.SetNodeAddr (i, Convert.ToInt32(node.Attributes.GetNamedItem ("address").Value));
					break;
				case "network":
					nm.SetNodeType (i, NetworkModel.NETWORK_NODE);
					nm.SetNodeName (i, node.Attributes.GetNamedItem ("name").Value);
					nm.SetNodeAddr (i, Convert.ToInt32(node.Attributes.GetNamedItem ("address").Value));
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
				int capa = Convert.ToInt32(link.Attributes.GetNamedItem ("capacity"));
				//verifikace
				if (nm.HaveNode (n1) && nm.HaveNode (n2)) {
					nm.SetConnected (nm.GetNodeNum (n1), nm.GetNodeNum (n2), capa);
					nm.SetLinkName (nm.GetNodeNum (n1), nm.GetNodeNum (n2), name);
				}else 
					break;
			}
			return nm;
		}

		public SimulationModel LoadSM(){
			SimulationModel sm = new SimulationModel ();
			XmlElement ttr = xd.GetElementsByTagName ("simulation").Item(0) as XmlElement;
			sm.Time=Convert.ToInt32(ttr.GetAttribute ("time_run"));
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