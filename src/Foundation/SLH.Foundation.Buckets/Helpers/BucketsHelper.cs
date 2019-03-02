using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using SLH.Foundation.Buckets.Models;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Fields;
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

                return results.Select(x => x.GetItem()).OrderBy(x => x.DisplayName);
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

                return results.Select(x => x.GetItem()).OrderBy(x => x.DisplayName);
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
                templateFields.Add(new TemplateField { FieldName = field.Name, FieldValue = GetFieldValue(field, item) });
            }

            return templateFields;
        }

        public static string GetFieldValue(Sitecore.Data.Templates.TemplateField field, Item item)
        {
            switch (field.Type.ToLower())
            {
                case "tristate":
                    return "TriState";
                case "checkbox":
                    return item.GetCheckBoxValueByFieldName(field.Name).ToString();

                case "date":
                    DateField dateField = item.Fields[field.Name];
                    return dateField.DateTime.ToString("d");

                case "datetime":
                    DateField datetimeField = item.Fields[field.Name];
                    return datetimeField.DateTime.ToString("g");

                case "number":
                    return item.Fields[field.Name].Value;

                case "integer":
                    return item.Fields[field.Name].Value;

                case "treelist with search":
                case "treelist":
                case "treelistex":
                case "treelist descriptive":
                case "checklist":
                case "multilist with search":
                case "multilist":
                    var items = item[field.Name].GetMultiListParameterItems();
                    return items != null ? string.Join(", ", items.Select(x => x.DisplayName)) : string.Empty;

                case "grouped droplink":
                case "droplink":
                case "lookup":
                case "droptree":
                case "reference":
                case "tree":
                    return item.GetPropertyAsItem(field.Name)?.DisplayName;

                case "file":
                    FileField fileField = item.Fields[field.Name];
                    return fileField.MediaItem.DisplayName;

                case "image":
                    ImageField imgField = item.Fields[field.Name];
                    return $"<img src=\"{imgField.MediaItem.GetImageUrl()}\" alt=\"{imgField.Alt}\" class=\"content-image\" />"; 

                case "general link":
                case "general link with search":
                case "custom general link":
                    LinkField linkField = item.Fields[field.Name];
                    return $"<a href=\"{linkField.GetGeneralLinkUrl()}\" target=\"{linkField.GetTarget()}\" >{linkField.Title}</a>";

                case "password":
                case "icon":
                case "rich text":
                case "html":
                case "single-line text":
                case "multi-line text":
                case "frame":
                case "text":
                case "memo":
                case "droplist":
                case "grouped droplist":
                case "valuelookup":
                    return item.Fields[field.Name].Value;
            }

            return string.Empty;
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