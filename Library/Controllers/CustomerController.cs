using Library.Models;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace Library.Controllers
{
	public class CustomerController : Controller
	{
		// GET: Customer
		public ActionResult Login()
		{
			if (TempData["logOutMessage"] != null)
			{
				ViewBag.LogOutMessage = TempData["logOutMessage"].ToString();
			}
			/* To Display a list of fields in database
			LibraryEntities db = new LibraryEntities();
			return View(db.Customers.ToList());*/
			return View();
		}

		// GET: Customer
		/*The Index() method of the Home controller is the default method for an ASP.NET MVC application. 
		  When you run an ASP.NET MVC application, the Index() method is the first controller method that
		  is called.*/

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login(Customer objUser)
		{
			if (TempData["loginMessage"] != null)
			{
				ViewBag.LoginMessage = TempData["loginMessage"].ToString();
			}

			//used to validate that the rules of the model are being applied. ie required fields and correct format 
			if (ModelState.IsValid)
			{

				using (LibraryEntities db = new LibraryEntities())
				{

					var obj = db.Customers.Where(a => a.CustEmail.Equals(objUser.CustEmail) && a.CPassword.Equals(objUser.CPassword)).FirstOrDefault();

					if (obj != null)
					{
						Session["custID"] = obj.CustID.ToString();
						Session["custName"] = obj.CustName.ToString();
						Session["custEmail"] = obj.CustEmail.ToString();
						Debug.WriteLine(Session["custID"].ToString());
						return RedirectToAction("UserArea");
					}
				}
			}
			return View(objUser);
		}


		public ActionResult UserArea()
		{
			if (Session["custID"] != null)
			{
				if (TempData["borrowMessage"] != null)
				{
					ViewBag.BorrowedMessage = TempData["borrowMessage"].ToString();
				}
				if (TempData["copyUnavailable"] != null)
				{
					ViewBag.UnavailableMessage = TempData["copyUnavailable"].ToString();
				}
				if (TempData["reserveMessage"] != null)
				{
					ViewBag.ReserveMessage = TempData["reserveMessage"].ToString();
				}

				if (TempData["returnMessage"] != null)
				{
					ViewBag.ReturnMessage = TempData["returnMessage"].ToString();
				}
				Item it = new Item();
				LibraryEntities db = new LibraryEntities();

				/*dynamic model = new ExpandoObject();
				model.Items = db.Items.ToList();
				model.Copies = db.Copies.ToList();
				model.Transactions = db.Transactions.ToList();*/


				int cID = Int32.Parse(Session["custID"].ToString());
				var showBooks = from i in db.Items
								join c in db.Copies on i.Isbn equals c.Isbn
								join t in db.Transactions on c.CopyID equals t.CopyID
								where c.CopyID == t.CopyID && t.CustID == cID && i.Isbn == c.Isbn && c.Borrowed != "n" && t.TransacType == "Borrowed" && c.Borrow_Date != null
								select new CopyItemTransacViewModel { Name = i.Name, Borrow_Date = c.Borrow_Date, Return_Date = c.Return_Date, Isbn = i.Isbn };

				showBooks.ToList();

				return View(showBooks);
			}
			else
			{
				return RedirectToAction("Login");
			}
		}

		//Method for displaying previous books page
		public ActionResult PreviousBook()
		{
			if (Session["custID"] != null)
			{
				LibraryEntities db = new LibraryEntities();

				int cID = Int32.Parse(Session["custID"].ToString());
				var showBooks = from i in db.Items
								join c in db.Copies on i.Isbn equals c.Isbn
								join t in db.Transactions on c.CopyID equals t.CopyID
								where c.CopyID == t.CopyID && t.CustID == cID && i.Isbn == c.Isbn && c.Borrowed == "n" && t.TransacType == "Returned"
								select new CopyItemTransacViewModel { Name = i.Name, Borrow_Date = c.Borrow_Date, Return_Date = c.Return_Date, Isbn = i.Isbn };
				showBooks.ToList();

				return View(showBooks);
			}
			else
			{
				return RedirectToAction("Login");
			}
		}

		//[ActionName("AddBook")]
		public ActionResult BookBorrowed(long bookId)
		{
			if (Session["custID"] != null)
			{
				using (LibraryEntities db = new LibraryEntities())
				{
					Debug.WriteLine("!!!LEVEL 1");
					int cID = Int32.Parse(Session["custID"].ToString());
					var checkTrans = from c in db.Copies
									 from t in db.Transactions
									 where c.CopyID == t.CopyID && c.Isbn == bookId && t.CustID == cID
									 select c;

					foreach (Copy cp in checkTrans.ToList())
					{
						if (cp.Borrowed != "n")
						{
							TempData["borrowMessage"] = "Already Borrowed This Book";
							return RedirectToAction("UserArea");
						}
					}

					Transaction t1 = new Transaction();
					var copyQuery = from b in db.Copies
									where b.Isbn == bookId && b.Borrowed == "n"
									select b;
					if (ModelState.IsValid)
					{
						foreach (Copy cp1 in copyQuery.ToList())
						{
							Debug.WriteLine("!!!LEVEL 2");
							if (cp1.Borrowed != "y")
							{

								cp1.Borrowed = "y";
								// Get date-only portion of date, without its time.
								//DateTime dateOnly = DateTime.Now.ToShortDateString();
								var date = DateTime.Now.ToShortDateString();
								//DateTime today = DateTime.Now.Date;
								//DateTime retDay = today.AddDays(30);
								//cp1.Borrow_Date = today;
								//cp1.Return_Date = retDay;
								t1.CopyID = cp1.CopyID;
								t1.CustID = Int32.Parse(Session["custID"].ToString());
								t1.TransacType = "Borrowed";
								db.Transactions.Add(t1);
								db.SaveChanges();
								Debug.WriteLine("!!!LEVEL 3");
								var book = db.Items.Where(a => a.Isbn.Equals(cp1.Isbn)).FirstOrDefault();
								TempData["reserveMessage"] = book.Name.ToString() + ". Awaiting Collection Confirmation.";
								return RedirectToAction("UserArea");

							}
							else
							{
								TempData["copyUnavailable"] = "No copies Available";
								return RedirectToAction("UserArea");
							}



						}//end foreach()
						ModelState.Clear();
					}//end ModelState IF
					return View();
				}


			}//end session If
			else
			{
				TempData["loginMessage"] = "Please  Login to borrow books";
				return RedirectToAction("Login");
			}
		}

		//Method for returning books
		public ActionResult BookReturned(long bookId)
		{
			using (LibraryEntities db = new LibraryEntities())
			{

				if (ModelState.IsValid)
				{
					/*UPDATE DATA MODEL SO TRANSACTION HOLDS BORROW AND RETURN FIELDS COPY ONLY FOR COPY ID ISBN AND Y/N BORRED OR NOT*/
					int cuID = Int32.Parse(Session["custID"].ToString());
					var returnQuery = from c in db.Copies
									  join t in db.Transactions
									  on c.CopyID equals t.CopyID
									  where c.CopyID == t.CopyID && c.Isbn == bookId && t.CustID == cuID
									  select c;

					var returnTransQuery = from t in db.Transactions
										   join c in db.Copies
										   on t.CopyID equals c.CopyID
										   where c.CopyID == t.CopyID && c.Isbn == bookId && t.CustID == cuID
										   select t;



					foreach (Copy cp in returnQuery.ToList())
					{
						DateTime today = DateTime.Now.Date;
						cp.Borrow_Date = null;
						cp.Return_Date = null;
						cp.Borrowed = "n";

					}

					foreach (Transaction t in returnTransQuery.ToList())
					{
						t.TransacType = "Returned";

					}

					try
					{
						db.SaveChanges();
						TempData["returnMessage"] = "Book Returned";
						return RedirectToAction("UserArea");
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						// Provide for exceptions.
					}


				}//end model state if
				ModelState.Clear();

			}

			return RedirectToAction("UserArea");
		}
		public ActionResult LogOut()
		{
			Session.Remove("custID");
			TempData["logOutMessage"] = "Successfully Logged Out";
			return RedirectToAction("Login");
		}


		public ActionResult Search()
		{
			return View();
		}

		[HttpGet]
		public ActionResult Search(string searching)
		{
			using (LibraryEntities db = new LibraryEntities())
			{

				var book = db.Items.Where(a => a.Name.Contains(searching));

				return View(book.ToList());
			}


		}
	}
}
