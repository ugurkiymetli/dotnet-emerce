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
                    Console.WriteLine($" Removed user: {user.Email}. ({DateTime.Now})");
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
                Console.WriteLine($"Update Prices job starts. USD/TRY is {usd}. Date: {DateTime.Now}");
                var products = service.Product.Where(p => p.IsActive && !p.IsDeleted).ToList();
                foreach ( var product in products )
                {
                    var oldPrice = product.PriceUsd;
                    product.PriceUsd = product.Price * usd;
                    product.Udatetime = DateTime.Now;
                    Console.WriteLine($"Product: {product.Id} updated. Price: {oldPrice} -> {product.PriceUsd}. ({DateTime.Now})");
                }
                service.SaveChanges();
                Console.WriteLine($"Update Prices job finished. Updated {products.Count} products. Date: {DateTime.Now}");
            }
        }
        public void SendWelcomeMail()
        {
            using ( var service = new Emerce_DB.EmerceContext() )
            {
                Console.WriteLine($"Sending welcome mail job started! Date: {DateTime.Now}");
                //Console.WriteLine($"{DateTime.Now.AddDays(-1)}");
                var users = service.User.Where(u => u.IsActive && !u.IsDeleted && !u.IsWelcomeMailSent && u.Idatetime < DateTime.Now.AddDays(-1)).ToList();
                foreach ( var user in users )
                {
                    user.IsWelcomeMailSent = true;
                    //sendWelcomeMail(user.Email);
                    Console.WriteLine($"Welcome to Emerce Id:{user.Id} - Mail:{user.Email}!");
                }
                service.SaveChanges();
                Console.WriteLine($"Sending welcome mail job finished. Sent {users.Count} mails. Date: {DateTime.Now}");
            }
        }
    }
}