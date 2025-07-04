using DeviceManagement.Application.Common;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Domain.Repositories;

namespace DeviceManagement.Application.UseCases.Clientes;

public sealed class UpdateClienteUseCase(IUnitOfWork unitOfWork)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public async Task<Result<Cliente>> ExecuteAsync(Guid id, UpdateClienteRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id, cancellationToken);

            if (cliente is null)
                return Result<Cliente>.Failure("Cliente não encontrado");

            if (cliente.Email != request.Email && await _unitOfWork.Clientes.ExistsByEmailAsync(request.Email, cancellationToken))
                return Result<Cliente>.Failure("Email já está em uso por outro cliente");

            cliente.UpdateNome(request.Nome);
            cliente.UpdateEmail(request.Email);
            cliente.UpdateTelefone(request.Telefone);

            await _unitOfWork.Clientes.UpdateAsync(cliente, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Cliente>.Success(cliente);
        }
        catch (ArgumentException ex)
        {
            return Result<Cliente>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<Cliente>.Failure($"Erro interno: {ex.Message}");
        }
    }
}

public sealed record UpdateClienteRequest(
    string Nome,
    string Email,
    string? Telefone);
