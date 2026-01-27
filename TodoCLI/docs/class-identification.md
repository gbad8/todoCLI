# 🏗️ Class Identification - TODO CLI Application

## 1. Substantive/Verb Analysis

### **From Requirements & Use Cases Analysis:**

#### **Substantivos Identificados (Classes Candidatas):**
- **Task** - Core entity with description and status
- **Hash** - Unique identifier for tasks  
- **Description** - Task content text
- **Status** - Task completion state
- **Developer** - User of the system
- **GitHub Token** - Authentication credential
- **Gist** - Remote storage location
- **Command** - User CLI input
- **Authentication** - Security process
- **Sync** - Synchronization operation
- **Session** - User authenticated state
- **Conflict** - Data inconsistency
- **Error** - Exception conditions

#### **Verbos Identificados (Métodos Candidatos):**
- **Add** task, **Remove** task, **List** tasks
- **Mark** as completed, **Complete** all tasks
- **Generate** hash, **Resolve** hash prefix
- **Authenticate** user, **Store** token, **Validate** token
- **Sync** with remote, **Push** changes, **Pull** changes  
- **Parse** command, **Execute** command, **Display** output
- **Format** output, **Handle** errors

---

## 2. Class Candidate Filtering

### **Core Business Classes (Keep):**
- ✅ **Task** - Primary business entity
- ✅ **TaskStatus** - Business value object (enum)
- ✅ **Hash** - Business identifier (or value object)

### **Service Classes (Keep):**
- ✅ **TaskManager** - Core business logic
- ✅ **AuthManager** - Authentication logic  
- ✅ **SyncManager** - Synchronization logic
- ✅ **GistClient** - External service integration

### **Interface Classes (Keep):**
- ✅ **CommandParser** - User interaction
- ✅ **CommandHandlers** - Command routing
- ✅ **OutputFormatter** - Display logic

### **Utility Classes (Keep):**
- ✅ **HashGenerator** - Hash creation and resolution
- ✅ **TokenStorage** - Security utility
- ✅ **Configuration** - System configuration

### **Eliminated Candidates:**
- ❌ **Description** - Simple string, not worth a class
- ❌ **Developer** - Not modeled in our simple system
- ❌ **Session** - Managed by framework (Spectre.Console)
- ❌ **Conflict** - Handled as method logic, not entity
- ❌ **Error** - Standard exceptions sufficient

---

## 3. Final Class List by Module

### **Tasks Module:**
- **Task** (entity)
- **TaskManager** (service)
- **HashGenerator** (utility)
- **TaskFormatter** (utility)

### **Auth Module:**
- **AuthManager** (service)
- **TokenStorage** (utility)

### **Sync Module:**
- **SyncManager** (service)
- **GistClient** (service)

### **CLI Module:**
- **CommandParser** (service)
- **CommandHandlers** (service)  
- **OutputFormatter** (utility)
- **Command** (model)

### **Root Level:**
- **Configuration** (utility)
- **Program** (entry point)
- **TodoExceptions** (custom exceptions)

---

## 4. Interface Extraction (for DIP)

### **Service Interfaces:**
- **ITaskService** - Task operations interface
- **IAuthService** - Authentication operations interface
- **ISyncService** - Sync operations interface
- **IGistClient** - Gist API client interface
- **ICommandParser** - Command parsing interface

### **Utility Interfaces:**
- **IHashGenerator** - Hash generation interface
- **ITokenStorage** - Token storage interface
- **IOutputFormatter** - Output formatting interface

---
