using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoogleDrive.WebApi.Infrastructure
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        public byte Level { get; set; }
        [Required]
        public DateTime JoinDate { get; set; }
        public string FolderId { get; set; } //the id of the drive folder under which user's files are avelable
    }
}