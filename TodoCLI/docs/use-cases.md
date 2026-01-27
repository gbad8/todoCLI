# 🎯 Use Cases - TODO CLI Application

## Actors
- **Developer**: Primary user who manages tasks
- **GitHub API**: External system for data persistence

## Use Cases Overview
1. **UC01** - Configure Authentication
2. **UC02** - Add Task
3. **UC03** - List Tasks
4. **UC04** - Mark Task as Completed
5. **UC05** - Remove Task
6. **UC06** - Mark All Tasks as Completed
7. **UC07** - Remove All Tasks
8. **UC08** - Sync with GitHub Gist

---

## UC01 - Configure Authentication

### **Actor**: Developer

### **Preconditions**: 
- Developer has a GitHub account
- Developer has internet connection
- CLI application is installed

### **Main Flow (Success Scenario)**:
1. Developer runs authentication command: `todo auth setup`
2. System prompts for GitHub personal access token
3. Developer enters valid GitHub token
4. System validates token with GitHub API
5. GitHub API confirms token is valid
6. System stores token securely (encrypted/keychain)
7. System confirms successful authentication
8. System creates/identifies default Gist for tasks storage

### **Alternative Flows**:

**A1 - Invalid Token**:
- 4a. GitHub API rejects token as invalid
- 4b. System displays error: "Invalid token. Please check your GitHub token."
- 4c. Return to step 2

**A2 - Network Error**:
- 4a. Network connection fails during validation
- 4b. System displays error: "Unable to connect to GitHub. Check internet connection."
- 4c. System offers retry option
- 4d. If retry: return to step 4, else: end use case

**A3 - No Internet Permission**:
- 4a. GitHub API returns permission error
- 4b. System displays: "Token lacks required permissions. Ensure 'gist' scope is enabled."
- 4c. Return to step 2

### **Post-conditions**:
- GitHub token is securely stored
- Default Gist is identified/created
- System is ready for task operations
- Authentication status is "authenticated"

### **Business Rules**:
- Token must have 'gist' scope permissions
- Only one authentication per system/user
- Token is stored using system's secure storage mechanism

---

## UC02 - Add Task

### **Actor**: Developer

### **Preconditions**: 
- Developer is authenticated with GitHub
- CLI application has valid GitHub token
- Internet connection available

### **Main Flow (Success Scenario)**:
1. Developer runs command: `todo add "task description"`
2. System validates authentication status
3. System validates command syntax and parameters
4. System generates unique hash for the task (e.g., "a1b2c3d4...")
5. System creates task object with description and hash
6. System adds task to local task list
7. System syncs with GitHub Gist
8. GitHub API confirms task was saved
9. System displays all tasks including the newly added one

### **Alternative Flows**:

**A1 - Authentication Expired**:
- 2a. System detects invalid/expired token
- 2b. System displays error: "Authentication expired. Run 'todo auth setup' to re-authenticate."
- 2c. End use case

**A2 - Empty Task Description**:
- 3a. System detects empty or whitespace-only description
- 3b. System displays error: "Task description cannot be empty"
- 3c. End use case

**A3 - Invalid Command Syntax**:
- 3a. System detects malformed command (missing quotes, etc.)
- 3b. System displays usage help: "Usage: todo add \"task description\""
- 3c. End use case

**A4 - Sync Failure**:
- 7a. Network error or GitHub API unavailable
- 7b. System displays warning: "Task added locally. Will sync when connection restored."
- 7c. System stores task in pending sync queue
- 7d. Continue to step 9

**A5 - Hash Collision** (extremely rare):
- 4a. Generated hash conflicts with existing task
- 4b. System regenerates new hash
- 4c. Continue to step 5

### **Post-conditions**:
- New task exists in the system with unique hash
- Task is synced to GitHub Gist (or queued for sync)
- Task appears in subsequent list operations
- System ready for next command

### **Business Rules**:
- Task description must be non-empty string
- Each task has unique 3+ character hash identifier
- Hash is case-insensitive for user commands
- Tasks are immediately synced when possible

---

## UC03 - List Tasks

### **Actor**: Developer

### **Preconditions**: 
- Developer is authenticated with GitHub
- CLI application has valid GitHub token

### **Main Flow (Success Scenario)**:
1. Developer runs command: `todo list`
2. System validates authentication status
3. System retrieves tasks from local storage
4. System syncs with GitHub Gist to get latest updates
5. GitHub API returns current task data
6. System merges any remote changes with local data
7. System formats and displays all tasks with their hash prefixes (first 3 characters)
8. System shows tasks in order (newest first or by creation time)

### **Alternative Flows**:

**A1 - No Tasks Available**:
- 7a. System detects empty task list
- 7b. System displays: "No tasks found. Use 'todo add \"task\"' to create your first task."
- 7c. End use case

**A2 - Sync Failure**:
- 4a. Network error or GitHub API unavailable
- 4b. System displays warning: "Using local data. Unable to sync with GitHub."
- 4c. System proceeds with local task data only
- 4d. Continue to step 7

**A3 - Authentication Expired**:
- 2a. System detects invalid/expired token
- 2b. System displays error: "Authentication expired. Run 'todo auth setup' to re-authenticate."
- 2c. End use case

### **Post-conditions**:
- All current tasks are displayed to user
- Local data is synchronized with remote Gist
- System ready for next command

### **Business Rules**:
- Tasks are displayed in format: `abc [ ] task description`
- Completed tasks are displayed as: `abc [x] task description`
- Hash prefix is first 3 characters of full hash
- Display order should be consistent and predictable

---

## UC04 - Mark Task as Completed

### **Actor**: Developer

### **Preconditions**: 
- Developer is authenticated with GitHub
- CLI application has valid GitHub token
- At least one task exists in the system

### **Main Flow (Success Scenario)**:
1. Developer runs command: `todo done abc` (where 'abc' is hash prefix)
2. System validates authentication status
3. System validates hash prefix format (3 characters minimum)
4. System searches for task matching the hash prefix
5. System finds unique matching task
6. System updates task status to completed
7. System syncs changes with GitHub Gist
8. GitHub API confirms update was saved
9. System displays all tasks with the completed task marked as [x]

### **Alternative Flows**:

**A1 - Authentication Expired**:
- 2a. System detects invalid/expired token
- 2b. System displays error: "Authentication expired. Run 'todo auth setup' to re-authenticate."
- 2c. End use case

**A2 - Invalid Hash Format**:
- 3a. System detects invalid hash format (too short, invalid characters)
- 3b. System displays error: "Invalid hash format. Use at least 3 characters."
- 3c. End use case

**A3 - Task Not Found**:
- 4a. System cannot find any task matching the hash prefix
- 4b. System displays error: "Task with hash 'abc' not found."
- 4c. End use case

**A4 - Ambiguous Hash Prefix**:
- 4a. System finds multiple tasks matching the hash prefix
- 4b. System displays error: "Hash 'abc' matches multiple tasks. Use more characters."
- 4c. System shows matching tasks for clarification
- 4d. End use case

**A5 - Task Already Completed**:
- 6a. System detects task is already marked as completed
- 6b. System displays message: "Task 'abc' is already completed."
- 6c. Continue to step 9

**A6 - Sync Failure**:
- 7a. Network error or GitHub API unavailable
- 7b. System displays warning: "Task marked locally. Will sync when connection restored."
- 7c. System stores change in pending sync queue
- 7d. Continue to step 9

### **Post-conditions**:
- Task status is updated to completed
- Change is synced to GitHub Gist (or queued for sync)
- Task displays with [x] marker in future list operations
- System ready for next command

### **Business Rules**:
- Hash prefix must be at least 3 characters
- Hash matching is case-insensitive
- Completed tasks remain in the system (not deleted)
- Multiple tasks cannot have conflicting hash prefixes for the minimum length

---

## UC05 - Remove Task

### **Actor**: Developer

### **Preconditions**: 
- Developer is authenticated with GitHub
- CLI application has valid GitHub token
- At least one task exists in the system

### **Main Flow (Success Scenario)**:
1. Developer runs command: `todo rm abc` (where 'abc' is hash prefix)
2. System validates authentication status
3. System validates hash prefix format (3 characters minimum)
4. System searches for task matching the hash prefix
5. System finds unique matching task
6. System removes task from task list
7. System syncs changes with GitHub Gist
8. GitHub API confirms update was saved
9. System displays remaining tasks

### **Alternative Flows**:

**A1 - Authentication Expired**:
- 2a. System detects invalid/expired token
- 2b. System displays error: "Authentication expired. Run 'todo auth setup' to re-authenticate."
- 2c. End use case

**A2 - Invalid Hash Format**:
- 3a. System detects invalid hash format (too short, invalid characters)
- 3b. System displays error: "Invalid hash format. Use at least 3 characters."
- 3c. End use case

**A3 - Task Not Found**:
- 4a. System cannot find any task matching the hash prefix
- 4b. System displays error: "Task with hash 'abc' not found."
- 4c. End use case

**A4 - Ambiguous Hash Prefix**:
- 4a. System finds multiple tasks matching the hash prefix
- 4b. System displays error: "Hash 'abc' matches multiple tasks. Use more characters."
- 4c. System shows matching tasks for clarification
- 4d. End use case

**A5 - Sync Failure**:
- 7a. Network error or GitHub API unavailable
- 7b. System displays warning: "Task removed locally. Will sync when connection restored."
- 7c. System stores change in pending sync queue
- 7d. Continue to step 9

### **Post-conditions**:
- Task is permanently removed from the system
- Change is synced to GitHub Gist (or queued for sync)
- Task no longer appears in list operations
- System ready for next command

### **Business Rules**:
- Hash prefix must be at least 3 characters
- Hash matching is case-insensitive
- Task removal is permanent and irreversible
- Removing a task frees up its hash for potential reuse

---

## UC06 - Mark All Tasks as Completed

### **Actor**: Developer

### **Preconditions**: 
- Developer is authenticated with GitHub
- CLI application has valid GitHub token

### **Main Flow (Success Scenario)**:
1. Developer runs command: `todo done-all`
2. System validates authentication status
3. System retrieves all current tasks
4. System updates all pending tasks to completed status
5. System syncs changes with GitHub Gist
6. GitHub API confirms updates were saved
7. System displays all tasks with [x] markers

### **Alternative Flows**:

**A1 - Authentication Expired**:
- 2a. System detects invalid/expired token
- 2b. System displays error: "Authentication expired. Run 'todo auth setup' to re-authenticate."
- 2c. End use case

**A2 - No Tasks Available**:
- 3a. System detects no tasks exist
- 3b. System displays message: "No tasks found to mark as completed."
- 3c. End use case

**A3 - All Tasks Already Completed**:
- 4a. System detects all tasks are already marked as completed
- 4b. System displays message: "All tasks are already completed."
- 4c. Continue to step 7

**A4 - Sync Failure**:
- 5a. Network error or GitHub API unavailable
- 5b. System displays warning: "Tasks marked locally. Will sync when connection restored."
- 5c. System stores changes in pending sync queue
- 5d. Continue to step 7

### **Post-conditions**:
- All tasks in the system are marked as completed
- Changes are synced to GitHub Gist (or queued for sync)
- All tasks display with [x] markers in future list operations
- System ready for next command

### **Business Rules**:
- Operation affects all tasks regardless of current status
- Already completed tasks remain completed (idempotent operation)
- Bulk operation is atomic (all tasks updated together)
- No confirmation prompt required for this operation

---

## UC07 - Remove All Tasks

### **Actor**: Developer

### **Preconditions**: 
- Developer is authenticated with GitHub
- CLI application has valid GitHub token

### **Main Flow (Success Scenario)**:
1. Developer runs command: `todo rm all`
2. System validates authentication status
3. System retrieves all current tasks
4. System removes all tasks from the task list
5. System syncs changes with GitHub Gist
6. GitHub API confirms updates were saved
7. System displays empty task list or confirmation message

### **Alternative Flows**:

**A1 - Authentication Expired**:
- 2a. System detects invalid/expired token
- 2b. System displays error: "Authentication expired. Run 'todo auth setup' to re-authenticate."
- 2c. End use case

**A2 - No Tasks Available**:
- 3a. System detects no tasks exist
- 3b. System displays message: "No tasks found to remove."
- 3c. End use case

**A3 - Sync Failure**:
- 5a. Network error or GitHub API unavailable
- 5b. System displays warning: "Tasks removed locally. Will sync when connection restored."
- 5c. System stores changes in pending sync queue
- 5d. Continue to step 7

### **Post-conditions**:
- All tasks are permanently removed from the system
- Changes are synced to GitHub Gist (or queued for sync)
- Task list is empty
- All task hashes are freed for potential reuse
- System ready for next command

### **Business Rules**:
- Operation removes all tasks regardless of completion status
- Bulk removal is atomic (all tasks removed together)
- Operation is permanent and irreversible
- No confirmation prompt required for this operation
- All hashes become available for reuse

---

## UC08 - Sync with GitHub Gist

### **Actor**: Developer

### **Preconditions**: 
- Developer is authenticated with GitHub
- CLI application has valid GitHub token
- Internet connection available

### **Main Flow (Success Scenario)**:
1. Developer runs command: `todo sync`
2. System validates authentication status
3. System retrieves local task data
4. System fetches remote task data from GitHub Gist
5. System compares local and remote data timestamps
6. System merges changes (remote takes precedence for conflicts)
7. System updates local data with merged results
8. System pushes any local-only changes to GitHub Gist
9. GitHub API confirms synchronization completed
10. System displays sync status and current task list

### **Alternative Flows**:

**A1 - Authentication Expired**:
- 2a. System detects invalid/expired token
- 2b. System displays error: "Authentication expired. Run 'todo auth setup' to re-authenticate."
- 2c. End use case

**A2 - Network Error**:
- 4a. Unable to connect to GitHub API
- 4b. System displays error: "Sync failed. Check internet connection."
- 4c. End use case

**A3 - No Remote Gist Found**:
- 4a. GitHub API indicates no existing Gist for tasks
- 4b. System creates new Gist with local data
- 4c. Continue to step 9

**A4 - Sync Conflict Resolution**:
- 6a. System detects conflicting changes to same task
- 6b. System applies "remote wins" strategy
- 6c. System logs conflict resolution
- 6d. Continue to step 7

**A5 - Local Changes Only**:
- 5a. Remote data is older than local data
- 5b. System skips merge step
- 5c. Continue to step 8

**A6 - Remote Changes Only**:
- 5a. Local data is older than remote data
- 5b. System skips push step
- 5c. Continue to step 10

### **Post-conditions**:
- Local and remote data are synchronized
- Any conflicts are resolved with remote precedence
- System displays current unified task state
- All pending sync operations are completed

### **Business Rules**:
- Remote data takes precedence in conflicts ("remote wins")
- Sync is bidirectional (pull remote, push local)
- Deleted tasks locally should be removed from remote
- New tasks locally should be added to remote
- Sync operation is atomic (all or nothing)

---
