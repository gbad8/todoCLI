# TodoCLI - Task Management Tool

[![NuGet](https://img.shields.io/nuget/v/TodoCLI.svg)](https://www.nuget.org/packages/TodoCLI/)
[![Downloads](https://img.shields.io/nuget/dt/TodoCLI.svg)](https://www.nuget.org/packages/TodoCLI/)

A simple, powerful command-line task management tool that syncs with **GitHub Gist** for seamless cross-device access.

## ✨ Features

- 🚀 **Lightning fast** CLI task management
- 🔄 **Automatic sync** with GitHub Gist (cloud storage)
- 🔐 **Secure authentication** with GitHub personal access tokens
- 🏷️ **Hash-based task IDs** (like Docker containers)
- 💾 **Local persistence** with automatic cloud backup
- 🖥️ **Cross-platform** (Windows, macOS, Linux)

## 📦 Installation

Install as a global .NET tool:

```bash
dotnet tool install --global TodoCLI
```

Update to latest version:

```bash
dotnet tool update --global TodoCLI
```

## 🚀 Quick Start

### 1. Setup GitHub Authentication
```bash
todo auth setup
# Enter your GitHub personal access token with 'gist' scope
```

### 2. Manage Tasks
```bash
# Add a new task
todo add "Review pull request #123"

# List all tasks
todo list

# Mark task as completed (use first 3 characters of hash)
todo done abc

# Remove a task
todo rm def

# Sync manually (automatic after each operation)
todo sync
```

## 📋 Commands

| Command | Description |
|---------|-------------|
| `todo add "description"` | Add a new task |
| `todo list` | List all tasks |
| `todo done <hash>` | Mark task as completed |
| `todo rm <hash>` | Remove a task |
| `todo done-all` | Mark all tasks as completed |
| `todo rm-all` | Remove all tasks |
| `todo sync` | Manual sync with GitHub |
| `todo help` | Show help information |

## 🔧 Requirements

- **.NET 8.0** or higher
- **GitHub account** with personal access token
- **Internet connection** for sync operations

## 🔑 GitHub Token Setup

1. Go to [GitHub Settings > Developer settings > Personal access tokens](https://github.com/settings/tokens)
2. Click "Generate new token"
3. Select **"gist"** scope
4. Copy the generated token
5. Run `todo auth setup` and paste the token

## 📖 Example Usage

```bash
$ todo add "Buy groceries"
Task added successfully: [a1b] Buy groceries

$ todo add "Finish project documentation"
Task added successfully: [c2d] Finish project documentation

$ todo list
You have 2 task(s):

a1b [ ] Buy groceries
c2d [ ] Finish project documentation

$ todo done a1b
Task completed successfully: [a1b]

$ todo list
You have 2 task(s):

a1b [X] Buy groceries
c2d [ ] Finish project documentation
```

## 📄 License

MIT License - see [LICENSE](https://github.com/gbad8/todoCLI/blob/main/LICENSE) for details.