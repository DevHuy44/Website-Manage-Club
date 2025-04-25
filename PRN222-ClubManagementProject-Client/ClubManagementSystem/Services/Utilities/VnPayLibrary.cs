using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Services.Utilities
{
    public class VnPayLibrary
    {
        private SortedDictionary<string, string> _requestData = new SortedDictionary<string, string>();
        private SortedDictionary<string, string> _responseData = new SortedDictionary<string, string>();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.ContainsKey(key) ? _responseData[key] : null;
        }

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            StringBuilder data = new StringBuilder();
            foreach (var kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(HttpUtility.UrlEncode(kv.Key) + "=" + HttpUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string queryString = data.ToString();
            string rawData = GetRequestRaw();
            string vnp_SecureHash = HmacSHA512(vnp_HashSecret, rawData);
            return baseUrl + "?" + queryString + "vnp_SecureHash=" + vnp_SecureHash;
        }

        public bool ValidateSignature(string vnp_HashSecret)
        {
            string vnp_SecureHash = _responseData["vnp_SecureHash"];
            _responseData.Remove("vnp_SecureHashType");
            _responseData.Remove("vnp_SecureHash");

            string rawData = GetResponseRaw();
            string checkSum = HmacSHA512(vnp_HashSecret, rawData);
            return checkSum.Equals(vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetRequestRaw()
        {
            return string.Join("&", _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)).Select(kv => $"{kv.Key}={kv.Value}"));
        }

        private string GetResponseRaw()
        {
            return string.Join("&", _responseData.Where(kv => !string.IsNullOrEmpty(kv.Value)).Select(kv => $"{kv.Key}={kv.Value}"));
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
