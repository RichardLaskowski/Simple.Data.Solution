using System;

namespace Simple.Data.Abstractions;
public abstract class AuditableEntity
{
    /// <summary>
    /// The date the entity was created.
    /// </summary>
    public DateTime CreatedOn { get; set; }
    /// <summary>
    /// Who the entity was created by.
    /// </summary>
    public string? CreatedBy { get; set; }
    /// <summary>
    /// The date the entity updated.
    /// </summary>
    public DateTime? LastModified { get; set; }
    /// <summary>
    /// Who the entity was updated by.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}
