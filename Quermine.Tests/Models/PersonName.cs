using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Quermine;

namespace Quermine.Tests
{
	[DbTable("people")]
	public class PersonName
	{
		[DbField("name", Formatter = typeof(AsciiFormatter))]
		public byte[] NameAscii;
	}
}
