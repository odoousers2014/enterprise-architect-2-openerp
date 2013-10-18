using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;
using System.Security.Permissions;
using System.Reflection;
using System.Linq;
using System.Xml.Linq;

namespace EnterpriseArchitect2OpenERP
{
    public class XmlUtility
    {
        // Fields
        private static volatile Hashtable _configFileHashTable = null;
        private const string PROTOCOL_SEPARATOR = "://";

        // Methods
        public static bool FileExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                return true;
            }
            FileIOPermission permission = null;
            try
            {
                permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, filePath);
            }
            catch
            {
                return false;
            }
            try
            {
                permission.Demand();
            }
            catch (Exception exception)
            {
                //
            }
            return false;
        }

        public static XmlDocument GetConfigAsXmlDocument(string resourcePath)
        {
            XmlDocument document = new XmlDocument();
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(resourcePath);
                document.Load(reader);
            }
            catch (Exception exception)
            {
                //
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return document;
        }


        public static XmlDocument GetConfigAsXmlDocument(Stream resourceStream)
        {
            XmlDocument document = new XmlDocument();
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(resourceStream);
                document.Load(reader);
            }
            catch (Exception exception)
            {
                //
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return document;
        }

        public static string GetFileSystemResourceWithoutProtocol(string filePath)
        {
            int index = filePath.IndexOf("://");
            if (index == -1)
            {
                return filePath;
            }
            if (filePath.Length > (index + "://".Length))
            {
                while (filePath[++index] == '/')
                {
                }
            }
            return filePath.Substring(index);
        }

        public static string getKeyValueFromXmlConfigFile(string configFileName, string xmlNodeName, string key)
        {
            string attribute;
            try
            {
                configFileName = GetFileSystemResourceWithoutProtocol(configFileName);
                XmlDocument document = null;
                if (_configFileHashTable != null)
                {
                    document = (XmlDocument) _configFileHashTable[configFileName];
                }
                if ((document == null) || (_configFileHashTable == null))
                {
                    lock (typeof(Hashtable))
                    {
                        if ((document == null) || (_configFileHashTable == null))
                        {
                            LoadAndCacheXmlConfigFile(configFileName);
                            document = (XmlDocument) _configFileHashTable[configFileName];
                        }
                    }
                }
                XmlNodeList elementsByTagName = document.GetElementsByTagName(xmlNodeName);
                XmlNodeReader reader = null;
                attribute = null;
                foreach (XmlNode node in elementsByTagName)
                {
                    reader = new XmlNodeReader(node);
                    reader.Read();
                    attribute = reader.GetAttribute(key);
                    attribute.Trim();
                }
            }
            catch (Exception exception)
            {
                return null;
            }

            return attribute;
        }

        public static string getKeyValueFromXmlConfigResource(string resourceName, string xmlNodeName, string key)
        {
            string attribute;
            try
            {
                XmlDocument document = null;
                Assembly assembly = Assembly.GetCallingAssembly();
                string[] names = assembly.GetManifestResourceNames();
                Stream resource = assembly.GetManifestResourceStream(resourceName);

                XmlDocument configAsXmlDocument = GetConfigAsXmlDocument(resource);

                if (_configFileHashTable == null)
                {
                    _configFileHashTable = new Hashtable();
                }
                _configFileHashTable[resourceName] = configAsXmlDocument;

                document = (XmlDocument) _configFileHashTable[resourceName];
                
                XmlNodeList elementsByTagName = document.GetElementsByTagName(xmlNodeName);
                XmlNodeReader reader = null;
                attribute = null;
                foreach (XmlNode node in elementsByTagName)
                {
                    reader = new XmlNodeReader(node);
                    reader.Read();
                    attribute = reader.GetAttribute(key);
                    attribute.Trim();
                }
            }
            catch (Exception exception)
            {
                return null;
            }

            return attribute;
        }

        public static void LoadAndCacheXmlConfigFile(string xmlConfigFile)
        {
            xmlConfigFile = GetFileSystemResourceWithoutProtocol(xmlConfigFile);
            FileExists(xmlConfigFile);
            XmlDocument configAsXmlDocument = GetConfigAsXmlDocument(xmlConfigFile);

            if (_configFileHashTable == null)
            {
                _configFileHashTable = new Hashtable();
            }
            _configFileHashTable[xmlConfigFile] = configAsXmlDocument;
        }

        public static XmlNode GetXmlDocument(string fileName)
        {
            XmlDocument document = new XmlDocument();
            XmlTextReader reader = null;
            try
            {
                Stream resource = new FileStream(fileName, FileMode.Open);

                reader = new XmlTextReader(resource);
                document.Load(reader);
            }
            catch (Exception exception)
            {
                //
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            if (document.ChildNodes.Count > 1)
            {
                return document.ChildNodes[1];
            }
            else
            {
                return document.ChildNodes[0];
            }
        }

        public static XDocument GetXDocument(string fileName)
        {
            XDocument document = new XDocument();
            XmlTextReader reader = null;
            try
            {
                Stream resource = new FileStream(fileName, FileMode.Open);

                reader = new XmlTextReader(resource);
                document = XDocument.Load(resource);
            }
            catch (Exception exception)
            {
                //
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return document;
        }

        public static List<XmlNode> GetXmlNode(XmlNode xmlNode, string id)
        {
            List<XmlNode> listXmlNode = new List<XmlNode>();

            foreach (XmlNode x in xmlNode.ChildNodes)
            {
                if ((x.Attributes != null) && (x.Attributes["xmi:idref"] != null) && (x.Attributes["xmi:idref"].Value.Equals(id)))
                {
                    listXmlNode.Add(x);
                }
                else if ((x.Attributes != null) && (x.Attributes["xmi:id"] != null) && (x.Attributes["xmi:id"].Value.Equals(id)))
                {
                    listXmlNode.Add(x);
                }
                else
                {
                    List<XmlNode> list = GetXmlNode(x, id);
                    foreach (XmlNode y in list)
                    {
                        listXmlNode.Add(y);
                    }
                }
            }

            return listXmlNode;
        }

        public static List<XmlNode> GetXmlNodeChildren(XmlNode xmlNode, XmlNode xmlElementNode)
        {
            List<XmlNode> listXmlNode = new List<XmlNode>();

            /*string attrib = string.Empty;
            if ((xmlNode.Attributes != null) && (xmlNode.Attributes["xmi:idref"] != null) && (xmlNode.Attributes["xmi:idref"].Value.Equals(id)))
            {
                attrib = "xmi:idref";
            }
            else if ((xmlNode.Attributes != null) && (xmlNode.Attributes["xmi:id"] != null) && (xmlNode.Attributes["xmi:id"].Value.Equals(id)))
            {
                attrib = "xmi:id";
            }

            if (!attrib.Equals(string.Empty))
            {
                foreach (XmlNode x in xmlElementNode.ChildNodes)
                {
                    if ((x["model"] != null) && (x["model"].Attributes["package"] != null))
                    {
                        if (x["model"].Attributes["package"].Value.Equals(xmlNode.Attributes[attrib].Value))
                        {
                            listXmlNode.Add(x);
                        }
                    }
                }
            }*/

            return listXmlNode;
        }

        public static XmlNode GetNodeLinded(XmlNode xmlNode, XmlNode xmlExtension)
        {
            return null;
        }
    }
}
