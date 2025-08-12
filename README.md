# NET.BackgroundServices

**NET.BackgroundServices** is a sample .NET project demonstrating how to build efficient and maintainable **background services**.  
It serves as a clean template for implementing long-running background tasks that can be reused in various .NET applications such as Worker Services or ASP.NET Core.

Key objectives:
- Define recurring or continuous background tasks.
- Properly handle dependency injection, especially scoped services, in a non-HTTP request environment.
- Provide a clear and extensible structure for long-running services.

## Tech Stack

- **.NET (Worker Service / BackgroundService)** – Uses the `BackgroundService` base class from `Microsoft.Extensions.Hosting` to implement background jobs.  
- **Dependency Injection & Scoped Services** – Creates and manages `IServiceScope` or uses `IServiceScopeFactory` to safely resolve scoped services within a `BackgroundService`.  
- **Task Scheduling / Timer-based Execution** – Supports periodic execution of tasks using `Timer`, `PeriodicTimer`, or similar scheduling mechanisms.  
