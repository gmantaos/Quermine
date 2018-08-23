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

		public bool Contains(object obj)
		{
			return parts.Contains(obj);
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

		public static Sequence ConcatUnique(Sequence s1, Sequence s2)
		{
			HashSet<object> parts = new HashSet<object>(); ;
			foreach (object obj in s1.parts)
			{
				parts.Add(obj);
			}
			foreach (object obj in s2.parts)
			{
				parts.Add(obj);
			}
			return new Sequence(parts.ToArray());
		}

		public static Sequence operator +(Sequence s1, Sequence s2)
		{
			if (s1.parts == null)
				return s2;
			if (s2.parts == null)
				return s1;
			return new Sequence(s1.parts.Concat(s2.parts).ToArray());
		}

		public static Sequence operator +(Sequence seq, object obj)
		{
			if (seq.parts == null)
				return new Sequence(obj);
			if (obj == null)
				return seq;
			return new Sequence(seq.parts.Concat(new object[] { obj }).ToArray());
		}
	}
}
