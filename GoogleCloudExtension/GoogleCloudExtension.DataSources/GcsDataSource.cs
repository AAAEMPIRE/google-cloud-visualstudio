﻿// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1;
using Google.Apis.Storage.v1.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GoogleCloudExtension.DataSources
{
    /// <summary>
    /// Data source that returns information about Google Cloud Storage buckets for a particular project and credentials.
    /// </summary>
    public class GcsDataSource : DataSourceBase<StorageService>
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="credential"></param>
        public GcsDataSource(string projectId, GoogleCredential credential) : base(projectId, CreateService(credential))
        { }

        private static StorageService CreateService(GoogleCredential credential)
        {
            return new StorageService(new Google.Apis.Services.BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
            });
        }

        /// <summary>
        /// Fetches the list of buckets for the given project.
        /// </summary>
        /// <returns>The list of buckets.</returns>
        public Task<IList<Bucket>> GetBucketListAsync()
        {
            return LoadPagedListAsync(
                (token) =>
                {
                    if (String.IsNullOrEmpty(token))
                    {
                        Debug.WriteLine("Fetching first page.");
                        return Service.Buckets.List(ProjectId).ExecuteAsync();
                    }
                    else
                    {
                        Debug.WriteLine($"Fetchin page: {token}");
                        var request = Service.Buckets.List(ProjectId);
                        request.PageToken = token;
                        return request.ExecuteAsync();
                    }
                },
                x => x.Items,
                x => x.NextPageToken);
        }
    }
}