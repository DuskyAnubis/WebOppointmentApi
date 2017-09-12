using System.ComponentModel.DataAnnotations;

namespace WebOppointmentApi.Dtos
{
    public class UserOutput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserTypeCode { get; set; }
        public string UserTypeName { get; set; }
        public int OrganazitionId { get; set; }
        public string OrganazitionCode { get; set; }
        public string OrganazitionName { get; set; }
        public int RoleId { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public string GenderCode { get; set; }
        public string GenderName { get; set; }
        public string UserRankCode { get; set; }
        public string UserRankName { get; set; }
        public string RegisteredRankCode { get; set; }
        public string RegisteredRankName { get; set; }
        public string Tel { get; set; }
        public string Info { get; set; }
        public string KeyWord { get; set; }
        public string PicUrl { get; set; }
        public string Status { get; set; }
    }
}
