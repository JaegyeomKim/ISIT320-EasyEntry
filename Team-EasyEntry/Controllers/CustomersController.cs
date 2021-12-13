using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using QRCoder;
using Newtonsoft.Json;
using Team_EasyEntry.Data;
using Team_EasyEntry.Models;
namespace Team_EasyEntry.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context) // This is for creating DataBase constructor 
        {
            _context = context;
        }

        // Tihs Method is for showing the Index.cshtml if user click some button which calls the method, then user navigates to index.cshtml page.
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // Tihs Method is for showing the Create.cshtml if user click some button which calls the method, then user navigates to Create.cshtml page.

        public IActionResult Create()
        {
            // Create the list data which include Vaccination tpye as viewBag 
            Check_Vaccine();  
            return View();
        }

        // This method works after user type the information and click the Crate Button.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,FirstShotDate,FirstShotName,SecondShotDate,SecondShotName,ThirdShotDate,ThirdShotName")] Customer customer)
        {
            // Create the list data which include Vaccination tpye as viewBag 
            Check_Vaccine();

            // Call Customer Table DataBase in "check_email"
            var check_email = from m in _context.Customer
                              select m;

            // As Using the "check_email (which have customer database)", check count of user Email.  
            int check = check_email.Count(j => j.Email.Contains(customer.Email));
            //If the email has more than one, shows ViewBag.Error. and navigate to Create page again 
            if (check > 0)
            {
                ViewBag.Error = "--- Error: The Email has Already been added ---";
                return View();
            }
            // Add to DataBase If user type vaild informaion and go back to main page.
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(); 
        }


        // Tihs Method is for showing the Edit.cshtml if user click Modift Cutomer Infor button which calls the method, then user navigates to Edit.cshtml page.
        public IActionResult Edit()
        {
            return View();
        }
        // This method works through search butten to check the Email account.
        public IActionResult Edit2(string email)
        {
            // Create the list data which include Vaccination tpye as viewBag 
            Check_Vaccine();

            // Call Customer Table DataBase in "check_email"
            var check_email = from m in _context.Customer
                              select m;

            // As Using the "check_email (which have customer database)", check count of user Email.  
            int check = check_email.Count(j => j.Email.Contains(email));

            //If check is 0, that means there is no user email. Shows  ViewBag.Error and navigate same page.
            if (check <= 0)
            {
                ViewBag.Error = "----- Error: Check Email -----";
                return View("Edit");
            }
            // If check is more thab 0, then shows Edit page.
            else
            {
                ViewBag.Error = "Change Your Informaion";
                return View("Edit");
            }
        }

        // If user tpye the modified informaion and click the Change button. The Customer DB is updated with modified informaion
        [HttpPost]
        public async Task<IActionResult> Edit3Async(string Email, string FirstName, string LastName, string FirstShotDate, string FirstShotName, string SecondShotDate, string SecondShotName, string ThirdShotDate, string ThirdShotName)
        {
            // Find User Email in the Customer DataBase
            int userID = _context.Customer.Where(m => m.Email == Email).Select(m => m.ID).FirstOrDefault();
            // Search required table row by email and save modified informaion
            var customer = await _context.Customer.FindAsync(userID);
            customer.Email = Email;
            customer.FirstName = FirstName;
            customer.LastName = LastName;
            customer.FirstShotDate = FirstShotDate;
            customer.FirstShotName = FirstShotName;
            customer.SecondShotDate = SecondShotDate;
            customer.SecondShotName = SecondShotName;
            customer.ThirdShotDate = ThirdShotDate;
            customer.ThirdShotName = ThirdShotName;
            // Update to Customer DB with modified Informaion 
            _context.Update(customer);
            // Save function
            _context.SaveChanges();
            //Navigate to HomePage
            return View("index");

        }

        // Tihs Method is for showing the QRIndex.cshtml if user click Check QR button which calls the method, then user navigates to QRIndex.cshtml page.
        public IActionResult QRIndex()
        {
            return View();
        }

        // This Method shows user's QR code if user type vaild email address. 
        [HttpPost]
        public async Task<IActionResult> QRIndex(string email)  
        {
            // Call the Customer DB Data and check the email 
            var check_email = from m in _context.Customer select m;
            int check = check_email.Count(j => j.Email.Contains(email));

            // If the email is in the Customer DataBase Create QR code. 
            if (email != null && check > 0)
            {
                // Search required table row by email and save modified informaion
                var customer = await _context.Customer.Where(j => j.Email.Contains(email)).FirstAsync();
                
                // Download QRCoderNetCore library
                using (MemoryStream ms = new MemoryStream()) 
                {
                    QRCodeGenerator oRCodeGenerator = new QRCodeGenerator();

                    // make new Customer class and copy the DB data which saved in "var customer"
                    Customer cus = new Customer();
                    cus.ID = customer.ID;
                    cus.Email = customer.Email;
                    cus.FirstName = customer.FirstName;
                    cus.LastName = customer.LastName;
                    cus.FirstShotName = customer.FirstShotName;
                    cus.FirstShotDate = customer.FirstShotDate;
                    cus.SecondShotDate = customer.SecondShotDate;
                    cus.SecondShotName = customer.SecondShotName;
                    cus.ThirdShotDate = customer.ThirdShotDate;
                    cus.ThirdShotName = customer.ThirdShotName;

                    if (customer.ThirdShotDate == "")
                    {
                        // save and convert string type date to DateTime type
                        DateTime firstDate = Convert.ToDateTime(customer.FirstShotDate);
                        DateTime secondDate = Convert.ToDateTime(customer.SecondShotDate);
                        TimeSpan difference = secondDate - firstDate;
                        //check the vaccine data
                        if (difference.TotalDays > 14)
                        {
                            cus.Vaccinated = true;
                        }
                    }
                    else
                    {
                        // save and convert string type date to DateTime type
                        DateTime secondDate = Convert.ToDateTime(customer.SecondShotDate);
                        DateTime thirdDate = Convert.ToDateTime(customer.ThirdShotDate);
                        TimeSpan difference = thirdDate - secondDate;
                        //check the vaccine data
                        if (difference.TotalDays > 14)
                        {
                            cus.Vaccinated = true;
                        }
                    }
                    //Convert Customer Data to Json type and save as string. 
                    string jsonString = JsonConvert.SerializeObject(cus);

                    // Generator QR code as using jsonString(customer) data
                    QRCodeData oQRCodeDate = oRCodeGenerator.CreateQrCode(jsonString, QRCodeGenerator.ECCLevel.Q); //for new just try first name
                    QRCode oQECode = new QRCode(oQRCodeDate);
                    using (Bitmap oBitmap = oQECode.GetGraphic(20))
                    {
                        oBitmap.Save(ms, ImageFormat.Png);
                        // the ViewBag.QRCode shows the QRcode on the page.
                        ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            // if the email is not in the DB then shows the Error Message and navigate to same page. 
            else
            {
                ViewBag.Error = "----- Error: Check Email -----";
            }
            return View();
        }

        // This method is for creating list of vaccine type. 
        public void Check_Vaccine()
        {
            List<Vaccine> ListVaccine = new List<Vaccine>();
            ListVaccine.Add(new Vaccine { ID = 1, vaccine_name = "None", Check = false });
            ListVaccine.Add(new Vaccine { ID = 1, vaccine_name = "Pfizer", Check = false });
            ListVaccine.Add(new Vaccine { ID = 1, vaccine_name = "Moderna", Check = false });
            ListVaccine.Add(new Vaccine { ID = 1, vaccine_name = "Johnson", Check = false });

            ViewBag.vaccine = ListVaccine;
        }

    }
}
