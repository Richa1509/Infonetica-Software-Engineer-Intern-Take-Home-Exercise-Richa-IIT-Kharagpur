using InfoneticaWorkflow.Models;
using InfoneticaWorkflow.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<WorkflowEngine>();
var app = builder.Build();

// -- Create a new workflow definition --
app.MapPost("/definition", (WorkflowDefinition def, WorkflowEngine engine) =>
{
    var id = engine.CreateDefinition(def);
    return Results.Ok(new { definitionId = id });
});

// -- Get a workflow definition by ID --
app.MapGet("/definition/{id}", (string id, WorkflowEngine engine) =>
{
    return Results.Ok(engine.GetDefinition(id));
});

// -- Start a new instance of a workflow --
app.MapPost("/instance/{definitionId}", (string definitionId, WorkflowEngine engine) =>
{
    var instance = engine.StartInstance(definitionId);
    return Results.Ok(instance);
});

// -- Execute an action on an instance --
app.MapPost("/instance/{instanceId}/action/{actionId}", (string instanceId, string actionId, WorkflowEngine engine) =>
{
    engine.ExecuteAction(instanceId, actionId);
    return Results.Ok("Action executed.");
});

// -- Get the current state and history of an instance --
app.MapGet("/instance/{id}", (string id, WorkflowEngine engine) =>
{
    return Results.Ok(engine.GetInstance(id));
});
app.MapGet("/", () => "✅ Infonetica Workflow API is running!");

app.Run();
