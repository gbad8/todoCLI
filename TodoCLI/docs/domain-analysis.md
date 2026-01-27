# 📊 Domain Analysis - TODO CLI Application

## 1. Domain Concepts Identification

### **From Requirements & Use Cases:**

#### **Core Business Concepts:**
- **Task** - Primary entity representing a developer's work item
- **Hash** - Unique identifier for each task (Docker-like prefix)
- **Description** - Text content of the task
- **Status** - Task state (pending/completed)
- **Developer** - Primary user of the system

#### **Technical Concepts:**
- **GitHub Token** - Authentication credential
- **Gist** - Remote storage mechanism
- **Sync** - Data synchronization process
- **CLI Command** - User interaction interface

#### **System Concepts:**
- **Authentication** - Security mechanism
- **Session** - User's authenticated state
- **Local Storage** - Client-side data persistence
- **Remote Storage** - GitHub Gist storage

---

## 2. Domain Glossary

### **Task**
A work item that a developer needs to complete before their next commit. Contains a description and completion status.

### **Hash**
A unique identifier generated for each task, similar to Docker container IDs. Users reference tasks by the first 3+ characters.

### **Description** 
The textual content describing what the developer needs to do. Must be non-empty.

### **Status**
The current state of a task - either "pending" (not completed) or "completed" (finished).

### **Developer**
The primary user who creates, manages, and completes tasks using the CLI application.

### **GitHub Token**
A personal access token with 'gist' scope permissions, used for authenticated access to GitHub's Gist API.

### **Gist**
A GitHub service for storing and sharing code snippets/text. Used as remote persistence for tasks.

### **Sync**
The process of synchronizing local task data with the remote Gist, resolving conflicts using "remote wins" strategy.

---

## 3. Business Rules Identified

### **Task Rules:**
- BR01: Task description cannot be empty or whitespace-only
- BR02: Each task must have a unique hash identifier
- BR03: Hash prefixes must be at least 3 characters for user commands
- BR04: Completed tasks remain in system (not auto-deleted)
- BR05: Task display format: `abc [x] description` or `abc [ ] description`

### **Hash Rules:**
- BR06: Hash collision detection must prevent duplicates
- BR07: Hash matching is case-insensitive
- BR08: Hash prefix must uniquely identify a task
- BR09: Removed tasks free their hash for reuse

### **Authentication Rules:**
- BR10: All operations (except auth setup) require valid GitHub token
- BR11: Token must have 'gist' scope permissions
- BR12: Authentication is persistent until token expires
- BR13: Expired tokens require re-authentication

### **Synchronization Rules:**
- BR14: Remote data takes precedence in conflicts ("remote wins")
- BR15: Sync operations are atomic (all or nothing)
- BR16: Local changes sync automatically when possible
- BR17: Manual sync available via explicit command

### **Data Persistence Rules:**
- BR18: All tasks stored in a single GitHub Gist
- BR19: No local backup required (GitHub is authoritative)
- BR20: Offline operations queue for later sync

---

## 4. Relationship Mapping

### **Core Relationships:**
- Developer **creates** Tasks
- Task **has** Hash (1:1)
- Task **has** Description (1:1) 
- Task **has** Status (1:1)
- Developer **authenticates** with GitHub Token
- Tasks **are stored in** GitHub Gist
- System **syncs** Local Storage with Remote Storage

### **Interaction Flows:**
- Developer → CLI Command → Task Operations → Sync → GitHub Gist
- GitHub Token → Authentication → Authorized Operations
- Hash Prefix → Hash Resolution → Task Identification

### **Dependencies:**
- Task Operations depend on Authentication
- Sync Operations depend on Network Connectivity
- Hash Resolution depends on Task Collection
- Display Operations depend on Task Status

---

## 5. Domain Invariants

- At any time, all task hashes in the system must be unique
- A task cannot exist without a description and hash
- Authentication state determines available operations
- Local and remote data will eventually be consistent
- Hash prefixes must be unambiguous for the minimum required length
