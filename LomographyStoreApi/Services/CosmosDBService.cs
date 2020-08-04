using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using LomographyStoreApi.Models;
using LomographyStoreApi.Services.Interfaces;

namespace LomographyStoreApi.Services
{
    public class CosmosDBService : IDocumentDBService
    {
        private readonly DocumentClient _docClient;
        private readonly string _dbName, _collectionName, _partionKey; 
        private readonly Uri _productCollectionUri;
        
        public CosmosDBService(IOptions<CosmosDBServiceOptions> options, DocumentClient client)
        {
            _dbName = options.Value.DBName;
            _collectionName = options.Value.DBCollection;
            _partionKey = options.Value.DBPartitionKey;

            _docClient = client;
            _productCollectionUri = UriFactory.CreateDocumentCollectionUri(_dbName, _collectionName);
            
        }
        public async Task AddImageToProductAsync(string id, string camera, string imageUri)
        {
            var docUri = UriFactory.CreateDocumentUri(_dbName, _collectionName, id);
            var doc = await _docClient.ReadDocumentAsync(docUri, new RequestOptions { PartitionKey = new PartitionKey(camera) });
            doc.Resource.SetPropertyValue(nameof(PhotoItem.Image), imageUri);
            await _docClient.ReplaceDocumentAsync(doc);
        }

        public async Task<PhotoItem> AddProductAsync(PhotoItem product)
        {
            var dbResponse = await _docClient.CreateDocumentAsync(_productCollectionUri, product);
            return (dynamic)dbResponse.Resource;
        }

        public async Task<PhotoItem> GetProductAsync(string id, string camera)
        {
            var uri = UriFactory.CreateDocumentUri(_dbName, _collectionName, id);
            var productDocument = await _docClient.ReadDocumentAsync<PhotoItem>(uri, new RequestOptions{ PartitionKey = new PartitionKey(camera) });
            return productDocument.Document;
        }

        public async Task<List<PhotoItem>> GetProductsAsync()
        {
            var productList = new List<PhotoItem>();
            var products = await _docClient.ReadDocumentFeedAsync(_productCollectionUri);
            foreach(var product in products)
            {
                productList.Add((PhotoItem)product);
            }
            return productList;           
        }
    }
}