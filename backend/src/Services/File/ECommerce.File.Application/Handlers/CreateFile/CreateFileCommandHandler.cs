using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.File.Application.DTOs;
using ECommerce.File.Application.Commands.CreateFile;
using ECommerce.File.Application.Interfaces;
using ECommerce.File.Domain.Entities;
using ECommerce.File.Domain.Enums;

namespace ECommerce.File.Application.Handlers.CreateFile;

public class CreateFileCommandHandler : IRequestHandler<CreateFileCommand, ApiResponse<FileDto>>
{
    private readonly IFileRepository _fileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFileCommandHandler(IFileRepository fileRepository, IUnitOfWork unitOfWork)
    {
        _fileRepository = fileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<FileDto>> Handle(CreateFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Generate unique file name
            var fileName = $"{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmss}";
            var filePath = $"/uploads/{request.Type.ToString().ToLower()}/{fileName}.{request.FileExtension}";

            // Create File entity
            var file = ECommerce.File.Domain.Entities.File.Create(
                fileName,
                request.OriginalFileName,
                filePath,
                request.ContentType,
                request.FileSize,
                request.FileExtension,
                request.Type,
                request.UserId,
                request.OrderId,
                request.ProductId,
                request.Description,
                request.IsPublic,
                request.ExpiresAt
            );

            // Save file
            var createdFile = await _fileRepository.AddAsync(file);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var fileDto = MapToDto(createdFile);

            return ApiResponse<FileDto>.SuccessResult(fileDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<FileDto>.ErrorResult($"Error creating file: {ex.Message}");
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
