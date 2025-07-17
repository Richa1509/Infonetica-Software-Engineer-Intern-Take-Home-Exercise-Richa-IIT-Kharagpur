namespace InfoneticaWorkflow.Models;

public class WorkflowInstance
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string WorkflowDefinitionId { get; set; } = "";
    public string CurrentStateId { get; set; } = "";

    public List<(string ActionId, DateTime Timestamp)> History { get; set; } = new();
}
