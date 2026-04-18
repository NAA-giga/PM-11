using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace форма_сотрудника.Helpers
{
    public static class NotificationHelper
    {
        public static async Task SendNotification(string email, string subject, string message)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notifications.txt");
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n" +
                               $"Кому: {email}\n" +
                               $"Тема: {subject}\n" +
                               $"Сообщение: {message}\n" +
                               $"{new string('-', 50)}\n";

            await File.AppendAllTextAsync(logPath, logMessage);
            Debug.WriteLine(logMessage);
        }
    }
}
