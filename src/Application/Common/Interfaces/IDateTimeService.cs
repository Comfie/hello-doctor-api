namespace HelloDoctorApi.Application.Common.Interfaces
{
    public interface IDateTimeService
    {
        public DateTime Now { get; }
        DateTimeOffset OffsetNow { get; }
    }
}