namespace RSAAPI.Models
{
    public class DbUser
    {
        public long Id { get; set; }
        public string? Email { get; set; }
        public string? LicenseKey {  get; set; }
        public string? ApiToken { get; set; }
        public string? SandBoxToken { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
