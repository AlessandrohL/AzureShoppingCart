using AzureShoppingCart.Common;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureShoppingCart.Features.Customers.RegisterCustomer;

using RegisterCustomerResult = Results<Ok<SuccessResponse>, ProblemHttpResult>;

public sealed class RegisterCustomerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("customers", async Task<RegisterCustomerResult> (
            [FromBody] RegisterCustomerCommand command,
            ISender sender) =>
        {
            Result registerResult = await sender.Send(command);

            return registerResult.IsSuccess
                ? ApiResults.Ok()
                : ApiResults.Problem(registerResult.Error);
        })
            .WithTags(EndpointTags.Customers)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .MapToApiVersion(1);
    }
}
