using Microsoft.AspNetCore.Components.Forms;

using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace EngramaCore.PDFs
{
	/// <summary>
	/// Helper to join n PDFs
	/// </summary>
	public class JoinPDFs
	{
		private static IList<IBrowserFile> Files;
		private const long maxFileSize = 1024L * 1024L * 1024L * 2L;

		private MemoryStream FinalFile;

		public JoinPDFs()
		{
			Files = new List<IBrowserFile>();
			FinalFile = new MemoryStream();
		}

		public JoinPDFs(IList<IBrowserFile> files)
		{
			Files = files;
			FinalFile = new MemoryStream();
		}

		public void AddNewFile(IBrowserFile browserFile)
		{
			Files.Add(browserFile);
		}

		public IList<IBrowserFile> GetListFiles()
		{
			return Files;
		}

		public async Task<MemoryStream> GetFinalFile()
		{

			FinalFile = await ReadPdfsDocuments();
			return FinalFile;
		}


		private async Task<MemoryStream> ReadPdfsDocuments()
		{
			if (Files.Count > 1)
			{



				var Firstfile = Files.FirstOrDefault();
				Files.Remove(Firstfile);
				var stream1 = new MemoryStream();
				await Firstfile.OpenReadStream(maxFileSize).CopyToAsync(stream1);



				foreach (var Secundfile in Files)
				{
					// Read PDF content into memory streams
					using (var stream2 = new MemoryStream())
					{
						await Secundfile.OpenReadStream(maxFileSize).CopyToAsync(stream2);

						// Merge PDFs in memory
						stream1 = MergePdfStreams(stream1, stream2);

						// Handle the mergedStream as needed (e.g., save to another IBrowserFile or process further)

						Console.WriteLine("PDFs merged successfully!");
					}
				}
				return stream1;
			}
			else
			{
				var Firstfile = Files.FirstOrDefault();

				using (var memoryStream = new MemoryStream())
				{
					await Firstfile.OpenReadStream(maxFileSize).CopyToAsync(memoryStream);
					memoryStream.Position = 0;
					return memoryStream;
				}
			}

		}
		private MemoryStream MergePdfStreams(MemoryStream stream1, MemoryStream stream2)
		{
			var mergedStream = new MemoryStream();

			// Create PdfDocuments from the input streams
			var pdfDocument1 = PdfReader.Open(stream1, PdfDocumentOpenMode.Import);
			var pdfDocument2 = PdfReader.Open(stream2, PdfDocumentOpenMode.Import);

			// Create a new PDF document for the merged result
			var mergedDocument = new PdfDocument();

			// Add all pages from the first document
			foreach (var page in pdfDocument1.Pages)
			{
				mergedDocument.AddPage(page);
			}

			// Add all pages from the second document
			foreach (var page in pdfDocument2.Pages)
			{
				mergedDocument.AddPage(page);
			}

			// Save the merged document to the output stream
			mergedDocument.Save(mergedStream, false);

			return mergedStream;
		}

	}
}
