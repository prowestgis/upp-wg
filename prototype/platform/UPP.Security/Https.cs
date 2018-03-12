using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UPP.Security
{
    public static class Https
    {
        /// <summary>
        /// The UPP security scheme is to assume that a UPP client will sign its requests using a primate key that
        /// was also used to issue the SSL certificate of its API endpoint.  Thus, a UPP client can validate a request
        /// using public URLs and avoid any external key-sharing scheme.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] GetPublicKeyFromUrl(string url)
        {
            // Create a service point object that references the URL
            var uri = new Uri(url);
            var sp = ServicePointManager.FindServicePoint(uri);

            // Create an empty request
            var groupName = Guid.NewGuid().ToString();
            var request = HttpWebRequest.Create(uri) as HttpWebRequest;
            request.ConnectionGroupName = groupName;

            using (WebResponse resp = request.GetResponse())
            {
            }

            sp.CloseConnectionGroup(groupName);
            return sp.Certificate.GetPublicKey();
        }
    }
}
