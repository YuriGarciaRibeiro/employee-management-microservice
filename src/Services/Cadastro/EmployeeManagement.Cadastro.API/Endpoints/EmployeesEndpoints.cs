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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        })
        .WithName("CreateEmployee")
        .WithSummary("Creates a new employee")
        .WithDescription("Creates a new employee with the provided details.")
        .Produces<EmployeeDto>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        employees.MapGet("/{id}", async (IMediator mediator, Guid id) =>
        {
            var query = new GetEmployeeByIdQuery(id);
            var result = await mediator.Send(query);
            return result == null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetEmployeeById")
        .WithSummary("Gets an employee by ID")
        .WithDescription("Retrieves the details of an employee by their unique ID.")
        .Produces<EmployeeDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        employees.MapGet("/", async (IMediator mediator, int page = 1, int pageSize = 10) =>
        {
            var query = new GetEmployeesQuery(page, pageSize);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetEmployees")
        .WithSummary("Gets a paginated list of employees")
        .WithDescription("Retrieves a paginated list of employees.")
        .Produces<PaginatedResultDto<EmployeeDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        employees.MapGet("/by-date-range", async (IMediator mediator, DateTime startDate, DateTime endDate) =>
        {
            var query = new GetEmployeesByDateRangeQuery(startDate, endDate);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetEmployeesByDateRange")
        .WithSummary("Gets employees by start date range")
        .WithDescription("Retrieves employees who started within the specified date range.")
        .Produces<IEnumerable<EmployeeDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        employees.MapGet("/grouped-by-department", async (IMediator mediator, DateTime? startDate, DateTime? endDate) =>
        {
            var query = new GetEmployeesGroupedByDepartmentQuery(startDate, endDate);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetEmployeesGroupedByDepartment")
        .WithSummary("Gets employees grouped by department")
        .WithDescription("Retrieves employees grouped by their respective departments, optionally filtered by start date range.")
        .Produces<List<EmployeeGroupedByDepartmentDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        employees.MapPut("/{id}", async (IMediator mediator, Guid id, UpdateEmployeeDto dto) =>
        {
            var command = new UpdateEmployeeCommand(id, dto);
            await mediator.Send(command);
            return Results.NoContent();
        })
        .WithName("UpdateEmployee")
        .WithSummary("Updates an existing employee")
        .WithDescription("Updates the details of an existing employee.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        employees.MapPut("/{id}/start-date", async (IMediator mediator, Guid id, UpdateStartDateDto dto) =>
        {
            var command = new UpdateStartDateCommand(id, dto.StartDate);
            await mediator.Send(command);
            return Results.NoContent();
        })
        .WithName("UpdateEmployeeStartDate")
        .WithSummary("Updates an employee's start date")
        .WithDescription("Updates the start date of an existing employee.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        employees.MapDelete("/{id}", async (IMediator mediator, Guid id) =>
        {
            var command = new DeleteEmployeeCommand(id);
            await mediator.Send(command);
            return Results.NoContent();
        })
        .WithName("DeleteEmployee")
        .WithSummary("Deletes an employee")
        .WithDescription("Deletes an existing employee by their unique ID.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}
