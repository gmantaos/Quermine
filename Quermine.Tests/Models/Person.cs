using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine.Tests
{
	[DbTable("people")]
	public class Person
	{
		[DbField("id")]
		public long ID;

		[DbField("name")]
		public string Name;

		[DbField("birthday")]
		public DateTime Birthday;
	}
}
