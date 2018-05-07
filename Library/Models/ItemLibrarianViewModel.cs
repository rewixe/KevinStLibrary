using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Models
{
	public class ItemLibrarianViewModel
	{
		public Librarian Lib { get; set; }
		public Author Aut { get; set; }
		public Item It { get; set; }
		public Copy Cp { get; set; }
		public Transaction Tc { get; set; }
		public Customer C { get; set; }


	}
}