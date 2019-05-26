using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OPIDEntities
{
    public class Invitation
    {
        [Key]
        public int Id { get; set; }

        public DateTime Extended { get; set; }

        public DateTime Accepted { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }
    }
}
