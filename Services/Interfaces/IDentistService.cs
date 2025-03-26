using Services.Models.Dentist;

public interface IDentistService
{
    string GetDentistNameById(int dentistId);

    Task RegisterDentist(DentistRegisterDto registerDto);
}
