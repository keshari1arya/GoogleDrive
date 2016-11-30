using Google.Apis.Drive.v2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDrive.Model
{
    public class GoogleDriveFile
    {
        public GoogleDriveFile()
        {

        }
        public GoogleDriveFile(File file)
        {
            CreatedDate = Convert.ToDateTime(file.CreatedDate);
            IconLink = file.IconLink;
            Id = file.Id;
            Labels = new GoogleDriveFileLabel
            {
                Hidden = file.Labels.Hidden.Value,
                Restricted = file.Labels.Restricted.Value,
                Starred = file.Labels.Starred.Value,
                Trashed = file.Labels.Trashed.Value,
                Viewed = file.Labels.Viewed.Value
            };
            ModifiedDate = Convert.ToDateTime(file.ModifiedDate);
            OriginalFilename = file.OriginalFilename;
            ThumbnailLink = file.ThumbnailLink;
            Title = file.Title;
            WebContentLink = file.WebContentLink;
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string OriginalFilename { get; set; }
        public string ThumbnailLink { get; set; }
        public string IconLink { get; set; }
        public string WebContentLink { get; set; }
        internal DateTime createdDate;
        public DateTime CreatedDate
        {
            get
            {
                var istDate = TimeZoneInfo.ConvertTime(this.createdDate,TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.Id));
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
                var istDate = TimeZoneInfo.ConvertTime(this.modifiedDate, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.Id));
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
