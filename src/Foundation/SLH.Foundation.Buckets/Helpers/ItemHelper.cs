using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Fields;
using Sitecore.Links;
using Sitecore.Resources.Media;

namespace SLH.Foundation.Buckets.Helpers
{
    public static class ItemHelper
    {
        public static IEnumerable<Item> GetMultiListParameterItems(this string itemIds)
        {
            if (string.IsNullOrEmpty(itemIds))
                return null;

            var splitItemList = itemIds.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            var itemList = splitItemList.Select(itemId =>
            {
                Item item = null;
                Guid id = Guid.Empty;
                if (Guid.TryParse(itemId, out id))
                {
                    item = Context.Database.GetItem(new ID(id));
                }
                return item;
            });

            return itemList.Where(f => f != null);
        }

        public static bool GetCheckBoxValueByFieldName(this Item item, string fieldName)
        {
            bool flag = false;

            try
            {
                flag = item.Fields[fieldName].Value == "1";
            }
            catch (Exception)
            {
                // ignored
            }

            return flag;
        }

        public static Item GetPropertyAsItem(this Item item, string propertyName)
        {
            if (item == null || string.IsNullOrEmpty(propertyName))
                return null;

            var ret = Context.Database.GetItem(item[propertyName]);

            return ret;
        }

        public static string GetGeneralLinkUrl(this LinkField linkField)
        {
            string url = string.Empty;
            if (linkField == null)
                return string.Empty;

            switch (linkField.LinkType)
            {
                case "internal":
                    url = linkField.TargetItem != null ? GetItemUrl(linkField.TargetItem) : String.Empty;
                    return url;
                case "external":
                case "mailto":
                case "anchor":
                case "javascript":
                    url = linkField.Url;
                    return url;
                case "media":
                    url = linkField.TargetItem != null
                        ? MediaManager.GetMediaUrl((Item)new MediaItem(linkField.TargetItem))
                        : String.Empty;
                    return url;
                default:
                    return String.Empty;
            }
        }

        public static string GetItemUrl(Item item)
        {
            if (item == null)
                return string.Empty;
            return LinkManager.GetItemUrl(item);
        }

        public static string GetTarget(this LinkField targetValue)
        {
            switch (targetValue.Target)
            {
                case "_blank":
                case "blank":
                case "New Browser":
                    return "_blank";
                default:
                    return "";
            }
        }

        public static string GetImageUrl(this Item currentItem, int maxWidth = 0, int width = 0, int maxHeight = 0)
        {
            if (currentItem == null)
                return null;

            var image = (MediaItem)currentItem;

            // If there's options specified, add them to the options object.
            if (width > 0 || maxWidth > 0 || maxHeight > 0)
            {
                var options = new MediaUrlOptions();
                if (width > 0)
                    options.Width = width;
                if (maxWidth > 0)
                    options.MaxWidth = maxWidth;
                if (maxHeight > 0)
                    options.MaxHeight = maxHeight;

                return StringUtil.EnsurePrefix('/',
                    HashingUtils.ProtectAssetUrl(MediaManager.GetMediaUrl(image, options)));
            }

            // Otherwise, get the image with no options specified.
            return StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(image));
        }
    }
}