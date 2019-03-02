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
    public class BucketsController : Controller
    {
        [HttpGet]
        public ActionResult GetBuckets()
        {
            var response = new List<BucketResponse>();

            var buckets = BucketsHelper.GetAllBuckets("/sitecore/content/");
            foreach (var bucket in buckets)
            {
                response.Add(new BucketResponse {BucketId = bucket.ID.Guid, BucketName = bucket.DisplayName});
            }
            
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetBucketableItems(string bucketId, string name = null)
        {
            var response = new List<BucketableResponse>();
            var bucketItemId = new ID(bucketId);
            
            var bucketableItems = BucketsHelper.GetItemsFromBucket(bucketItemId, name);
            foreach (var bucketable in bucketableItems)
            {
                var fields = BucketsHelper.GetTemplateFields(bucketable);
                response.Add(new BucketableResponse
                {
                    BucketId = bucketItemId.Guid,
                    ItemId = bucketable.ID.Guid,
                    ItemName = bucketable.DisplayName,
                    Url = BucketsHelper.GenerateFieldEditorUrl(bucketable, fields),
                    Fields = fields
                });
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PublishItem(string itemId)
        {
            Database source = Factory.GetDatabase("master");
            Item item = source.GetItem(itemId);

            if (item == null) return Json(false, JsonRequestBehavior.AllowGet);
            
            Database target = Factory.GetDatabase("web");

            try
            {
                PublishManager.PublishItem(item.Parent, new[] {target}, item.Languages, true, false);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Exception publishing items from custom pipeline! : " + ex, this);
            }


            return Json(false, JsonRequestBehavior.AllowGet);
        }

    }
}