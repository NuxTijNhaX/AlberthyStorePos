using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace AlberthyPosService
{
    [DataContract]
    public class InvoiceHeaderData
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public string PaymentMethodName { get; set; }
    }
}