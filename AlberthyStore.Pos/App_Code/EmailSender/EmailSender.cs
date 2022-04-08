using AlberthyPosService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

/// <summary>
/// Summary description for EmailSender
/// </summary>
public class EmailSender
{
    public EmailSender()
    {

    }

    public static bool SendEmail(string toCustomer, string content)
    {
        bool isSuccess = false;

        string mailUsername = ConfigurationManager.AppSettings["mailName"];
        string mailPassword = ConfigurationManager.AppSettings["mailPassword"];

        SmtpClient client = new SmtpClient()
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential()
            {
                UserName = mailUsername,
                Password = mailPassword,
            },
        };

        MailMessage mailMessage = new MailMessage()
        {
            From = new MailAddress(mailUsername),
            Subject = "[Alberthy CoffeeShop] - Cảm ơn bạn đã mua hàng tại Alberthy CoffeeShop",
            IsBodyHtml = true,
            Body = content,
        };

        mailMessage.To.Add(new MailAddress(toCustomer));

        try
        {
            client.Send(mailMessage);
            isSuccess = true;
        }
        catch (Exception)
        {

        }

        return isSuccess;
    }

    public static string GenerateEmailContent(InvoiceHeaderData invoiceHeader, List<InvoiceDetailData> invoiceDetails, string amount)
    {
        string customer, dateTime, paymentMethod, invoiceId;

        dateTime = invoiceHeader.Date.ToString();
        invoiceId = invoiceHeader.Id.ToString();
        paymentMethod = invoiceHeader.PaymentMethodName;

        using (var database = new AlberthyDbContext())
        {
            customer = (from cus in database.Customers
                        where cus.Id == invoiceHeader.CustomerId
                        select cus.Name).FirstOrDefault();
        }

        string content = "<h1 style=\"color: #005F69; text-align:center; \">Alberthy CoffeeShop</h1> <hr/>" +
                         "<p>Khách hàng: " + customer + "</p>" +
                         "<p>Ngày: " + dateTime + "</p>" +
                         "<p>Phương thức thanh toán: " + paymentMethod + "</p>" +
                         "<p>Mã hóa đơn: " + invoiceId + "</p> <hr/>" +
                         "<h2 style=\"color: #F26F33;\">Sản phẩm đã mua</h2>" +
                         GenerateOrderList(invoiceDetails) +
                         "<hr/> <p>Tổng tiền: " + amount +
                         "VNĐ</p><hr/><p style=\"text-align:center;\">Cảm ơn quý khách đã sử dụng dịch vụ. <br/> Alberthy CoffeeShop</p>";
        return content;
    }

    private static string GenerateOrderList(List<InvoiceDetailData> invoiceDetails)
    {
        string orderList = "<table style=\"width:100%;\"> <tr><th style=\"width:50%;text-align:left;\"> Tên </th> <th style=\"width:25%;text-align:left;\"> Giá </th> <th style=\"width:25%;text-align:left;\"> T.Tiền </th></tr>";

        foreach (InvoiceDetailData item in invoiceDetails)
        {
            string name, price, orderLine = "";

            using (var database = new AlberthyDbContext())
            {
                name = (from pro in database.Products
                        where pro.Id == item.ProductId
                        select pro.Name).FirstOrDefault();

                price = (from pro in database.Products
                         where pro.Id == item.ProductId
                         select pro.Price.ToString()).FirstOrDefault();
            }

            orderLine = "<tr><td>" + name + "</td><td>" + item.Quantity + "x" + price + "</td><td>"
                 + item.TotalCost + "</td></tr>";

            orderList += orderLine;
        }

        return orderList + "</table>";
    }
}