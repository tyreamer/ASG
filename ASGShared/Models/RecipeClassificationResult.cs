using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASGShared.Models
{
    public class RecipeClassificationResult
    {
        [Key]
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string ClassificationLabel { get; set; }
        public float Confidence { get; set; }
        public DateTime ClassifiedAt { get; set; }
    }
}
