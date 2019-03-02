using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;

namespace SLH.Foundation.Buckets.Models
{
    public class SearchBucketItem : SearchResultItem
    {
        [IndexField("isbucket_text_s")]
        public virtual string IsBucket { get; set; }

        [IndexField("__bucketable")]
        public virtual bool IsBucketable { get; set; }
    }
}