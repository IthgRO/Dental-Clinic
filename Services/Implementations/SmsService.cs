using System;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using Services.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class SmsService : ISmsService
{
    private readonly IConfiguration _config;
    private readonly IClinicService _clinicService;
    private readonly IServiceService _serviceService;
    private readonly IDentistService _dentistService;

    public SmsService(IConfiguration config, IClinicService clinicService, IServiceService serviceService, IDentistService dentistService)
    {
        _config = config;
        _clinicService = clinicService;
        _serviceService = serviceService;
        _dentistService = dentistService;

        TwilioClient.Init(
            _config["Twilio:AccountSid"],
            _config["Twilio:AuthToken"]
        );
    }

    public void SendSms(SmsDto request)
    {
        var clinicName = _clinicService.GetClinicNameById(request.ClinicId);
        var serviceName = _serviceService.GetServiceNameById(request.ServiceId);
        var dentistName = _dentistService.GetDentistNameById(request.DentistId);
        var formattedStartTime = request.StartTime.ToString("dd MMM yyyy, h:mm tt");

        string messageBody = $"Your appointment at {clinicName} is confirmed!\n" +
                             $"Date: {formattedStartTime}\n" +
                             $"Service: {serviceName}\n" +
                             $"Dentist: {dentistName}";

        var message = MessageResource.Create(
            to: new PhoneNumber(request.To),
            from: new PhoneNumber(_config["Twilio:FromPhoneNumber"]),
            body: messageBody
        );

        Console.WriteLine($"SMS Sent: {message.Sid}");
    }
}
