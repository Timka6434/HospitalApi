using AutoMapper;
using HospitalApi.DTO;
using HospitalApi.Models;

namespace HospitalApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Patient, PatientDto>().ReverseMap();
            CreateMap<Patient, PatientListDto>()
                .ForMember(dest => dest.RegionNumber, opt => opt.MapFrom(src => src.Region.Number));

            CreateMap<Doctor, DoctorDto>().ReverseMap();
            CreateMap<Doctor, DoctorListDto>()
                .ForMember(dest => dest.CabinetNumber, opt => opt.MapFrom(src => src.Cabinet.Number))
                .ForMember(dest => dest.SpecializationName, opt => opt.MapFrom(src => src.Specialization.Name))
                .ForMember(dest => dest.RegionNumber, opt => opt.MapFrom(src => src.Region.Number));

        }
    }
}
