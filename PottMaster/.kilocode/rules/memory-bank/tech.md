# Technologies Used - PottMaster

## Mobile Framework

- **.NET MAUI**: Cross-platform framework for building native mobile and desktop apps with C# and XAML.

- **Platforms Supported**: Android, iOS, Mac Catalyst, Windows

- **UI**: XAML for declarative UI, MAUI Controls for native look and feel

- **MVVM**: CommunityToolkit.Mvvm for data binding and dependency injection

## Data Layer

- **Local Database**: SQLite with SQLite-net-pcl for ORM

- **Serialization**: System.Text.Json for JSON handling

- **Image Handling**: .NET MAUI built-in image support with compression

## Backend

- **BaaS**: Supabase for authentication, database, storage

- **Database**: PostgreSQL with Row Level Security

- **Client Library**: Supabase-CSharp for .NET integration

- **Edge Functions**: Deno/TypeScript for serverless logic

## Development Tools

- **IDE**: Visual Studio 2022

- **.NET Version**: .NET 9

- **Build System**: MSBuild

- **Testing**: xUnit or NUnit for unit tests, MAUI UI Tests

- **CI/CD**: GitHub Actions with MAUI workload installation

## Dependencies

- CommunityToolkit.Mvvm
- SQLite-net-pcl
- Supabase-CSharp
- Microsoft.Extensions.DependencyInjection (if needed)

## Technical Constraints

- Offline-first architecture
- Cross-platform compatibility
- Secure data synchronization
- Performance on mobile devices
- Supabase integration for backend services
- Comprehensive glaze management with detailed properties

---

**Last Updated**: 2026-01-11
**Document Owner**: Development Team