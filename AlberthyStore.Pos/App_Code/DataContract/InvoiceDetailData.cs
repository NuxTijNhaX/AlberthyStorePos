using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace AlberthyPosService
{
    [DataContract]
    public class InvoiceDetailData
    {
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public decimal TotalCost { get; set; }
    }
}