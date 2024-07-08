namespace MeWhenAPI.Domain.DTO
{
    public class CompressedImageDTO
    {
        public string FileName { get; set; }
        public string Extension => FileName.Split('.').Length == 2 ? FileName.Split('.', 2)[1] : "";
        public MemoryStream Content { get; set; }
        public long Length { get; set; } = 0;

        public CompressedImageDTO(IFormFile file)
        {
            FileName = file.FileName;
            Length = file.Length;

            var stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            file.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            Content = stream;
        }
    }
}
