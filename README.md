# JobForge 💼

**JobForge** is a REST API platform for aggregating and managing job offers, internships, trainings, and courses — built with ASP.NET Core and PostgreSQL, containerized with Docker.

---

## 🚀 Features

- Browse and filter job offers, internships, trainings, and courses
- RESTful API with clean separation of concerns
- Containerized environment with Docker Compose for easy local setup
- PostgreSQL database with a dedicated Docker container

---

## 🛠 Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core (C#) |
| Database | PostgreSQL |
| Containerization | Docker, Docker Compose |
| IDE | JetBrains Rider |

---

## 📦 Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (see `global.json` for required version)
- [Docker](https://www.docker.com/) & Docker Compose

### Run with Docker

```bash
git clone https://github.com/Brizuu/JobForge.git
cd JobForge
docker compose up --build
```

The API will be available at `http://localhost:5000`.

### Run locally (without Docker)

```bash
# Start the PostgreSQL container only
docker compose up postgres-db -d

# Run the API
cd JobForge
dotnet run
```

---

## 📁 Project Structure

```
JobForge/
├── .containers/
│   └── postgres-db/       # PostgreSQL Docker configuration
├── JobForge/              # ASP.NET Core application
├── Dockerfile.postgres    # Custom PostgreSQL image
├── compose.yaml           # Docker Compose configuration
└── global.json            # .NET SDK version pin
```

---

## 👤 Author

**Fabian Kur** — [github.com/Brizuu](https://github.com/Brizuu) · [LinkedIn](https://www.linkedin.com/in/fabian-kur-03274b248/)
