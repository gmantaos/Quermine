﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Queerie
{
	internal static class Utils
	{
		public static string GetParameterName(string columnName)
		{
			return string.Format("@param_{0}_{1}", columnName, StaticRandom.Rand());
		}

		public static class StaticRandom
		{
			static int seed = Environment.TickCount;

			static readonly ThreadLocal<Random> random =
				new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

			public static int Rand()
			{
				return random.Value.Next();
			}
		}
	}
}
