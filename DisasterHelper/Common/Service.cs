using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Service
    {
        private static MobileServiceClient _mobileService = new MobileServiceClient(
            "--url--",
            "--key--");

        public static MobileServiceClient MobileService { get { return _mobileService; } }

        public static IMobileServiceTable<T> GetTable<T>()
        {
            return MobileService.GetTable<T>();
        }

        public static async Task InsertItemAsync<T>(T item)
        {
            var table = Service.MobileService.GetTable<T>();
            await table.InsertAsync(item);
        }

        public static async Task UpdateItemAsync<T>(T item)
        {
            var table = Service.MobileService.GetTable<T>();
            await table.UpdateAsync(item);
        }

        public static async Task DeleteItemAsync<T>(T item)
        {
            var table = Service.MobileService.GetTable<T>();
            await table.DeleteAsync(item);
        }
    }
}
