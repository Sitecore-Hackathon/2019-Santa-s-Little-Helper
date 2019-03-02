using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore;
using Sitecore.Mvc.Extensions;

namespace SLH.Website.Controllers
{
    public class SLHApiController : Controller
    {
        protected static Dictionary<string, string> BucketList;

        public SLHApiController()
        {
            if (BucketList == null)
            {
                BucketList = new Dictionary<string, string>()
                {
                    { "923710932", "Bucket 1" },
                    { "923710933", "Bucket 2" },
                };
                //DictionaryExtensions.AddRange<string, string>(SLHApiController.BucketList, this.SLHManager.GetAllCategories(Context.ContentDatabase));
            }
        }

        //[Authorize]
        //public ActionResult Buckets(string type)
        //{


        //    ViewBag.ArticleType = type;
        //    return base.PartialView(BucketList);
        //}

        //[Authorize]
        [HttpGet]
        public string SLHMain()
        {
            //ViewBag.BucketList = BucketList;
            return "ok";//View(BucketList);
        }
    }
}