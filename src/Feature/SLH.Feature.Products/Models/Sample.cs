using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;

namespace SLH.Feature.Products.Models
{
    public class Bucket
    {
        public Item Item { get; set; }
        public List<BucketableItem> BucketableItems { get; set; }
    }
    
    public class BucketableItem
    {
        public Item Item { get; set; }
        public IEnumerable<TemplateField> TemplateFields { get; set; }
    }

}