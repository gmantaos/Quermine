using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Quermine;

namespace Quermine.Tests
{
	[DbTable("books")]
	public class Book
	{
		[DbField("id", FieldProprties = FieldProperties.PrimaryKey)]
		public int ID;

		[DbField("year")]
		public int Year;

		[DbField("title", Length = 12)]
		public string Title;
	}
}
