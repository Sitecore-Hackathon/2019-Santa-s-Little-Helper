using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Publishing;
using SLH.Foundation.Buckets.Helpers;
using SLH.Foundation.Buckets.Models;

namespace SLH.Foundation.Buckets.Controllers
{
    public class BucketManagerController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var response = new List<BucketResponse>();

            var buckets = BucketsHelper.GetAllBuckets("/sitecore/content/");
            foreach (var bucket in buckets)
            {
                response.Add(new BucketResponse { BucketId = bucket.ID.Guid, BucketName = bucket.DisplayName });
            }

            return View(response);
        }

        public ActionResult BucketList(string bucketId)
        {
            ViewBag.ArticleType = bucketId;

            return PartialView();
        }
    }
}