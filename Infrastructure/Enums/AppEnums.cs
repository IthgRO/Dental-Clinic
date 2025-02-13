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
        Email,
        Both
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
    public enum ClinicTimezone
{
    // North America
    EasternStandardTime,      // UTC-5 / UTC-4 (Daylight Saving)
    CentralStandardTime,      // UTC-6 / UTC-5 (Daylight Saving)
    MountainStandardTime,     // UTC-7 / UTC-6 (Daylight Saving)
    PacificStandardTime,      // UTC-8 / UTC-7 (Daylight Saving)
    AlaskaStandardTime,       // UTC-9 / UTC-8 (Daylight Saving)
    HawaiiAleutianStandardTime, // UTC-10

    // South America
    ArgentinaStandardTime,    // UTC-3
    BrasiliaStandardTime,     // UTC-3
    ChileStandardTime,        // UTC-4 / UTC-3 (Daylight Saving)

    // Europe
    GreenwichMeanTime,        // UTC+0
    CentralEuropeanTime,      // UTC+1 / UTC+2 (Daylight Saving)
    EasternEuropeanTime,      // UTC+2 / UTC+3 (Daylight Saving)

    // Africa
    SouthAfricaStandardTime,  // UTC+2
    EgyptStandardTime,        // UTC+2
    WestAfricaStandardTime,   // UTC+1

    // Asia
    IndiaStandardTime,        // UTC+5:30
    ChinaStandardTime,        // UTC+8
    JapanStandardTime,        // UTC+9
    KoreaStandardTime,        // UTC+9
    IndochinaTime,            // UTC+7
    ArabianStandardTime,      // UTC+3

    // Australia
    AustralianEasternStandardTime,  // UTC+10 / UTC+11 (Daylight Saving)
    AustralianCentralStandardTime,  // UTC+9:30 / UTC+10:30 (Daylight Saving)
    AustralianWesternStandardTime,  // UTC+8

    // UTC (for default/fallback)
    CoordinatedUniversalTime  // UTC
}