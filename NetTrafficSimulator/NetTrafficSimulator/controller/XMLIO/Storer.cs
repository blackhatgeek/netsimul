
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
		/**
		 */
		public const int decimals=2;

		/**
		 * Create new storer - prepare to write to file and create necessary objects
		 */ 
		public Storer(string fname){
			XmlWriterSettings xws = new XmlWriterSettings ();
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
				version.Value = "0.04";
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

						XmlAttribute packetsMalreceived = xs.CreateAttribute ("packetsMisrouted");
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

					if (rm.GetPacketTraces ().Length != 0) {
						log.Debug ("Processing packet traces");
						XmlElement traces = xs.CreateElement ("packetTraces");
						LinkedList<KeyValuePair<string,int>>[] arr_traces = rm.GetPacketTraces ();
						for(int i=0;i<arr_traces.Length;i++){
							log.Debug ("Processing "+i+". traced packet");
							XmlElement trace = xs.CreateElement ("packet");
							LinkedList<KeyValuePair<string,int>> list_trace = arr_traces[i];
							log.Debug ("Steps recorded: " + list_trace.Count);
							foreach (KeyValuePair<string,int> step_rec in list_trace) {
								XmlElement step = xs.CreateElement ("step");

								XmlAttribute name = xs.CreateAttribute ("name");
								name.Value = step_rec.Key;
								step.Attributes.Append (name);

								XmlAttribute time = xs.CreateAttribute ("time");
								time.Value = step_rec.Value + "";
								step.Attributes.Append (time);

								trace.AppendChild (step);
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

		/**
		 * Saves network and simulation models (generated in GUI) to XML file
		 * @param nm NetworkModel
		 * @param sm SimulationModel
		 */
		public void StoreModel(NetworkModel nm,SimulationModel sm){
			if ((nm != null) && (sm != null)) {
				log.Debug ("Create simulation el");
				XmlElement simulation = xs.CreateElement ("simulation");
				xs.AppendChild (simulation);
				XmlAttribute xmlns = xs.CreateAttribute ("xmlns");
				xmlns.Value = "http://ms.mff.cuni.cz/~mansuroa/netsimul/model/v0";
				simulation.Attributes.Append (xmlns);
				XmlAttribute xmlnsxsi = xs.CreateAttribute ("xmlns:xsi");
				xmlnsxsi.Value = "http://www.w3.org/2001/XMLSchema-instance";
				simulation.Attributes.Append (xmlnsxsi);
				XmlAttribute xsischemaloc = xs.CreateAttribute("xsi:schemaLocation","http://www.w3.org/2001/XMLSchema-instance");
				xsischemaloc.Value = "http://ms.mff.cuni.cz/~mansuroa/netsimul/model/v0 model.xsd";
				simulation.Attributes.Append (xsischemaloc);

				log.Debug ("TTR");
				XmlAttribute time_run = xs.CreateAttribute ("time_run");
				time_run.Value = sm.Time + "";
				simulation.Attributes.Append (time_run);

				log.Debug ("Max hop");
				XmlAttribute max_hop = xs.CreateAttribute ("max_hop");
				max_hop.Value = sm.MaxHop + "";
				simulation.Attributes.Append (max_hop);

				log.Debug ("Version");
				XmlAttribute version = xs.CreateAttribute ("version");
				version.Value = "0.06";
				simulation.Attributes.Append (version);

				log.Debug ("Model");
				XmlElement model = xs.CreateElement ("model");
				simulation.AppendChild (model);

				if (nm.NodeCount > 0) {
					log.Debug ("Nodes");
					XmlElement nodes = xs.CreateElement ("nodes");
					model.AppendChild (nodes);

					string[] node_names = nm.GetNodeNames ();
					foreach (string node in node_names) {
						log.Debug ("Name - not connected yet");
						XmlAttribute name = xs.CreateAttribute ("name");
						name.Value = node;

						switch (nm.GetNodeType (node)) {
						case NetworkModel.END_NODE:
							log.Debug ("end");
							XmlElement end = xs.CreateElement ("end");
							log.Debug ("append name");
							end.Attributes.Append (name);

							log.Debug ("address");
							XmlAttribute addr = xs.CreateAttribute ("address");
							addr.Value = nm.GetEndpointNodeAddr (node) + "";
							end.Attributes.Append (addr);

							log.Debug ("mps");
							XmlAttribute mps = xs.CreateAttribute ("mps");
							mps.Value = nm.GetEndNodeMaxPacketSize (node) + "";
							end.Attributes.Append (mps);

							log.Debug ("rt");
							XmlAttribute rt = xs.CreateAttribute ("randomTalk");
							rt.Value = (sm.IsRandomTalker (node) + "").ToLower();
							end.Attributes.Append (rt);

							log.Debug ("append end");
							nodes.AppendChild (end);
							break;
						case NetworkModel.SERVER_NODE:
							log.Debug ("server");
							XmlElement server = xs.CreateElement ("server");
							log.Debug ("append name");
							server.Attributes.Append (name);

							log.Debug ("address");
							XmlAttribute addr1 = xs.CreateAttribute ("address");
							addr1.Value = nm.GetEndpointNodeAddr (node) + "";
							server.Attributes.Append (addr1);

							log.Debug ("append server");
							nodes.AppendChild (server);
							break;
						case NetworkModel.NETWORK_NODE:
							log.Debug ("network");
							XmlElement network = xs.CreateElement ("network");
							log.Debug ("append name");
							network.Attributes.Append (name);

							log.Debug ("default");
							XmlAttribute der = xs.CreateAttribute ("default");
							der.Value = nm.GetNetworkNodeDefaultRoute (node);
							network.Attributes.Append (der);

							log.Debug("append network");
							nodes.AppendChild (network);
							break;
						}
					}
				}

				if (nm.GetLinkCount () > 0) {
					log.Debug("links");
					XmlElement links = xs.CreateElement ("links");
					model.AppendChild (links);

					string[] link_names = nm.GetLinkNames ();
					foreach (string link in link_names) {
						log.Debug("link");
						XmlElement l = xs.CreateElement ("link");
						links.AppendChild (l);

						log.Debug("name");
						XmlAttribute name = xs.CreateAttribute ("name");
						name.Value = link;
						l.Attributes.Append (name);

						log.Debug("node1");
						XmlAttribute node1 = xs.CreateAttribute ("node1");
						node1.Value = nm.GetLinkNode1 (link);
						l.Attributes.Append (node1);

						log.Debug("node2");
						XmlAttribute node2 = xs.CreateAttribute ("node2");
						node2.Value = nm.GetLinkNode2 (link);
						l.Attributes.Append (node2);

						log.Debug("capacity");
						XmlAttribute capacity = xs.CreateAttribute ("capacity");
						capacity.Value = nm.GetLinkCapacity (link) + "";
						l.Attributes.Append (capacity);

						log.Debug("tp");
						XmlAttribute tp = xs.CreateAttribute ("toggle_probability");
						tp.Value = nm.GetLinkToggleProbability (link) + "";
						l.Attributes.Append (tp);
					}
				}

				LinkedList<SimulationModel.Event> event_ar = sm.GetEvents ();
				if (event_ar.Count > 0) {
					log.Debug("events");
					XmlElement events = xs.CreateElement ("events");
					simulation.AppendChild (events);

					foreach (SimulationModel.Event e in event_ar) {
						log.Debug("event");
						XmlElement ev = xs.CreateElement ("event");
						events.AppendChild (ev);

						log.Debug("who");
						XmlAttribute who = xs.CreateAttribute ("who");
						who.Value = e.node1;
						ev.Attributes.Append (who);

						log.Debug("when");
						XmlAttribute when = xs.CreateAttribute ("when");
						when.Value = e.when + "";
						ev.Attributes.Append (when);

						log.Debug("where");
						XmlAttribute where = xs.CreateAttribute ("where");
						where.Value = e.node2;
						ev.Attributes.Append (where);

						log.Debug("size");
						XmlAttribute size = xs.CreateAttribute ("size");
						size.Value = e.size + "";
						ev.Attributes.Append (size);
					}
				}
				xs.WriteContentTo (xw);
				xw.Flush ();
				xw.Close ();
			} else
				throw new ArgumentException ("Model null");
		}
	}
}

