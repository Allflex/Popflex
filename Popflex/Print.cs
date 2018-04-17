namespace Popflex
{
    public class Print
    {
        private static readonly TuesPechkin.IConverter PdfConverter = CreatePdfConverter();
        
        public static string SalesOrder(AllfleXML.FlexOrder.OrderHeader order, string htmlTempalte = null, string outputPath = null)
        {
            var config = new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<AllfleXML.FlexOrder.OrderHeader, OrderTemplate>());
            var mapper = config.CreateMapper();

            return SalesOrder(mapper.Map<OrderTemplate>(order), htmlTempalte, outputPath);
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
            var document = new TuesPechkin.HtmlToPdfDocument
            {
                Objects = {
                    new TuesPechkin.ObjectSettings
                    {
                        HtmlText = html,

                        FooterSettings = new TuesPechkin.FooterSettings
                        {
                            FontSize = 8,
                            RightText = "[page] of [topage]"
                        }
                    }
                }
            };

            return PdfConverter.Convert(document);
        }

        private static TuesPechkin.IConverter CreatePdfConverter()
        {
            var tempDeployment = new TuesPechkin.TempFolderDeployment();
            var deployment = new TuesPechkin.WinAnyCPUEmbeddedDeployment(tempDeployment);
            var toolset = new TuesPechkin.PdfToolset(deployment);
            return new TuesPechkin.ThreadSafeConverter(toolset);
        }
    }
}
