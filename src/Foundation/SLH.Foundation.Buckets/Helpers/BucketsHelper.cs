using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using SLH.Foundation.Buckets.Models;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Shell.Applications.ContentEditor;

namespace SLH.Foundation.Buckets.Helpers
{
    public static class BucketsHelper
    {
        public static IEnumerable<Item> GetAllBuckets(string bucketPath)
        {
            var index = ContentSearchManager.GetIndex("sitecore_master_index");

            using (var context = index.CreateSearchContext())
            {
                var results = context.GetQueryable<SearchBucketItem>().Where(x => x.Path.StartsWith(bucketPath) && x.IsBucket == "1").ToList();

                return results.Select(x => x.GetItem());
            }
        }

        public static IEnumerable<Item> GetItemsFromBucket(ID bucketPath, string name)
        {
            var index = ContentSearchManager.GetIndex("sitecore_master_index");

            var predicate = PredicateBuilder.True<SearchBucketItem>();
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(x =>
                    x.Name.Contains(name));
            }

            using (var context = index.CreateSearchContext())
            {
                var results = context.GetQueryable<SearchBucketItem>()
                    .Where(x => x.Paths.Contains(bucketPath) && x.IsBucketable)
                    .Where(predicate)
                    .ToList();

                return results.Select(x => x.GetItem());
            }
        }

        public static IEnumerable<TemplateField> GetTemplateFields(Item item)
        {
            var template = TemplateManager.GetTemplate(item);

            var inheritTemplates = template.GetBaseTemplates().Where(x => x.DescendsFrom(new ID("{1930BBEB-7805-471A-A3BE-4858AC7CF696}")));

            var templateFields = new List<TemplateField>();

            var fields = inheritTemplates.SelectMany(x => x.GetFields(false)).Distinct();
            foreach (var field in fields)
            {
                templateFields.Add(new TemplateField { FieldName = field.Name, FieldValue = item[field.ID] });
            }

            return templateFields;
        }

        public static string GenerateFieldEditorUrl(Item item, IEnumerable<TemplateField> fields)
        {
            var fieldsEditor = string.Join("|", fields.Select(x => x.FieldName));

            FieldEditorOptions fieldEditorOption = new FieldEditorOptions(CreateFieldDescriptors(item, fieldsEditor));
            fieldEditorOption.SaveItem = true;
            fieldEditorOption.PreserveSections = true;
            return fieldEditorOption.ToUrlString().ToString();
        }

        #region Private methods

        private static List<FieldDescriptor> CreateFieldDescriptors(Item item, string fields)
        {
            List<FieldDescriptor> fieldDescriptors = new List<FieldDescriptor>();
            foreach (string str in new Sitecore.Text.ListString(fields))
            {
                fieldDescriptors.Add(new FieldDescriptor(item, str));
            }
            return fieldDescriptors;
        }

        #endregion
    }
}