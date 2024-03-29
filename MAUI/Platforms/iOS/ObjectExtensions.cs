﻿using Anyline.SDK.NET.iOS;
using System.Diagnostics;
using UIKit;

namespace Anyline.Examples.MAUI.Platforms.iOS
{
    /// <summary>
    /// Extension class to parse results of any kind via reflection to a list
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Creates a property dictionary via reflection of the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> CreatePropertyDictionary(this object obj)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            if (obj is Array array)
            {
                dict.Add($"Composite Results ({array.GetType()})", array.ProcessArray());
            }
            try
            {
                var props = obj.GetType().GetProperties();
                foreach (var prop in props)
                {
                    switch (prop.Name)
                    {
                        // filter out properties that we don't want to display
                        case "Handle":
                        case "PeerReference":
                        case "Outline":
                        case "Class":
                        case "Self":
                        case "ClassHandle":
                        case "Description":
                        case "DebugDescription":
                        case "IsProxy":
                        case "IsFixedSize":
                        case "IsSynchronized":
                        case "SyncRoot":
                        case "RetainCount":
                        case "Superclass":
                        case "Zone":
                        case "SuperHandle":
                        case "AccessibilityAttributedUserInputLabels":
                        case "AccessibilityRespondsToUserInteraction":
                        case "AccessibilityTextualContext":
                        case "AccessibilityUserInputLabels":
                        case "Length":
                        case "LongLength":
                        case "IsReadOnly":
                        case "Rank":
                            break;
                        default:
                            var value = prop.GetValue(obj, null);

                            if (value == null) { continue; }
                            // For Anyline objects, expand to show each property
                            if (value.GetType().Namespace == "Anyline.SDK.NET.iOS")
                            {
                                if (value is Array valueArray)
                                {
                                    dict.Add($"{prop.Name} ({prop.PropertyType})", valueArray.ProcessArray());
                                }
                                else
                                {
                                    dict.Add($"{prop.Name} ({prop.PropertyType})", value.CreatePropertyDictionary());
                                }
                            }
                            else
                            {
                                // Non-Anyline objects will be displayed with their default value, and only minor treatment
                                dict.AddProperty($"{prop.Name} ({prop.PropertyType})", value);
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return dict;
        }

        public static Dictionary<string, object> ProcessArray(this Array array)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            for (int i = 0; i < array.Length; i++)
            {
                var v = array.GetValue(i);
                dict.Add($"[{i}] ({v.GetType()})", v.CreatePropertyDictionary());
            }
            return dict;
        }

        /// <summary>
        /// Adds an object with a given name to the dictionary.
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void AddProperty(this Dictionary<string, object> dict, string name, object value)
        {
            try
            {
                if (value != null)
                {
                    if (value is UIImage && value != null)
                    {
                        dict.Add(name, (value as UIImage).AsJPEG().ToArray());
                    }
                    else if (value is byte[])
                    {
                        dict.Add(name, value);
                    }
                    else if (value.ToString() != "")
                    {
                        var str = value.ToString().Replace("\\n", Environment.NewLine);
                        dict.Add(name, str);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}