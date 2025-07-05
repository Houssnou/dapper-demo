namespace VideoRental.API.Shared.Options
{
    public class ConfigOptions
    {
        public const string SectionName = "ConnectionStrings";
        public string DefaultConnection { get; set; } = string.Empty;
    }

}
