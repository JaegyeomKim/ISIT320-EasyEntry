using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRCoder;
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
            return View(await _context.Customer.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.ID == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,FirstShotDate,FirstShotName,SecondShotDate,SecondShotName,ThirdShotDate,ThirdShotName")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,FirstShotDate,FirstShotName,SecondShotDate,SecondShotName,ThirdShotDate,ThirdShotName")] Customer customer)
        {
            if (id != customer.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.ID == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customer.FindAsync(id);
            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customer.Any(e => e.ID == id);
        }

        public IActionResult QRIndex()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QRIndex(int ID) // inputText is from QRIndex 
        {

            var customer = await _context.Customer.FindAsync(7);
            //string name = "lalala";
            //var customer = await _context.Customer.FindAsync("Kay");
                using (MemoryStream ms = new MemoryStream()) // What is this for?
                {
                    QRCodeGenerator oRCodeGenerator = new QRCodeGenerator();
                    QRCodeData oQRCodeDate = oRCodeGenerator.CreateQrCode(customer.SecondShotDate + customer.FirstName, QRCodeGenerator.ECCLevel.Q); //for new just try first name
                    QRCode oQECode = new QRCode(oQRCodeDate);
                    using (Bitmap oBitmap = oQECode.GetGraphic(20))
                    {
                        oBitmap.Save(ms, ImageFormat.Png);
                        ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }

            //if (inputText != null)
            //{
            //    using (MemoryStream ms = new MemoryStream()) // What is this for?
            //    {
            //        QRCodeGenerator oRCodeGenerator = new QRCodeGenerator();
            //        QRCodeData oQRCodeDate = oRCodeGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
            //        QRCode oQECode = new QRCode(oQRCodeDate);
            //        using (Bitmap oBitmap = oQECode.GetGraphic(20))
            //        {
            //            oBitmap.Save(ms, ImageFormat.Png);
            //            ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
            //        }
            //    }
            //}

            return View();
        }
    }
}
