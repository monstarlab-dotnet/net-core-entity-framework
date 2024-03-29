﻿namespace Monstarlab.EntityFramework.Extension.Models;

public abstract class EntityBase<TId>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public TId Id { get; set; } 

    [Required]
    [ReadOnly(true)]
    public DateTime Created { get; set; }

    [Required]
    [ReadOnly(true)]
    public DateTime Updated { get; set; }
}
