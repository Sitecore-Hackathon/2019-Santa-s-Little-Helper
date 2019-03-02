using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;

namespace SLH.Feature.Products.Models
{
    public class Bucket
    {
        public Item Item { get; set; }
        public List<Item> BucketableItems { get; set; }
    }

    public class Bucketable
    {
        public List<Item> BucketableItems { get; set; }
    }
}