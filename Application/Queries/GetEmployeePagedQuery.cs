using Application.Responses;
using AutoMapper;
using Domain.Common;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries
{
    public class GetEmployeePagedQuery : IRequest<PagedResult<EmployeeBasicResponse>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetEmployeePagedQueryHandler : IRequestHandler<GetEmployeePagedQuery, PagedResult<EmployeeBasicResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _repository;

        public GetEmployeePagedQueryHandler(IMapper mapper, IEmployeeRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<PagedResult<EmployeeBasicResponse>> Handle(GetEmployeePagedQuery request, CancellationToken cancellationToken)
        {
            var pagedData = await _repository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
            var mappedEmployees = _mapper.Map<List<EmployeeBasicResponse>>(pagedData.Items);

            return new PagedResult<EmployeeBasicResponse>
            {
                Items = mappedEmployees,
                TotalCount = pagedData.TotalCount,
                PageSize = pagedData.PageSize,
                CurrentPage = pagedData.CurrentPage
            };
        }
    }
}
