# 📋 TODO CLI Application Requirements

## Overview
Simple CLI application for developers to list small tasks that should be done for the next commit. System focused on simplicity and practicality.

## 📝 Functional Requirements

### FR01 - Add Task
- Allow adding new task with simple text
- **Command:** `todo add "task description"`

### FR02 - Generate Unique Hash
- Each task must have a unique hash generated automatically
- User can reference task by the first 3 digits of the hash
- Similar to Docker containers system

### FR03 - List Tasks
- Display all tasks with their hashes (first 3 digits)
- **Command:** `todo list`

### FR04 - Remove Task
- Allow task removal by hash (3 digits)
- **Command:** `todo rm abc`

### FR05 - Mark Task as Completed
- Mark task as completed by hash (3 digits)
- **Command:** `todo done abc`

### FR06 - Store in GitHub Gists
- All tasks must be stored in GitHub Gists
- Remote data persistence

### FR07 - Mark All Tasks as Completed
- Mark all tasks as completed at once
- **Command:** `todo done-all` or `todo clear`

### FR08 - Remove All Tasks
- Remove all tasks at once
- **Command:** `todo rm-all` or `todo clear`

### FR09 - Synchronization
- Synchronize data with remote Gist
- Single global list (not separated by project/repository)

## 🔧 Non-Functional Requirements

### Performance
- ⚡ Fast initialization (< 2s even with many tasks)
- 📡 Sync with GitHub Gists in background when possible

### Usability
- 🎯 Intuitive and consistent CLI interface
- 🔤 Auto-complete for commands
- 📝 Clear and helpful error messages

### Reliability
- 🔄 Automatic retry on network failure
- ✅ Data integrity validation

### Portability
- 🖥️ Work on Windows, Linux and macOS

### Security
- 🔑 Secure authentication with GitHub (token)
- 🔒 Do not store credentials in plain text
