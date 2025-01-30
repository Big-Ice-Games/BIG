#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BIG
{
    public static class IP
    {
        private static readonly List<string> _ipServices = new List<string>()
        {
            "http://icanhazip.com",
            "https://api.ipify.org",
            "https://ipinfo.io/ip"
        };

        public static async Task<string> GetPublicIpAddress()
        {
            using var httpClient = new HttpClient();
     
            foreach (var service in _ipServices)
            {
                try
                {
                    var response = await httpClient.GetStringAsync(service);
                    return Regex.Replace(response, @"\t|\n|\r", "").Trim();
                }
                catch
                {
                    // Continue to the next service if the current one fails.
                }
            }

            throw new Exception("Unable to fetch public IP from all services.");
        }


        /// <summary>
        /// Get local ip address.
        /// </summary>
        /// <returns>Local ip address.</returns>
        /// <exception cref="Exception">Exception if failed to get local ip address.</exception>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("Failed to get local IP address.");
        }
    }
}