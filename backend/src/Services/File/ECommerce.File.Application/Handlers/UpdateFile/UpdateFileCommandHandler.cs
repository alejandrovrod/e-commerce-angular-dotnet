using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.File.Application.DTOs;
using ECommerce.File.Application.Commands.UpdateFile;
using ECommerce.File.Application.Interfaces;

namespace ECommerce.File.Application.Handlers.UpdateFile;

public class UpdateFileCommandHandler : IRequestHandler<UpdateFileCommand, ApiResponse<FileDto>>
{
    private readonly IFileRepository _fileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFileCommandHandler(IFileRepository fileRepository, IUnitOfWork unitOfWork)
    {
        _fileRepository = fileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<FileDto>> Handle(UpdateFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var file = await _fileRepository.GetByIdAsync(request.Id);
            if (file == null)
            {
                return ApiResponse<FileDto>.ErrorResult("File not found");
            }

            // Update file properties
            if (request.Description != null)
            {
                file.UpdateDescription(request.Description);
            }

            if (request.IsPublic.HasValue)
            {
                file.SetPublic(request.IsPublic.Value);
            }

            if (request.ExpiresAt.HasValue)
            {
                file.SetExpiration(request.ExpiresAt.Value);
            }

            if (request.Metadata != null)
            {
                foreach (var kvp in request.Metadata)
                {
                    file.UpdateMetadata(kvp.Key, kvp.Value);
                }
            }

            var updatedFile = await _fileRepository.UpdateAsync(file);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var fileDto = MapToDto(updatedFile);
            return ApiResponse<FileDto>.SuccessResult(fileDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<FileDto>.ErrorResult($"Error updating file: {ex.Message}");
        }
    }

    private FileDto MapToDto(ECommerce.File.Domain.Entities.File file)
    {
        return new FileDto
        {
            Id = file.Id,
            FileName = file.FileName,
            OriginalFileName = file.OriginalFileName,
            FilePath = file.FilePath,
            ContentType = file.ContentType,
            FileSize = file.FileSize,
            FileExtension = file.FileExtension,
            Type = file.Type,
            Status = file.Status,
            UserId = file.UserId,
            OrderId = file.OrderId,
            ProductId = file.ProductId,
            Description = file.Description,
            Metadata = file.Metadata,
            ThumbnailPath = file.ThumbnailPath,
            IsPublic = file.IsPublic,
            ExpiresAt = file.ExpiresAt,
            DownloadCount = file.DownloadCount,
            Checksum = file.Checksum,
            CreatedAt = file.CreatedAt,
            UpdatedAt = file.UpdatedAt,
            IsExpired = file.IsExpired,
            CanBeDownloaded = file.CanBeDownloaded
        };
    }
}
