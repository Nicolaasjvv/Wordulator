using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WordulatorCore.Services
{
    public class EpubService
    {

        private string _filePath;        


        public EpubService(string filePath)
        {
            _filePath = filePath;            
        }

        /// <summary>
        /// Get container entry / file that serves as the entry point for an epub
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public ZipArchiveEntry GetContainerEntry(Stream fileStream)
        {
            ZipArchive bookArchive;

            bookArchive = new ZipArchive(fileStream, ZipArchiveMode.Read);
            ZipArchiveEntry metaEntry =
                bookArchive.Entries.FirstOrDefault(x => x.FullName == "META-INF/container.xml");//This is the standard entry point for an epub, per epub spec.
            return metaEntry;
                      
        }

        /// <summary>
        /// Get an the text of a page or section file.
        /// </summary>
        /// <param name="archivePath">the path of the file to retrieve for</param>
        /// <returns>string containing html of page or section</returns>
        public string GetPageAsString(string archivePath)
        {
            try
            {
  
                ZipArchive bookArchive;

                using (var fileStream = new FileStream(_filePath, FileMode.Open))
                {
                    bookArchive = new ZipArchive(fileStream, ZipArchiveMode.Read);
                    var entries = bookArchive.Entries.Select(x => x.FullName);
                    using (var entryStream = bookArchive.GetEntry(archivePath)?.Open())
                    {
                        StreamReader sr = new StreamReader(entryStream);
                        StringBuilder sb = new StringBuilder();

                        while (!sr.EndOfStream)
                        {
                            sb.AppendLine(sr.ReadLine());
                        }

                        return sb.ToString();
                    }
                }

            }
            catch (Exception ex)
            {               
                throw new Exception("Error Retrieving content - " + ex.Message);
            }

        }


        #region NavigationHelpers

        /// <summary>
        /// Given a stream, get the path of the OPF file, the file which specifies the layout of an epub, including spine pages.
        /// </summary>
        /// <param name="entryStream">Stream of container file</param>
        /// <returns>Path to the opf file/entry</returns>
        protected string GetOpfLocationFromStream(Stream entryStream)
        {
            if (entryStream != null)
            {               
                    XDocument xdocument = XDocument.Load(entryStream);
                    XElement opfLocNode = (from xml in xdocument.Descendants() where xml.Name.LocalName == "rootfile" select xml).FirstOrDefault();
                    return (string)(from atrr in opfLocNode?.Attributes("full-path") select atrr).FirstOrDefault();                
            }
            throw new Exception("Provided stream was null");
        }

        public List<string> GetPagesFromSpine()
        {
            List<string> returnList = new List<string>();

            using (var fileStream = new FileStream(_filePath, FileMode.Open))
            {
                using (var metaStream = GetContainerEntry(fileStream).Open())
                {
                    string opfLocationPath = GetOpfLocationFromStream(metaStream);
                    string opfDirPath = opfLocationPath.Replace(opfLocationPath.Split('/').Last(), "");
                   /* opfDirPath =
                        opfDirPath.StartsWith("/")
                            ? opfDirPath
                            : "/" + opfDirPath;*/ 

                    //when specifying spine pages for usage where relative references is always needed (such as webview UriResolvers) add a relative path marker
                    //to do this, remove the above commented code


                    returnList = GetPagesFromSpine(fileStream, opfLocationPath, opfDirPath);

                }
            }


            return returnList;
        }

        private List<string> GetPagesFromSpine(Stream fileStream, string opfLocationPath, string opfParentDir)
        {
            //Process:
            //Get spine from content.opf
            //get first element from spine
            //Get href from the id, this is the path in the zip/epub for the relevant spine page
            List<string> returnList = new List<string>();

            ZipArchive bookArchive;

           
            bookArchive = new ZipArchive(fileStream, ZipArchiveMode.Read);
            ZipArchiveEntry opfEntry = bookArchive.Entries.First(x => x.FullName == opfLocationPath);

            using (var openStream = opfEntry.Open())
            {
                XDocument xdocument = XDocument.Load(openStream, LoadOptions.None);
                XElement spineNode = (from xml in xdocument.Descendants() where xml.Name.LocalName == "spine" select xml).FirstOrDefault();

                foreach (var spineChild in spineNode.Elements())
                {
                    string itemId = (string)(from atrr in spineChild.Attributes("idref") select atrr).FirstOrDefault();

                    string hrefAttr = (from node in xdocument.Descendants()
                        where node.Name.LocalName == "item"
                        from atrr in node.Attributes("id")
                        where atrr.Value == itemId
                        select node.Attribute("href")?.Value).FirstOrDefault();

                    returnList.Add(opfParentDir + hrefAttr); 
                }

            }

            

            return returnList;

        }

        #endregion
    }
}
