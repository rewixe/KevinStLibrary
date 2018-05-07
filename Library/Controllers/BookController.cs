using Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class BookController : Controller
    {
		// GET: Book
		public ActionResult BookList()
		{
			LibraryEntities db = new LibraryEntities();
			var books = from Aut in db.Authors
						join It in db.Items
						on Aut.AuthorID equals It.AuthorID
						select new ItemLibrarianViewModel { It = It, Aut = Aut};

			/*foreach (ItemLibrarianViewModel i in books.ToList())
			{
				Debug.WriteLine("YOOOOOOOOO");
				Copy cp = new Copy();
				for (int z = 0; z < 5; z++)
				{
					cp.Isbn = i.It.Isbn;
					cp.Borrowed = "n";
					db.Copies.Add(cp);
				}
				db.SaveChanges();

			}*/

			books.ToList();
			

			return View(books);
        }
    }
}