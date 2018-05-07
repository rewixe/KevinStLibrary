using Library.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Controllers
{
	public class LibrarianController : Controller
	{
		// GET: Librarian
		public ActionResult StaffLogin()
		{
			if (TempData["logOutMessage"] != null)
			{
				ViewBag.LogOutMessage = TempData["logOutMessage"].ToString();
			}
			return View();

		}

		//Tells runtime where to send form post data back to
		[HttpPost]
		//Protects against hackers
		[ValidateAntiForgeryToken]
		public ActionResult StaffLogin(Librarian libUser)
		{
			//Check that form data is in correct format for the model and obeys all the rules
			if (ModelState.IsValid)
			{
				//Database object for accessing data
				using (LibraryEntities db = new LibraryEntities())
				{
					//Check the password and ID match
					var obj = db.Librarians.Where(a => a.LibrarianID.Equals(libUser.LibrarianID) && a.Password.Equals(libUser.Password)).FirstOrDefault();
					Debug.WriteLine(obj.Name.ToString());
					if (obj != null)
					{
						if (obj.Position == "Admin")
						{

							//Set the session variables with values from the database
							Session["libID"] = obj.LibrarianID.ToString();
							Session["libLocation"] = obj.LibLocation.ToString();
							Session["libName"] = obj.Name.ToString();
							Session["pos"] = obj.Position.ToString();
							//redirect to the librarian page
							return RedirectToAction("LibrarianList");
						}
						else
						{
							//Set the session variables with values from the database
							Session["libID"] = obj.LibrarianID.ToString();
							Session["libLocation"] = obj.LibLocation.ToString();
							Session["libName"] = obj.Name.ToString();
							Session["pos"] = obj.Position.ToString();
							//redirect to the librarian page
							return RedirectToAction("BookList");
						}

					}

				}
			}
			return View(libUser);

		}

		public ActionResult AdminArea()
		{
			if (Session["libID"] != null)
			{

				return View();
			}
			else
			{
				return RedirectToAction("StaffLogin");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AdminArea(Librarian l)
		{

			using (LibraryEntities db = new LibraryEntities())
			{
				// Add books.
				if (ModelState.IsValid)
				{


					db.Librarians.Add(l);
					try
					{
						db.SaveChanges();
						if (l.LibrarianID > 0)
						{
							ViewBag.Success = l.Name.ToString();

						}
						return View();
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						// Provide for exceptions.
					}

					ModelState.Clear();
				}


				return View();
			}//end ModelStateif
		}//end ActionResult 

		//List of Librarians to be edited
		public ActionResult LibrarianList()
		{
			if (Session["libID"] != null)
			{
				if (TempData["deleteMessage"] != null)
				{
					ViewBag.LibDelete = TempData["deleteMessage"].ToString();
				}

				if (TempData["editMessage"] != null)
				{
					ViewBag.LibEdit = TempData["editMessage"].ToString();
				}
				using (LibraryEntities db = new LibraryEntities())
				{

					return View(db.Librarians.ToList());
				}
			}
			else
			{
				return RedirectToAction("StaffLogin");
			}

		}


		public ActionResult EditLibrarian(int libID)
		{
			if (Session["libID"] != null)
			{
				using (LibraryEntities db = new LibraryEntities())
				{
					var cust = db.Librarians.Where(s => s.LibrarianID == libID).FirstOrDefault();
					return View(cust);
				}

			}
			else
			{
				return RedirectToAction("StaffLogin");
			}

		}

		//Edit student
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditLibrarian([Bind(Include = "LibrarianID,Name,LibLocation,Password,Position")]Librarian l)
		{

			using (LibraryEntities db = new LibraryEntities())
			{
				if (ModelState.IsValid)
				{
					db.Entry(l).State = EntityState.Modified;
					db.SaveChanges();
					TempData["editMessage"] = "Library Records Updated.";
					return RedirectToAction("LibrarianList");

				}
				return View(l);
			}

		}

		public ActionResult DeleteLibrarian(int libID)
		{
			if (Session["libID"] != null)
			{

				if (TempData["deleteMessage"] != null)
				{
					ViewBag.LibDelete = TempData["deleteMessage"].ToString();
				}
				using (LibraryEntities db = new LibraryEntities())
				{

					if (ModelState.IsValid)
					{
						var librarianQuery = db.Librarians.Where(s => s.LibrarianID == libID).FirstOrDefault();

						db.Librarians.Remove(librarianQuery);

						try
						{

							db.SaveChanges();
							TempData["deleteMessage"] = "Librarian Record";
							return RedirectToAction("LibrarianList");

						}
						catch (Exception e)
						{
							Console.WriteLine(e);
							// Provide for exceptions.
						}

					}
					return RedirectToAction("LibrarianList");
				}
			}
			else
			{
				return RedirectToAction("StaffLogin");
			}

		}




		//Action result for the Librarian Area
		public ActionResult StaffArea()
		{
			if (Session["libID"] != null)
			{

				return View();
			}
			else
			{
				return RedirectToAction("StaffLogin");
			}


		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult StaffArea(ItemLibrarianViewModel libObj, int numCopies)
		{
			using (LibraryEntities db = new LibraryEntities())
			{
				// Add books.
				if (ModelState.IsValid)
				{

					Item it = new Item();
					it.AuthorID = libObj.Aut.AuthorID;
					it.Name = libObj.It.Name;
					it.Isbn = libObj.It.Isbn;
					it.Subject = libObj.It.Subject;
					it.Type = "Book";
					it.Year = libObj.It.Year;
					libObj.Aut.Isbn = libObj.It.Isbn;
					db.Authors.Add(libObj.Aut);
					db.Items.Add(it);
					for (int i = 0; i < numCopies; i++)
					{
						Copy cp = new Copy();
						cp.Isbn = libObj.It.Isbn;
						cp.Borrowed = "n";
						db.Copies.Add(cp);
					}

					db.SaveChanges();
					if (libObj.It.Isbn > 0)
					{
						ViewBag.Success = libObj.It.Name.ToString();

					}
					ModelState.Clear();
				}


				return View();
			}//end ModelStateif
		}//end ActionResult 


		public ActionResult AddStudent()
		{

			if (Session["libID"] != null)
			{
				if (TempData["addStudent"] != null)
				{
					ViewBag.StudentAdded = TempData["addStudent"].ToString();
				}
				return View();
			}
			else
			{
				return RedirectToAction("StaffLogin");
			}

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddStudent(ItemLibrarianViewModel libObj)
		{

			using (LibraryEntities db = new LibraryEntities())
			{

				//Add students
				if (ModelState.IsValid)
				{
					Customer cust = new Customer();
					cust.CustName = libObj.C.CustName;
					cust.CustEmail = libObj.C.CustEmail;
					cust.Field = libObj.C.Field;
					cust.Privalige = libObj.C.Privalige;
					cust.CPassword = libObj.C.CPassword;
					db.Customers.Add(cust);

					ModelState.Clear();
				}

				try
				{
					db.SaveChanges();
					TempData["addStudent"] = "Student Added: " + libObj.C.CustName;
					return RedirectToAction("AddStudent");
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					// Provide for exceptions.
				}



			}//end ModelStateif
			return View();
		}//end ActionResult 

		//Log User Out
		public ActionResult LogOut()
		{
			Session.Remove("libID");
			TempData["logOutMessage"] = "Successfully Logged Out";
			return RedirectToAction("StaffLogin");
		}

		public ActionResult SubjectGuide()
		{
			if (Session["libID"] != null)
			{

				using (LibraryEntities db = new LibraryEntities())
				{

					return View(db.Items.ToList());
				}
			}
			else
			{
				return RedirectToAction("StaffLogin");
			}


		}

		//List of students to be edited
		public ActionResult StudentList()
		{
			if (Session["libID"] != null)
			{
				if (TempData["deleteMessage"] != null)
				{
					ViewBag.StudentDelete = TempData["deleteMessage"].ToString();
				}

				if (TempData["editMessage"] != null)
				{
					ViewBag.EditStudent = TempData["editMessage"].ToString();
				}
				using (LibraryEntities db = new LibraryEntities())
				{

					return View(db.Customers.ToList());
				}
			}
			else
			{
				return RedirectToAction("StaffLogin");
			}

		}

		public ActionResult EditStudent(int custID)
		{
			if (Session["libID"] != null)
			{
				using (LibraryEntities db = new LibraryEntities())
				{
					var cust = db.Customers.Where(s => s.CustID == custID).FirstOrDefault();
					return View(cust);
				}

			}
			else
			{
				return RedirectToAction("StaffLogin");
			}

		}

		//Edit student
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditStudent([Bind(Include = "CustID,CustName,CustEmail,CPassword,Privalige,Field,Fine")]Customer c)
		{
			using (LibraryEntities db = new LibraryEntities())
			{
				if (ModelState.IsValid)
				{
					db.Entry(c).State = EntityState.Modified;
					db.SaveChanges();
					TempData["editMessage"] = "Student Records Updated.";
					return RedirectToAction("StudentList");

				}
				return View(c);
			}

		}

		//List the books to be edited
		public ActionResult BookList()
		{

			if (Session["libID"] != null)
			{
				if (TempData["deleteMessage"] != null)
				{
					ViewBag.BookDelete = TempData["deleteMessage"].ToString();
				}
				if (TempData["editMessage"] != null)
				{
					ViewBag.BookEdit = TempData["editMessage"].ToString();
				}


				using (LibraryEntities db = new LibraryEntities())
				{

					return View(db.Items.ToList());
				}

			}
			else
			{
				return RedirectToAction("StaffLogin");
			}


		}

		public ActionResult EditBook(long isbn)
		{
			if (Session["libID"] != null)
			{
				using (LibraryEntities db = new LibraryEntities())
				{

					var book = db.Items.Where(s => s.Isbn == isbn).FirstOrDefault();
					return View(book);
				}


			}
			else
			{
				return RedirectToAction("StaffLogin");
			}

		}

		//Edit Book
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditBook([Bind(Include = "Isbn,Name,Year,Subject,Type")]Item it)
		{
			using (LibraryEntities db = new LibraryEntities())
			{
				if (ModelState.IsValid)
				{
					var book = from i in db.Items
							   join a in db.Authors on i.AuthorID equals a.AuthorID
							   where it.Isbn == i.Isbn && i.AuthorID == a.AuthorID
							   select i;

					foreach (Item i in book.ToList())
					{
						if (it.Isbn == i.Isbn)
						{
							i.Name = it.Name;
							i.Subject = it.Subject;
							i.Type = it.Type;

						}

					}
					try
					{
						db.SaveChanges();
						TempData["editMessage"] = "Book Records Updated.";
						return RedirectToAction("BookList");
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						// Provide for exceptions.
					}

				}
				return View(it);
			}

		}

		public ActionResult DeleteBook(long isbn)
		{
			if (Session["libID"] != null)
			{

				if (TempData["deleteMessage"] != null)
				{
					ViewBag.BookDelete = TempData["deleteMessage"].ToString();
				}
				using (LibraryEntities db = new LibraryEntities())
				{

					if (ModelState.IsValid)
					{


						var bookQuery = from it in db.Items
										join c in db.Copies on it.Isbn equals c.Isbn
										join t in db.Transactions on c.CopyID equals t.CopyID
										where it.Isbn == isbn && it.Isbn == c.Isbn && c.CopyID == t.CopyID
										select new ItemLibrarianViewModel { It = it, Cp = c, Tc = t };

						if (bookQuery.Count() < 1)
						{
							var bookQuery2 = from it in db.Items
											 join c in db.Copies on it.Isbn equals c.Isbn
											 where it.Isbn == isbn && it.Isbn == c.Isbn
											 select new ItemLibrarianViewModel { It = it, Cp = c };

							foreach (ItemLibrarianViewModel t in bookQuery2.ToList())
							{

								db.Copies.Remove(t.Cp);
								db.Items.Remove(t.It);

							}
							try
							{

								db.SaveChanges();
								TempData["deleteMessage"] = "Book Record";
								return RedirectToAction("BookList");

							}
							catch (Exception e)
							{
								Console.WriteLine(e);
								// Provide for exceptions.
							}

						}
						else
						{

							foreach (ItemLibrarianViewModel t in bookQuery.ToList())
							{

								db.Transactions.Remove(t.Tc);
								db.Copies.Remove(t.Cp);
								db.Items.Remove(t.It);

							}
							try
							{

								db.SaveChanges();
								TempData["deleteMessage"] = "Book Record";
								return RedirectToAction("BookList");

							}
							catch (Exception e)
							{
								Console.WriteLine(e);
								// Provide for exceptions.
							}
						}

					}
					return RedirectToAction("BookList");
				}
			}
			else
			{
				return RedirectToAction("StaffLogin");
			}

		}

		public ActionResult DeleteStudent(int custID)
		{
			if (Session["libID"] != null)
			{

				if (TempData["deleteMessage"] != null)
				{
					ViewBag.StudentDelete = TempData["deleteMessage"].ToString();
				}
				using (LibraryEntities db = new LibraryEntities())
				{

					if (ModelState.IsValid)
					{
						var studentQuery = db.Customers.Where(s => s.CustID == custID).FirstOrDefault();

						db.Customers.Remove(studentQuery);

						try
						{

							db.SaveChanges();
							TempData["deleteMessage"] = "Student Record";
							return RedirectToAction("StudentList");

						}
						catch (Exception e)
						{
							Console.WriteLine(e);
							// Provide for exceptions.
						}

					}
					return RedirectToAction("StudentList");
				}
			}
			else
			{
				return RedirectToAction("StaffLogin");
			}

		}

		public ActionResult BorrowedList()
		{
			if (Session["libID"] != null)
			{

				if (TempData["collectedMessage"] != null)
				{
					ViewBag.CollectionConfirmed = TempData["collectedMessage"].ToString();

				}

				using (LibraryEntities db = new LibraryEntities())
				{
					var loanQuery = from i in db.Items
									join c in db.Copies on i.Isbn equals c.Isbn
									join t in db.Transactions on c.CopyID equals t.CopyID
									join cS in db.Customers on t.CustID equals cS.CustID
									where i.Isbn == c.Isbn && c.CopyID == t.CopyID && t.TransacType != "Returned" && t.CustID == cS.CustID && c.Borrow_Date == null
									select new ItemLibrarianViewModel { It = i, Cp = c, Tc = t, C = cS };


					return View(loanQuery.ToList());

				}

			}
			else
			{
				return RedirectToAction("StaffLogin");
			}
		}

		public ActionResult BookCollected(long isbn)
		{
			if (Session["libID"] != null)
			{
				if (TempData["collectedMessage"] != null)
				{
					ViewBag.CollectionConfirmed = TempData["collectedMessage"].ToString();
				}
				using (LibraryEntities db = new LibraryEntities())
				{
					var loanQuery = from i in db.Items
									join c in db.Copies on i.Isbn equals c.Isbn
									join t in db.Transactions on c.CopyID equals t.CopyID
									join cS in db.Customers on t.CustID equals cS.CustID
									where i.Isbn == isbn && i.Isbn == c.Isbn && c.CopyID == t.CopyID && t.TransacType != "Returned" && t.CustID == cS.CustID && c.Borrow_Date == null
									select new ItemLibrarianViewModel { It = i, Cp = c, Tc = t, C = cS };



					foreach (ItemLibrarianViewModel item in loanQuery.ToList())
					{
						DateTime today = DateTime.Now.Date;
						DateTime retDay = today.AddDays(30);
						item.Cp.Borrow_Date = today;
						item.Cp.Return_Date = retDay;

					}
					try
					{

						db.SaveChanges();
						TempData["collectedMessage"] = "Book Collection Confirmed";
						return RedirectToAction("BorrowedList");

					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						// Provide for exceptions.
					}

				}
				return RedirectToAction("BorrowedList");

			}
			else
			{
				return RedirectToAction("StaffLogin");
			}
		}


	}
}

