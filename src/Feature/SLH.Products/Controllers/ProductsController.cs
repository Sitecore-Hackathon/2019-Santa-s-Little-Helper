using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SLH.Products.Controllers
{
    class ProductsController : Controller
    {
        public ActionResult Product()
        {
            return View();
        }
    }
}
