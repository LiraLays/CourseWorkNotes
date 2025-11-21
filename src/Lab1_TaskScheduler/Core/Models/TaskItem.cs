using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTaskScheduler.Library.Core.Models
{
    public class TaskItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public int Priority { get; set; } // 1-4, где 1 - высший
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Вычисляемые свойства
        public bool IsOverdue => !IsCompleted && Deadline < DateTime.Now;
        public bool IsUrgent => Priority == 1 && Deadline <= DateTime.Now.AddHours(24);
    }
}
