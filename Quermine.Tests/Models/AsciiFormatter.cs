using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine.Tests
{
	public class AsciiFormatter : IValueFormatter<byte[]>
	{
		public object GetValue(byte[] val)
		{
			return Encoding.ASCII.GetString(val);
		}

		public byte[] SetValue(object val)
		{
			return Encoding.ASCII.GetBytes(val.ToString());
		}
	}
}
