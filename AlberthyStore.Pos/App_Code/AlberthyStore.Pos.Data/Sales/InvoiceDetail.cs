namespace AlberthyPosService
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class InvoiceDetail
    {
        public int Id { get; set; }

        public Guid guidInvoice { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal TotalCost { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual Product Product { get; set; }
    }
}
