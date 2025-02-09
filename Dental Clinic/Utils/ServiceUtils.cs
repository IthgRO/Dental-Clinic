using Dental_Clinic.Responses.Service;

namespace Dental_Clinic.Utils
{
    public class ServiceUtils
    {
        public static List<AvailableServiceViewModel> GetAvailableServices()
        {
            return [
            new()
            {
                Name = "Teeth Cleaning",
                Category = "Dental Hygene"
            },
            new ()
            {
                Name = "Cavity Filling",
                Category = "Restorative Dentistry"
            },
            new()
            {
                Name = "Teeth Whitening",
                Category = "Cosmetic Dentistry"
            },
            new()
            {
                Name = "Orthodontic Consultation",
                Category = "Orthodontics"
            },
            new()
            {
                Name = "Root Canal Treatment",
                Category = "Endodontics"
            }
            ];
        }

        public static void VerifyIfServiceIsApproved(AvailableServiceViewModel service)
        {

            if (!GetAvailableServices().Any(x => x.Name == service.Name && x.Category == service.Category))
            {
                throw new Exception("This service is not approved yet!");
            }
        }

        public static void VerifyIfListOfServicesIsApproved(List<AvailableServiceViewModel> services)
        {
            foreach (AvailableServiceViewModel service in services)
            {
                VerifyIfServiceIsApproved(service);
            }
        }
    }
}
