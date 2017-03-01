using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace K2WorkflowUnitTest
{
    public enum TestType
    {

         ProcessName,
         Folio,
         ProcessInstance,
         StartStatus,
         TaskCount,
         SerialNumber,
         Destination,
         TaskActivity,
         Actions,
         Activities,
        
    }


    public enum TestSubTypes
    {

        Action,
        Activity,
        Event,
    }

    public class Results
    {
        private string assemblyFolder;
        private string xmlFileName;
        public Results()
        {
            this.assemblyFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            this.xmlFileName = Path.Combine(assemblyFolder, "DataStoreResults.xml");
        }
        public void SetUpXML(string ProcessName, string Folio)
        {
            //Gets the settings file 


            if (!File.Exists(xmlFileName))
            {
                XmlTextWriter writer = new XmlTextWriter(xmlFileName, System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartElement("Settings");
                writer.WriteStartElement("ProcessName");
                writer.WriteString(ProcessName);
                writer.WriteEndElement();
                writer.WriteStartElement("Folio");
                writer.WriteString(Folio);
                writer.WriteEndElement();
                writer.WriteStartElement("ProcessInstanceId");
                writer.WriteString("0");
                writer.WriteEndElement();
                writer.WriteStartElement("StartStatus");
                writer.WriteString("Stop");
                writer.WriteEndElement();
                writer.WriteStartElement("TaskCount");
                writer.WriteAttributeString("Actual","0");
                writer.WriteEndAttribute();
                writer.WriteString("0");
                writer.WriteEndElement();
                writer.WriteStartElement("SerialNumber");
                writer.WriteString("0");
                writer.WriteEndElement();
                writer.WriteStartElement("Destination");
                writer.WriteString("0");
                writer.WriteEndElement();
                writer.WriteStartElement("TaskActivity");
                writer.WriteString("0");
                writer.WriteEndElement();
                writer.WriteStartElement("Actions");
                    writer.WriteStartElement("Action");
                    writer.WriteString("0");
                    writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("ActionResult");
                writer.WriteString("0");
                writer.WriteEndElement();
                writer.WriteStartElement("Activities");
     
                writer.WriteStartElement("Activity");
               
                writer.WriteString("0");
                writer.WriteStartElement("Event");
                writer.WriteString("0");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
            else
            {

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFileName);
                xmlDoc.ChildNodes[1].ChildNodes[0].InnerXml = ProcessName;
                xmlDoc.ChildNodes[1].ChildNodes[1].InnerXml = Folio;
                xmlDoc.Save(xmlFileName);
            }



        }


     




        /// <summary>
        /// Saves the result into XML Document
        /// </summary>
        /// <param name="testType"></param>
        /// <param name="Result"></param>
        public void SaveResult(TestType testType, string Result)
        {
            int childNodeIndex = 0;
            childNodeIndex =   (int)testType;
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFileName);
                xmlDoc.ChildNodes[1].ChildNodes[childNodeIndex].InnerXml = Result;
                xmlDoc.Save(xmlFileName);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

        }


        /// <summary>
        /// Saves  the result of sub node in XML Document
        /// </summary>
        /// <param name="testType"></param>
        /// <param name="subTestType"></param>
        /// <param name="Result"></param>

        public void SaveResult(TestType testType, TestSubTypes subTestType ,string Result)
        {
            int childNode = 0;
            childNode = (int)testType;

            int childNodeIndex = 0;
            childNodeIndex = (int)subTestType;

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFileName);
                xmlDoc.ChildNodes[1].ChildNodes[childNode].ChildNodes[childNodeIndex].InnerXml = Result;
                xmlDoc.ChildNodes[1].ChildNodes[childNode].ChildNodes[childNodeIndex].Attributes["Name"].Value = Result;
                xmlDoc.Save(xmlFileName);
        }
            catch (Exception ex)
            {

            }
            finally
            {

            }

        }


 
        /// <summary>
        /// Gets a saved result
        /// </summary>
        /// <param name="testType"></param>
        /// <returns></returns>
        public string GetResult(TestType testType)
        {

            int childNodeIndex = 0;
            childNodeIndex = (int)testType;

            string results = string.Empty;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName);
            results = xmlDoc.ChildNodes[1].ChildNodes[childNodeIndex].InnerXml;
            return results;

        }



    }
}
