using InfoneticaWorkflow.Models;

namespace InfoneticaWorkflow.Services;

public class WorkflowEngine
{
    // Simple in-memory stores
    private readonly Dictionary<string, WorkflowDefinition> _defs = new();
    private readonly Dictionary<string, WorkflowInstance> _inst = new();

    // -------------------- Definitions --------------------
    public string CreateDefinition(WorkflowDefinition def)
    {
        // Basic null defense
        def ??= new WorkflowDefinition();

        // ID required
        if (string.IsNullOrWhiteSpace(def.Id))
            throw new Exception("WorkflowDefinition.Id is required.");

        // Must not already exist
        if (_defs.ContainsKey(def.Id))
            throw new Exception($"Definition '{def.Id}' already exists.");

        // Validate structure
        ValidateDefinition(def);

        _defs[def.Id] = def;
        return def.Id;
    }

    public WorkflowDefinition GetDefinition(string id)
    {
        if (!_defs.TryGetValue(id, out var def))
            throw new Exception($"Definition '{id}' not found.");
        return def;
    }

    public IEnumerable<WorkflowDefinition> ListDefinitions() => _defs.Values;

    // -------------------- Instances --------------------
    public WorkflowInstance StartInstance(string defId)
    {
        var def = GetDefinition(defId); // throws if missing

        var initial = def.States.FirstOrDefault(s => s.IsInitial && s.Enabled);
        if (initial == null)
            throw new Exception("Initial state missing or disabled.");

        var inst = new WorkflowInstance
        {
            WorkflowDefinitionId = defId,
            CurrentStateId = initial.Id
        };

        _inst[inst.Id] = inst;
        return inst;
    }

    public WorkflowInstance GetInstance(string id)
    {
        if (!_inst.TryGetValue(id, out var inst))
            throw new Exception($"Instance '{id}' not found.");
        return inst;
    }

    public IEnumerable<WorkflowInstance> ListInstances() => _inst.Values;

    // -------------------- Runtime Transition --------------------
    public void ExecuteAction(string instanceId, string actionId)
    {
        var inst = GetInstance(instanceId); // throws if missing
        var def = GetDefinition(inst.WorkflowDefinitionId);

        var action = def.Actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null)
            throw new Exception($"Action '{actionId}' not found in workflow '{def.Id}'.");
        if (!action.Enabled)
            throw new Exception($"Action '{actionId}' is disabled.");

        // Current state object
        var curState = def.States.FirstOrDefault(s => s.Id == inst.CurrentStateId)
                       ?? throw new Exception($"Current state '{inst.CurrentStateId}' not found in def '{def.Id}'.");

        if (curState.IsFinal)
            throw new Exception("Cannot execute actions from a final state.");

        // Is current state allowed as a source?
        if (!action.FromStates.Contains(curState.Id))
            throw new Exception($"Action '{actionId}' cannot be run from state '{curState.Id}'.");

        // Target state exists & enabled?
        var toState = def.States.FirstOrDefault(s => s.Id == action.ToState);
        if (toState == null)
            throw new Exception($"Target state '{action.ToState}' not found.");
        if (!toState.Enabled)
            throw new Exception($"Target state '{toState.Id}' is disabled.");

        // All good: move
        inst.CurrentStateId = toState.Id;
        inst.History.Add((action.Id, DateTime.UtcNow));
    }

    // -------------------- Validation Helpers --------------------
    private static void ValidateDefinition(WorkflowDefinition def)
    {
        // Must have states
        if (def.States == null || def.States.Count == 0)
            throw new Exception("Workflow must contain at least one state.");

        // State IDs unique
        var stateIds = new HashSet<string>();
        foreach (var s in def.States)
        {
            if (string.IsNullOrWhiteSpace(s.Id))
                throw new Exception("State Id required.");
            if (!stateIds.Add(s.Id))
                throw new Exception($"Duplicate state Id '{s.Id}'.");
        }

        // Exactly one initial
        if (def.States.Count(s => s.IsInitial) != 1)
            throw new Exception("Workflow must have exactly one initial state.");

        // Actions list may be empty (valid but useless)
        if (def.Actions == null)
            def.Actions = new();

        // Action IDs unique + validate edges
        var actionIds = new HashSet<string>();
        foreach (var a in def.Actions)
        {
            if (string.IsNullOrWhiteSpace(a.Id))
                throw new Exception("Action Id required.");
            if (!actionIds.Add(a.Id))
                throw new Exception($"Duplicate action Id '{a.Id}'.");

            if (string.IsNullOrWhiteSpace(a.ToState))
                throw new Exception($"Action '{a.Id}' missing ToState.");

            // Target must exist
            if (!stateIds.Contains(a.ToState))
                throw new Exception($"Action '{a.Id}' references unknown ToState '{a.ToState}'.");

            // At least one fromState
            if (a.FromStates == null || a.FromStates.Count == 0)
                throw new Exception($"Action '{a.Id}' must have at least one FromState.");

            // Check each from
            foreach (var fs in a.FromStates)
            {
                if (!stateIds.Contains(fs))
                    throw new Exception($"Action '{a.Id}' references unknown FromState '{fs}'.");
            }
        }
    }
}
