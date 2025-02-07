using System;
using System.Text.Json.Serialization;
using Dental_Clinic.Enums;

namespace Services.Models
{
    public class SmsDto
    {
        public string To { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public int ClinicId { get; set; }
        public int ServiceId { get; set; }
        public int DentistId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentStatus Status { get; set; }
    }
}


