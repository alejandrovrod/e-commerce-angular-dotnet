using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.File.Application.Commands.DeleteFile;
using ECommerce.File.Application.Interfaces;

namespace ECommerce.File.Application.Handlers.DeleteFile;

public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, ApiResponse<bool>>
{
    private readonly IFileRepository _fileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteFileCommandHandler(IFileRepository fileRepository, IUnitOfWork unitOfWork)
    {
        _fileRepository = fileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var file = await _fileRepository.GetByIdAsync(request.Id);
            if (file == null)
            {
                return ApiResponse<bool>.ErrorResult("File not found");
            }

            // Delete file
            await _fileRepository.DeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResult($"Error deleting file: {ex.Message}");
        }
    }
}
