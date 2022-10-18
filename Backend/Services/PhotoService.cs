using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Helpers;
using Backend.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Backend.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(
            cloud: config.Value.CloudName,
            apiKey: config.Value.ApiKey,
            apiSecret: config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);
    }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var upldResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var imgUpldParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
            };

            upldResult = await _cloudinary.UploadAsync(imgUpldParams);
        }

        return upldResult;

    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId) => await _cloudinary.DestroyAsync(new DeletionParams(publicId));
    
}