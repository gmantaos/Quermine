using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	internal struct Sequence
	{
		object[] parts;

		public Sequence(params object[] parts)
		{
			this.parts = parts;
		}

		public override string ToString()
		{
			StringBuilder str = new StringBuilder();
			for (int i = 0; i < parts?.Length; i++)
			{
				if (i != 0) str.Append(", ");
				str.Append(parts[i]);
			}
			return str.ToString();
		}

		public static Sequence operator +(Sequence s1, Sequence s2)
		{
			if (s1.parts == null)
				return s2;
			if (s2.parts == null)
				return s1;
			return new Sequence(s1.parts.Concat(s2.parts).ToArray());
		}
	}
}
