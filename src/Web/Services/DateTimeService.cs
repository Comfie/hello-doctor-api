using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Web.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
