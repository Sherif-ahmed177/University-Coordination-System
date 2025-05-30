# University Coordination System

A comprehensive web application for managing university admissions, student applications, and academic programs.

## Overview

The University Coordination System is an ASP.NET Core MVC application designed to streamline the university admission process. It provides a centralized platform for managing schools, majors, student applications, and payments.

## Features

### School Management
- Create and manage different schools within the university
- View school details and associated majors
- Track school capacity and enrollment

### Major Management
- Create and manage academic majors
- Set capacity limits and duration for each major
- Associate majors with specific schools
- View detailed information about each major

### Student Applications
- Process student applications for different majors
- Track application status and progress
- Manage student information and documents
- View application history

### Payment Processing
- Handle application fees and payments
- Track payment status
- Generate payment receipts
- Manage payment records

## Technical Stack

- **Framework**: ASP.NET Core MVC
- **Database**: SQL Server
- **Frontend**: 
  - Bootstrap for responsive design
  - jQuery for dynamic interactions
  - DataTables for enhanced table functionality
- **Authentication**: ASP.NET Core Identity

## Project Structure

```
University-Coordination-System/
├── Controllers/         # MVC Controllers
├── Models/             # Data Models
│   └── ViewModels/     # View-specific Models
├── Views/              # Razor Views
├── Services/           # Business Logic Services
├── wwwroot/           # Static Files (CSS, JS, Images)
└── Program.cs         # Application Entry Point
```

## Key Models

- **School**: Represents different schools within the university
- **Major**: Academic programs offered by schools
- **Student**: Student information and details
- **Application**: Student applications for majors
- **Payment**: Payment records and transactions

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- SQL Server
- Visual Studio 2022 or Visual Studio Code

### Installation

1. Clone the repository:
   ```bash
   git clone [repository-url]
   ```

2. Navigate to the project directory:
   ```bash
   cd University-Coordination-System
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Update the database connection string in `appsettings.json`

5. Run database migrations:
   ```bash
   dotnet ef database update
   ```

6. Start the application:
   ```bash
   dotnet run
   ```

## Usage

1. Access the application through your web browser
2. Log in with appropriate credentials
3. Navigate through different sections using the menu
4. Manage schools, majors, applications, and payments as needed

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For support, please contact the development team or create an issue in the repository.