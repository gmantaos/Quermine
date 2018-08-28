using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChanceNET.Attributes;

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

		[DbField("release")]
		public DateTime Release;
	}
}
