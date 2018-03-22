namespace Popflex
{
    public class Print
    {
        private static readonly TuesPechkin.IConverter PdfConverter = CreatePdfConverter();

        public void SalesOrder(AllfleXML.FlexOrder.OrderHeader order)
        {
            SalesOrder(order, GetHtmlTemplate());
        }

        public void SalesOrder(AllfleXML.FlexOrder.OrderHeader order, string htmlTemplate)
        {
            // TODO: Do something with the result
        }
        
        public static string ToHtml(AllfleXML.FlexOrder.OrderHeader order, string htmlTemplate)
        {
            var template = HandlebarsDotNet.Handlebars.Compile(htmlTemplate);
            return template(order);
        }

        public static string SavePdf(string html)
        {
            var tmpFile = System.IO.Path.GetTempFileName();
            var pdfFile = System.IO.Path.ChangeExtension(tmpFile, ".pdf");
            System.IO.File.Move(tmpFile, pdfFile);
            SavePdf(html, pdfFile);
            return pdfFile;
        }

        public static void SavePdf(string html, string path)
        {
            var pdfBuffer = HtmlToPdf(html);
            System.IO.File.WriteAllBytes(path, pdfBuffer);
        }

        public static byte[] HtmlToPdf(string html)
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

        public static string GetHtmlTemplate()
        {
            var BootstrapStyle = $"\n<style type=\"text/css\">\n\t{Popflex.Properties.Resources.bootstrap_min}\n</style>\n";
            var SalesOrderStyle = $"\n<style type=\"text/css\">\n\t{Popflex.Properties.Resources.salesOrderStyle}\n</style>\n";
            var HtmlHeader = "<head>" + BootstrapStyle + SalesOrderStyle + "\n</head>";
            return $"<!DOCTYPE html>\n<html lang=\"en\">\n\t{HtmlHeader}\n\t<body>\n{Popflex.Properties.Resources.salesOrderBody}\n\t</body>\n</html>";
        }
    }
}
