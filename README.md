# PottMaster

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![MAUI](https://img.shields.io/badge/MAUI-9.0-green.svg)](https://dotnet.microsoft.com/en-us/apps/maui)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

PottMaster is a cross-platform mobile application designed to support ceramic artists in managing their pottery production processes. Built with .NET MAUI, it provides offline-first functionality for tracking drying stages, cataloging ceramic works with unique identification codes, and maintaining glaze inventories.

## ?? Problem Statement

Ceramic artists face several critical challenges in their workflow:
- **Drying Control**: Lack of precise control over drying time leads to cracking risk
- **Work Identification**: Visual identification is unreliable in shared studios, causing mix-ups
- **Material Documentation**: Difficulty maintaining reliable records of materials used (glazes, clays)
- **Efficiency Tracking**: No data on studio efficiency (Yield Rate)

## ?? Solution

PottMaster addresses these challenges through:
- **Offline-First Architecture**: Full functionality without internet connection
- **Automated Timers**: Intelligent drying time calculation based on wall thickness
- **Unique Coding System**: Automatic generation of identification codes (Initials-CatCode-MMYY-Counter)
- **Material Cataloging**: Personal glaze inventory with stock control
- **Knowledge Base**: Public Wiki with expert verification system
- **Analytics**: Monthly performance reports ("Pottery Wrapped")

## ? Key Features

### Core Functionality
- ? **Work Registration**: Add new ceramic works with photos, categories, and wall thickness
- ? **Unique Identification**: Automatic code generation (e.g., MK-CUP-1224-001)
- ? **Process Timer Management**: Real-time countdown based on drying requirements
- ? **Manual Status Correction**: Advance work status manually when ready
- ? **Offline Operation**: Full functionality without network connectivity
- ? **Automatic Synchronization**: Background sync when network available

### Material Management
- ? **Glaze Inventory**: Personal catalog with detailed properties
- ? **Glaze Properties**: Comprehensive technical specifications (firing temp, color, behavior)
- ? **Work-Glaze Linking**: Associate glazes with specific works

### Knowledge Base
- ?? **Wiki Browsing**: Search public database of materials (in development)
- ?? **Expert Verification**: Community and expert-approved entries (planned)

### Analytics & Reporting
- ?? **Pottery Wrapped**: Monthly yield rate and performance reports (planned)

### User Experience
- ? **Multi-Language Support**: Polish and English localization
- ? **Secure Authentication**: Email/password and One-Tap sign-in (Google/Apple)
- ? **Cross-Platform**: iOS, Android, Windows, macOS

## ??? Architecture

### Technology Stack
- **Frontend**: .NET MAUI 9.0 (C#)
- **Backend**: Supabase (PostgreSQL, Auth, Storage, Edge Functions)
- **Local Storage**: SQLite with SQLite-net-pcl
- **Architecture Pattern**: MVVM with Dependency Injection
- **UI Framework**: XAML with Shell navigation

### System Components
- **Domain Layer**: Core business models and logic
- **Data Layer**: Repositories for local (SQLite) and remote (Supabase) data
- **Service Layer**: Business logic and external integrations
- **Presentation Layer**: ViewModels and XAML views
- **Infrastructure**: Cross-platform services (auth, sync, alerts)

## ?? Screenshots

*Screenshots will be added during development*

## ??? Installation & Setup

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 (17.8+) with MAUI workload
- Supabase account and project

### Clone and Build
```bash
git clone https://github.com/mklekowiecki/PottMaster.git
cd PottMaster
dotnet restore
dotnet build
```

### Configuration
1. Create a Supabase project
2. Copy `.env.example` to `.env` and configure:
   - `SUPABASE_URL`
   - `SUPABASE_ANON_KEY`
3. Run database migrations from `supabase/migrations/`
4. Build and deploy to your target platform

### Running the App
```bash
# For Android
dotnet build -t:Run -f net9.0-android

# For iOS (macOS only)
dotnet build -t:Run -f net9.0-ios

# For Windows
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

## ?? Usage

### Getting Started
1. **Sign Up/Login**: Create account with email or One-Tap sign-in
2. **Add Your First Work**: Take a photo, select category, set wall thickness
3. **Monitor Progress**: View real-time drying countdown
4. **Manage Inventory**: Add glazes to your personal catalog
5. **Sync Data**: Automatic background synchronization

### Work Lifecycle
1. **Wet Clay** ? Initial registration
2. **Leather Hard** ? Partially dried
3. **Bone Dry** ? Fully dried, ready for bisque firing
4. **Bisque Fired** ? First firing complete
5. **Glazed** ? Glaze applied
6. **Glaze Fired** ? Final firing complete
7. **Completed** ? Finished piece

### Drying Time Algorithm
| Wall Thickness | Drying Time |
|----------------|-------------|
| ? 5mm          | 4 days      |
| 6-10mm         | 7 days      |
| 11-15mm        | 10 days     |
| > 15mm         | 14 days     |

## ?? Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

### Development Setup
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests: `dotnet test`
5. Submit a pull request

### Code Standards
- Follow .NET MAUI guidelines in `.kilocode/rules/maui-guidelines.md`
- Use MVVM pattern consistently
- Maintain offline-first architecture
- Update memory bank documentation for significant changes

## ?? Project Status

### MVP Phase 1 (Completed ?)
- Authentication & User Management
- Work Lifecycle Management
- Offline & Synchronization
- Basic Material Management

### MVP Phase 2 (In Progress ??)
- Knowledge Base (Wiki)
- Enhanced Analytics

### Post-MVP (Planned ??)
- Expert Verification System
- Advanced Analytics ("Pottery Wrapped")
- Social Features

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ?? Acknowledgments

- Ceramic artists community for domain expertise
- .NET MAUI team for the excellent framework
- Supabase for the backend-as-a-service platform

## ?? Support

For questions or support:
- Create an issue on GitHub
- Check the [Wiki](https://github.com/mklekowiecki/PottMaster/wiki) for documentation
- Join our [Discussions](https://github.com/mklekowiecki/PottMaster/discussions)

---

**Built with ?? for ceramic artists worldwide**