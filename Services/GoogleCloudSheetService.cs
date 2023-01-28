using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Yerevan_Housing_API.Helpers;
using Yerevan_Housing_API.Models;

namespace Yerevan_Housing_API.Services
{
    public class GoogleCloudSheetService
    {
        static readonly string sheet = "house";
        private static readonly string SpreadSheetId;

        static GoogleCloudSheetService()
        {
            SpreadSheetId = Startup.SpreadSheetIdSettings;
        }
        public static async Task<Media> DownloadFiles(string[] fileUrls)
        {
            var allMedia = new Media();
            allMedia.Videos = new List<string>();
            allMedia.Photos = new List<MemoryStream>();
            var service = CredentialHelper.SetCredentialsForDrive();
            var i = 0;
            foreach (var fileUrl in fileUrls)
            {
                try
                {
                    var imageId = RegexHelper.GetImageId(fileUrl);
                    if (string.IsNullOrEmpty(imageId)) continue;

                    var request = service.Files.Get(imageId);
                    var stream = new MemoryStream();

                    var fileInfo = request.Execute();
                    var fileType = fileInfo.MimeType;
                    var driveFileName = fileInfo.Name;
                    if (fileType.Contains("video"))
                    {
                        var originType = string.Empty;
                        var mimeType = fileType.Replace("video/", "");
                        string ext = Path.GetExtension(driveFileName);
                        switch (ext)
                        {
                            case ".MP4":
                                originType = "mp4";
                                break;
                            case ".mp4":
                                originType = "mp4";
                                break;
                            default:
                                originType = ext.Replace(".", "");
                                break;
                        }
                        if (!string.IsNullOrEmpty(originType))
                        {
                            if (originType == "mp4")
                            {
                                var fileName = $"video_{i}.mp4";
                                using (FileStream fsStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
                                {
                                    request.Download(fsStream);
                                }
                                allMedia.Videos.Add(fileName);
                                i++;
                            }
                            else
                            {
                                var fileName = $"video_{i}.{originType}";
                                using (FileStream fsStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
                                {
                                    request.Download(fsStream);
                                }
                                await CloudConvertHelper.Convert(fileName, originType);
                                File.Delete(fileName);
                                var mp4FileName = $"video_{i}.mp4";
                                allMedia.Videos.Add(mp4FileName);
                                i++;
                            }
                        }
                    }
                    else
                    {
                        request.Download(stream);
                        allMedia.Photos.Add(stream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return allMedia;
        }

        public static async Task<House> ReadEntries()
        {
            var service = CredentialHelper.SetCredentialsForSpreadSheets();
            var range = $"{sheet}!A:K";
            try
            {
                SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(SpreadSheetId, range);

                var response = request.Execute();
                IList<IList<object>> values = response.Values;
                if (values != null && values.Count > 0)
                {
                    var count = values.Count;
                    var lastHouse = values[count - 1];
                    var media = lastHouse[8].ToString().Replace(" ", "").Split(",");

                    var house = new House
                    {
                        Id = count,
                        Address = lastHouse[1].ToString(),
                        Area = lastHouse[2].ToString(),
                        MinimalPeriod = lastHouse[3].ToString(),
                        RoomCount = lastHouse[4].ToString(),
                        Floor = lastHouse[5].ToString(),
                        Animals = lastHouse[6].ToString(),
                        Rent = Convert.ToDecimal(lastHouse[7].ToString()),
                        Media = await DownloadFiles(media),
                        Name = lastHouse[10].ToString(),
                        Percent = lastHouse[9].ToString(),
                    };
                    return house;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
    }
}
