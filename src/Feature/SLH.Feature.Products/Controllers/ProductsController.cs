using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sitecore.Buckets.Managers;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using SLH.Feature.Products.Models;

namespace SLH.Feature.Products.Controllers
{
    class ProductsController : Controller
    {
        public ActionResult ProductsListing()
        {
            var buckets = GetAllBuckets("/sitecore/content/");
            
            var bucketList = new List<Bucket>();
            foreach (var bucket in buckets)
            {
                var bucketItem = new Bucket {Item = bucket};

                var bucketables = GetAllItemsFromBucket(bucket.Paths.Path);
                bucketItem.BucketableItems = bucketables.ToList();

                bucketList.Add(bucketItem);
            }
            
            return View(bucketList);
        }

        public IEnumerable<Item> GetAllItemsFromBucket(string bucketPath)
        {
            var index = ContentSearchManager.GetIndex("sitecore_master_index");
            using (var context = index.CreateSearchContext())
            {
                var results = context.GetQueryable<SearchBucketItem>().Where(x => x.Path.StartsWith(bucketPath) && x.IsBucketable).ToList();
                
                return results.Select(x => x.GetItem());
            }
        }

        public IEnumerable<Item> GetAllBuckets(string bucketPath)
        {
            var index = ContentSearchManager.GetIndex("sitecore_master_index");

            using (var context = index.CreateSearchContext())
            {
                var results = context.GetQueryable<SearchBucketItem>().Where(x => x.Path.StartsWith(bucketPath) && x.IsBucket == "1").ToList();

                return results.Select(x => x.GetItem());
            }
        }
    }
}
