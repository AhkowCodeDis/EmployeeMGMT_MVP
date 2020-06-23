using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmployeeMGMT_MVP.Models;
using PagedList;

namespace EmployeeMGMT_MVP.Controllers
{
    public class EmployeesController : Controller
    {
        private Entities db = new Entities();

        // GET: Employees
        public ActionResult Index(string sortOrder , string searchString , string currentFilter,  int? page , string minSalary , string maxSalary)
        {
            //Filter Options
            int minSalaryFilter = 0;
            int maxSalaryFilter = 0;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParam = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSortParam = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.LoginSortParam = sortOrder == "login" ? "login_desc" : "login";
            ViewBag.SalarySortParam = sortOrder == "salary" ? "salary_desc" : "salary";
            ViewBag.CurrentFilter = searchString;
            ViewBag.MinSalaryFilter = minSalary;
            ViewBag.MaxSalaryFilter = maxSalary;
            //If SearchString is null or empty , will view at page one
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            //Default Sorting is by id 
            var employees = from e in db.Employees
                            select e;
            employees = employees.OrderBy(e => e.Id);

            //If there is a search string input , will find data that is from search string
            if (!String.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.Id.Contains(searchString));
                                 
            }

            //Filtering list that has more than min salary 
            if (!String.IsNullOrEmpty(minSalary))
            {
                minSalaryFilter = int.Parse(minSalary);
                employees = employees.Where(e => e.Salary >= minSalaryFilter) ;

            }

            //Filtering list that has less than min salary 
            if (!String.IsNullOrEmpty(maxSalary))
            {
                maxSalaryFilter = int.Parse(maxSalary);
                employees = employees.Where(e => e.Salary <= maxSalaryFilter);
            }

            //If user click on the header it will order based on what the user click
            switch (sortOrder)
            {
                case "id_desc":
                    employees = employees.OrderByDescending(e => e.Id);
                    break;
                case "name_desc":
                    employees = employees.OrderByDescending(e => e.Name);
                    break;
                case "login_desc":
                    employees = employees.OrderByDescending(e => e.Login);
                    break;
                case "salary_desc":
                    employees = employees.OrderByDescending(e => e.Salary);
                    break;   
            }

            //Each Page holds 30 records
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            return View(employees.ToPagedList(pageNumber, pageSize));

            
        }



        // GET: Employees/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Login,Name,Salary")] Employee employee)
        {
            Employee employeeStaging = db.Employees.Find(employee.Id);
            if (employeeStaging != null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest , "An Existing Employee is Present");
            }
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Login,Name,Salary")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        } 
    }
}
