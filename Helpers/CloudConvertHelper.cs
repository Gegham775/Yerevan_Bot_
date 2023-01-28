using CloudConvert.API;
using CloudConvert.API.Models.ExportOperations;
using CloudConvert.API.Models.ImportOperations;
using CloudConvert.API.Models.JobModels;
using CloudConvert.API.Models.TaskOperations;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Yerevan_Housing_API.Helpers
{
    public class CloudConvertHelper
    {
        public static async Task Convert(string originFileName, string mimeType)
        {
            var _cloudConvert = new CloudConvertAPI("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIxIiwianRpIjoiNzViZTc5NTJhNDYzZTBmZWFiODY3ZDhhMzgzOGQzNjk4Yjk4ODdlMWFkZmY5NzlkNjk3MDA1M2RiNThhZmZiZGIyN2MwNTVkZDI5NTJhMjgiLCJpYXQiOjE2NjQ2NjY1NDkuNjM0ODM1LCJuYmYiOjE2NjQ2NjY1NDkuNjM0ODM2LCJleHAiOjQ4MjAzNDAxNDkuNjMxMDE0LCJzdWIiOiI2MDA5MTYyNSIsInNjb3BlcyI6WyJ1c2VyLnJlYWQiLCJ1c2VyLndyaXRlIiwidGFzay5yZWFkIiwidGFzay53cml0ZSIsIndlYmhvb2sud3JpdGUiLCJ3ZWJob29rLnJlYWQiLCJwcmVzZXQucmVhZCIsInByZXNldC53cml0ZSJdfQ.UZVnGNDBlzlQwSXObtQU7K5suEnGS_k8x5O1ZJO4n54pLf5LvWW6zcXcMnj2I4t_bNnHUfXlw9FnYRjYdJEY7q7XFvc0dL0qje053R9Ll7Y-R_KlYYGoXeCgLR-oM62cZXuZFtu_VdR_PXWK7BKiHvHLu4VcpdytSV3HcAwuqB9YyMa3HNyr7himrUIf3haDvj0L_XtNxFj4sIZ56KEoITrPO0B7yrf7-yRNCgFElwAFwegM2KzeOCle1GE_Pd_z7nonHuc_yj1SnUgcXowjwvDePVLnnOZYILhil7rpnrU9pbF877G_W4V0Wrv9KgPN6UsDjlVYPxPDVYHorbuy029Ih14yfHYqmdvEhj5jRbqhw32i-HyAo83nGmP8fdIFJLdZZJrwHTEZafM6-P89cQYkidFoYO1aGVmzribLLIOhIncwAklMDJiJ_a5Tg8o6GCRHghhkjgXwo1NeSNvW6V-oMalu7Uuy_-4M6iwQrJ2zJU5aEX-DTfqzN8Alt9Zm5-RTE8YK1WH67KTmYM7MuO2_6Z3ZMFdgNklmWdQRxAMloeveTvju9VEchBBmzNdeWBI5SbsQTvzoLBYiYsVs0mkThJw8hLDMwgET_p-56qC1peSvTylZeGuQSJtY3N_EfmIizisYn2oMWwavvq5zu0IHmQiN9qU6j-y-m_lKyDM");
            try
            {
                var job = await _cloudConvert.CreateJobAsync(new JobCreateRequest
                {
                    Tasks = new
                    {
                        import_example_1 = new ImportUploadCreateRequest(),
                        convert = new ConvertCreateRequest
                        {
                            Input = "import_example_1",
                            Input_Format = mimeType,
                            Output_Format = "mp4"
                        },
                        export = new ExportUrlCreateRequest
                        {
                            Input = "convert",
                            Archive_Multiple_Files = true
                        }
                    },
                    Tag = "Convert"
                });

                var uploadTask = job.Data.Tasks.FirstOrDefault(t => t.Name == "import_example_1");

                var path = originFileName;
                byte[] file = await File.ReadAllBytesAsync(path);
                string fileName = originFileName.Replace("Videos/", "");

                await _cloudConvert.UploadAsync(uploadTask.Result.Form.Url.ToString(), file, fileName, uploadTask.Result.Form.Parameters);

                var jobdwwd = await _cloudConvert.WaitJobAsync(job.Data.Id);


                var exportTask = jobdwwd.Data.Tasks.FirstOrDefault(t => t.Name == "export");

                var fileExport = exportTask.Result.Files.FirstOrDefault();

                using (var client = new WebClient()) client.DownloadFile(fileExport.Url, fileExport.Filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }
        }

    }
}
