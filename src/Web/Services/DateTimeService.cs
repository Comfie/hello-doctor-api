using ApiBaseTemplate.Application.Common.Interfaces;

namespace ApiBaseTemplate.Web.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
