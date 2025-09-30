using System;

namespace citas.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int BusinessId { get; set; }
        public Business Business { get; set; }
        public bool IsCancelled { get; set; }
    }
}
