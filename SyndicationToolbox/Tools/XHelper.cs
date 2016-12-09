using System;
using System.Net;
using System.Xml.Linq;
using System.Globalization;

namespace CodeKoenig.SyndicationToolbox.Tools
{
    /// <summary>
    /// Helper class to safely retrieve values from XML elements
    /// </summary>
    public static class XHelper
    {
        /// <summary>
        /// Safely gets the value of the given XElement as a string. Returns NULL if the string is NULL or EMPTY.
        /// </summary>
        /// <param name="xElement">The XElement instance from which to retrieve the value</param>
        /// <returns>String value of the XElement value</returns>
        public static string SafeGetString(XElement xElement)
        {
            if (xElement == null)
            {
                return null;
            }

            return String.IsNullOrEmpty(xElement.Value) ? null : xElement.Value;
        }

        /// <summary>
        /// Safely gets the value of the attribute with the given attributeName from the given XElement as a string. Returns NULL if the string is NULL or EMPTY.
        /// </summary>
        /// <param name="xElement">The XElement instance that contains the attribute</param>
        /// <param name="attributeName">The name of the attribute from which to retrieve the value</param>
        /// <returns>String value of the XElement value</returns>
        public static string SafeGetString(XElement xElement, XName attributeName)
        {
            if (xElement != null)
            {
                XAttribute result = xElement.Attribute(attributeName);

                if (result != null)
                {
                    return String.IsNullOrEmpty(result.Value) ? null : result.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Safely gets the value of the given XElement as a DateTime. Returns NULL if the value is not a valid date.
        /// </summary>
        /// <param name="xElement">The XElement instance from which to retrieve the value</param>
        /// <returns>DateTime value of the XElement value</returns>
        public static DateTime? SafeGetDateTime(XElement xElement)
        {
            DateTime? result = null;

            if (xElement != null)
            {
                DateTime parsedDate;

                if (DateTime.TryParse(xElement.Value, out parsedDate))
                {
                    result = parsedDate;
                }
            }

            return result;
        }

        /// <summary>
        /// Safely gets the value of the attribute with the given attributeName from given XElement as a DateTime. Returns NULL if the value is not a valid date.
        /// </summary>
        /// <param name="xElement">The XElement instance that contains the attribute</param>
        /// <param name="attributeName">The name of the attribute from which to retrieve the value</param>
        /// <returns>DateTime value of the XElement value</returns>
        public static DateTime? SafeGetDateTime(XElement xElement, XName attributeName)
        {
            DateTime? result = null;

            if (xElement != null)
            {
                XAttribute attr = xElement.Attribute(attributeName);

                if (attr != null)
                {
                    DateTime parsedDate;

                    if (DateTime.TryParse(attr.Value, out parsedDate))
                    {
                        result = parsedDate;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Safely gets the value of the given XElement as a Boolean. Returns NULL if the value is not a valid boolean value.
        /// </summary>
        /// <param name="xElement">The XElement instance from which to retrieve the value</param>
        /// <returns>Boolean value of the XElement value</returns>
        public static bool? SafeGetBool(XElement xElement)
        {
            if (xElement == null)
            {
                return null;
            }

            return String.IsNullOrEmpty(xElement.Value) ? (bool?)null : Boolean.Parse(xElement.Value);
        }

        /// <summary>
        /// Safely gets the value of the given XElement as an Integer. Returns NULL if the value is not a valid integer value.
        /// </summary>
        /// <param name="xElement">The XElement instance from which to retrieve the value</param>
        /// <returns>Integer value of the XElement value</returns>
        public static int? SafeGetInt(XElement xElement)
        {
            int? result = null;

            if (xElement != null)
            {
                int parsedInt;

                if (int.TryParse(xElement.Value, out parsedInt))
                {
                    result = parsedInt;
                }
            }

            return result;
        }

        /// <summary>
        /// Safely gets the value of the attribute with the given attributeName from given XElement as an Integer. Returns NULL if the value is not a valid integer.
        /// </summary>
        /// <param name="xElement">The XElement instance that contains the attribute</param>
        /// <param name="attributeName">The name of the attribute from which to retrieve the value</param>
        /// <returns>Integer value of the XElement value</returns>
        public static int? SafeGetInt(XElement xElement, XName attributeName)
        {
            int? result = null;

            if (xElement != null)
            {
                XAttribute attrib = xElement.Attribute(attributeName);

                if (attrib != null)
                {
                    int parsedInt;

                    if (int.TryParse(attrib.Value, out parsedInt))
                    {
                        result = parsedInt;
                    }
                }
            }

            return result;
        }
    }
}
