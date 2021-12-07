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

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            Check_Vaccine();
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,FirstShotDate,FirstShotName,SecondShotDate,SecondShotName,ThirdShotDate,ThirdShotName")] Customer customer)
        {
            // 중복 방지
            Check_Vaccine();
            var check_email = from m in _context.Customer
                              select m;
            int check = check_email.Count(j => j.Email.Contains(customer.Email));
            if (check > 0)
            {
                ViewBag.Error = "--- Error: The Email has Already been added ---";
                return View();
            }

            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer); //why go back to main page?
        }

        // GET: Customers/Edit/5
        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Edit2(string email)
        {
            Check_Vaccine();
            var check_email = from m in _context.Customer
                              select m;
            int check = check_email.Count(j => j.Email.Contains(email));
            if (check <= 0)
            {
                ViewBag.Error = "----- Error: Check Email -----";
                return View("Edit");
            }
            else
            {
                ViewBag.Error = "Change Your Informaion";
                return View("Edit");
            }
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit3Async(string Email, string FirstName, string LastName, string FirstShotDate, string FirstShotName, string SecondShotDate, string SecondShotName, string ThirdShotDate, string ThirdShotName)
        {
            int userID = _context.Customer.Where(m => m.Email == Email).Select(m => m.ID).FirstOrDefault();
            var customer = await _context.Customer.FindAsync(userID);
            customer.Email = Email;
            customer.FirstName = FirstName;
            customer.LastName = LastName;
            customer.FirstShotDate = FirstShotDate;
            customer.FirstShotName = FirstShotName;
            customer.SecondShotDate = SecondShotDate;
            customer.SecondShotName = SecondShotName;
            customer.ThirdShotDate = ThirdShotDate;
            customer.ThirdShotName = ThirdShotName;  // ******  Find more effective way   ******
            _context.Update(customer);
            _context.SaveChanges();
            return View("index");

        }

        public IActionResult QRIndex()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> QRIndex(string email) // inputText is from QRIndex 
        {

            var check_email = from m in _context.Customer
                              select m;
            int check = check_email.Count(j => j.Email.Contains(email));


            //string test = check.ToString();
            if (email != null && check > 0)
            {
                var customer = await _context.Customer.Where(j => j.Email.Contains(email)).FirstAsync();
                using (MemoryStream ms = new MemoryStream()) // What is this for?
                {
                    QRCodeGenerator oRCodeGenerator = new QRCodeGenerator();
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
                        //use second shot.
                        DateTime firstDate = Convert.ToDateTime(customer.FirstShotDate);
                        DateTime secondDate = Convert.ToDateTime(customer.SecondShotDate);
                        TimeSpan difference = secondDate - firstDate;
                        if (difference.TotalDays > 14)
                        {
                            cus.Vaccinated = true;
                        }
                    }
                    else
                    {
                        DateTime secondDate = Convert.ToDateTime(customer.SecondShotDate);
                        DateTime thirdDate = Convert.ToDateTime(customer.ThirdShotDate);
                        TimeSpan difference = thirdDate - secondDate;
                        if (difference.TotalDays > 14)
                        {
                            cus.Vaccinated = true;
                        }
                    }

                    string jsonString = JsonConvert.SerializeObject(cus);

                    QRCodeData oQRCodeDate = oRCodeGenerator.CreateQrCode(jsonString, QRCodeGenerator.ECCLevel.Q); //for new just try first name
                    QRCode oQECode = new QRCode(oQRCodeDate);
                    using (Bitmap oBitmap = oQECode.GetGraphic(20))
                    {
                        oBitmap.Save(ms, ImageFormat.Png);
                        ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            else
            {
                ViewBag.Error = "----- Error: Check Email -----";
            }
            return View();
        }

        public void Check_Vaccine()
        {
            List<Vaccine> ListVaccine = new List<Vaccine>();
            ListVaccine.Add(new Vaccine { ID = 1, vaccine_name = "Pfizer", Check = false });
            ListVaccine.Add(new Vaccine { ID = 1, vaccine_name = "Moderna", Check = false });
            ListVaccine.Add(new Vaccine { ID = 1, vaccine_name = "Johnson", Check = false });

            ViewBag.vaccine = ListVaccine;
        }

    }
}
