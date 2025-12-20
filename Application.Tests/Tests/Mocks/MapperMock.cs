using Application.Requests;
using AutoMapper;
using Domain.Entities;

namespace Application.Tests.Tests.Mocks
{
    public static class MapperMock
    {
        public static IMapper Create()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateEmployeeRequest, Employee>()
                    .ForMember(x => x.Phones, o => o.Ignore())
                    .ForMember(x => x.BirthDate, o => o.MapFrom(src =>
                        !string.IsNullOrEmpty(src.BirthDate)
                            ? DateOnly.ParseExact(src.BirthDate, "yyyy/MM/dd")
                            : default(DateOnly)));

                cfg.CreateMap<UpdateEmployeeRequest, Employee>()
                    .ForMember(x => x.Phones, o => o.Ignore())
                    .ForMember(x => x.BirthDate, o => o.Condition(src => !string.IsNullOrEmpty(src.BirthDate)))
                    .ForMember(x => x.BirthDate, o => o.MapFrom(src =>
                        DateOnly.ParseExact(src.BirthDate, "yyyy/MM/dd")));

                cfg.CreateMap<EmployeePhoneRequest, EmployeePhone>();
            });

            return config.CreateMapper();
        }
    }
}