using System.Collections.Generic;
using System.Linq;

namespace VeraSoft.Plugins.PhotosManagement.Config
{
    public class PhotosManagementConfig
    {
        public PhotosManagementConfig()
        {
            PhotoTypes = string.Empty;
            DocumentTypes = string.Empty;
            VideoTypes = string.Empty;
            MusicTypes = string.Empty;
        }

        public string PhotoTypes { get; set; }
        public string DocumentTypes { get; set; }
        public string VideoTypes { get; set; }
        public string MusicTypes { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public List<string> PhotoTypesList { get { return GetExtensions(PhotoTypes.Split(';')?.ToList()); } }
        [System.Xml.Serialization.XmlIgnore()]
        public List<string> DocumentTypesList { get { return GetExtensions(DocumentTypes.Split(';')?.ToList()); } }
        [System.Xml.Serialization.XmlIgnore()]
        public List<string> VideoTypesList { get { return GetExtensions(VideoTypes.Split(';')?.ToList()); } }
        [System.Xml.Serialization.XmlIgnore()]
        public List<string> MusicTypesList { get { return GetExtensions(MusicTypes.Split(';')?.ToList()); } }

        private List<string> GetExtensions(List<string> types)
        {
            List<string> extensions = new List<string>();
            foreach (var type in types)
            {
                if (!string.IsNullOrEmpty(type))
                    extensions.Add("." + type);
            }
            return extensions;
        }
    }
}
