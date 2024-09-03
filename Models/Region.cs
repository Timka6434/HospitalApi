namespace HospitalApi.Models
{
    public class Region
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public ICollection<Patient> Patients { get; set; }
        public ICollection<Doctor> Doctors { get; set; }
    }
}
