using System.Collections.Generic;

namespace Popflex
{
    public class OrderTemplate : AllfleXML.FlexOrder.OrderHeader
    {
        public List<string> styles = new List<string>();
        public string CompanyLogo = @"data:image/gif;base64," + System.Convert.ToBase64String(System.IO.File.ReadAllBytes(@"Resources\Images\DestronFearing.png"));
        public readonly string AllflexLogo = @"data:image/gif;base64," + System.Convert.ToBase64String(System.IO.File.ReadAllBytes(@"Resources\Images\Allflex.png"));
        public readonly string DestronLogo = @"data:image/gif;base64," + System.Convert.ToBase64String(System.IO.File.ReadAllBytes(@"Resources\Images\DestronFearing.png"));
    }
}
