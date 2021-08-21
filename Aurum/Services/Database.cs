using System.Collections.Generic;
using Aurum.Models;

namespace Aurum.Services
{
    public class Connections
    {
        public IEnumerable<ConnectionItem> GetItems() => new[]
        {
            new ConnectionItem {ConnectionName = "US Heroku 1"},
            new ConnectionItem {ConnectionName = "US Heroku 1 CF"},
            new ConnectionItem {ConnectionName = "US Vultr 1 XTLS", IsConnected = true}
        };
    }
}