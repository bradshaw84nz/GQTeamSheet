using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;

namespace GQTeamsheet
{
    public class DriveClient
    {
        public void CreateFolder()
        {
            try
            {
                string[] scopes = new string[] { DriveService.Scope.Drive }; // Full access

                var serviceAccountEmail = "testserviceaccount-958@teamsheets.iam.gserviceaccount.com";

                var keyFile = @"/Users/cbradshaw/Downloads/teamsheets-a0a4888837ef.p12";

                var bytess = System.IO.File.ReadAllBytes(keyFile);
                //loading the Key file
                var certificate = new X509Certificate2(bytess, "notasecret", X509KeyStorageFlags.Exportable);
                var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
                {
                    Scopes = scopes
                }.FromCertificate(certificate));

                var services = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Drive API Sample",
                });

                // TODO LIST AND FIND GQ Teamsheet
                // TODO CREATE WEEKLY FOLDER
                // UPLOAD TEAMSHEET

                var request = services.Files.Create(new File
                {
                    MimeType = "application/vnd.google-apps.folder",
                    Name = "TESTGQ"
                });

                var folderResponse = request.Execute();

                CreatePermissionsForFiles(folderResponse.Id, services);;

                //FilesResource.GetRequest request = services.Files.Get("TestFolder123");
                var fileRequest = services.Files.Create(new File
                {
                    MimeType = "application/text",
                    Name = "TestGQFile",
                    Parents = new List<string>() { folderResponse.Id },

                });

                var result = fileRequest.Execute();

                CreatePermissionsForFiles(result.Id, services);

                var getRequest = services.Files.Get(folderResponse.Id);
                getRequest.Fields = "id,webViewLink";

                var getResponse = getRequest.Execute();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void CreatePermissionsForFiles(string fieldId, DriveService services)
        {
            CreatePermission(fieldId, "writer", "group", services, "committee@gridironqld.asn.au");
            CreatePermission(fieldId, "reader", "anyone", services);
        }
        public void CreatePermission(string fileId, string role, string type, DriveService services, string emailAddress = null)
        {
                var permission = new Permission()
                {
                    Role = role,
                    Type = type,
                    EmailAddress = emailAddress
                };

                var permissionRequest = services.Permissions.Create(permission, fileId);
                permissionRequest.SendNotificationEmail = false;        
        }
    }
}