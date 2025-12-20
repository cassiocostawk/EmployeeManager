using Application.Requests;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class RequestsProfile : Profile
    {
        public RequestsProfile()
        {
            CreateMap<CreateEmployeeRequest, Employee>()
                .ForMember(x => x.Phones, o => o.Ignore())
                .ForMember(x => x.BirthDate, o => o.MapFrom(src => DateOnly.ParseExact(src.BirthDate, "yyyy/MM/dd")))
                .ForMember(x => x.Password, o => o.Condition(src => !string.IsNullOrEmpty(src.Password)));
            
            CreateMap<UpdateEmployeeRequest, Employee>()
                .ForMember(x => x.Phones, o => o.Ignore())
                .ForMember(x => x.BirthDate, o => o.Condition(src => !string.IsNullOrEmpty(src.BirthDate)))
                .ForMember(x => x.BirthDate, o => o.MapFrom(src => DateOnly.ParseExact(src.BirthDate!, "yyyy/MM/dd")))
                .ForMember(x => x.Password, o => o.Condition(src => !string.IsNullOrEmpty(src.Password)))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<EmployeePhoneRequest, EmployeePhone>();
        }
    }
}
