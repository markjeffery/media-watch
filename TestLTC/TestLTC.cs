using System;
using Sony.Media.Utilities;

namespace TestLTC
{
	/// <summary>
	/// Test Linear Timecode class.
	/// </summary>
	/// <remarks>
	/// Simple console application to test reading and writing of timecode information
	/// </remarks>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]

		static void Main(string[] args)
		{
			Class1 c = new Class1();
			LinearTimeCode myltc = new LinearTimeCode(25,"2000-06-23","01:15:23:21");
			c.goon5(myltc);
			myltc = new LinearTimeCode(25,"2000-06-23","20:24:36:23");
			Console.WriteLine(myltc.ToString());
			myltc.addFrames(2); 
			Console.WriteLine("2 frames, Should take it onto next second.");
			Console.WriteLine(myltc.ToString());
			myltc.addFrames(600); 
			Console.WriteLine("600 frames, 24 seconds, should take it onto next minute.");
			Console.WriteLine(myltc.ToString());
			myltc.addFrames(52500); 
			Console.WriteLine("52,500 frames, 35 minutes, should take it onto next hour.");
			Console.WriteLine(myltc.ToString());
			myltc.addFrames(270000); 
			Console.WriteLine("270,000 frames, 3 hours, should take it onto next day.");
			Console.WriteLine(myltc.ToString());
			Console.WriteLine("Now lets try to go back!");
			try
			{
				myltc.addFrames(-270000); 
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			// myltc = new LinearTimeCode(30,"2000-06-23","01:01:59:28");
			// c.goon5(myltc);
			// myltc = new LinearTimeCode(30,"2000-06-23","01:00:59:28");
			// c.goon5(myltc);
			// myltc = new LinearTimeCode(30,"2000-06-23","01:09:59:28");
			// c.goon5(myltc);
			// Console.WriteLine("This should result in a drop frame exception");
			// try
			// {
			// 	myltc = new LinearTimeCode(30,"2000-06-23","01:01:00:00");
			// }
			// catch (ArgumentException ae)
			// {
			// 	Console.WriteLine(ae.ToString());
			// }
			// Console.WriteLine("and this.");
			// try
			// {
			// myltc = new LinearTimeCode(30,"2000-06-23","01:01:00:01");
			// }
			// catch (ArgumentException ae)
			// {
			// 	Console.WriteLine(ae.ToString());
			// }
			// Console.WriteLine("but not this.");
			// try
			// {
			// 	myltc = new LinearTimeCode(30,"2000-06-23","01:01:00:02");
			// }
            //     catch (ArgumentException ae)
//			{
//				Console.WriteLine(ae.ToString());
//			}
//			Console.WriteLine("frame out of range.");
//			try
//			{
//				myltc = new LinearTimeCode(30,"2000-06-23","01:09:59:36");
//			}
//			catch (ArgumentException ae)
//			{
//				Console.WriteLine(ae.ToString());
//			}
//			try
//			{
//				myltc = new LinearTimeCode(25,"2000-06-23","01:09:59:26");
//			}
//			catch (ArgumentException ae)
//			{
//				Console.WriteLine(ae.ToString());
//			}
//			Console.WriteLine("wrong fps");
//			try
//			{
//				myltc = new LinearTimeCode(29,"2000-06-23","01:09:59:21");
//			}
//			catch (ArgumentException ae)
//			{
//				Console.WriteLine(ae.ToString());
//			}
//			Console.WriteLine("wrong date");
//			try
//			{
//				myltc = new LinearTimeCode(30,"frefsafsdf-06-23","01:09:59:21");
//			}
//			catch (ArgumentException ae)
//			{
//				Console.WriteLine(ae.ToString());
//			}
			String wait = Console.ReadLine();
		}

		public Class1()
		{
		}


		public void goon5(LinearTimeCode ltc) 
		{
			Console.WriteLine();
			Console.WriteLine(ltc.fps.ToString());
			Console.WriteLine(ltc.ToString());
			for (int i=1; i<5; i++) 
			{
				ltc.nextFrame();
				Console.WriteLine(ltc.ToString());
			}
		}
	}
}
