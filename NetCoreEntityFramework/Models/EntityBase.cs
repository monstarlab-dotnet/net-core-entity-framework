using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodes.NetCore.EntityFramework.Models
{
    public abstract class EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public DateTime Updated { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool Deleted { get; set; }
    }
}
