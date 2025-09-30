using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using citas.Data;
using citas.Services;
using System.Linq;

namespace citas.Services
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public ReminderBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var wa = scope.ServiceProvider.GetRequiredService<WhatsAppService>();
                    var now = DateTime.UtcNow;
                    var appointments = await db.Appointments
                        .Include(a => a.Client)
                        .Where(a => !a.IsCancelled && a.Date > now)
                        .ToListAsync();
                    var reminders = appointments.Where(a =>
                        ((a.Date - now).TotalHours <= 24 && (a.Date - now).TotalHours > 23.5) ||
                        ((a.Date - now).TotalHours <= 1 && (a.Date - now).TotalHours > 0.5)
                    );
                    foreach (var appt in reminders)
                    {
                        await wa.SendMessageAsync(appt.Client.Phone, $"Recordatorio: tienes una cita el {appt.Date}");
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
