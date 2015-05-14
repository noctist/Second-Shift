using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SecondShiftMobile.Networking
{
    public enum URLDisplayMode { ExcludeNullValues, ShowNullAsEmptyString, IncludeNullValues }
    /// <summary>
    /// A class which makes it easier to construct and modify URLs with parameters
    /// </summary>
    public class URLConstructor
    {
        //This dictionary holds the parameters for the URL
        Dictionary<string, string> values;

        
        public string this[string key]
        {
            get
            {
                return this.GetValue(key);
            }

            
            set
            {
                this.SetValue(key, value);
            }
        }

        URLDisplayMode displayMode = URLDisplayMode.ExcludeNullValues;
        /// <summary>
        /// This is the mode used to construct the URL string when ToString() is called
        /// </summary>
        public URLDisplayMode DisplayMode
        {
            get
            {
                return displayMode;
            }
            set
            {
                displayMode = value;
            }
        }
        string baseAddress;
        /// <summary>
        /// This is the base address of the URL (without the parameters)
        /// </summary>
        public string BaseAddress
        {
            get
            {
                return baseAddress;
            }
            set
            {
                setBaseAddress(value);
            }
        }
        public URLConstructor()
        {
            values = new Dictionary<string, string>();
        }

        public URLConstructor(Uri uri)
            : this(uri.ToString())
        {

        }

        public URLConstructor(string url)
            : this()
        {
            setBaseAddress(url);
        }
        void setBaseAddress(string url)
        {
            string[] splitUrl = url.Split('?');

            //set the base address to to be the value that comes before the '?'
            if (splitUrl.Length > 1)
            {
                baseAddress = splitUrl[0];
            }
            else
            {
                if (splitUrl[0].Contains('='))
                    baseAddress = "";
                else baseAddress = splitUrl[0];
            }

            //if splitURL contains more than one string (and the second string isn't empty), then that means that there was a question mark and URL parameters after it
            if ((!string.IsNullOrWhiteSpace(splitUrl.Last()) && splitUrl.Last().Contains('=')))
            {
                //separate the different queries into an array ('&' is used to separate URL parameters)
                string[] queries = splitUrl.Last().Split('&');

                foreach (var query in queries)
                {
                    if (query.Contains("="))
                    {
                        string[] nameValue = query.Split('=');
                        if (nameValue.Length > 0)
                        {
                            SetValue(nameValue[0], decode(nameValue[1]));
                        }
                        else SetValue(nameValue[0], null);
                    }
                    else
                    {
                        SetValue(query, "");
                    }
                }
            }
        }
        string decode(string s)
        {
#if WINDOWS_PHONE
            return WebUtility.UrlDecode(s);
#else
            return System.Web.HttpUtility.UrlDecode(s);
#endif
        }
        public bool ContainsKey(string key)
        {
            return values.ContainsKey(key);
        }
        public string GetValue(string key)
        {
            //return the value with this key from the dictionary, and return null if it doesn't exist
            if (values.ContainsKey(key))
            {
                return values[key];
            }
            else
            {
                return null;
            }
        }

        public void SetValue(string key, object value)
        {
            //set the string with this key to the specified value, and create the key if it doesn't exist
            if (value != null)
            {
                if (values.ContainsKey(key))
                {
                    values[key] = value.ToString();
                }
                else
                {
                    values.Add(key, value.ToString());
                }
            }
        }

        public void RemoveValue(string key)
        {
            //removes the value with this key if it exists
            if (values.ContainsKey(key))
            {
                values.Remove(key);
            }
        }
        public override string ToString()
        {
            return ToString(displayMode);
        }

        /// <summary>
        /// Return the URL as a string
        /// </summary>
        /// <param name="mode">The mode used to convert the URL into a string</param>
        /// <returns></returns>
        public string ToString(URLDisplayMode mode)
        {
            string url = ((string.IsNullOrEmpty(baseAddress)) ? "" : (baseAddress + "?"));
            var queries = values.ToArray();
            for (int i = 0; i < queries.Length; i++)
            {
                KeyValuePair<string, string> query = queries[i];
                if (query.Key != null)
                {
                    switch (mode)
                    {
                        case URLDisplayMode.IncludeNullValues:
                            if (i != 0)
                            {
                                url += '&';
                            }
                            if (query.Value == null)
                            {
                                url += query.Key + "=null";
                            }
                            else
                            {
                                url += query.Key + "=" + decode(query.Value);
                            }
                            break;
                        case URLDisplayMode.ExcludeNullValues:
                            if (query.Value != null)
                            {
                                if (i != 0)
                                {
                                    url += '&';
                                }
                                url += query.Key + "=" + decode(query.Value);
                            }
                            break;
                        case URLDisplayMode.ShowNullAsEmptyString:
                            if (i != 0)
                            {
                                url += '&';
                            }
                            if (query.Value == null)
                            {
                                url += query.Key + "=";
                            }
                            else
                            {
                                url += query.Key + "=" + decode(query.Value);
                            }
                            break;
                    }
                }
                
            }
            return url;
        }

        /// <summary>
        /// Convert the URL into a URI using the default URLDisplayMode
        /// </summary>
        /// <param name="uriKind">The kind of URI to create</param>
        /// <returns>A URI of the current URL</returns>
        public Uri ToUri(UriKind uriKind)
        {
            return ToUri(uriKind, displayMode);
        }

        /// <summary>
        /// Convert the URL into a URI using the given URLDisplayMode
        /// </summary>
        /// <param name="uriKind">The kind of URI to create</param>
        /// <param name="mode">The mode used to convert the URL into a string</param>
        /// <returns>A URI of the current URL</returns>
        public Uri ToUri(UriKind uriKind, URLDisplayMode mode)
        {
            return new Uri(ToString(mode), uriKind);
        }

        
    }
}
