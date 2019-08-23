using System;
using System.Diagnostics;

namespace KFF
{
	/// <summary>
	/// Class containing methods for testing the KFF reader and KFF writer.
	/// </summary>
	public static class TagTester
	{
		/// <summary>
		/// An output of the tag tester.
		/// </summary>
		public class Output
		{
			/// <summary>
			/// Average write speed.
			/// </summary>
			public double writeSpeedAvg;
			/// <summary>
			/// Min write speed.
			/// </summary>
			public double writeSpeedMin;
			/// <summary>
			/// Median write speed.
			/// </summary>
			public double writeSpeedMedian;
			/// <summary>
			/// Max write speed.
			/// </summary>
			public double writeSpeedMax;
			/// <summary>
			/// All write speeds (reps).
			/// </summary>
			public double[] sortedWrites;

			/// <summary>
			/// Average read speed.
			/// </summary>
			public double readSpeedAvg;
			/// <summary>
			/// Min read speed.
			/// </summary>
			public double readSpeedMin;
			/// <summary>
			/// Median read speed.
			/// </summary>
			public double readSpeedMedian;
			/// <summary>
			/// Max read speed.
			/// </summary>
			public double readSpeedMax;
			/// <summary>
			/// All read speeds (reps).
			/// </summary>
			public double[] sortedReads;

			/// <summary>
			/// Converts the output into a string containing the write performance.
			/// </summary>
			public string ToStringWrite()
			{
				return "Write :  " + writeSpeedMin + " / " + writeSpeedMedian + " / " + writeSpeedMax + "  ( " + writeSpeedAvg + " )";
			}

			/// <summary>
			/// Converts the output into a string containing the read performance.
			/// </summary>
			public string ToStringRead()
			{
				return "Read :  " + readSpeedMin + " / " + readSpeedMedian + " / " + readSpeedMax + "  ( " + readSpeedAvg + " )";
			}
		}

		//			 =#= SPEEDS IN MILISECONDS PER KFF VERSION =#= (10k iterations)

		//############
		//	v1.0
		// (TAG)
		//     - Named :	165 ms
		//     - Indexed :	100 ms
		// (Value)
		//     - Named :	134 ms
		//     - Indexed :	 80 ms

		//############
		//	v2.0
		//     - Named :	106 ms
		//     - Indexed :	 96 ms

		//############
		//	v3.0
		//     - Named :	126 ms
		//     - Indexed :	 66 ms

		//############
		//	v4.0
		//     - Named :	  9 ms

		//############
		//	v5.0
		// N/A

		//############
		//	v6.0

		// Basic pathfind times, no type conversion:
		// - By Name  {}:
		//		TAverage:  (7,78 ms)
		//		TMin:		7.6  ms
		//		TMedian:	7.8  ms
		//		TMax:	   10.1  ms

		//		File used:		"Abc = true; A = { Health = 50; Id = \"abcd\"; Name = \"Abcd\"; Num = { }; Position = { X = 3.178; Y = 8.512; Z = -4.001; }; }; e = 5.5;";
		//		Path used :		"A.Position.Z"


		// - By Index []:
		//		TAverage:  (3.06 ms)
		//		TMin:		3.0  ms
		//		TMedian:	3.1  ms
		//		TMax:	    3.1  ms

		//		File used:		"Abc = true; A = [ [ 1.0, 3.0, 6.666 ], [ 3.0, -1.2, 6.05 ], [ 7.0, 9.0, -4.32 ], [ 11.0, -3.542, -5.054 ], [ 3.178, 8.512, -4.001 ] ]; e = 5.5;";
		//		Path used:		"A.4.2"
		

		// v6.0
		/// <summary>
		/// Tests the speeds of the KFFReader v6.0.
		/// </summary>
		/// <param name="serializer">The reader, containing the file to test.</param>
		/// <param name="path">The path to read from.</param>
		/// <param name="iterations">Number of iterations per test.</param>
		/// <param name="repetitions">Number of repetitions (averaging).</param>
		public static Output GetEfficiency( KFFSerializer serializer, Path path, int iterations = 10000, int repetitions = 100 )
		{
			// NOTE! - Does 10 times more iterations than specified and divides by 10 for greater accuracy!

			Stopwatch sw = new Stopwatch();
			double[] values = new double[repetitions];
			double total = 0;
			double avg = 0;
			double min = 0;
			double max = 0;
			double median = 0;
			float val;
			//object val;
			for( int i = 0; i < repetitions; i++ )
			{
				sw.Start();
				for( int j = 0; j < iterations; j++ )
				{
					val = serializer.ReadFloat( path );
					//val = reader.PathFind( path ); // - later changed to KFFFile.PathFind
				}
				sw.Stop();
				total += sw.ElapsedMilliseconds;
				values[i] = sw.ElapsedMilliseconds;
				sw.Reset();
			}
			avg = total / repetitions;
			Array.Sort( values );
			if( repetitions % 2 == 0 )
			{
				median = (values[repetitions / 2 - 1] + values[repetitions / 2]) / 2;
			}
			else
			{
				median = values[repetitions / 2];
			}
			max = values[repetitions - 1];
			min = values[0];

			return new Output()
			{
				readSpeedAvg = avg / 10.0,
				readSpeedMin = min / 10.0,
				readSpeedMedian = median / 10.0,
				readSpeedMax = max / 10.0,
				sortedReads = values
			};
		}
	}
}