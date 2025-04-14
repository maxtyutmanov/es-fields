using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowApi.Models;

namespace WorkflowApi.Services
{
    public interface IWorkflowService
    {
        Task<Workflow> CreateAsync(Workflow workflow);
        Task<Workflow> GetByIdAsync(string id);
        Task<IEnumerable<Workflow>> GetAllAsync();
        Task<Workflow> UpdateAsync(string id, Workflow workflow);
        Task<bool> DeleteAsync(string id);
    }
} 