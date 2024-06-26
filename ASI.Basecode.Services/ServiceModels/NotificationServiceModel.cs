using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    /// <summary>
    /// A model for service
    /// </summary>
    public class NotificationServiceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
    }
}
