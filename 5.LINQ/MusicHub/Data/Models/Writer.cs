using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicHub.Data.Models
{
    public class Writer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        public string? Pseudonym { get; set; }

        public virtual ICollection<Song> Songs { get; set; }

    }
}
