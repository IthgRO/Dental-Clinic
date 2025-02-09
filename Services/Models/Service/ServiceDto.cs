namespace Services.Models.Service
{
    public class ServiceDto
    {
        public int DentistId {  get; set; }
        public required string Name { get; set; }
        public required string Category { get; set; }
        public int Duration { get; set; }
    }
}
