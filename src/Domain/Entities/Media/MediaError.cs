using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Media
{
    public class MediaError
    {
        public static Error NameEmpty() => Error.Validation(
            "Media.NameEmpty",
            $"File name can not be empty");

        public static Error InvalidFileFize() => Error.Validation(
            "Media.InvalidFileSize",
            "File size must be greater than 0"
        );

        public static Error FilePathEmpty() => Error.Validation(
            "Media.FilePathEmpty",
            "File path can not be empty"
        );

        public static Error BucketDoesNotExist() => Error.NotFound(
            "Media.BucketDoesNotExist",
            "Bucket does not exist"
        );
    }
}