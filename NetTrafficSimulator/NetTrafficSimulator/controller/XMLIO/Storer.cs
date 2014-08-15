using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Collections.Generic;
using log4net;

namespace NetTrafficSimulator
{
	/**
	 * XMLIO Storer stores ResultModel data into XML file
	 */
	public class Storer
	{
		static readonly ILog log=LogManager.GetLogger(typeof(Storer));
		XmlDocument xs;
		XmlTextWriter xw;
		const int decimals=2;

		/**
		 * Create new storer - prepare to write to file and create necessary objects
		 */ 
		public Storer(string fname){
			XmlWriterSettings xws = new XmlWriterSettings ();
			//xws.WriteEndDocumentOnClose = true;
			xws.CheckCharacters = true;
			xws.CloseOutput = true;

			if(File.Exists(fname))
			   throw new IOException("File already exists");
			FileStream fs = new FileStream (fname,FileMode.CreateNew,FileAccess.Write);
			StreamWriter sw = new StreamWriter (fs);
			//xw = XmlWriter.Create (fs,xws);
			xw = new XmlTextWriter (sw);
			xw.Formatting = Formatting.Indented;
			xw.WriteStartDocument ();
			xs = new XmlDocument ();
			//const string schema = "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:vc=\"http://www.w3.org/2007/XMLSchema-versioning\" elementFormDefault=\"qualified\" attributeFormDefault=\"unqualified\" vc:minVersion=\"1.0\"\ntargetNamespace=\"http://ms.mff.cuni.cz/~mansuroa/netsimul/result/v0\" xmlns:ns=\"http://ms.mff.cuni.cz/~mansuroa/netsimul/result/v0\">\n<xs:element name=\"simulation\">\n<xs:complexType>\n<xs:choice maxOccurs=\"1\" minOccurs=\"1\">\n<xs:element name=\"result\">\n<xs:complexType>\n<xs:sequence>\n<xs:element name=\"endNodes\" minOccurs=\"0\" maxOccurs=\"1\">\n<xs:complexType>\n<xs:choice minOccurs=\"1\" maxOccurs=\"unbounded\">\n<xs:element name=\"endNode\" type=\"ns:tEndNode\" />\n</xs:choice>\n</xs:complexType>\n</xs:element>\n<xs:element name=\"serverNodes\" minOccurs=\"0\" maxOccurs=\"1\">\n<xs:complexType>\n<xs:choice minOccurs=\"1\" maxOccurs=\"unbounded\">\n<xs:element name=\"serverNode\" type=\"ns:tServerNode\" />\n</xs:choice>\n</xs:complexType>\n</xs:element>\n<xs:element name=\"networkNodes\" minOccurs=\"0\" maxOccurs=\"1\">\n<xs:complexType>\n<xs:choice minOccurs=\"1\" maxOccurs=\"unbounded\">\n<xs:element name=\"networkNode\" type=\"ns:tNetworkNode\" />\n</xs:choice>\n</xs:complexType>\n</xs:element>\n<xs:element name=\"links\" minOccurs=\"0\" maxOccurs=\"1\">\n<xs:complexType>\n<xs:choice minOccurs=\"1\" maxOccurs=\"unbounded\">\n<xs:element name=\"link\" type=\"ns:tLink\" />\n</xs:choice>\n</xs:complexType>\n</xs:element>\n</xs:sequence>\n</xs:complexType>\n</xs:element>\n</xs:choice>\n<xs:attribute name=\"version\" use=\"required\" fixed=\"0.03\"/>\n</xs:complexType>\n</xs:element>\n<xs:complexType name=\"tEndNode\">\n<xs:attribute name=\"name\" type=\"xs:ID\" use=\"required\" />\n<xs:attribute name=\"address\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"packetsSent\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"packetsReceived\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"packetsMalreceived\" type=\"xs:timeWaited\" use=\"required\" />\n<xs:attribute name=\"percentTimeIdle\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"averageWaitTime\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"averagePacketSize\" type=\"xs:decimal\" use=\"required\" />\n</xs:complexType>\n<xs:complexType name=\"tServerNode\">\n<xs:attribute name=\"name\" type=\"xs:ID\" use=\"required\" />\n<xs:attribute name=\"address\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"packetsProcessed\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"packetsMalreceived\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"timeWaited\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"percentTimeIdle\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"averageWaitTime\" type=\"xs:decimal\" use=\"required\" />\n</xs:complexType>\n<xs:complexType name=\"tNetworkNode\">\n<xs:attribute name=\"name\" type=\"xs:ID\" use=\"required\" />\n<xs:attribute name=\"packetsProcessed\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"timeWaited\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"percentTimeIdle\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"averageWaitTime\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"packetsDropped\" type=\"xs:nonNegativeInteger\" use=\"required\"/>\n<xs:attribute name=\"percentPacketsDropped\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"routingPacketsSent\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"routingPacketsReceived\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"percentProcessedRoutingPackets\" type=\"xs:nonNegativeInteger\" use=\"required\"/>\n</xs:complexType>\n<xs:complexType name=\"tLink\">\n<xs:attribute name=\"name\" type=\"xs:ID\" use=\"required\" />\n<xs:attribute name=\"packetsCarried\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"activeTime\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"passiveTime\" type=\"xs:nonNegativeInteger\" use=\"required\" />\n<xs:attribute name=\"percentTimeIdle\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"dataCarried\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"dataPerTic\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"usage\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"dataSent\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"dataLost\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"percentDataLost\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"percentDataDelivered\" type=\"xs:decimal\" use=\"required\" />\n<xs:attribute name=\"percentLostInCarry\" type=\"xs:decimal\" use=\"required\" />\n</xs:complexType>\n</xs:schema>";
			//StringReader sr = new StringReader (schema);
			//XmlSchema model_schema = XmlSchema.Read (sr, resultSchemaValidationEventHandler);
			//xs.Schemas.Add (model_schema);
			//xs.Schemas.Add ("http://ms.mff.cuni.cz/~mansuroa/netsimul/result/v0", "result.xsd");
		}

		/**
		 * Save provided ResultModel into output file
		 * @param rm Results of simulation
		 */
		public void StoreResultModel(ResultModel rm){
			if (rm != null) {
				XmlElement simulation = xs.CreateElement ("simulation");
				xs.AppendChild (simulation);

				XmlNode result = xs.CreateElement ("result");

				XmlAttribute version = xs.CreateAttribute ("version");
				version.Value = "0.03";
				result.Attributes.Append (version);

				if (rm.EndNodeNames.Length != 0) {
					XmlElement endNodes = xs.CreateElement ("endNodes");
					foreach (string endNodeName in rm.EndNodeNames) {
						XmlElement endNode = xs.CreateElement ("endNode");
						XmlAttribute name = xs.CreateAttribute ("name");
						name.Value = endNodeName;
						endNode.Attributes.Append (name);

						XmlAttribute address = xs.CreateAttribute ("address");
						address.Value = rm.GetEndNodeAddress (endNodeName)+"";
						endNode.Attributes.Append (address);

						XmlAttribute packetsSent = xs.CreateAttribute ("packetsSent");
						packetsSent.Value = rm.GetEndNodePacketsSent (endNodeName)+"";
						endNode.Attributes.Append (packetsSent);

						XmlAttribute packetsReceived = xs.CreateAttribute ("packetsReceived");
						packetsReceived.Value = rm.GetEndNodePacketsReceived (endNodeName)+"";
						endNode.Attributes.Append (packetsReceived);

						XmlAttribute packetsMalreceived = xs.CreateAttribute ("packetsMalreceived");
						packetsMalreceived.Value = rm.GetEndNodePacketsMalreceived (endNodeName)+"";
						endNode.Attributes.Append (packetsMalreceived);

						XmlAttribute percentTimeIdle = xs.CreateAttribute ("percentTimeIdle");
						percentTimeIdle.Value = Math.Round(rm.GetEndNodePercentTimeIdle (endNodeName),decimals)+"";
						endNode.Attributes.Append (percentTimeIdle);

						XmlAttribute averageWaitTime = xs.CreateAttribute ("averageWaitTime");
						averageWaitTime.Value = Math.Round(rm.GetEndNodeAverageWaitTime (endNodeName),decimals)+"";
						endNode.Attributes.Append (averageWaitTime);

						XmlAttribute averagePacketSize = xs.CreateAttribute ("averagePacketSize");
						averagePacketSize.Value = Math.Round(rm.GetEndNodeAveragePacketSize (endNodeName),decimals)+"";
						endNode.Attributes.Append (averagePacketSize);

						endNodes.AppendChild (endNode);
					}
					result.AppendChild (endNodes);

					XmlElement serverNodes = xs.CreateElement ("serverNodes");
					foreach (string serverNodeName in rm.ServerNodeNames) {
						XmlElement serverNode = xs.CreateElement ("serverNode");

						XmlAttribute name = xs.CreateAttribute ("name");
						name.Value = serverNodeName;
						serverNode.Attributes.Append (name);

						XmlAttribute address = xs.CreateAttribute ("address");
						address.Value = rm.GetServerNodeAddress (serverNodeName)+"";
						serverNode.Attributes.Append (address);

						XmlAttribute packetsProcessed = xs.CreateAttribute ("packetsProcessed");
						packetsProcessed.Value = rm.GetServerNodePacketsProcessed (serverNodeName) + "";
						serverNode.Attributes.Append (packetsProcessed);

						XmlAttribute packetsMalreceived = xs.CreateAttribute ("packetsMalreceived");
						packetsMalreceived.Value = rm.GetServerNodePacketsMalreceived (serverNodeName)+"";
						serverNode.Attributes.Append (packetsMalreceived);

						XmlAttribute timeWaited = xs.CreateAttribute ("timeWaited");
						timeWaited.Value = rm.GetServerNodeTimeWaited (serverNodeName)+"";
						serverNode.Attributes.Append (timeWaited);

						XmlAttribute percentageTimeIdle = xs.CreateAttribute ("percentTimeIdle");
						percentageTimeIdle.Value = Math.Round(rm.GetServerNodePercentTimeIdle (serverNodeName),decimals)+"";
						serverNode.Attributes.Append (percentageTimeIdle);

						XmlAttribute averageWaitTime = xs.CreateAttribute ("averageWaitTime");
						averageWaitTime.Value = Math.Round(rm.GetServerNodeAverageWaitTime (serverNodeName),decimals)+"";
						serverNode.Attributes.Append (averageWaitTime);

						serverNodes.AppendChild (serverNode);
					}
					result.AppendChild (serverNodes);

					XmlElement networkNodes = xs.CreateElement ("networkNodes");
					foreach (string networkNodeName in rm.NetworkNodeNames) {
						XmlElement networkNode = xs.CreateElement ("networkNode");

						XmlAttribute name = xs.CreateAttribute ("name");
						name.Value = networkNodeName;
						networkNode.Attributes.Append (name);

						XmlAttribute packetsProcessed = xs.CreateAttribute ("packetsProcessed");
						packetsProcessed.Value = rm.GetNetworkNodePacketsProcessed(networkNodeName) + "";
						networkNode.Attributes.Append (packetsProcessed);

						XmlAttribute timeWaited = xs.CreateAttribute ("timeWaited");
						timeWaited.Value = rm.GetNetworkNodeTimeWaited (networkNodeName)+"";
						networkNode.Attributes.Append (timeWaited);

						XmlAttribute percentTimeIdle = xs.CreateAttribute ("percentTimeIdle");
						percentTimeIdle.Value = Math.Round(rm.GetNetworkNodePercentTimeIdle (networkNodeName),decimals) + "";
						networkNode.Attributes.Append (percentTimeIdle);

						XmlAttribute averageWaitTime = xs.CreateAttribute ("averageWaitTime");
						averageWaitTime.Value = Math.Round(rm.GetNetworkNodeAverageWaitTime (networkNodeName),decimals) + "";
						networkNode.Attributes.Append (averageWaitTime);

						XmlAttribute packetsDropped = xs.CreateAttribute ("packetsDropped");
						packetsDropped.Value = rm.GetNetworkNodePacketsDropped (networkNodeName) + "";
						networkNode.Attributes.Append (packetsDropped);

						XmlAttribute percentPacketsDropped = xs.CreateAttribute ("percentPacketsDropped");
						percentPacketsDropped.Value = Math.Round(rm.GetNetworkNodePercentagePacketsDropped (networkNodeName),decimals) + "";
						networkNode.Attributes.Append (percentPacketsDropped);

						XmlAttribute routingPacketsSent = xs.CreateAttribute ("routingPacketsSent");
						routingPacketsSent.Value = rm.GetNetworkNodeRoutingPacketsSent (networkNodeName) + "";
						networkNode.Attributes.Append (routingPacketsSent);

						XmlAttribute routingPacketsReceived = xs.CreateAttribute ("routingPacketsReceived");
						routingPacketsReceived.Value = rm.GetNetworkNodeRoutingPacketsReceived (networkNodeName) + "";
						networkNode.Attributes.Append(routingPacketsReceived);

						XmlAttribute percentageProcessedRoutingPackets = xs.CreateAttribute ("percentProcessedRoutingPackets");
						percentageProcessedRoutingPackets.Value = Math.Round(rm.GetNetworkNodePercentageRoutingPackets (networkNodeName),decimals) + "";
						networkNode.Attributes.Append (percentageProcessedRoutingPackets);

						networkNodes.AppendChild (networkNode);
					}
					result.AppendChild (networkNodes);

					XmlElement links = xs.CreateElement ("links");
					foreach (string linkName in rm.LinkNames) {
						//Console.WriteLine (linkName);
						XmlElement link = xs.CreateElement ("link");

						XmlAttribute name = xs.CreateAttribute("name");
						name.Value = linkName;
						link.Attributes.Append (name);

						XmlAttribute packetsCarried = xs.CreateAttribute ("packetsCarried");
						packetsCarried.Value = rm.GetLinkPacketsCarried (linkName)+"";
						link.Attributes.Append (packetsCarried);

						XmlAttribute activeTime = xs.CreateAttribute ("activeTime");
						activeTime.Value = rm.GetLinkActiveTime (linkName)+"";
						link.Attributes.Append (activeTime);

						XmlAttribute passiveTime = xs.CreateAttribute ("passiveTime");
						passiveTime.Value = rm.GetLinkPassiveTime (linkName)+"";
						link.Attributes.Append (passiveTime);

						XmlAttribute timeIdle = xs.CreateAttribute ("percentTimeIdle");
						timeIdle.Value = Math.Round(rm.GetLinkIdleTimePercentage (linkName),decimals)+"";
						link.Attributes.Append (timeIdle);

						XmlAttribute dataCarried = xs.CreateAttribute("dataCarried");
						dataCarried.Value = Math.Round(rm.GetLinkDataCarried (linkName),decimals) + "";
						link.Attributes.Append (dataCarried);

						XmlAttribute dataPerTic = xs.CreateAttribute ("dataPerTic");
						dataPerTic.Value = Math.Round(rm.GetLinkDataPerTic (linkName),decimals) + "";
						link.Attributes.Append (dataPerTic);

						XmlAttribute usage = xs.CreateAttribute ("usage");
						usage.Value = Math.Round(rm.GetLinkUsage (linkName),decimals)+"";
						link.Attributes.Append (usage);

						XmlAttribute dataSent = xs.CreateAttribute ("dataSent");
						dataSent.Value = Math.Round(rm.GetLinkDataSent (linkName),decimals)+"";
						link.Attributes.Append (dataSent);

						XmlAttribute dataLost = xs.CreateAttribute ("dataLost");
						dataLost.Value = Math.Round(rm.GetLinkDataLost (linkName),decimals)+"";
						link.Attributes.Append (dataLost);

						XmlAttribute percentageDataLost = xs.CreateAttribute ("percentDataLost");
						percentageDataLost.Value = Math.Round(rm.GetLinkPercentageDataLost (linkName),decimals)+"";
						link.Attributes.Append (percentageDataLost);

						XmlAttribute percentageDataDelivered = xs.CreateAttribute ("percentDataDelivered");
						percentageDataDelivered.Value = Math.Round(rm.GetLinkPercentageDataDelivered (linkName),decimals)+"";
						link.Attributes.Append (percentageDataDelivered);

						XmlAttribute lostInCarry = xs.CreateAttribute ("percentLostInCarry");
						lostInCarry.Value = Math.Round(rm.GetLinkPercentageDataLostInCarry (linkName),decimals) + "";
						link.Attributes.Append (lostInCarry);

						links.AppendChild (link);
					}
					result.AppendChild (links);

					if (rm.GetPacketTraces ().Count != 0) {
						log.Debug ("Processing packet traces");
						XmlElement traces = xs.CreateElement ("packetTraces");
						LinkedList<LinkedList<KeyValuePair<Node,int>>> list_traces = rm.GetPacketTraces ();
						while (list_traces.Count != 0) {
							log.Debug ("Processing traced packet");
							XmlElement trace = xs.CreateElement ("packet");
							LinkedListNode<LinkedList<KeyValuePair<Node,int>>> list_trace_node = list_traces.First;
							LinkedList<KeyValuePair<Node,int>> list_trace = list_trace_node.Value;
							while (list_trace.Count>0) {
								XmlElement step = xs.CreateElement ("step");
								XmlAttribute name = xs.CreateAttribute ("name");
								name.Value = list_trace.First.Value.Key.Name;
								step.Attributes.Append (name);
								XmlAttribute time = xs.CreateAttribute ("time");
								time.Value = list_trace.First.Value.Value + "";
								step.Attributes.Append (time);
								trace.AppendChild (step);
								list_trace.RemoveFirst ();
								log.Debug ("\tProcessed traced step");
							}
							log.Debug ("Processed traced packet");
							traces.AppendChild (trace);
						}
						result.AppendChild (traces);
					}
				}

				simulation.AppendChild (result);
				xs.WriteContentTo (xw);
				xw.Flush ();
				xw.Close();
			}
		}

		
		private void resultSchemaValidationEventHandler(object sender, ValidationEventArgs e)
		{
			if (e.Severity == XmlSeverityType.Warning)
			{
				log.Warn (e.Message);
			}
			else if (e.Severity == XmlSeverityType.Error)
			{
				log.Error (e.Message);
				throw new Exception ("Result validation failed");
			}
		}
	}
}

