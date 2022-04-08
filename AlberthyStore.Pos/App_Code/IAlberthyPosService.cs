using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace AlberthyPosService
{
    [ServiceContract]
    public interface IAlberthyPosService
    {
        [OperationContract]
        [Description("Gets all products")]
        List<ProductData> GetProducts();

        [OperationContract]
        [Description("Gets products by category")]
        List<ProductData> GetProductsByCategory(string productType);

        [OperationContract]
        [Description("Gets the product category")]
        List<string> GetProductCategory();

        [OperationContract]
        [Description("Gets all customers")]
        List<CustomerData> GetCustomers();

        [OperationContract]
        [Description("Adds customer to database")]
        bool AddCustomer(CustomerData Customer);

        [OperationContract]
        [Description("Searches customer by personal information")]
        List<CustomerData> SearchCustomer(string infor);

        [OperationContract]
        [Description("Gets all available payment methods")]
        List<string> GetPaymentMethods();

        [OperationContract]
        [Description("Pays bill using MomMo method and receives response message")]
        string PayUsingMoMo(string amount, string orderInfor);

        [OperationContract]
        [Description("Adds invoice to database")]
        bool AddInvoice(InvoiceHeaderData invoiceHeader, List<InvoiceDetailData> invoiceDetails, string amount);
    }
}
