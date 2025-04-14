using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using WorkflowApi.Models;

namespace WorkflowApi.Services
{
    public class ElasticsearchWorkflowService : IWorkflowService
    {
        private readonly IElasticClient _elasticClient;
        private const string IndexName = "workflows";

        public ElasticsearchWorkflowService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
            CreateIndexIfNotExists();
        }

        private void CreateIndexIfNotExists()
        {
            if (!_elasticClient.Indices.Exists(IndexName).Exists)
            {
                var createIndexResponse = _elasticClient.Indices.Create(IndexName, c => c
                    .Settings(s => s
                        .NumberOfShards(1)
                        .NumberOfReplicas(0)
                    )
                    .Mappings(m => m
                        .Map<Workflow>(mm => mm
                            .Properties(p => p
                                .Text(t => t.Name(n => n.Name))
                                .Date(d => d.Name(n => n.CreatedAt))
                                .Date(d => d.Name(n => n.UpdatedAt))
                                .Object<Dictionary<string, object>>(o => o
                                    .Name(n => n.Properties)
                                    .Dynamic()
                                )
                            )
                        )
                    )
                );

                if (!createIndexResponse.IsValid)
                {
                    throw new Exception($"Failed to create index: {createIndexResponse.DebugInformation}");
                }
            }
        }

        public async Task<Workflow> CreateAsync(Workflow workflow)
        {
            workflow.Id = Guid.NewGuid().ToString();
            workflow.CreatedAt = DateTime.UtcNow;
            
            var response = await _elasticClient.IndexDocumentAsync(workflow);
            if (!response.IsValid)
            {
                throw new Exception($"Failed to create workflow: {response.DebugInformation}");
            }

            return workflow;
        }

        public async Task<Workflow> GetByIdAsync(string id)
        {
            var response = await _elasticClient.GetAsync<Workflow>(id);
            if (!response.IsValid)
            {
                throw new Exception($"Failed to get workflow: {response.DebugInformation}");
            }

            return response.Source;
        }

        public async Task<IEnumerable<Workflow>> GetAllAsync()
        {
            var response = await _elasticClient.SearchAsync<Workflow>(s => s
                .Query(q => q.MatchAll())
                .Size(1000)
            );

            if (!response.IsValid)
            {
                throw new Exception($"Failed to get workflows: {response.DebugInformation}");
            }

            return response.Documents;
        }

        public async Task<Workflow> UpdateAsync(string id, Workflow workflow)
        {
            workflow.Id = id;
            workflow.UpdatedAt = DateTime.UtcNow;

            var response = await _elasticClient.UpdateAsync<Workflow>(id, u => u
                .Doc(workflow)
                .DocAsUpsert()
            );

            if (!response.IsValid)
            {
                throw new Exception($"Failed to update workflow: {response.DebugInformation}");
            }

            return workflow;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var response = await _elasticClient.DeleteAsync<Workflow>(id);
            return response.IsValid;
        }
    }
} 