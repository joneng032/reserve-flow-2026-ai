Of course. Here is the complete development plan, formatted as a document for your records.

An Agentic Development Framework for the Reserve Study PWA (Enhanced with awesome-copilot Tools)
This framework operationalizes your project_plan.md by creating a "Copilot-native" development environment. This plan outlines the use of specific file-based customizations to train GitHub Copilot on your project's unique requirements, making it a highly effective and specialized AI partner.   

Phase 1: Foundational Setup — Creating a "Project-Aware" Copilot
Your first step is to create a set of instructions and resources that Copilot will use as its permanent context. This is the most critical investment you can make for ensuring high-quality, consistent AI assistance throughout the project.   

1. Create Comprehensive Custom Instructions
The .github/copilot-instructions.md file is your project's constitution for Copilot. It should be detailed and specific, acting as the single source of truth for the AI.

Workflow:

Create a file at the root of your repository: .github/copilot-instructions.md.

Populate it with the following enhanced template, which is based on best practices for providing Copilot with high-quality context.   

Enhanced Instructions Template (.github/copilot-instructions.md):

Project Overview: Reserve Study PWA
This project is a Progressive Web App (PWA) for reserve study data collection. Its primary function is a robust, offline-first data acquisition and organization engine. It bridges in-field collection with back-office analysis systems. Crucially, it does NOT perform financial modeling.

Core Architectural Principles
Data Integrity is Sacrosanct: Prevent data loss, especially during offline-to-online sync. All data must be auditable.

The Field is the Primary Arena: The UI/UX must be optimized for mobile, offline-first usage (large touch targets, high contrast).

Engine, Not a Vehicle: Specialize in data acquisition. Avoid feature creep into financial analysis or report generation.

Technology Stack
Architecture: Progressive Web App (PWA)

Framework: React with TypeScript

Styling: Tailwind CSS

State Management: React Hooks & Context API for simple state; consider Zustand for complex state.

Offline Storage: IndexedDB, using the Dexie.js wrapper.

Testing: Vitest for unit tests and React Testing Library for component tests.

Build Tool: Vite

Coding Guidelines
TypeScript: Use strict mode. All functions, components, and variables must be explicitly typed. Avoid any.

Style: Follow the Airbnb JavaScript Style Guide.

Components: Create functional components using React Hooks. Components should be small and single-purpose.

Comments: Use JSDoc for all functions and component props.

Tests: Unit tests are required for all business logic (services, hooks). Component tests are required for all UI components. Follow the Arrange-Act-Assert pattern.

Project Structure
src/components: Reusable UI components (e.g., Button.tsx, Input.tsx).

src/features: Feature-sliced modules (e.g., src/features/projects, src/features/components). Each feature folder contains its own components, hooks, and services.

src/services: Core services, like the offline synchronization engine (SyncService.ts).

src/db: IndexedDB schema and configuration (db.ts).

src/hooks: Global, reusable React hooks.

src/types: Global TypeScript type definitions and interfaces.

Available Resources
Full Specification: project_plan.md contains all feature epics and data models.

Database Schema: src/types/database.ts is the source of truth for all data structures.

Phase 2: The Development Workflow — Using Specialized Tools
With the foundational instructions in place, you can now use specific tools from the awesome-copilot collection to accelerate development for each part of your plan.   

1. Project Scaffolding (Agent Mode)
Use Copilot's agent mode for the initial setup. It will read your new instructions file and act on them.   

Agent Prompt:

@workspace /new Create a new PWA project based on the instructions in.github/copilot-instructions.md. Use Vite to scaffold a React + TypeScript application. Set up the specified project directory structure and install dependencies: tailwindcss, dexie, vitest, @testing-library/react.

2. Building with Reusable Prompts
Reusable prompts are markdown files that allow you to save and reuse complex instructions for common tasks, ensuring consistency for you and your team.

Workflow:

Create a .github/prompts directory.

Create task-specific .prompt.md files inside it.

In the Copilot Chat, invoke them by typing #prompt: followed by the filename.

Example 1: Reusable Prompt for UI Components

File: .github/prompts/react-component.prompt.md

Your goal is to generate a new React component. Ask for the component name and its props if not provided.

Requirements:

Use TypeScript for all props and internal state.

Use Tailwind CSS for styling. Do not use inline styles.

The component must be a functional component using React Hooks.

Generate a corresponding Vitest/React Testing Library test file that checks for basic rendering and prop handling.

Adhere to all standards defined in .github/copilot-instructions.md.

Usage in Chat:

#prompt:react-component Create a ProjectCard component. It should accept props for clientName, projectName, and completionPercentage. The percentage should be displayed as a progress bar.

Example 2: Reusable Prompt for Unit Tests

File: .github/prompts/vitest-unit-test.prompt.md

Your goal is to generate a comprehensive unit test file for the selected code using Vitest.

Requirements:

Follow the Arrange-Act-Assert (AAA) pattern for test structure.

Mock all external dependencies and functions.

Include tests for the primary success case.

Include tests for failure cases with invalid inputs.

Include tests for at least one edge case (e.g., empty arrays, null values).

Usage in Chat (after highlighting a function):

#selection #prompt:vitest-unit-test

3. Using Slash Commands for Quick Tasks
For common, in-context tasks, use slash commands for maximum speed.

Explain Code: Highlight a complex function and type /explain in the chat.

Fix Bugs: Highlight buggy code and type /fix.

Generate Tests: Highlight a function and type /tests for a quick test file.

Phase 3: Advanced Workflow — Creating a Project "Persona"
For the most tailored experience, you can create a custom chat mode. This gives Copilot a specific persona to adopt during your conversations, making its responses more specialized.

1. Define a Custom Chat Mode
Workflow:

Create a .github/copilot/chats directory.

Create a JSON file defining your chat mode.

Example Chat Mode File: .github/copilot/chats/reserve-study-expert.json

JSON

{
  "name": "Reserve Study Expert",
  "description": "An expert PWA developer specializing in offline-first architecture for field data collection apps.",
  "instructions":
}
Usage:

In the Copilot Chat view, you can now select "Reserve Study Expert" from the chat mode dropdown. All subsequent prompts in that session will be filtered through this expert persona, resulting in highly relevant and specialized assistance.

By implementing these tools from the awesome-copilot repository, you are building a sophisticated, customized AI development environment. This structured approach ensures that GitHub Copilot acts as a true extension of your team, fully aligned with your project's specific architecture, standards, and goals.