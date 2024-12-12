using System;

namespace Services.Models;
using Dental_Clinic.Enums;
using System.Text.Json.Serialization;

public class EmailDto
{
    public string To { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public int ClinicId { get; set; }
        public int ServiceId { get; set; }
        public int DentistId { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentStatus Status { get; set; }
}
