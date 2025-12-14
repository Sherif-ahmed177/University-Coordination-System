# University Coordination System

A robust web application built to manage university admissions, student applications, academic programs, and associated payments.

## 📌 Overview

The **University Coordination System** is developed using **ASP.NET Core MVC** to centralize and automate administrative tasks related to student enrollment and academic program management across different schools within a university.

## ✨ Features

### 🏫 School Management
- Add, edit, and remove university schools
- Display school details with linked academic majors
- Monitor school capacity and current enrollment

### 🎓 Major Management
- Create and manage academic majors per school
- Define capacity limits and program durations
- Access detailed information for each major

### 📝 Student Applications
- Submit and manage student applications
- Track application status and progress
- Manage student profiles and uploaded documents
- View complete application history

### 💳 Payment Processing
- Handle application payments and fees
- Track payment statuses
- Generate downloadable payment receipts
- Maintain payment transaction records

## 🛠️ Tech Stack

- **Backend Framework**: ASP.NET Core MVC (.NET 6+)
- **Database**: MySQL
- **ORM**: Entity Framework Core
- **Frontend**:
  - [Bootstrap](https://getbootstrap.com/) for responsive UI
  - [jQuery](https://jquery.com/) for interactivity
  - [DataTables](https://datatables.net/) for dynamic table features
- **Authentication**: ASP.NET Core Identity

## 📁 Project Structure

```text
University-Coordination-System/
├── Controllers/
├── Models/
│   └── ViewModels/
├── Services/
├── Views/
├── wwwroot/
├── database/
│   └── universityapplicationsystem.sql
├── Program.cs
└── README.md
````

## 🔑 Key Entities

  - **School** – Represents a college/school within the university
  - **Major** – Represents an academic program
  - **Student** – Contains student details and documents
  - **Application** – Links students to majors and tracks their admission process
  - **Payment** – Manages application fees and receipts

## 🚀 Getting Started

### ✅ Prerequisites

  - .NET 6.0 SDK or later
  - MySQL Server 8.0+
  - Visual Studio 2022 / VS Code
  - MySQL Workbench (optional)

-----

## 🧩 Installation Steps

### 1️⃣ Clone the Repository

```bash
git clone [https://github.com/your-username/University-Coordination-System.git](https://github.com/your-username/University-Coordination-System.git)
cd University-Coordination-System
```

-----

### 2️⃣ Restore .NET Dependencies

```bash
dotnet restore
```

-----

### 3️⃣ Database Setup (MySQL)

#### Create Database

```sql
CREATE DATABASE universityapplicationsystem;
```

#### Import Database

Using **PowerShell**:

```powershell
Get-Content database\universityapplicationsystem.sql | mysql -u root -p universityapplicationsystem
```

Or using **MySQL Workbench**:

  * Server → Data Import
  * Import `database/universityapplicationsystem.sql`

-----

### 4️⃣ Configure Connection String

Update `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=universityapplicationsystem;user=root;password=YOUR_PASSWORD;"
}
```

-----

### 5️⃣ Run the Application

```bash
dotnet run
```

Open:

```
http://localhost:5000
```

-----

## 👨‍💻 Usage

  * Log in as admin or user
  * Navigate through the dashboard
  * Manage schools, majors, applications, and payments

-----

## 🤝 Contributing

1.  Fork the repository
2.  Create a feature branch
3.  Commit changes
4.  Push to your branch
5.  Open a Pull Request

-----

## 📄 License

Licensed under the MIT License.

## 🆘 Support

For issues or feature requests, please open an issue on GitHub.
