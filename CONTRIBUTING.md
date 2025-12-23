# Contributing to POS System

Thank you for your interest in contributing to the POS System project! This document provides guidelines and instructions for contributing.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Project Structure](#project-structure)
- [Coding Standards](#coding-standards)
- [Making Changes](#making-changes)
- [Testing](#testing)
- [Submitting Changes](#submitting-changes)
- [Review Process](#review-process)

## Code of Conduct

This project adheres to a code of conduct. By participating, you are expected to uphold this code. Please be respectful and constructive in all interactions.

## Getting Started

1. **Fork the Repository**
   ```bash
   # Click the "Fork" button on GitHub
   ```

2. **Clone Your Fork**
   ```bash
   git clone https://github.com/YOUR-USERNAME/dot-net-core-pos.git
   cd dot-net-core-pos
   ```

3. **Add Upstream Remote**
   ```bash
   git remote add upstream https://github.com/fuadwasi/dot-net-core-pos.git
   ```

4. **Install Prerequisites**
   - .NET 9 SDK
   - MAUI workload: `dotnet workload install maui`
   - Visual Studio 2022 or VS Code with C# Dev Kit

## Development Setup

### Initial Setup

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests (once test projects are added)
dotnet test
```

### IDE Configuration

#### Visual Studio 2022
1. Open `POSSystem.sln`
2. Set `POSSystem.Maui` as startup project
3. Select target platform (Windows, Android, iOS, MacCatalyst)
4. Press F5 to run

#### VS Code
1. Install C# Dev Kit extension
2. Install .NET MAUI extension
3. Open folder in VS Code
4. Use command palette for build/run commands

## Project Structure

```
src/
├── POSSystem.Domain/          # Core business logic (no dependencies)
│   ├── Entities/              # Domain entities
│   └── Interfaces/            # Repository interfaces
├── POSSystem.Infrastructure/  # Data access implementations
│   ├── Data/                  # DbContext
│   └── Repositories/          # Repository implementations
├── POSSystem.Application/     # Application services
│   └── Services/              # Business logic services
└── POSSystem.Maui/           # UI layer
    ├── Pages/                 # XAML pages
    ├── ViewModels/            # MVVM ViewModels
    ├── Resources/             # Images, fonts, styles
    └── Platforms/             # Platform-specific code
```

## Coding Standards

### C# Style Guidelines

- Follow [C# Coding Conventions](https://docs.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful names for variables, methods, and classes
- Keep methods small and focused (single responsibility)
- Use async/await for all I/O operations
- Enable nullable reference types

### Naming Conventions

```csharp
// Classes and Methods: PascalCase
public class ProductService { }
public async Task<Product> GetProductAsync(int id) { }

// Private fields: _camelCase
private readonly IUnitOfWork _unitOfWork;

// Properties: PascalCase
public string Name { get; set; }

// Local variables and parameters: camelCase
var productList = await GetProducts();
public void ProcessSale(int customerId) { }

// Constants: PascalCase
public const int MaxRetryAttempts = 3;
```

### Architecture Guidelines

1. **Domain Layer**
   - Pure C# with no external dependencies
   - Contains only entities and interfaces
   - No business logic in entities (anemic domain model)

2. **Infrastructure Layer**
   - Implements domain interfaces
   - Contains all database-related code
   - Should not reference UI layer

3. **Application Layer**
   - Contains business logic
   - Orchestrates domain objects
   - No UI or database dependencies

4. **UI Layer (MAUI)**
   - MVVM pattern with CommunityToolkit
   - No business logic in code-behind
   - ViewModels handle all UI logic

### XAML Guidelines

```xml
<!-- Use data binding -->
<Label Text="{Binding ProductName}" />

<!-- Use meaningful x:Name for controls that need code-behind access -->
<Button x:Name="SaveButton" Text="Save" />

<!-- Group related UI elements -->
<VerticalStackLayout Spacing="10">
    <!-- Related controls -->
</VerticalStackLayout>

<!-- Use styles for consistent appearance -->
<Label Style="{StaticResource Headline}" />
```

## Making Changes

### Creating a Feature Branch

```bash
# Update your fork
git checkout main
git pull upstream main

# Create feature branch
git checkout -b feature/your-feature-name
```

### Branch Naming

- `feature/` - New features
- `fix/` - Bug fixes
- `refactor/` - Code refactoring
- `docs/` - Documentation changes
- `test/` - Test additions/changes

Examples:
- `feature/add-inventory-management`
- `fix/sale-calculation-bug`
- `refactor/repository-pattern`

### Commit Messages

Follow the [Conventional Commits](https://www.conventionalcommits.org/) specification:

```
<type>(<scope>): <subject>

<body>

<footer>
```

Types:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

Examples:
```
feat(products): add barcode scanning functionality

Implemented barcode scanning using the device camera.
Supports multiple barcode formats.

Closes #123
```

```
fix(sales): correct tax calculation rounding

Fixed rounding error in tax calculation that caused
discrepancies in total amounts.
```

## Testing

### Writing Tests

Tests should be added for:
- New features
- Bug fixes
- Refactored code

```csharp
// Example unit test
[Fact]
public async Task GetProductById_ShouldReturnProduct()
{
    // Arrange
    var mockRepo = new Mock<IProductRepository>();
    mockRepo.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new Product { Id = 1, Name = "Test" });
    
    var service = new ProductService(mockRepo.Object);
    
    // Act
    var result = await service.GetProductByIdAsync(1);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test", result.Name);
}
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/POSSystem.Application.Tests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Submitting Changes

### Before Submitting

1. **Build Successfully**
   ```bash
   dotnet build --configuration Release
   ```

2. **Run Tests**
   ```bash
   dotnet test
   ```

3. **Update Documentation**
   - Update README.md if needed
   - Add XML documentation comments
   - Update MIGRATION.md if architecture changes

4. **Check Code Quality**
   - Remove unused usings
   - Fix warnings
   - Format code consistently

### Creating a Pull Request

1. **Push to Your Fork**
   ```bash
   git push origin feature/your-feature-name
   ```

2. **Create PR on GitHub**
   - Go to the original repository
   - Click "New Pull Request"
   - Select your fork and branch
   - Fill in the PR template

3. **PR Description Should Include**
   - What changes were made
   - Why the changes are needed
   - How to test the changes
   - Screenshots (for UI changes)
   - Related issue numbers

### PR Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
How to test these changes

## Screenshots
(If applicable)

## Checklist
- [ ] Code builds without errors
- [ ] Tests added/updated
- [ ] Documentation updated
- [ ] Code follows style guidelines
```

## Review Process

1. **Automated Checks**
   - Build status
   - Test results
   - Code coverage

2. **Code Review**
   - At least one maintainer approval required
   - Address review comments
   - Keep PR updated with main branch

3. **Merge**
   - Squash and merge preferred
   - Maintain clean commit history

## Common Contribution Areas

### Easy First Contributions

- Documentation improvements
- Adding code comments
- Fixing typos
- Adding unit tests
- UI improvements

### Feature Requests

- Check existing issues first
- Create detailed feature proposal
- Discuss design before implementation
- Break large features into smaller PRs

### Bug Reports

Include:
- .NET version
- Platform (Windows/macOS/Android/iOS)
- Steps to reproduce
- Expected vs actual behavior
- Error messages/stack traces

## Questions?

- Open an issue for questions
- Join discussions in existing issues
- Check documentation first

## Recognition

Contributors will be:
- Listed in CONTRIBUTORS.md
- Mentioned in release notes
- Credited in commit messages

Thank you for contributing! 🎉
