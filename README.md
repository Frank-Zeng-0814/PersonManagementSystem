# Person Management System

A production-ready full-stack CRUD application built with .NET 8 and React 19, featuring advanced search, pagination, cloud image upload, and comprehensive error handling.

## Features

- **Advanced Search & Filtering** - Real-time search with multi-field sorting using LINQ queries
- **Smart Pagination** - Efficient data handling with customizable page sizes
- **Cloud Image Upload** - Avatar upload with Cloudinary integration and validation
- **Error Handling & Logging** - Production-grade error handling with Serilog structured logging
- **API Documentation** - Interactive Swagger/OpenAPI documentation
- **Responsive UI** - Modern, mobile-friendly interface with Tailwind CSS

## Tech Stack

### Backend
- **.NET 8** - Web API
- **Entity Framework Core 8** - ORM with Code-First approach
- **PostgreSQL (Neon)** - Database for development and production
- **Serilog** - Structured logging (Console + File)
- **Cloudinary** - Cloud image storage
- **Swagger** - API documentation

### Frontend
- **React 19** - UI framework
- **Tailwind CSS** - Styling
- **React Router** - Navigation
- **React Hook Form** - Form handling and validation
- **Axios** - HTTP client
- **React Hot Toast** - Toast notifications
- **Lucide React** - Icons

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- PostgreSQL database (or use [Neon](https://neon.tech/) for free)
- [Cloudinary account](https://cloudinary.com/) (free tier available)

### Backend Setup

1. Navigate to backend directory:
```bash
cd Backend/Backend
```

2. Configure User Secrets (secure local configuration):
```bash
dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=your-host;Database=your-db;Username=your-user;Password=your-password;SSL Mode=Require"
dotnet user-secrets set "Cloudinary:CloudName" "your-cloud-name"
dotnet user-secrets set "Cloudinary:ApiKey" "your-api-key"
dotnet user-secrets set "Cloudinary:ApiSecret" "your-api-secret"
dotnet user-secrets set "AllowedOrigins" "http://localhost:5173"
```

3. Run the backend:
```bash
dotnet run
```

The API will start at `http://localhost:3000` and automatically apply database migrations.

**Note:** User Secrets are stored in `~/.microsoft/usersecrets/` and never committed to Git.

### Frontend Setup

1. Navigate to frontend directory:
```bash
cd client
```

2. Install dependencies:
```bash
npm install
```

3. Create `.env` file:
```
VITE_BASE_API_URL=http://localhost:3000/api
```

4. Start development server:
```bash
npm run dev
```

The app will be available at `http://localhost:5173`

## API Endpoints

- `GET /api/people` - Get paginated people with search & filters
- `GET /api/people/{id}` - Get person by ID
- `POST /api/people` - Create new person
- `PUT /api/people/{id}` - Update person
- `DELETE /api/people/{id}` - Delete person
- `POST /api/people/{id}/upload-avatar` - Upload person avatar

Visit `http://localhost:3000/swagger` for interactive API documentation.

## Deployment

### Backend (Railway)

1. Push to GitHub
2. Create new project on [Railway](https://railway.app/)
3. Connect GitHub repository
4. Add environment variables:
   - `ConnectionStrings__PostgreSQL`
   - `AllowedOrigins`
   - `Cloudinary__CloudName`
   - `Cloudinary__ApiKey`
   - `Cloudinary__ApiSecret`

### Frontend (Vercel)

1. Create new project on [Vercel](https://vercel.com/)
2. Import GitHub repository
3. Set root directory to `client`
4. Add environment variable:
   - `VITE_BASE_API_URL=https://your-backend-url.railway.app/api`

## Project Structure

```
DotnetReactCrud/
├── Backend/
│   └── Backend/
│       ├── Controllers/       # API endpoints
│       ├── Models/           # Data models
│       ├── Services/         # Business logic
│       ├── Middleware/       # Error handling
│       ├── DTOs/            # Data transfer objects
│       └── Migrations/      # Database migrations
└── client/
    ├── src/
    │   ├── components/      # React components
    │   ├── pages/          # Page components
    │   └── App.jsx         # App root
    └── public/             # Static assets
```

## Architecture Highlights

- **Clean Architecture** - Separation of concerns with DTOs, Services, and Controllers
- **Auto Migrations** - Database schema updates on application startup
- **User Secrets** - Secure local development configuration (never committed to Git)
- **CORS Configuration** - Environment-based origin allowlist
- **Global Exception Handling** - Centralized error handling middleware
- **Structured Logging** - Request/response logging with Serilog

