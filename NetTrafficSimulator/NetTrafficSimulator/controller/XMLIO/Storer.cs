using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace NetTrafficSimulator
{
	/**
	 * XMLIO Storer stores ResultModel data into XML file
	 */
	public class Storer
	{
		XmlDocument xs;
		XmlTextWriter xw;
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
			xs.Schemas.Add ("http://ms.mff.cuni.cz/~mansuroa/netsimul/result/v0", "result.xsd");
		}

		public void StoreResultModel(ResultModel rm){
			if (rm != null) {
				XmlElement simulation = xs.CreateElement ("simulation");
				xs.AppendChild (simulation);

				XmlNode result = xs.CreateElement ("result");

				XmlAttribute version = xs.CreateAttribute ("version");
				version.Value = "0.00";
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
						percentTimeIdle.Value = rm.GetEndNodePercentTimeIdle (endNodeName)+"";
						endNode.Attributes.Append (percentTimeIdle);

						XmlAttribute averageWaitTime = xs.CreateAttribute ("averageWaitTime");
						averageWaitTime.Value = rm.GetEndNodeAverageWaitTime (endNodeName)+"";
						endNode.Attributes.Append (averageWaitTime);

						XmlAttribute averagePacketSize = xs.CreateAttribute ("averagePacketSize");
						averagePacketSize.Value = rm.GetEndNodeAveragePacketSize (endNodeName)+"";
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

						XmlAttribute percentageTimeIdle = xs.CreateAttribute ("percentageTimeIdle");
						percentageTimeIdle.Value = rm.GetServerNodePercentTimeIdle (serverNodeName)+"";
						serverNode.Attributes.Append (percentageTimeIdle);

						XmlAttribute averageWaitTime = xs.CreateAttribute ("averageWaitTime");
						averageWaitTime.Value = rm.GetServerNodeAverageWaitTime (serverNodeName)+"";
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
						percentTimeIdle.Value = rm.GetNetworkNodePercentTimeIdle (networkNodeName) + "";
						networkNode.Attributes.Append (percentTimeIdle);

						XmlAttribute averageWaitTime = xs.CreateAttribute ("averageWaitTime");
						averageWaitTime.Value = rm.GetNetworkNodeAverageWaitTime (networkNodeName) + "";
						networkNode.Attributes.Append (averageWaitTime);

						networkNodes.AppendChild (networkNode);
					}
					result.AppendChild (networkNodes);

					XmlElement links = xs.CreateElement ("links");
					Console.WriteLine ("Links: " + rm.LinkNames.GetLength (0));
					foreach (string linkName in rm.LinkNames) {
						//Console.WriteLine (linkName);
						XmlElement link = xs.CreateElement ("link");

						XmlAttribute name = xs.CreateAttribute("name");
						name.Value = linkName;
						link.Attributes.Append (name);

						XmlAttribute packetsCarried = xs.CreateAttribute ("packetsCarried");
						packetsCarried.Value = rm.GetLinkPacketsCarried (linkName)+"";
						link.Attributes.Append (packetsCarried);

						XmlAttribute packetsDropped = xs.CreateAttribute ("packetsDropped");
						packetsDropped.Value = rm.GetLinkPacketsDropped (linkName)+"";
						link.Attributes.Append (packetsDropped);

						XmlAttribute dropPercentage = xs.CreateAttribute ("dropPercentage");
						dropPercentage.Value = rm.GetLinkDropPercentage (linkName)+"";
						link.Attributes.Append (dropPercentage);

						XmlAttribute activeTime = xs.CreateAttribute ("activeTime");
						activeTime.Value = rm.GetLinkActiveTime (linkName)+"";
						link.Attributes.Append (activeTime);

						XmlAttribute passiveTime = xs.CreateAttribute ("passiveTime");
						passiveTime.Value = rm.GetLinkPassiveTime (linkName)+"";
						link.Attributes.Append (passiveTime);

						XmlAttribute timeIdle = xs.CreateAttribute ("percentTimeIdle");
						timeIdle.Value = rm.GetLinkIdleTimePercentage (linkName)+"";
						link.Attributes.Append (timeIdle);

						links.AppendChild (link);
					}
					result.AppendChild (links);
				}

				simulation.AppendChild (result);
				xs.WriteContentTo (xw);
				xw.Flush ();
				xw.Close();
			}
		}
	}
}

