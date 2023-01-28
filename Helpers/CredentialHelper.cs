using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.IO;

namespace Yerevan_Housing_API.Helpers
{
    internal class CredentialHelper
    {
        static readonly string[] SpreadsheetsScopes = { SheetsService.Scope.Spreadsheets };
        static readonly string[] DriveScopes = { SheetsService.Scope.Drive };
        static readonly string ApplicationName = "Yerevan";

        public static SheetsService SetCredentialsForSpreadSheets()
        {
            GoogleCredential credential;
            try
            {
                using (var stream = new FileStream("Configs/googleCreds.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                       .CreateScoped(SpreadsheetsScopes);
                }

                SheetsService service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
                return service;
            }
            catch (System.Exception)
            {
                return null;
            }

        }

        public static DriveService SetCredentialsForDrive()
        {
            GoogleCredential credential;
            try
            {
                using (var MemStream = new FileStream("Configs/googleCreds.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(MemStream)
                       .CreateScoped(DriveScopes);
                }

                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Yerevan"
                });
                return service;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
