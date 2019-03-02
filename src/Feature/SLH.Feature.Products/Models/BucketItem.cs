using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;

namespace SLH.Feature.Products.Models
{
    public class SearchBucketItem : SearchResultItem
    {
        [IndexField("isbucket_text_s")]
        public virtual string IsBucket { get; set; }

        [IndexField("__bucketable")]
        public virtual bool IsBucketable { get; set; }
    }
}