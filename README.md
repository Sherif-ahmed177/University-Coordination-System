# University Coordination System

A robust web application built to manage university admissions, student applications, academic programs, and associated payments.

## 📌 Overview

The **University Coordination System** is developed using **ASP.NET Core MVC** to centralize and automate various administrative tasks related to student enrollment and academic program management across different schools within a university.

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
- **Database**: SQL Server
- **Frontend**:
  - [Bootstrap](https://getbootstrap.com/) for responsive UI
  - [jQuery](https://jquery.com/) for interactivity
  - [DataTables](https://datatables.net/) for dynamic table features
- **Authentication**: ASP.NET Core Identity

## 📁 Project Structure

```
University-Coordination-System/
├── Controllers/         # Handles HTTP requests
├── Models/              # Core and ViewModels
│   └── ViewModels/      
├── Views/               # Razor Pages (UI)
├── Services/            # Business logic layer
├── wwwroot/             # Static content (CSS, JS, Images)
└── Program.cs           # Entry point
```

## 🔑 Key Entities

- **School** – Represents a college/school within the university
- **Major** – Represents an academic program
- **Student** – Contains student details and documents
- **Application** – Links students to majors and tracks their admission process
- **Payment** – Manages application fees and receipts

## 🚀 Getting Started

### ✅ Prerequisites
- .NET 6.0 SDK or later
- SQL Server
- Visual Studio 2022 / Visual Studio Code

### 🧩 Installation Steps

1. **Clone the repository**:
   ```bash
   git clone https://github.com/your-username/university-coordination-system.git
   ```

2. **Navigate to the project directory**:
   ```bash
   cd University-Coordination-System
   ```

3. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

4. **Configure the database**:  
   Update the `appsettings.json` file with your SQL Server connection string.

5. **Apply migrations**:
   ```bash
   dotnet ef database update
   ```

6. **Run the application**:
   ```bash
   dotnet run
   ```

7. Open a browser and go to `http://localhost:5000` (or the specified port).

## 👨‍💻 Usage

1. Log in with your admin or user credentials.
2. Navigate using the top menu bar.
3. Use available modules to manage schools, majors, student applications, and payments efficiently.

## 🤝 Contributing

Contributions are welcome!

1. Fork the repository  
2. Create a feature branch (`git checkout -b feature-name`)  
3. Commit your changes (`git commit -m "Add feature"`)  
4. Push to your branch (`git push origin feature-name`)  
5. Open a Pull Request  

## 📄 License

Licensed under the [MIT License](LICENSE).

## 🆘 Support

For issues or feature requests, please open an issue in the GitHub repository or contact the development team directly.
