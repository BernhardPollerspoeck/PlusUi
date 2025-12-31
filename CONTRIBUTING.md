# Contributing to PlusUi

First off, thank you for considering contributing to PlusUi! Every contribution matters, whether it's fixing a typo, improving documentation, reporting a bug, or implementing a new feature. This project thrives because of community involvement, and we're genuinely excited to have you here.

## All Contributions Are Welcome

We believe that the best software is built collaboratively. Whether you're a seasoned developer or just getting started, your perspective and ideas are valuable. Here are some ways you can contribute:

- **Bug Reports:** Found something that doesn't work as expected? Let us know!
- **Feature Requests:** Have an idea that would make PlusUi better? We'd love to hear it.
- **Code Contributions:** Bug fixes, new features, performance improvements - all welcome.
- **Documentation:** Improvements to README, code comments, or new guides.
- **Examples:** New sample applications or usage examples.
- **Code Reviews:** Your feedback on open PRs helps maintain code quality.
- **Discussions:** Share your experiences, ask questions, help others.

No contribution is too small. Even fixing a single typo helps make the project better for everyone.

## How to Contribute

### 1. Open an Issue First

Before starting work on a significant change, please open an issue to discuss it. This helps:
- Ensure your work aligns with the project's direction
- Avoid duplicating effort if someone else is already working on it
- Get early feedback on your approach

For small fixes (typos, obvious bugs), you can skip this step and go straight to a PR.

### 2. Pull Request Requirements

To ensure changes can be reviewed and merged efficiently, we require a specific commit structure for bug fixes and feature changes:

#### For Bug Fixes

Your PR should contain **at least two commits**:

1. **First Commit - Tests Only (No Code Changes)**
   - Add unit tests that **reproduce and confirm the bug**
   - These tests should **fail** at this commit
   - Do not include any fix in this commit
   - Commit message example: `Add tests confirming issue #123 - Button click not triggering command`

2. **Second Commit - The Fix**
   - Implement the actual fix
   - The tests from the first commit should now **pass**
   - Commit message example: `Fix Button command binding when IconPosition is None`

**Why this structure?**
- It proves the bug actually exists and is reproducible
- It demonstrates that your fix actually solves the problem
- It makes code review much faster and more reliable
- It ensures we have regression tests for the future

#### For New Features

1. **First Commit - Tests describing the expected behavior**
   - Write tests that define what the feature should do
   - These tests will initially fail (the feature doesn't exist yet)

2. **Following Commits - Implementation**
   - Implement the feature
   - All tests should pass when complete

### 3. Code Standards

- Follow the existing code style (see `CLAUDE.md` for detailed guidelines)
- Use PascalCase for types, methods, and properties
- Use camelCase for local variables and parameters
- Enable nullable reference types
- Write self-documenting code; add comments only where the logic isn't obvious

### 4. Keep PRs Focused

- One bug fix or feature per PR
- Avoid mixing refactoring with functional changes
- Keep changes as small as reasonably possible

## Development Setup

```bash
# Clone the repository
git clone https://github.com/BernhardPollerspoeck/PlusUi.git
cd PlusUi

# Build the solution
dotnet build PlusUi.sln

# Run tests
dotnet test PlusUi.sln
```

## Questions?

If you're unsure about anything, just ask! Open an issue with your question or join our [Discord community](https://discord.gg/Je3kNpcmqn). We're happy to help you get started.

---

Thank you for helping make PlusUi better!
