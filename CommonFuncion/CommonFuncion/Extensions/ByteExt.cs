using System.IO.Compression;
using System.Text;

namespace CommonFuncion.Extensions
{
	public static class ByteExt
	{
		public static string Encode(this byte[] self)
		{
			return Encoding.Default.GetString(self);
		}

		public static string ToBase64(this byte[] self)
		{
			return Convert.ToBase64String(self);
		}

		public static byte[] Compress(this byte[] self, string fileName)
		{
			using (var outStream = new MemoryStream(self))
			{
				using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
				{
					var fileInArchive = archive.CreateEntry(fileName, CompressionLevel.Optimal);

					using (var entryStream = fileInArchive.Open())
					using (var fileToCompressStream = new MemoryStream(self))
					{
						fileToCompressStream.CopyTo(entryStream);
					}
				}

				return outStream.ToArray();
			}
		}

		public static void CompressAndWrite(this byte[] self, string path, string fileName, string extension)
		{
			using (var memoryStream = new MemoryStream())
			{
				using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
				{
					var demoFile = archive.CreateEntry($"{fileName}.{extension}");

					using (var entryStream = demoFile.Open())
					using (var streamWriter = new StreamWriter(entryStream))
					{
						var stringValue = Encoding.UTF8.GetString(self);

						streamWriter.Write(stringValue);
					}
				}

				using (var fileStream = new FileStream($"{path}{fileName}.zip", FileMode.Create))
				{
					memoryStream.Seek(0, SeekOrigin.Begin);
					memoryStream.CopyTo(fileStream);
				}
			}
		}

		public static void WriteFile(this byte[] self, string path, string fileName, string extension)
		{
			using (var fileToCompressStream = new MemoryStream(self))
			{
				using (var fileStream = new FileStream($"{path}{fileName}.{extension}", FileMode.Create))
				{
					fileToCompressStream.Seek(0, SeekOrigin.Begin);
					fileToCompressStream.CopyTo(fileStream);
				}
			}
		}
	}
}
