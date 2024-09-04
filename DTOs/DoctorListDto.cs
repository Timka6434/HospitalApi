namespace HospitalApi.DTO
{
    public class DoctorListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string CabinetNumber { get; set; }  
        public string SpecializationName { get; set; }  
        public string? RegionNumber { get; set; } 
    }
}
