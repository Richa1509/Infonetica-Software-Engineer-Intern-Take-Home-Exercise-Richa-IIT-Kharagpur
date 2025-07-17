# Infonetica Software Engineer Intern Take-Home Exercise  
## Submitted by: Richa | B.Tech (Hons.) Chemical Engineering | IIT Kharagpur

This repository contains my solution to the take-home exercise for the Software Engineer Intern role at Infonetica. I have implemented a lightweight and extensible workflow engine using .NET 8 and C#, following the provided requirements and guidelines.

---

## Task: Configurable Workflow Engine (State-Machine API)

The goal was to build a backend service that enables clients to:

- Define custom workflows using states and actions (transitions)
- Start workflow instances from these definitions
- Perform actions to move instances between states, with validation
- Inspect the state and history of any instance

This project uses ASP.NET Core Minimal APIs and stores data in-memory for simplicity, as instructed.

---

## Getting Started

### 1. Clone & Run

```bash
git clone https://github.com/Richa1509/Infonetica-Software-Engineer-Intern-Take-Home-Exercise-Richa-IIT-Kharagpur.git
cd InfoneticaWorkflow
dotnet run
```

The API will start at:  
`http://localhost:5047`

You can test the endpoints using Postman or curl.

---

## Sample Workflow JSON

To create a workflow, use the following request body:

### POST `/definition`

```json
{
  "id": "approvalFlow",
  "states": [
    { "id": "draft", "name": "Draft", "isInitial": true, "isFinal": false, "enabled": true },
    { "id": "review", "name": "Review", "isInitial": false, "isFinal": false, "enabled": true },
    { "id": "approved", "name": "Approved", "isInitial": false, "isFinal": true, "enabled": true }
  ],
  "actions": [
    { "id": "submit", "name": "Submit", "fromStates": ["draft"], "toState": "review", "enabled": true },
    { "id": "approve", "name": "Approve", "fromStates": ["review"], "toState": "approved", "enabled": true }
  ]
}
```

---

## Endpoints Summary

| Method | URL                                             | Description                       |
|--------|--------------------------------------------------|-----------------------------------|
| POST   | `/definition`                                   | Create a new workflow definition |
| GET    | `/definition/{id}`                              | View a workflow definition       |
| POST   | `/instance/{definitionId}`                      | Start a new workflow instance    |
| POST   | `/instance/{instanceId}/action/{actionId}`      | Perform a transition             |
| GET    | `/instance/{id}`                                | Get instance state and history   |

---

## Assumptions & Notes

- Each workflow definition must have exactly one initial state
- Action IDs and state IDs must be unique
- Disabled states or actions are ignored
- Final states cannot be transitioned from
- The backend uses in-memory persistence (no database used)

---

## Technologies Used

- C# with .NET 8 SDK
- ASP.NET Core Minimal API
- Postman / curl for API testing

---

## Possible Extensions

If given more time, I would consider adding:

- Swagger UI for better API exploration
- Unit tests for each component
- Persistent storage using local JSON or a lightweight database
- Frontend interface for easier workflow management

---

## Contact

Feel free to reach out to me if you'd like to discuss this project in more detail.

**Richa **  
IIT Kharagpur  
