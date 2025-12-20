using Application.Requests;
using Application.Responses;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Tests.Tests.Mocks
{
    public static class MapperMock
    {
        public static IMapper Create()
        {
            var services = new ServiceCollection();
            
            services.AddLogging();
            
            services.AddAutoMapper(cfg =>
            {
                // Request to Entity mappings
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
                        DateOnly.ParseExact(src.BirthDate!, "yyyy/MM/dd")));

                cfg.CreateMap<EmployeePhoneRequest, EmployeePhone>();

                // Entity to Response mappings
                cfg.CreateMap<Employee, EmployeeResponse>();
                cfg.CreateMap<Employee, EmployeeBasicResponse>();
                cfg.CreateMap<EmployeePhone, EmployeePhoneResponse>();
            });

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IMapper>();
        }
    }
}