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
using Sitecore.Data.Managers;
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

                var bucketableItems = new List<BucketableItem>();
                foreach (var bucketable in bucketables)
                {
                    var item = new BucketableItem();
                    var template = TemplateManager.GetTemplate(bucketable);

                    var tem = template.GetBaseTemplates().Where(x => x.DescendsFrom(new ID("{1930BBEB-7805-471A-A3BE-4858AC7CF696}")));

                    var fields = tem.SelectMany(x => x.GetFields(false)).Distinct();
                    item.Item = bucketable;
                    item.TemplateFields = fields;
                    bucketableItems.Add(item);

                }
                bucketItem.BucketableItems = bucketableItems;

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
