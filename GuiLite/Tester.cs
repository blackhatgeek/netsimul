using System;
using System.Collections.Generic;

namespace GuiLite
{
	public class TestNode:Node{
		private int proposed_result;
		public TestNode(string name,int fppt):base(name,fppt){
		}

		public void Send(EtherFrame f,Model m){
			this.SendFrame (f, m);
		}

		public int SetProposedResult{
			set{ this.proposed_result = value;}
		}

		public override int ProcessFrame (EtherFrame f, Model m)
		{
			//throw new NotImplementedException ();
			Console.WriteLine ("FRAME PROCESSED @"+m.Cas);
			return proposed_result;
		}


	}

	public static class Tester
	{
		public static void Test(){
			try{
				//basic model test
				Console.WriteLine ("Empty model test 0");
					m = new Model (null, -1);
					m.Simulace ();
				Console.WriteLine ("Empty model test 1");
					m = new Model (null, int.MaxValue);
					m.Simulace ();
				Console.WriteLine ("..");

				//node constructor & properties tests
				Tester.constructNode ();
				Tester.changeNodeAttrs ();

				//sample simulation
				Console.WriteLine ("..");
				m = new Model (constructedNodes.ToArray (), -1);
				Console.WriteLine ("Created model 0, sample simulation");
				m.Simulace ();
				Console.WriteLine ("..");
				m = new Model (constructedNodes.ToArray (), 1);
				Console.WriteLine ("Created model 1, sample simulation");
				m.Simulace ();
				Console.WriteLine ("#");

				//mac address factory
				Tester.macFactory();
				//mac address compare
				Tester.macCompare();

				//etherframe constructor
				Tester.constructFrame();
				//etherframe props
				Tester.frameProps();

				Console.WriteLine("###");

				tn=constructedNodes [0];
				tn.Name = "Test node";
				m = new Model (new Node[] { tn }, int.MinValue);
				//TestNode receive
				Tester.receive();
				//TestNode send
				Tester.send();

				//TestNode ZpravujUdalost -> stav processing
				Tester.nodeProcess1();
				Tester.nodeProcess2();
			}catch(Exception e){
				Tester.exceptionPrinter (e);
			}
		}

		private static List<TestNode> constructedNodes=new List<TestNode>();
		private static void constructNode(){
			string[] s = new string[] { null, "", "abc" };
			int[] i = new int[] { int.MaxValue, 0, 1 };
			try {
				foreach (string a in s) {
						foreach (int d in i) {
							Console.WriteLine("new TestNode("+a+","+d+");");
							constructedNodes.Add (new TestNode (a, d));
						}
				}
				try{
					Console.Write("new TestNode(,-1);\t");
					new TestNode(null,-1);
				}catch(ArgumentOutOfRangeException){
					Console.WriteLine("Not possible to create, good");
				}
			} catch (Exception e) {
				Tester.exceptionPrinter (e);

			}
		}

		private static void exceptionPrinter(Exception e){
			Console.WriteLine ("EXCEPTION\nTarget site: " + e.TargetSite + "\nSource: " + e.Source + "\nMessage: " + e.Message + "\n" + e.StackTrace+"\n\n\n");
		}

		private static void changeNodeAttrs(){
			Console.WriteLine ("Altering properties");
			foreach (Node n in constructedNodes) {
				Console.WriteLine ("FPPT\t");
				int fppt = n.Power;
				try{
					n.Power=fppt+1;
					Console.Write("*");
					n.Power=fppt-1;
					Console.Write("*");
					n.Power=fppt;
					Console.Write("*");
				}catch(Exception e){
					Console.WriteLine ("");
					Tester.exceptionPrinter (e);
				}
				Console.WriteLine ("");
			}
		}

		private static MACaddr a1,a2;
		private static void macFactory(){
			try{
				for (int i=0; i<=10; i++) {
					a1=a2;
					a2=MACaddr.Factory.Instance.GetMAC ();
					Console.WriteLine(a2);
				}
			}catch(Exception e){
				Tester.exceptionPrinter (e);
			}
			try{
				Console.WriteLine(MACaddr.Factory.Instance.GetMAC());
			}catch(MACException){
				Console.WriteLine ("MAC Address not available, good");
			}
		}
		private static void macCompare(){
			MACaddr a3 = a1;
			if ((a1 == a3) && (a1 != a2))
				Console.WriteLine ("MAC compare .. OK");
			else
				Console.WriteLine ("MAC compare .. fail");
		}
		 
		private static EtherFrame ef;
		private static void constructFrame(){
			try{
				try{
					new EtherFrame(a1,a2,null,2);
					Console.WriteLine("EtherFrame upper bound\tFail");
				}catch(ArgumentOutOfRangeException){
					Console.WriteLine("EtherFrame upper bound\tOK");
				}
				ef=new EtherFrame(a1,a2,null,-1);
				if(ef.Size==0) Console.WriteLine("EtherFrame lower bound\tOK");
				else Console.WriteLine("EtherFrame lower bound\tFail");

			}catch(Exception e){
				Tester.exceptionPrinter (e);
			}
		}
		private static void frameProps(){
			try{
				Console.WriteLine("Size: "+ef.Size+"\tChange size -> 2");
				ef.Size = 2;
				Console.WriteLine("Size: "+ef.Size+"\tChange size -> 1");
				ef.Size = 1;
				Console.WriteLine("Size: "+ef.Size+"\tChange size -> 0");
				ef.Size = 0;
				Console.WriteLine("Size: "+ef.Size+"\tChange size -> -1");
				ef.Size = -1;
				Console.WriteLine("Size: "+ef.Size);
			}catch (Exception e){
				Tester.exceptionPrinter (e);
			}
		}

		private static Model m;
		private static TestNode tn;
		private static void receive(){
			/*
			 * (a)
			 * nearest_receiving_scheduled<m.Cas
			 * OR
			 * nearest_receiving_scheduled>m.Cas+1
			 * 
			 * -> nearest_receiving_scheduled=m.Cas+1
			 * 
			 * nearest_receiving_scheduled implicitne -1
			 * scheduled implicitne nenastaveno (po init 0)
			 */
			int p = m.K.GetK ().Count;
			//Console.WriteLine ("Plan: "+p);

			m.Cas = 0;
			tn.ReceiveFrame (ef, m);//NRS=0=CAS			NRS=CAS
			m.Cas = -1;
			tn.ReceiveFrame (ef, m);//NRS=0=-1+1=CAS+1	NRS=CAS+1
			if (m.K.GetK ().Count == p)
				Console.WriteLine ("@@@\tNenaplanovani \t OK");
			else {
				Console.WriteLine ("@@@\tNenaplanovani \t FAIL");
			}
			m.Cas = 1;
			tn.ReceiveFrame (ef, m);//NRS=0<1=CAS		NRS<CAS
			m.Cas = 0;
			tn.ReceiveFrame (ef, m);//NRS=2>0+1=CAS+1	NRS>CAS+1
			if (m.K.GetK ().Count == p+2)
				Console.WriteLine ("@@@\tNaplanovani \t OK");
			else foreach (Udalost u in m.K.GetK().ToArray()) {
				Console.WriteLine ("@@@\t"+u.co+"\t"+u.kdo+"\t"+u.kdy);
			}
		}
		private static void send(){
			int p = m.K.GetK ().Count;
			m.Cas = 0;
			tn.Send (ef, m);//NSS=0=CAS			NSS=CAS
			m.Cas = -1;
			tn.Send (ef, m);//NSS=0=-1+1=CAS+1	NSS=CAS+1
			if (m.K.GetK ().Count == p)
				Console.WriteLine ("@@@\tNenaplanovani\tOK");
			else 
				Console.WriteLine("@@@\tNenaplanovani\tFAIL");
			m.Cas = 1;
			tn.Send (ef, m);
			m.Cas = 0;
			tn.Send (ef, m);
			if (m.K.GetK ().Count == p + 2)
				Console.WriteLine ("@@@\tNaplanovani\tOK");
			else
				foreach (Udalost u in m.K.GetK().ToArray()) 
					Console.WriteLine ("@@@\t" + u.co + "\t" + u.kdo + "\t" + u.kdy);

		}

		/*
		 * Zpracuj udalost
		 * 
		 * a) nodeProcess1
		 * -> PROCESSING
		 * 	q_in.Count = 0
		 *  q_in.Count > 0 && q_in.Count <= FPPT
		 *  q_in.Count > 0 && q_in.Count > FPPT
		 * 
		 * b) nodeProcess2
		 *  chceme se naplanovat na KDY protoze do te doby zpracovavame ramec!!
		 *  pokud je neco naplanovano 
		 *  NSS<KDY	..........N....C....K	N=K
		 *  NSS=CAS
		 *  NSS>CAS&NSS<KDY
		 *  NSS=KDY
		 *  NSS>KDY
		 *  
		 * -> SENDING
		 */

		//potreba test - bezi zpracovani a prijde ramec
		private static void nodeProcess1(){
			//q_in.Count == 0
			Console.WriteLine ("!!! NP");
			tn = constructedNodes.ToArray () [1];
			tn.Fin ();//zajistit q_in.count = 0
			m.Cas = 0;
			Console.WriteLine ("??$");
			tn.SetProposedResult = 0;
			tn.ZpracujUdalost (Stav.PROCESSING, m);

			//###
			//q_in.Count>0 &&q_in.Count < FPPT
			Console.WriteLine ("?%%$");
			tn.Fin ();
			tn.Power = 2;
			tn.ReceiveFrame (ef, m);//vlozit 1 ramec do q_in
			m.K.GetK ().Clear ();//cisty kalendar
			m.Cas = 0;
			tn.SetProposedResult = 1;//v kalendari ocekavam jednu udalost v case 2
			tn.ZpracujUdalost (Stav.PROCESSING, m);
			if (m.K.GetK ().Count == 1) {
				if (m.K.GetK ().ToArray () [0].kdy == 2)
					Console.WriteLine ("OK");
			} else
				Console.WriteLine ("FAIL");

			//###
			//q_in.Count>0 &&q_in.Count=FPPT
			Console.WriteLine ("*&^");
			tn.Fin ();
		//	tn.Init (m);
			tn.Power = 1;
			tn.ReceiveFrame (ef, m);
			m.K.GetK ().Clear ();
			m.Cas = 0;
			tn.SetProposedResult = 1;
			tn.ZpracujUdalost (Stav.PROCESSING, m);
			if (m.K.GetK ().Count == 1) {
				if (m.K.GetK ().ToArray () [0].kdy == 2)
					Console.WriteLine ("OK");
				else
					Console.WriteLine ("1/2");
			} else
				Console.WriteLine ("FAIL");

			//###
			//q_in.Count>0&&q_in.Count>FPPT
			Console.WriteLine ("$%^&*");
			tn.Fin ();
			//tn.Init (m);
			tn.Power = 1;
			m.Cas = 0;
			tn.ReceiveFrame (ef,m);
			tn.ReceiveFrame (ef,m);
			m.K.GetK ().Clear ();
			m.Cas = 0;
			tn.SetProposedResult = 1;
			tn.ZpracujUdalost (Stav.PROCESSING, m);//ve fronte zbyva jedna udalost??
			tn.ZpracujUdalost (Stav.PROCESSING, m);//ted fronta prazdna??
			tn.ZpracujUdalost (Stav.PROCESSING, m);//OK
		}

		//planovani
		private static void nodeProcess2(){
			//priprava
			Console.WriteLine ("#$%^&*()(*&^%$#@");
			m.Cas = 0;
			tn = constructedNodes.ToArray() [2];
			tn.Name = "Hokus pokus";
			tn.Fin ();
			//tn.Init (m);
			tn.SetProposedResult = 3;
			tn.Power = 10;
			//vypis
				Console.WriteLine ("\t###");
				foreach (Udalost u in m.K.GetK().ToArray()) {
					Console.WriteLine ("\t" + u.kdy + "\t" + u.kdo + "\t" + u.co);
				}
				Console.WriteLine ("\t###");

			//receive v case 0
			tn.ReceiveFrame (ef, m);
			//vypis
				Console.WriteLine ("\t###");
				foreach (Udalost u in m.K.GetK().ToArray()) {
					Console.WriteLine ("\t" + u.kdy + "\t" + u.kdo + "\t" + u.co);
				}
				Console.WriteLine ("\t###");

			//sending v case 2
			m.Cas = 2;
			tn.ZpracujUdalost (Stav.SENDING, m);
			//receive v case 2 ... ocekavam lock
			m.Cas = 2;
			tn.ReceiveFrame (ef, m);
			//vypis
				Console.WriteLine ("\t###");
				foreach (Udalost u in m.K.GetK().ToArray()) {
					Console.WriteLine ("\t" + u.kdy + "\t" + u.kdo + "\t" + u.co);
				}
				Console.WriteLine ("\t###");
			//1. process v case 3 - slozitost 3 (receiving)
			m.Cas = 3;
			tn.ZpracujUdalost(Stav.PROCESSING,m);
			//vypis
				Console.WriteLine ("\t###");
				foreach (Udalost u in m.K.GetK().ToArray()) {
					Console.WriteLine ("\t" + u.kdy + "\t" + u.kdo + "\t" + u.co);
				}
				Console.WriteLine ("\t###");
			//receive v case 3 ... ocekavam lock
			m.Cas = 3;
			tn.ReceiveFrame (ef, m);
			//vypis
				Console.WriteLine ("\t###");
				foreach (Udalost u in m.K.GetK().ToArray()) {
					Console.WriteLine ("\t" + u.kdy + "\t" + u.kdo + "\t" + u.co);
				}
				Console.WriteLine ("\t###");
			//receive v case 4 ... ocekavam lock
			m.Cas = 4;
			tn.ReceiveFrame (ef, m);
			//vypis
				Console.WriteLine ("\t###");
				foreach (Udalost u in m.K.GetK().ToArray()) {
					Console.WriteLine ("\t" + u.kdy + "\t" + u.kdo + "\t" + u.co);
				}
				Console.WriteLine ("\t###");
			//receive v case 5 ... ocekavam lock
			m.Cas = 5;
			tn.ReceiveFrame (ef, m);
			//vypis
				Console.WriteLine ("\t###");
				foreach (Udalost u in m.K.GetK().ToArray()) {
					Console.WriteLine ("\t" + u.kdy + "\t" + u.kdo + "\t" + u.co);
				}
				Console.WriteLine ("\t###");
			//2. sending v case 10 (sending naplanovany 1. processem)
			m.Cas = 10;
			tn.ZpracujUdalost (Stav.SENDING, m);
			//vypis
				Console.WriteLine ("\t###");
				foreach (Udalost u in m.K.GetK().ToArray()) {
					Console.WriteLine ("\t" + u.kdy + "\t" + u.kdo + "\t" + u.co);
				}
				Console.WriteLine ("\t###");
			//3. process v case 6 (processing naplanovany 2. processem)
			m.Cas = 6;
			tn.ZpracujUdalost (Stav.PROCESSING, m);
			//vypis
				Console.WriteLine ("\t###");
				foreach (Udalost u in m.K.GetK().ToArray()) {
					Console.WriteLine ("\t" + u.kdy + "\t" + u.kdo + "\t" + u.co);
				}
				Console.WriteLine ("\t###");
		}
	}
}

