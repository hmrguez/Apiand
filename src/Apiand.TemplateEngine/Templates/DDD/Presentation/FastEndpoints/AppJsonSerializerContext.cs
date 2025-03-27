using System.Text.Json.Serialization;
using Apiand.Extensions.Models;
using XXXnameXXX.Application.Identity.Commands.Login;
using XXXnameXXX.Application.Identity.Commands.Register;
using XXXnameXXX.Application.Identity.Dtos;
using XXXnameXXX.Application.Todos.Dtos;

namespace XXXnameXXX.Api;

[JsonSerializable(typeof(TodoDto[]))]
[JsonSerializable(typeof(TodoDto))]
[JsonSerializable(typeof(LoginCommand))]
[JsonSerializable(typeof(RegisterCommand))]
[JsonSerializable(typeof(LoginResponse))]
[JsonSerializable(typeof(FastEndpoints.ErrorResponse))]
[JsonSerializable(typeof(Result<LoginResponse>))]
[JsonSerializable(typeof(Result<List<TodoDto>>))]
public partial class AppJsonSerializerContext: JsonSerializerContext;