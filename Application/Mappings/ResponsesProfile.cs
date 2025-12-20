using Application.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class ResponsesProfile : Profile
    {
        public ResponsesProfile()
        {
            CreateMap<Employee, EmployeeResponse>();

            CreateMap<Employee, EmployeeBasicResponse>();

            CreateMap<EmployeePhone, EmployeePhoneResponse>();
        }
    }
}
