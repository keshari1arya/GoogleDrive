using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDrive.Model
{

    public class GoogleDriveFile
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string OriginalFilename { get; set; }
        public string ThumbnailLink { get; set; }
        public string IconLink { get; set; }
        public string WebContentLink { get; set; }
        private DateTime createdDate;
        public DateTime CreatedDate
        {
            get
            {
                var istDate = TimeZoneInfo.ConvertTimeFromUtc(this.createdDate,TimeZoneInfo.Local);
                return istDate;
            }
            set
            {
                this.createdDate = value;
            }
        }

        private DateTime modifiedDate;
        public DateTime ModifiedDate
        {
            get
            {
                var istDate = TimeZoneInfo.ConvertTimeFromUtc(this.modifiedDate, TimeZoneInfo.Local);
                return istDate;
            }
            set
            {
                this.modifiedDate = value;
            }
        }

        public GoogleDriveFileLabel Labels { get; set; }
    }

    public class GoogleDriveFileLabel
    {
        public bool Starred { get; set; }
        public bool Hidden { get; set; }
        public bool Trashed { get; set; }
        public bool Restricted { get; set; }
        public bool Viewed { get; set; }
    }
}
