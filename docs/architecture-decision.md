# 🏛️ Architectural Decision - TODO CLI Application

## Decision: Simple Modular Design

### **Rationale:**
Based on domain analysis, our TODO CLI has:
- **Simple core domain** (tasks with basic operations)
- **Clear functional boundaries** (auth, tasks, sync, CLI)
- **Small team/project scope**
- **Need for rapid development**

Simple Modular Design provides the right balance of organization and simplicity for this context.

---

## 📂 **Module Structure**

### **Core Modules:**

#### **1. Auth Module** (`Auth/`)
**Responsibility:** Handle GitHub authentication and token management
```
Auth/
├── AuthManager.cs        # Main authentication logic
├── TokenStorage.cs       # Secure token storage/retrieval
└── IAuthService.cs       # Authentication interface
```

#### **2. Tasks Module** (`Tasks/`)
**Responsibility:** Core task business logic and data management
```
Tasks/
├── Task.cs               # Task entity
├── TaskManager.cs        # Task operations (CRUD)
├── HashGenerator.cs      # Hash generation and resolution
├── TaskFormatter.cs      # Display formatting logic
├── ITaskService.cs       # Task service interface
└── TaskStatus.cs         # Task status enumeration
```

#### **3. Sync Module** (`Sync/`)
**Responsibility:** GitHub Gist integration and synchronization
```
Sync/
├── GistClient.cs         # GitHub Gist API client
├── SyncManager.cs        # Sync coordination and conflict resolution
├── IGistClient.cs        # Gist client interface
└── ISyncService.cs       # Sync service interface
```

#### **4. CLI Module** (`CLI/`)
**Responsibility:** Command-line interface and user interaction
```
CLI/
├── CommandParser.cs      # Parse user commands
├── CommandHandlers.cs    # Handle different commands
├── OutputFormatter.cs    # Format output for display
├── ICommandParser.cs     # Command parser interface
└── Command.cs            # Command model
```

#### **5. Root Level**
```
Program.cs               # Application entry point
Configuration.cs         # Configuration management
TodoExceptions.cs        # Custom exceptions
TodoCLI.csproj          # NuGet package project file
```

---

## 🔗 **Module Dependencies (Following DIP)**

### **Dependency Flow with Dependency Inversion:**
```
CLI Module
    ↓ (depends on abstractions)
ITaskService ← TaskManager (implements)
IAuthService ← AuthManager (implements)
ISyncService ← SyncManager (implements)
```

### **Dependency Rules (Corrected for DIP):**
- **CLI** depends on **ITaskService**, **IAuthService** abstractions (not concrete classes)
- **TaskManager** implements **ITaskService** and depends on **IAuthService** abstraction
- **SyncManager** implements **ISyncService** and depends on **IAuthService** abstraction  
- **Concrete implementations** depend on **abstractions** (interfaces)
- **High-level modules** (CLI) depend on **abstractions**, not **concretions**

---

## 🎯 **Design Principles Applied**

### **Dependency Inversion Principle (DIP):**
- High-level modules (CLI) depend on abstractions (interfaces)
- Low-level modules (concrete implementations) also depend on abstractions
- Abstractions don't depend on details; details depend on abstractions

### **Single Responsibility:**
- Each module handles one functional area
- Classes within modules have focused responsibilities

### **Interface Segregation:**
- Modules expose only necessary public interfaces
- Internal implementation details are hidden

### **Open/Closed:**
- New command types can be added without modifying existing code
- New sync strategies can be implemented without changing interfaces

---

## 📋 **Module Interfaces**

### **Auth Module Public Interface:**
- `AuthenticateUserAsync()` - Setup GitHub authentication
- `GetTokenAsync()` - Retrieve current valid token
- `IsAuthenticated()` - Check authentication status

### **Tasks Module Public Interface:**
- `AddTaskAsync(description)` - Create new task
- `ListTasksAsync()` - Get all tasks
- `CompleteTaskAsync(hashPrefix)` - Mark task as done
- `RemoveTaskAsync(hashPrefix)` - Delete task
- `CompleteAllTasksAsync()` - Mark all tasks done
- `RemoveAllTasksAsync()` - Delete all tasks

### **Sync Module Public Interface:**
- `SyncWithRemoteAsync()` - Full bidirectional sync
- `PushChangesAsync()` - Send local changes to remote
- `PullChangesAsync()` - Get remote changes

### **CLI Module Public Interface:**
- `ParseCommand(args)` - Parse user input
- `ExecuteCommandAsync(command)` - Route to appropriate handler
- `DisplayOutput(data)` - Format and show results

---

## 📦 **NuGet Package Structure:**
- **Package Name:** `TodoCLI` or `DevTasks.CLI`
- **Target Framework:** .NET 6+ (for broad compatibility)
- **CLI Tool:** Global tool installation via `dotnet tool install -g`
- **Dependencies:** Minimal external dependencies for lightweight package

---

## 🛠️ **Technology Choices**

### **CLI Framework: Spectre.Console**
**Decision:** Use Spectre.Console for command-line interface implementation

**Rationale:**
- **Rich console output** with colors, tables, progress bars
- **Built-in command parsing** and validation
- **Type-safe command definitions** via attributes
- **Excellent developer experience** and documentation
- **Dependency injection support** for our modular architecture
- **Testability** with mocking capabilities

**Impact on Architecture:**
- CLI module will use Spectre's command pattern (`Command<T>`)
- Dependency injection configured through Spectre's container integration
- Output formatting leverages Spectre's rich display capabilities
- Command validation handled by framework annotations

**Dependencies Added:**
- `Spectre.Console` (main framework)
- `Spectre.Console.Cli` (command-line parsing)

---

## ✅ **Benefits for our Project:**

1. **DIP Compliance:** Proper dependency direction using abstractions
2. **Easy Testing:** Modules can be tested independently via interfaces
3. **Simple Understanding:** Straightforward structure for developers
4. **Rapid Development:** Minimal architectural overhead
5. **Easy Extension:** New implementations can be swapped via interfaces
6. **NuGet Ready:** Structure suitable for packaging and distribution

---
