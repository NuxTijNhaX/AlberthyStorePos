using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace AlberthyPosService
{
    public class AlberthyPosService : IAlberthyPosService
    {
        public List<ProductData> GetProducts()
        {
            List<ProductData> productsData = new List<ProductData>();

            try
            {
                using (var database = new AlberthyDbContext())
                {
                    var products = (from product in database.Products
                                    select product).ToList();

                    foreach (var product in products)
                    {
                        var newProduct = new ProductData()
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            ImgUrl = product.ImgUrl
                        };

                        productsData.Add(newProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException is System.Data.SqlClient.SqlException)
                {
                    throw new FaultException(
                        "Exception accessing database: " + ex.InnerException.Message,
                        new FaultCode("Connect to database"));
                }
                else
                {
                    throw new FaultException(
                        "Exception reading product list: " + ex.InnerException.Message,
                        new FaultCode("Iterate through products"));
                }
            }

            return productsData;
        }

        public List<ProductData> GetProductsByCategory(string productType)
        {
            List<ProductData> productsData = new List<ProductData>();

            try
            {
                using (var database = new AlberthyDbContext())
                {
                    var products = (from product in database.Products
                                    join cate in database.ProductCategories
                                    on product.ProductCategory.Id equals cate.Id
                                    where cate.Name == productType
                                    select product).ToList();

                    foreach (var product in products)
                    {
                        var newProduct = new ProductData()
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            ImgUrl = product.ImgUrl
                        };

                        productsData.Add(newProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException is System.Data.SqlClient.SqlException)
                {
                    throw new FaultException(
                        "Exception accessing database: " + ex.InnerException.Message,
                        new FaultCode("Connect to database"));
                }
                else
                {
                    throw new FaultException(
                        "Exception reading product list by category: " + ex.InnerException.Message,
                        new FaultCode("Iterate through products"));
                }
            }

            return productsData;
        }

        public List<string> GetProductCategory()
        {
            List<string> productCategories = new List<string>();

            try
            {
                using (var database = new AlberthyDbContext())
                {
                    productCategories = (from cate in database.ProductCategories
                                         select cate.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException is System.Data.SqlClient.SqlException)
                {
                    throw new FaultException(
                        "Exception accessing database: " + ex.InnerException.Message,
                        new FaultCode("Connect to database"));
                }
                else
                {
                    throw new FaultException(
                        "Exception reading product category: " + ex.InnerException.Message,
                        new FaultCode("Iterate through productCategories"));
                }
            }

            return productCategories;
        }

        public List<CustomerData> GetCustomers()
        {
            List<CustomerData> customers = new List<CustomerData>();

            try
            {
                using (var database = new AlberthyDbContext())
                {
                    var datas = (from customer in database.Customers
                                 join contact in database.Contacts
                                 on customer.Id equals contact.CustomerId
                                 select new
                                 {
                                     Id = customer.Id,
                                     Name = customer.Name,
                                     Phone = contact.PhoneNumber,
                                     Email = contact.Email
                                 });

                    foreach (var customer in datas)
                    {
                        var newCustomer = new CustomerData()
                        {
                            Id = customer.Id,
                            Name = customer.Name,
                            PhoneNumber = customer.Phone,
                            Email = customer.Email
                        };

                        customers.Add(newCustomer);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException is System.Data.SqlClient.SqlException)
                {
                    throw new FaultException(
                        "Exception accessing database: " + ex.InnerException.Message,
                        new FaultCode("Connect to database"));
                }
                else
                {
                    throw new FaultException(
                        "Exception reading customer list: " + ex.InnerException.Message,
                        new FaultCode("Iterate through customers"));
                }
            }

            return customers;
        }

        public bool AddCustomer(CustomerData Customer)
        {
            bool isSuccess = false;
            try
            {
                using (var database = new AlberthyDbContext())
                {
                    database.Customers.Add(new Customer()
                    {
                        Name = Customer.Name
                    });

                    database.Contacts.Add(new Contact()
                    {
                        CustomerId = Customer.Id,
                        PhoneNumber = Customer.PhoneNumber,
                        Email = Customer.Email,
                    });

                    database.SaveChanges();

                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException is System.Data.SqlClient.SqlException)
                {
                    throw new FaultException(
                        "Exception accessing database: " + ex.InnerException.Message,
                        new FaultCode("Connect to database"));
                }
                else
                {
                    throw new FaultException(
                        "Exception adding customer: " + ex.InnerException.Message,
                        new FaultCode("Add customer"));
                }
            }

            return isSuccess;
        }

        public List<CustomerData> SearchCustomer(string infor)
        {
            List<CustomerData> customers = GetCustomers();

            var results = customers.Where(c =>
                c.Name.Contains(infor) ||
                c.PhoneNumber.Contains(infor) ||
                c.Email.Contains(infor)).ToList();

            return results;
        }

        public List<string> GetPaymentMethods()
        {
            List<string> paymentMethods = new List<string>();

            try
            {
                using (var database = new AlberthyDbContext())
                {
                    paymentMethods = (from payMethod in database.PaymentMethods
                                      select payMethod.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException is System.Data.SqlClient.SqlException)
                {
                    throw new FaultException(
                        "Exception accessing database: " + ex.InnerException.Message,
                        new FaultCode("Connect to database"));
                }
                else
                {
                    throw new FaultException(
                        "Exception reading payment method list: " + ex.InnerException.Message,
                        new FaultCode("Iterate through payment methods"));
                }
            }

            return paymentMethods;
        }

        public string PayUsingMoMo(string amount, string orderInfor)
        {
            return new MoMo(amount, orderInfor).GetResponseFromMoMo();
        }

        public bool AddInvoice(InvoiceHeaderData invoiceHeader, List<InvoiceDetailData> invoiceDetails, string amount)
        {
            bool isSuccess = false;

            try
            {
                using (var database = new AlberthyDbContext())
                {
                    var paymentMethodId = (from payMethod in database.PaymentMethods
                                           where payMethod.Name == invoiceHeader.PaymentMethodName
                                           select payMethod.Id).FirstOrDefault();

                    database.Invoices.Add(new Invoice()
                    {
                        guidInvoice = invoiceHeader.Id,
                        CustomerId = invoiceHeader.CustomerId,
                        Date = invoiceHeader.Date,
                        PaymentMethodId = paymentMethodId,
                    });

                    foreach (InvoiceDetailData detail in invoiceDetails)
                    {
                        database.InvoiceDetails.Add(new InvoiceDetail()
                        {
                            guidInvoice = invoiceHeader.Id,
                            ProductId = detail.ProductId,
                            Quantity = detail.Quantity,
                            TotalCost = detail.TotalCost,
                        });
                    }

                    database.SaveChanges();
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {

                if (ex.InnerException is System.Data.SqlClient.SqlException)
                {
                    throw new FaultException(
                        "Exception accessing database: " + ex.InnerException.Message,
                        new FaultCode("Connect to database"));
                }
                else
                {
                    throw new FaultException(
                        "Exception adding invoice: " + ex.InnerException.Message,
                        new FaultCode("Add invoice"));
                }
            }

            if (isSuccess)
            {
                string customerEmail;
                string content = EmailSender.GenerateEmailContent(invoiceHeader, invoiceDetails, amount);

                using (var database = new AlberthyDbContext())
                {
                    customerEmail = (from customer in database.Customers
                                     join contact in database.Contacts
                                     on customer.Id equals contact.CustomerId
                                     where invoiceHeader.CustomerId == customer.Id
                                     select contact.Email).FirstOrDefault();
                }

                EmailSender.SendEmail(customerEmail, content);
            }

            return isSuccess;
        }
    }
}
