using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.Domain.Common
{
    /// <summary>
    /// Base class cho tất cả entities có soft delete
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }

    /// <summary>
    /// Base class cho entities có tracking người xóa
    /// </summary>
    public abstract class BaseEntityWithDeletedBy : BaseEntity
    {
        public int? DeletedBy { get; set; }
    }
}
