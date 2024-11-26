namespace Dental_Clinic.Enums;

public enum UserRole
{
        Patient,
        Dentist,
        Admin
    }

public enum DaysOfWeek
    {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }

    public enum ReminderType
    {
        SMS,
        Email
    }

    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }

    public enum AppointmentChangeType
    {
        Reschedule,
        Cancel,
        Modify
    }

    public enum ReminderStatus
    {
        Pending,
        Sent,
        Failed
    }
