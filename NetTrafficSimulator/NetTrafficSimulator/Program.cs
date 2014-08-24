using System;
using System.IO;
using Gtk;
using log4net;
using log4net.Config;

namespace NetTrafficSimulator
{
	/**
	*/
	class MainClass
	{
		static readonly ILog log = LogManager.GetLogger(typeof(MainClass));
		const string USAGE="Usage:\nXML IO:\tapp xmlio <input> <output>\nGUI:\tno parameters";

		/**
		 * Loads logger configuration, based on console parameters starts GUI or XMLIO
		 */
		public static void Main (string[] args)
		{
			//nacist konfiguraci loggeru z XML pokud existuje
			if (System.IO.File.Exists ("log4net.xml"))
				XmlConfigurator.Configure (new System.IO.FileInfo ("log4net.xml"));
			else {
				//implicitni konfigurace loggeru
				const string logconf = "<log4net>\n    <!-- A1 is set to be a ConsoleAppender -->\n    <appender name=\"A1\" type=\"log4net.Appender.ConsoleAppender\">\n        <filter type=\"log4net.Filter.LevelRangeFilter\">\n\t\t<levelMin value=\"DEBUG\" />\n\t</filter>\n        <!-- A1 uses PatternLayout -->\n        <layout type=\"log4net.Layout.PatternLayout\">\n\t    <conversionPattern value=\"%5level [%thread] (%file:%line) - %message%newline\" />\n        </layout>\n    </appender>\n\n   <appender name=\"RollingFile\" type=\"log4net.Appender.RollingFileAppender\">\n        <file value=\"trasim.log\" />\n        <appendToFile value=\"true\" />\n        <maximumFileSize value=\"100KB\" />\n        <maxSizeRollBackups value=\"2\" />\n\n        <layout type=\"log4net.Layout.PatternLayout\">\n            <conversionPattern value=\"%level %thread %logger - %message%newline\" />\n        </layout>\n    </appender>\n    \n    <!-- Set root logger level to ALL and its appenders to A1  and RollingFile -->\n    <root>\n        <level value=\"ALL\" />\n        <appender-ref ref=\"A1\" />\n\t<appender-ref ref=\"RollingFile\" />\n    </root>\n</log4net>";
				XmlConfigurator.Configure(generateStreamFromString(logconf));
			}
			//pokud je konzolovy parametr XMLIO, nacist data z XML
			//jinak pustit GUI
			log.Info ("Entering application");
			try{
				if (args.Length == 3) {
					if (args [0].Equals ("xmlio")) {
						XMLIO.Simulate (args [1], args [2]);
					} else
						log.Error (USAGE);
				} else if (args.Length == 0) {
					Application.Init ();
					MainWindow win = new MainWindow ();
					win.Show ();
					Application.Run ();
				} else
					log.Error (USAGE);
			}catch(Exception e){
				log.Debug("EXCEPTION: "+e.Message+"\n"+e.StackTrace);
			}
			log.Info ("Leaving application");
		}

		/**
		 * Used to convert string based implicit log configuration to stream
		 */
		private static Stream generateStreamFromString(string s)
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}
