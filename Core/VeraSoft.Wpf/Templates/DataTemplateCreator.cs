using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace VeraSoft.Wpf.Templates
{
    public static class DataTemplateCreator
    {
        private const string dataTemplateString = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            xmlns:viewModels=""clr-namespace:{0};assembly={1}"" 
            xmlns:views=""clr-namespace:{2};assembly={3}"" 
            DataType=""{{x:Type viewModels:{4}}}"" >
                <views:{5} />
            </DataTemplate>";


        /// <summary>
        /// Creates a data template for templateKey types assigning them the control 
        /// in templateValue
        /// </summary>
        /// <param name="viewModelType">Type that is used as key to retrieve the template content</param>
        /// <param name="viewType">Type of the template content, that must be a control (FrameworkElement at least)</param>
        /// <returns></returns>

        public static DataTemplate CreateTemplateForType(Type viewModelType, Type viewType)
        {
            //Versión creada por nosotros a mano que recurre a menos clases de contexto del parser xaml
            StringBuilder dataTemplateXaml = new StringBuilder();
            dataTemplateXaml.AppendFormat(dataTemplateString, viewModelType.Namespace, viewModelType.Assembly.GetName().Name,
                                                  viewType.Namespace, viewType.Assembly.GetName().Name,
                                                  viewModelType.Name,
                                                  viewType.Name);


            DataTemplate dt = null;
            try
            {
                string xaml = dataTemplateXaml.ToString();//keep this in a separate line for debugging purposes (to see the generated xaml)
                XmlReader xmlReader = XmlReader.Create(new StringReader(xaml));
                dt = XamlReader.Load(xmlReader) as DataTemplate;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error creating data template for " + viewModelType.Name + " => " + viewType.Name + ": " + ex.Message);
            }

            return dt;

            //Esta versión sacada de https://www.ikriv.com/dev/wpf/DataTemplateCreation/ sí funciona
            //const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
            //var xaml = String.Format(xamlTemplate, viewModelType.Name, viewType.Name, viewModelType.Namespace, viewType.Namespace);

            //var context = new ParserContext();

            //context.XamlTypeMapper = new XamlTypeMapper(new string[0]);
            //context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
            //context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

            //context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            //context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            //context.XmlnsDictionary.Add("vm", "vm");
            //context.XmlnsDictionary.Add("v", "v");

            //var template = (DataTemplate)XamlReader.Parse(xaml, context);
            //return template;
        }

        public static DataTemplate CreateTemplateForType(Type type, string xamlUri)
        {
            XmlReader contentXml = XmlReader.Create(xamlUri);
            string dataTemplateContent = contentXml.ReadOuterXml();
            StringBuilder dataTemplateXaml = new StringBuilder();
            dataTemplateXaml.Append("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");
            dataTemplateXaml.Append(dataTemplateContent);
            dataTemplateXaml.Append("</DataTemplate>");

            XmlReader xmlReader = XmlReader.Create(new StringReader(dataTemplateXaml.ToString()));
            return XamlReader.Load(xmlReader) as DataTemplate;
        }
    }

}
