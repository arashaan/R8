using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace R8.Lib
{
    public static class Serializer
    {
        public static TModel Deserialize<TModel>(string json, JsonSerializerSettings jsonSettings = null) where TModel : class, new()
        {
            return string.IsNullOrEmpty(json)
                ? new TModel()
                : JsonConvert.DeserializeObject<TModel>(json, jsonSettings ?? JsonSettingsExtensions.JsonNetSettings);
        }

        private const string JsonItemDivider = "....";
        private const string JsonPropertyDivider = "|";
        private const string JsonPropertyMemberDivider = "..";

        public static string Encode(string json, Type modelType)
        {
            var listType = typeof(List<>).MakeGenericType(modelType);

            var jsonList = JsonConvert.DeserializeObject(json, listType);
            if (jsonList == null)
                return default;

            if (!(jsonList is IList jIList))
                return default;

            var itemsString = new StringBuilder();
            var itemIndicator = 0;
            foreach (var item in jIList)
            {
                var itemProps = item.GetPublicProperties();
                if (itemProps == null || itemProps.Count <= 0)
                    continue;

                var itemString = new StringBuilder();
                var propertyIndicator = 0;

                var oneProperty = modelType.GetProperties().Length == 1;
                foreach (var prop in itemProps)
                {
                    var propValue = prop.GetValue(item);
                    var key = prop.GetJsonProperty();

                    if (oneProperty)
                    {
                        itemString.Append(propValue);
                    }
                    else
                    {
                        itemString
                            .Append(key)
                            .Append(JsonPropertyMemberDivider)
                            .Append(propValue);
                    }

                    if (propertyIndicator != itemProps.Count - 1)
                        itemString.Append(JsonPropertyDivider);

                    propertyIndicator++;
                }

                itemsString.Append(itemString);
                if (itemIndicator != jIList.Count - 1)
                    itemsString.Append(JsonItemDivider);

                itemIndicator++;
            }

            var result = itemsString.ToString();
            return result;
        }

        public static string Decode<TModel>(string json)
        {
            // f..ert|t..ert|nm..متراژ|id..15bf9d15-07bc-4f3c-8339-8192c8fd0c18
            var modelType = typeof(TModel);
            if (string.IsNullOrEmpty(json))
                return default;

            if (json == "[]")
                return default;

            if (json.StartsWith("["))
                return json;

            var splitByItem = json.Split(JsonItemDivider);
            if (splitByItem?.Any() != true)
                return json;

            var properties = typeof(TModel).GetProperties();
            var oneProperty = properties.Length == 1;
            var firstProperty = properties.FirstOrDefault();

            var items = (from item in splitByItem
                         select item.Split(JsonPropertyDivider) into splitByProperty
                         where splitByProperty?.Any() == true
                         select (from property in splitByProperty
                                 select property.Split(JsonPropertyMemberDivider)
                                 into split
                                 where split?.Any() == true
                                 let key = oneProperty ? firstProperty.GetJsonProperty() : split[0]
                                 let propertyInfo = key.GetProperty<TModel>()
                                 let type = propertyInfo.PropertyType
                                 let isNumeric = type == typeof(int) || type == typeof(double) || type == typeof(decimal) || type == typeof(long)
                                 let value = oneProperty
                                     ? isNumeric ? split[0] : $"\"{split[0]}\""
                                     : isNumeric ? split[1] : $"\"{split[1]}\""
                                 select $"\"{key}\":{value}").ToList()
                         into props
                         select string.Join(", ", props)
                         into propsString
                         select "{" + propsString + "}").ToList();
            var itemsString = string.Join(", ", items);
            var array = $"[{itemsString}]";
            return array;
        }

        public static string Serialize<TModel>(this List<TModel> model, Action<JsonSerializerSettings> jsonSettings) where TModel : class
        {
            if (model?.Any() != true)
                return "[]";

            var jsonSetting = JsonSettingsExtensions.JsonNetSettings;
            jsonSettings.Invoke(jsonSetting);

            var json = model.Count > 0
                ? JsonConvert.SerializeObject(model, jsonSetting)
                : "[]";

            return json;
        }

        public static string Serialize<TModel>(this List<TModel> model, JsonSerializerSettings jsonSettings = null) where TModel : class
        {
            if (model?.Any() != true)
                return "[]";

            var json = model.Count > 0
              ? JsonConvert.SerializeObject(model, jsonSettings ?? JsonSettingsExtensions.JsonNetSettings)
              : "[]";

            return json;
        }

        public static string ToJson<TModel, TOutput>(this List<TModel> model, Func<TModel, TOutput> selector) where TModel : class
        {
            if (model?.Any() != true)
                return "[]";

            var jsonSettings = JsonSettingsExtensions.JsonNetSettings;
            jsonSettings.Formatting = Formatting.None;
            var result = model.Select(selector.Invoke).ToList();

            var json = result.Count > 0
                ? JsonConvert.SerializeObject(result, jsonSettings)
                : "[]";

            return json;
        }
    }
}