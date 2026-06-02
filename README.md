# JobForge 💼

> REST API platform for aggregating and managing job offers, internships, trainings, and courses.

![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=flat&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=flat&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker&logoColor=white)

---

## 📖 About

JobForge is a backend REST API built with **ASP.NET Core (.NET 8)** and **PostgreSQL**, designed to aggregate job offers, internships, trainings, and courses in a single platform. The application follows a clean RESTful architecture with full frontend/backend separation and is containerized with Docker Compose for a reproducible dev and production environment.

---

## ✨ Features

- 📋 Browse and filter **job offers, internships, trainings, and courses**
- 🔐 JWT-based **authentication and authorization** with Refresh Tokens
- 📧 Automated **email verification** on account registration
- 🐘 **PostgreSQL** database managed via Entity Framework Core
- 🐳 Fully **containerized** with Docker Compose (API + DB as separate services)
- 📄 **Swagger / OpenAPI** documentation for all endpoints
- ⚡ ~50% faster environment setup compared to manual configuration

---

## 🛠 Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core (.NET 8), C# |
| ORM | Entity Framework Core, LINQ |
| Database | PostgreSQL |
| Auth | JWT, Refresh Tokens |
| Containerization | Docker, Docker Compose |
| API Docs | Swagger / OpenAPI |
| IDE | JetBrains Rider |

---

## 📁 Project Structure

```
JobForge/
├── .containers/
│   └── postgres-db/        # PostgreSQL Docker configuration
├── JobForge/
│   ├── Controllers/        # API endpoint controllers
│   ├── Models/             # Domain models
│   ├── Services/           # Business logic layer
│   ├── Data/               # EF Core DbContext & migrations
│   └── Program.cs          # App entry point & DI configuration
├── Dockerfile.postgres     # Custom PostgreSQL image
├── compose.yaml            # Docker Compose configuration
└── global.json             # .NET SDK version pin
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (see `global.json` for pinned version)
- [Docker](https://www.docker.com/) & Docker Compose

### Run with Docker (recommended)

```bash
git clone https://github.com/Brizuu/JobForge.git
cd JobForge
docker compose up --build
```

API available at: `http://localhost:5000`  
Swagger UI at: `http://localhost:5000/swagger`

### Run locally (without Docker)

```bash
# Start only the PostgreSQL container
docker compose up postgres-db -d

# Run the API
cd JobForge
dotnet run
```

---

## 📬 API Overview

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register new account |
| POST | `/api/auth/login` | Login, receive JWT + Refresh Token |
| POST | `/api/auth/refresh` | Refresh access token |
| GET | `/api/jobs` | List job offers (filterable) |
| GET | `/api/trainings` | List trainings and courses |
| GET | `/api/internships` | List internships |

> Full documentation available via Swagger UI after running the project.

---

## 👤 Author

**Fabian Kur** — Junior .NET Backend Developer from Poznań, Poland

- GitHub: [github.com/Brizuu](https://github.com/Brizuu)
- LinkedIn: [linkedin.com/in/fabian-kur](https://www.linkedin.com/in/fabian-kur-03274b248/)
- Portfolio: [zenfix.pl](https://zenfix.pl)
