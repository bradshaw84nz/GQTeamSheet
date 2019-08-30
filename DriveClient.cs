using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;

namespace GQTeamsheet
{
    public class DriveClient
    {
        public readonly DriveService _serviceClient;

        public DriveClient()
        {
            _serviceClient = GetServiceObject();
        }

        public void CreateFolder()
        {
            try
            {
                // TODO LIST AND FIND GQ Teamsheet
                // TODO CREATE WEEKLY FOLDER
                // UPLOAD TEAMSHEET

                var rootFolderResponse = CreateFolder("GQ-TeamSheets-2019", "");

                var folderResponse = CreateFolder("Round 3", rootFolderResponse.Id);

                var fileName = "blah";
                var file = CreateFile(folderResponse.Id, fileName);

                var getRequest = _serviceClient.Files.Get(folderResponse.Id);
                getRequest.Fields = "id,webViewLink";

                var getResponse = getRequest.Execute();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private File CreateFile(string folderId, string fileName)
        {
            var fileRequest = _serviceClient.Files.Create(new File
            {
                MimeType = "application/text",
                Name = fileName,
                Parents = new List<string>() { folderId },

            });

            fileRequest.Fields = "id,webViewLink";

            var result = fileRequest.Execute();

            CreatePermissionsForFiles(result.Id);

            return result;
        }

        private File CreateFolder(string fileName, string parentFolderId)
        {
            // Check file already exist so it's not created multiple times
            var file = FindFile(fileName);

            if (file != null)
            {
                return file;
            }

            FilesResource.CreateRequest request;
            if (string.IsNullOrEmpty(parentFolderId))
            {
                request = _serviceClient.Files.Create(new File
                {
                    MimeType = "application/vnd.google-apps.folder",
                    Name = fileName,
                });
            }
            else
            {
                request = _serviceClient.Files.Create(new File
                {
                    MimeType = "application/vnd.google-apps.folder",
                    Name = fileName,
                    Parents = new List<string>() { parentFolderId },
                });
            }


            var folderResponse = request.Execute();

            CreatePermissionsForFiles(folderResponse.Id); ;
            return folderResponse;
        }

        private File FindFile(string fileName)
        {
            var findRequest = _serviceClient.Files.List();

            findRequest.Q = $"name='{fileName}'";
            var response = findRequest.Execute();

            if (response.Files == null || response.Files.Count == 0)
            {
                Console.WriteLine($"No file found with the name {fileName}");
                return null;
            }

            if (response.Files.Count() > 1)
            {
                throw new Exception("More than one folder with the same name was found");
            }

            return response.Files.FirstOrDefault();

        }

        private DriveService GetServiceObject()
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

            return services;
        }

        public void CreatePermissionsForFiles(string fieldId)
        {
            CreatePermission(fieldId, "writer", "group", "committee@gridironqld.asn.au");
            CreatePermission(fieldId, "reader", "anyone");
        }
        public void CreatePermission(string fileId, string role, string type, string emailAddress = null)
        {
            var permission = new Permission()
            {
                Role = role,
                Type = type,
                EmailAddress = emailAddress,
            };

            var permissionRequest = _serviceClient.Permissions.Create(permission, fileId);
            //permissionRequest.TransferOwnership = true;

            permissionRequest.SendNotificationEmail = false;

            permissionRequest.Execute();
        }
    }
}