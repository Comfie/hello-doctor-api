namespace HelloDoctorApi.Infrastructure.Data.Interceptors.Interfaces
{
    public interface ITimestamped
    {
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}