using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

public static class CompressionUtility
{
    public static async Task<byte[]> Compress(string source)
    {
        var bytes = Encoding.UTF8.GetBytes(source);

        await using var input = new MemoryStream(bytes);
        await using var output = new MemoryStream();
        await using var brotliStream = new BrotliStream(output, CompressionLevel.Fastest);

        await input.CopyToAsync(brotliStream);
        await brotliStream.FlushAsync();

        return output.ToArray();
    }

    public static async Task<string> Decompress(byte[] compressed)
    {
        await using var input = new MemoryStream(compressed);
        await using var brotliStream = new BrotliStream(input, CompressionMode.Decompress);
        await using var output = new MemoryStream();

        await brotliStream.CopyToAsync(output);
        await brotliStream.FlushAsync();

        return Encoding.UTF8.GetString(output.ToArray());
    }
}