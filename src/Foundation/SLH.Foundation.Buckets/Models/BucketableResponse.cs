using System;
using System.Collections.Generic;

namespace SLH.Foundation.Buckets.Models
{
    public class BucketableResponse
    {
        public Guid BucketId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public string Url { get; set; }
        public IEnumerable<TemplateField> Fields { get; set; }
    }
}