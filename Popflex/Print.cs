using System.Linq;

namespace Popflex
{
    // TODO: Only display price header on table if any orderLiens contain price. Do not display if no order lines contain price
    // TODO: Offset @index for order lines +1
    // 

    public class Print
    {
        public static string SalesOrder(AllfleXML.FlexOrder.OrderHeader order, string htmlTempalte = null, string outputPath = null)
        {
            return SalesOrder(MapOrder(order), htmlTempalte, outputPath);
        }

        private static OrderTemplate MapOrder(AllfleXML.FlexOrder.OrderHeader order)
        {
            var config = new AutoMapper.MapperConfiguration(
                cfg => {
                    cfg.CreateMap<AllfleXML.FlexOrder.OrderHeader, OrderTemplate>();
                    cfg.CreateMap<AllfleXML.FlexOrder.OrderLineHeader, OrderLineTemplate>();
                }
            );
            var mapper = config.CreateMapper();

            var result = mapper.Map<OrderTemplate>(order);

            result.OrderNumber = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "OrderNumber")?.Value;
            result.MasterId = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "MasterId")?.Value;
            result.OrderDate = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "OrderDate")?.Value;
            result.DueDate = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "DueDate")?.Value;

            result.Requisitioner = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "Requisitioner")?.Value;
            result.PrePaidFreight = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "PrePaidFreight")?.Value;

            result.BillToName = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "BillToName")?.Value;
            result.BillToAddress1 = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "BillToAddress1")?.Value;
            result.BillToAddress2 = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "BillToAddress2")?.Value;
            result.BillToAddress3 = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "BillToAddress3")?.Value;
            result.BillToCity = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "BillToCity")?.Value;
            result.BillToState = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "BillToState")?.Value;
            result.BillToPostalCode = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "BillToPostalCode")?.Value;
            result.BillToPhone = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "BillToPhone")?.Value;

            result.SubTotal = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "SubTotal")?.Value;
            result.Tax = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "Tax")?.Value;
            result.ShippingCharge = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "ShippingCharge")?.Value;
            result.Total = order.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "Total")?.Value;

            var i = 0;
            foreach(var orderLine in order.OrderLineHeaders)
            {
                result.OrderLineHeaders[i].JobNumber = orderLine.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "JobNumber")?.Value;
                result.OrderLineHeaders[i].SkuDescription = orderLine.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "SkuDescription")?.Value;
                result.OrderLineHeaders[i].UnitPrice = orderLine.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "UnitPrice")?.Value;
                result.OrderLineHeaders[i].SubTotal = orderLine.UserDefinedFields.Fields.SingleOrDefault(f => f.Key == "SubTotal")?.Value;

                i++;
            }

            return result;
        }
        
        public static string SalesOrder(OrderTemplate order, string htmlTempalte = null, string outputPath = null)
        {
            if (string.IsNullOrWhiteSpace(htmlTempalte))
            {
                htmlTempalte = System.IO.File.ReadAllText(@"Resources\salesOrderBody.html");
                order.styles.Add(System.IO.File.ReadAllText(@"Resources\bootstrap.min.css"));
                order.styles.Add(System.IO.File.ReadAllText(@"Resources\salesOrderStyle.css"));
            }
            
            var html = ToHtml(order, htmlTempalte);
            // https://www.codeproject.com/articles/12799/print-html-in-c-with-or-without-the-web-browser-co
            System.Diagnostics.Process.Start(SaveHTML(html));
            return SavePdf(html, outputPath);
        }
        
        private static string ToHtml(AllfleXML.FlexOrder.OrderHeader order, string htmlTemplate)
        {
            var template = HandlebarsDotNet.Handlebars.Compile(htmlTemplate);
            return template(order);
        }
        
        private static string GetTempHTMLOutputPath()
        {
            var tmpFile = System.IO.Path.GetTempFileName();
            var htmlFile = System.IO.Path.ChangeExtension(tmpFile, ".html");
            System.IO.File.Move(tmpFile, htmlFile);
            return htmlFile;
        }

        private static string SaveHTML(string html, string path = null)
        {
            if (string.IsNullOrWhiteSpace(path)) path = GetTempHTMLOutputPath();
            System.IO.File.WriteAllText(path, html);
            return path;
        }

        private static string GetTempPDFOutputPath()
        {
            var tmpFile = System.IO.Path.GetTempFileName();
            var pdfFile = System.IO.Path.ChangeExtension(tmpFile, ".pdf");
            System.IO.File.Move(tmpFile, pdfFile);
            return pdfFile;
        }

        private static string SavePdf(string html, string path = null)
        {
            if (string.IsNullOrWhiteSpace(path)) path = GetTempPDFOutputPath();
            var pdfBuffer = HtmlToPdf(html);
            System.IO.File.WriteAllBytes(path, pdfBuffer);
            return path;
        }

        private static byte[] HtmlToPdf(string html)
        {
            var PdfConverter = 
                new TuesPechkin.ThreadSafeConverter(
                    new TuesPechkin.PdfToolset(
                        new TuesPechkin.WinAnyCPUEmbeddedDeployment(
                                new TuesPechkin.TempFolderDeployment())));

            var globalConfig = new TuesPechkin.GlobalSettings
            {
                //DocumentTitle = "Sales Order",
                //PaperSize = System.Drawing.Printing.PaperKind.A4,
                //UseCompression = false,
                //DPI = 1200,
                Margins =
                {
                    All = 0.3,
                    Unit = TuesPechkin.Unit.Centimeters
                },
                //ImageDPI = 1200,
                //ImageQuality = 400
            };

            var document = new TuesPechkin.HtmlToPdfDocument
            {
                Objects = {
                    new TuesPechkin.ObjectSettings
                    {
                        //WebSettings = new TuesPechkin.WebSettings
                        //{
                        //    EnableJavascript = true,
                        //    LoadImages = true,
                        //    PrintBackground = true,
                        //    PrintMediaType = true
                        //},
                        //LoadSettings = new TuesPechkin.LoadSettings
                        //{
                        //    BlockLocalFileAccess = false
                        //},

                        HtmlText = html,

                        FooterSettings = new TuesPechkin.FooterSettings
                        {
                            FontSize = 8,
                            RightText = "[page] of [topage]"
                        }
                    }
                },
                GlobalSettings = globalConfig
            };


            return PdfConverter.Convert(document);
        }
    }
}
