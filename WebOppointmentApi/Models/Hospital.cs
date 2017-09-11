namespace WebOppointmentApi.Models
{
    public class Hospital
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string AccessTypeCode{ get; set; }
        public string AccessTypeName { get; set; }
        public string Address { get; set; }
        public string Info { get; set; }
        public string LogoUrl { get; set; }
        public string PicUrl { get; set; }
        public string Status { get; set; }
    }
}
