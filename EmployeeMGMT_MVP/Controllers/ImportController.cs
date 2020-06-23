using EmployeeMGMT_MVP.Models;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Collections.Generic;

namespace EmployeeMGMT_MVP.Controllers
{
    public class ImportController : Controller
    {
        private Entities db = new Entities();

        // GET: Import
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Post method for importing users 
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            String statusStr = "";
            String validateStr = "";
            if (postedFile != null)
            {
                try
                {
                    string fileExtension = Path.GetExtension(postedFile.FileName);

                    //Validate uploaded file and return error.
                    if (fileExtension != ".csv")
                    {
                        ViewBag.Message = "Please select the csv file with .csv extension";
                        return View();
                    }


                    var employees = new List<Employee>();
                    using (var sreader = new StreamReader(postedFile.InputStream))
                    {
                        //Loop through the records and adding records into list
                        while (!sreader.EndOfStream)
                        {
                            string[] rows = sreader.ReadLine().Split(',');

                            employees.Add(new Employee
                            {
                                Id = rows[0].ToString(),
                                Login = rows[1].ToString(),
                                Name = rows[2].ToString(),
                                Salary = decimal.Parse(rows[3].ToString())
                            });
                        }
                    }
                    //Validating list of employees
                    validateStr = validateCsv(employees);
                    //If there are no validation issues , it will do insertion / update
                    if (!(validateStr.Length > 0 ))
                    {
                        statusStr = insertFromCsv(employees);
                        statusStr = "UPLOAD SUCCESS :" + "<\br>" + statusStr;
                    }
                    else
                    {
                        validateStr = "UPLOAD FAILED :" + "<\br>" + validateStr;
                    }

                    ViewBag.Output = statusStr;
                    ViewBag.Message = validateStr;
                    //return View("View", employees);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "Please select the file first to upload.";
            }
            return View();
        }

        public void DeleteEmployee([Bind(Include = "Id,Login,Name,Salary")] Employee inputEmp)
        {
            Employee employee = db.Employees.Find(inputEmp.Id);
            db.Employees.Remove(employee);
            db.SaveChanges();
        }

        public void CreateEmployee([Bind(Include = "Id,Login,Name,Salary")] Employee inputEmp)
        {
            Employee employee = db.Employees.Find(inputEmp.Id);
            if (employee == null && ModelState.IsValid)
            {
                db.Employees.Add(inputEmp);
                db.SaveChanges();
            }
        }

        public string insertFromCsv(List<Employee> employees)
        {

            string statusString = "";
            foreach (var employee in employees)
            {
                Employee employeeStaging = db.Employees.Find(employee.Id);
                
                // if there are no records in DB and the line is not commented with a '#' , it will create or update record
                if (employeeStaging == null && !(employee.Id.StartsWith("#")))
                {
                    //Creating Employee
                    CreateEmployee(employee);
                    statusString += "Creating Record | " + employee.getStrInfo() + "<br/>";
                }
                else if (!(employee.Id.StartsWith("#")))
                {
                    //Updating if exists in DB
                    DeleteEmployee(employee);
                    CreateEmployee(employee);
                    statusString += "Updating Record | " + employee.getStrInfo() + "<br/>";
                }
            }
            return statusString;
        }
        public string validate( Employee employee)
        {
            //Validation if there are empty fields and return which row will are having issues with .getStrInfo function
            if ((employee.Id.Length == 0) || (employee.Login.Length == 0) || (employee.Name.Length == 0) || (employee.Salary.HasValue == false))
            {
                return ("Invalid Row | " + employee.getStrInfo() + "<br/>");
            }
            else
            {
                return "";        
            }
        }
        //method to validate the list and return information of bad records
        public string validateCsv(List<Employee> employees)
        {

            string statusString = "";
            foreach (var employee in employees)
            {
                statusString += validate(employee);
            }
            return statusString;
        }
    }
}