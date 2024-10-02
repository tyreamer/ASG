using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASGShared.Models
{
    public class UserCluster
    {
        [Key]
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ClusterId { get; set; }
        public DateTime ClusteredAt { get; set; }
    }
}
