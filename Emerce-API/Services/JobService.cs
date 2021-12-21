using System;
using System.Linq;
using System.Xml;

namespace Emerce_API.Services
{
    public class JobService : IJobService
    {
        public void CleanUserTable()
        {
            using ( var service = new Emerce_DB.EmerceContext() )
            {
                var users = service.User.Where(u => !u.IsActive && u.IsDeleted);
                foreach ( var user in users )
                    Console.WriteLine($" Removed user: {user.Email}. ({System.DateTime.Now})");
                service.User.RemoveRange(users);
                service.SaveChanges();
            }
        }


        public void UpdatePrices()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("http://www.tcmb.gov.tr/kurlar/today.xml");
            decimal usd = Convert.ToDecimal(xmlDoc.SelectSingleNode(string.Format("Tarih_Date/Currency[@Kod='{0}']/ForexSelling", "USD")).InnerText.Replace('.', ','));

            using ( var service = new Emerce_DB.EmerceContext() )
            {
                Console.WriteLine($"Update Prices job starts. USD/TRY is {usd}. Date: {System.DateTime.Now}");
                var products = service.Product.Where(p => p.IsActive && !p.IsDeleted).ToList();
                foreach ( var product in products )
                {
                    var oldPrice = product.PriceUsd;
                    product.PriceUsd = product.Price * usd;
                    Console.WriteLine($"Product: {product.Id} updated. Price: {oldPrice} -> {product.PriceUsd}. ({System.DateTime.Now})");
                }
                service.SaveChanges();
            }
        }
    }
}