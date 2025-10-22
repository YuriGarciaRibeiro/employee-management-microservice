using EmployeeManagement.Cadastro.Application.DTOs;
using EmployeeManagement.Cadastro.Application.UseCases.Commands.CreateEmployee;
using EmployeeManagement.Cadastro.Application.UseCases.Commands.DeleteEmployee;
using EmployeeManagement.Cadastro.Application.UseCases.Commands.UpdateEmployee;
using EmployeeManagement.Cadastro.Application.UseCases.Commands.UpdateStartDate;
using EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeeById;
using EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployees;
using EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeesByDateRange;
using EmployeeManagement.Cadastro.Application.UseCases.Queries.GetEmployeesGroupedByDepartment;
using MediatR;
using Microsoft.AspNetCore.Builder;
using System;

namespace EmployeeManagement.Cadastro.API.Endpoints;

public static class EmployeesEndpoints
{
    public static WebApplication MapEmployeesEndpoints(this WebApplication app)
    {
        var employees = app.MapGroup("api/employees")
        .WithTags("Employees")
        .RequireAuthorization();

        employees.MapPost("/", async (IMediator mediator, CreateEmployeeDto dto) =>
        {
            var command = new CreateEmployeeCommand(dto);
            var result = await mediator.Send(command);
            return Results.Created($"/api/employees/{result.Id}", result);
        });

        employees.MapGet("/{id}", async (IMediator mediator, Guid id) =>
        {
            var query = new GetEmployeeByIdQuery(id);
            var result = await mediator.Send(query);
            return result == null ? Results.NotFound() : Results.Ok(result);
        });

        employees.MapGet("/", async (IMediator mediator, int page = 1, int pageSize = 10) =>
        {
            var query = new GetEmployeesQuery(page, pageSize);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        });

        employees.MapGet("/by-date-range", async (IMediator mediator, DateTime startDate, DateTime endDate) =>
        {
            var query = new GetEmployeesByDateRangeQuery(startDate, endDate);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        });

        employees.MapGet("/grouped-by-department", async (IMediator mediator, DateTime? startDate, DateTime? endDate) =>
        {
            var query = new GetEmployeesGroupedByDepartmentQuery(startDate, endDate);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        });

        employees.MapPut("/{id}", async (IMediator mediator, Guid id, UpdateEmployeeDto dto) =>
        {
            var command = new UpdateEmployeeCommand(id, dto);
            await mediator.Send(command);
            return Results.NoContent();
        });

        employees.MapPut("/{id}/start-date", async (IMediator mediator, Guid id, UpdateStartDateDto dto) =>
        {
            var command = new UpdateStartDateCommand(id, dto.StartDate);
            await mediator.Send(command);
            return Results.NoContent();
        });

        employees.MapDelete("/{id}", async (IMediator mediator, Guid id) =>
        {
            var command = new DeleteEmployeeCommand(id);
            await mediator.Send(command);
            return Results.NoContent();
        });

        return app;
    }
}
