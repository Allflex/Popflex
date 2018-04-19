using System.Collections.Generic;

namespace Popflex
{
    public class OrderTemplate : AllfleXML.FlexOrder.OrderHeader
    {
        public List<string> styles = new List<string>();
        public string CompanyLogo = @"data:image/gif;base64," + System.Convert.ToBase64String(System.IO.File.ReadAllBytes(@"Resources\Images\Allflex.png"));
        public readonly string AllflexLogo = @"data:image/gif;base64," + System.Convert.ToBase64String(System.IO.File.ReadAllBytes(@"Resources\Images\Allflex.png"));
        public readonly string DestronLogo = @"data:image/gif;base64," + System.Convert.ToBase64String(System.IO.File.ReadAllBytes(@"Resources\Images\DestronFearing.png"));

        public string OrderNumber { get; set; }
        public string MasterId { get; set; }
        public string OrderDate { get; set; }
        public string DueDate { get; set; }

        public string Requisitioner { get; set; }
        public string PrePaidFreight { get; set; }

        public string BillToName { get; set; }
        public string BillToAddress1 { get; set; }
        public string BillToAddress2 { get; set; }
        public string BillToAddress3 { get; set; }
        public string BillToCity { get; set; }
        public string BillToState { get; set; }
        public string BillToPostalCode { get; set; }
        public string BillToPhone { get; set; }

        public string SubTotal { get; set; }
        public string Tax { get; set; }
        public string ShippingCharge { get; set; }
        public string Total { get; set; }

        public new List<OrderLineTemplate> OrderLineHeaders { get; set; }
    }

    public class OrderLineTemplate : AllfleXML.FlexOrder.OrderLineHeader
    {
        public string JobNumber { get; set; }
        public string SkuDescription { get; set; }
        public string UnitPrice { get; set; }
        public string SubTotal { get; set; }
    }
}
