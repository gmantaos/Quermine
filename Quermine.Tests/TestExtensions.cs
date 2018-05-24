using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Quermine.Tests
{
	public static class TestExtensions
	{
		
		public static void AssertEqual(this Query actual, string expected)
		{
			Assert.AreEqual(Normalize(expected), Normalize(actual.ParametrizedQueryString()));
		}

		static string Normalize(string str)
		{
			return Regex.Replace(str, @"\s+", " ").Trim();
		}
	}
}
