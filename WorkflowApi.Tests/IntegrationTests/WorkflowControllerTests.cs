using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using WorkflowApi.Models;
using Xunit;

namespace WorkflowApi.Tests.IntegrationTests;

public class WorkflowControllerTests
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public WorkflowControllerTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Add any test-specific service configuration here
                });
            });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateWorkflow_ReturnsCreatedResponse()
    {
        // Arrange
        var workflow = new Workflow
        {
            Name = "Test Workflow",
            Properties = new Dictionary<string, object>
            {
                { "status", "active" },
                { "priority", "high" }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/workflow", workflow);
        var createdWorkflow = await response.Content.ReadFromJsonAsync<Workflow>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(createdWorkflow);
        Assert.Equal(workflow.Name, createdWorkflow.Name);
        Assert.Equal(workflow.Properties["status"], createdWorkflow.Properties["status"]);
    }

    [Fact]
    public async Task GetWorkflow_ReturnsWorkflow()
    {
        // Arrange
        var workflow = new Workflow
        {
            Name = "Test Workflow",
            Properties = new Dictionary<string, object>
            {
                { "status", "active" },
                { "priority", "high" }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/workflow", workflow);
        var createdWorkflow = await createResponse.Content.ReadFromJsonAsync<Workflow>();

        // Act
        var response = await _client.GetAsync($"/api/workflow/{createdWorkflow.Id}");
        var retrievedWorkflow = await response.Content.ReadFromJsonAsync<Workflow>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(retrievedWorkflow);
        Assert.Equal(createdWorkflow.Id, retrievedWorkflow.Id);
        Assert.Equal(workflow.Name, retrievedWorkflow.Name);
    }

    [Fact]
    public async Task UpdateWorkflow_ReturnsUpdatedWorkflow()
    {
        // Arrange
        var workflow = new Workflow
        {
            Name = "Test Workflow",
            Properties = new Dictionary<string, object>
            {
                { "status", "active" },
                { "priority", "high" }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/workflow", workflow);
        var createdWorkflow = await createResponse.Content.ReadFromJsonAsync<Workflow>();

        // Update workflow
        createdWorkflow.Name = "Updated Workflow";
        createdWorkflow.Properties["status"] = "completed";

        // Act
        var response = await _client.PutAsJsonAsync($"/api/workflow/{createdWorkflow.Id}", createdWorkflow);
        var updatedWorkflow = await response.Content.ReadFromJsonAsync<Workflow>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updatedWorkflow);
        Assert.Equal("Updated Workflow", updatedWorkflow.Name);
        Assert.Equal("completed", updatedWorkflow.Properties["status"]);
    }

    [Fact]
    public async Task DeleteWorkflow_ReturnsNoContent()
    {
        // Arrange
        var workflow = new Workflow
        {
            Name = "Test Workflow",
            Properties = new Dictionary<string, object>
            {
                { "status", "active" },
                { "priority", "high" }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/workflow", workflow);
        var createdWorkflow = await createResponse.Content.ReadFromJsonAsync<Workflow>();

        // Act
        var response = await _client.DeleteAsync($"/api/workflow/{createdWorkflow.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify workflow is deleted
        var getResponse = await _client.GetAsync($"/api/workflow/{createdWorkflow.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
} 