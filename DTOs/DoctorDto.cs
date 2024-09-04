using HospitalApi.Models;

namespace HospitalApi.DTO
{
    public class DoctorDto
    {
        public int? Id { get; set; }
        public string FullName { get; set; }
        public int CabinetId { get; set; }
        public int SpecializationId { get; set; }
        public int? RegionId { get; set; }
    }
}
