using AfrikcreditData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AfrikcreditData.Interfaces
{
    public interface INotificationService
    {
        bool AddNotification(string notification, out string message);
        bool RemoveNotification(long notificationID, out string message);
        List<Notification> GetAll();
    }
}
