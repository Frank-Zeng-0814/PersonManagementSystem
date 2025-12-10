# 🚀 Deployment Guide - Employee Management System

## 📋 Overview

This guide covers deploying:
- **Frontend (React + Vite)** → Vercel
- **Backend (.NET 8 API)** → Railway / Azure / AWS
- **Database (PostgreSQL)** → Railway / Supabase / Azure

---

## 🎯 Architecture

```
┌─────────────────┐
│  Vercel         │  ← Frontend (React)
│  (Static Host)  │
└────────┬────────┘
         │
         │ API Calls
         ↓
┌─────────────────┐
│  Railway/Azure  │  ← Backend (.NET API + Swagger)
│  (API Server)   │
└────────┬────────┘
         │
         │ Database
         ↓
┌─────────────────┐
│  PostgreSQL     │  ← Database
│  (Railway/etc)  │
└─────────────────┘
```

---

## 1️⃣ Backend Deployment (Railway - Recommended)

### Why Railway?
- ✅ Free tier available
- ✅ PostgreSQL included
- ✅ Auto-deployment from GitHub
- ✅ HTTPS certificates automatic
- ✅ Easy environment variables

### Steps:

#### A. Prepare Backend for Deployment

1. **Ensure `Program.cs` is production-ready:**

```csharp
// Already configured in your code:
var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
builder.WebHost.UseUrls($"http://*:{port}");
```

2. **Check CORS configuration:**

```csharp
var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigins")
    ?? "http://localhost:5173";
```

3. **Verify Swagger is enabled in production:**

```csharp
// Keep these lines for production access
app.UseSwagger();
app.UseSwaggerUI();
```

#### B. Deploy to Railway

1. **Sign up at [Railway.app](https://railway.app)**

2. **Create New Project → Deploy from GitHub**
   - Connect your GitHub repository
   - Select the `Backend/Backend` folder as root

3. **Add PostgreSQL Database:**
   - Click "New" → "Database" → "Add PostgreSQL"
   - Railway automatically provides connection string

4. **Set Environment Variables:**

Go to your service → Variables tab:

```env
# Database (Railway auto-provides this)
ConnectionStrings__PostgreSQL=${{Postgres.DATABASE_URL}}

# Cloudinary (from your account)
Cloudinary__CloudName=your_cloudinary_name
Cloudinary__ApiKey=your_cloudinary_api_key
Cloudinary__ApiSecret=your_cloudinary_api_secret

# CORS (Important!)
AllowedOrigins=https://your-vercel-app.vercel.app,http://localhost:5173

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
```

5. **Get Your API URL:**
   - Railway assigns a URL like: `https://your-app.railway.app`
   - This is your `VITE_BASE_API_URL` for frontend

6. **Access Swagger:**
   - Your Swagger UI: `https://your-app.railway.app/swagger`
   - ✅ Publicly accessible!

---

## 2️⃣ Frontend Deployment (Vercel)

### Steps:

#### A. Prepare Frontend

1. **Update `.env.production` (create if not exists):**

```env
VITE_BASE_API_URL=https://your-app.railway.app
```

2. **Ensure `vercel.json` exists:**

```json
{
  "rewrites": [
    { "source": "/(.*)", "destination": "/index.html" }
  ]
}
```

#### B. Deploy to Vercel

1. **Install Vercel CLI (optional):**

```bash
npm install -g vercel
```

2. **Deploy via GitHub (Recommended):**
   - Go to [Vercel Dashboard](https://vercel.com)
   - "Add New Project"
   - Import your GitHub repository
   - **Root Directory:** `client`
   - **Framework Preset:** Vite
   - **Build Command:** `npm run build`
   - **Output Directory:** `dist`

3. **Environment Variables in Vercel:**
   - Go to Project Settings → Environment Variables
   - Add:
     ```
     VITE_BASE_API_URL=https://your-app.railway.app
     ```

4. **Deploy:**
   - Vercel auto-deploys on push to `main` branch
   - Get URL: `https://your-project.vercel.app`

5. **Update Backend CORS:**
   - Go back to Railway → Environment Variables
   - Update `AllowedOrigins`:
     ```
     AllowedOrigins=https://your-project.vercel.app
     ```

---

## 3️⃣ Accessing Swagger in Production

### ✅ Your Swagger URL:

Once deployed, your Swagger documentation will be available at:

```
https://your-app.railway.app/swagger
```

### How it works in the frontend:

The About page automatically displays the correct Swagger URL based on environment:

```javascript
// In About.jsx
const apiUrl = import.meta.env.VITE_BASE_API_URL || 'http://localhost:3000';
const swaggerUrl = `${apiUrl}/swagger`;
```

**Local Development:**
- `http://localhost:3000/swagger`

**Production (Vercel):**
- `https://your-app.railway.app/swagger`

---

## 4️⃣ Alternative Backend Hosting Options

### Option 2: Azure App Service

```bash
# Login to Azure
az login

# Create resource group
az group create --name myResourceGroup --location eastus

# Create App Service plan
az appservice plan create --name myPlan --resource-group myResourceGroup --sku B1 --is-linux

# Create web app
az webapp create --resource-group myResourceGroup --plan myPlan --name myapp --runtime "DOTNET|8.0"

# Configure environment variables
az webapp config appsettings set --resource-group myResourceGroup --name myapp --settings \
  ConnectionStrings__PostgreSQL="your_connection_string" \
  Cloudinary__CloudName="your_name" \
  AllowedOrigins="https://your-vercel-app.vercel.app"

# Deploy
dotnet publish -c Release
cd Backend/Backend/bin/Release/net8.0/publish
az webapp deployment source config-zip --resource-group myResourceGroup --name myapp --src publish.zip
```

**Swagger URL:** `https://myapp.azurewebsites.net/swagger`

### Option 3: AWS Elastic Beanstalk

1. Install AWS CLI and EB CLI
2. Initialize EB application
3. Configure environment variables
4. Deploy with `eb deploy`

**Swagger URL:** `https://your-app.elasticbeanstalk.com/swagger`

---

## 5️⃣ Database Options

### Option 1: Railway PostgreSQL (Included)
- Automatically provisioned with Railway
- No extra setup needed

### Option 2: Supabase (Free Tier)
1. Sign up at [supabase.com](https://supabase.com)
2. Create new project
3. Get connection string from Settings → Database
4. Add to Railway environment variables

### Option 3: Azure Database for PostgreSQL
```bash
az postgres flexible-server create \
  --resource-group myResourceGroup \
  --name mydb \
  --location eastus \
  --admin-user myadmin \
  --admin-password <password> \
  --sku-name Standard_B1ms \
  --tier Burstable \
  --storage-size 32
```

---

## 6️⃣ Environment Variables Summary

### Backend (Railway/Azure/AWS)

```env
# Database
ConnectionStrings__PostgreSQL=<your_postgres_connection_string>

# Cloudinary
Cloudinary__CloudName=<your_cloudinary_name>
Cloudinary__ApiKey=<your_cloudinary_key>
Cloudinary__ApiSecret=<your_cloudinary_secret>

# CORS - IMPORTANT!
AllowedOrigins=https://your-frontend.vercel.app,http://localhost:5173

# ASP.NET
ASPNETCORE_ENVIRONMENT=Production

# Port (Railway auto-sets this)
PORT=3000
```

### Frontend (Vercel)

```env
# API URL
VITE_BASE_API_URL=https://your-backend.railway.app
```

---

## 7️⃣ Testing Production Deployment

### ✅ Checklist:

1. **Backend Health Check:**
   ```
   curl https://your-app.railway.app/api/employees
   ```
   Should return `200 OK`

2. **Swagger Access:**
   - Visit: `https://your-app.railway.app/swagger`
   - Should show interactive API documentation
   - Try executing API calls

3. **Frontend Connection:**
   - Visit: `https://your-project.vercel.app`
   - Check browser console for API URL
   - Should show: `https://your-app.railway.app`

4. **CORS Check:**
   - Open DevTools → Network tab
   - Make API call from frontend
   - Should NOT see CORS errors

5. **SignalR Connection:**
   - Check console: "SignalR Connected"
   - Notification panel shows green dot

6. **Avatar Upload:**
   - Upload an image
   - Should save to Cloudinary
   - Display in frontend

---

## 8️⃣ Continuous Deployment

### Railway (Auto-deploy on push)
- Push to `main` branch → Auto-deploys
- View logs in Railway dashboard

### Vercel (Auto-deploy on push)
- Push to `main` branch → Auto-deploys
- Preview deployments for PRs

### Manual Deployment

**Backend (Railway CLI):**
```bash
railway up
```

**Frontend (Vercel CLI):**
```bash
cd client
vercel --prod
```

---

## 9️⃣ Monitoring & Logs

### Railway Logs
```bash
# Install Railway CLI
npm install -g @railway/cli

# Login
railway login

# View logs
railway logs
```

### Vercel Logs
- Dashboard → Your Project → Deployments → View Function Logs

---

## 🔟 Security Considerations

### ✅ Production Checklist:

1. **HTTPS Everywhere:**
   - ✅ Railway provides HTTPS
   - ✅ Vercel provides HTTPS

2. **Environment Variables:**
   - ✅ Never commit `.env` files
   - ✅ Use platform environment variables

3. **CORS:**
   - ✅ Only allow your Vercel domain
   - ❌ Don't use `*` in production

4. **Database:**
   - ✅ Use SSL connections
   - ✅ Strong passwords
   - ✅ IP whitelisting (if available)

5. **API Keys:**
   - ✅ Cloudinary keys secured in env vars
   - ✅ Never expose in frontend code

6. **Rate Limiting:**
   - Consider adding rate limiting middleware in production

---

## 🎯 Quick Start Commands

### Deploy Everything (from scratch):

```bash
# 1. Backend - Railway
# - Push to GitHub
# - Connect repository to Railway
# - Add PostgreSQL
# - Set environment variables
# - Done! Get API URL

# 2. Frontend - Vercel
cd client
echo "VITE_BASE_API_URL=https://your-railway-app.railway.app" > .env.production
git add .
git commit -m "Add production env"
git push origin main
# - Go to vercel.com
# - Import repository
# - Set VITE_BASE_API_URL
# - Deploy!

# 3. Test Swagger
# Visit: https://your-railway-app.railway.app/swagger
```

---

## 🆘 Troubleshooting

### Issue: Swagger 404 in Production
**Solution:** Ensure these lines are NOT removed:
```csharp
app.UseSwagger();
app.UseSwaggerUI();
```

### Issue: CORS Errors
**Solution:** Update `AllowedOrigins` in Railway to include your Vercel URL

### Issue: Database Connection Failed
**Solution:** Check `ConnectionStrings__PostgreSQL` format:
```
Server=host;Database=db;User Id=user;Password=pass;SslMode=Require;
```

### Issue: SignalR not connecting
**Solution:** Ensure WebSocket support is enabled (Railway supports it by default)

### Issue: Environment variables not working
**Solution:** Restart the service after updating environment variables

---

## 📚 Resources

- [Railway Documentation](https://docs.railway.app)
- [Vercel Documentation](https://vercel.com/docs)
- [ASP.NET Core Deployment](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/)
- [Swagger in Production](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger)

---

## ✅ Summary

1. **Backend → Railway** (Swagger at `https://your-app.railway.app/swagger`)
2. **Frontend → Vercel** (Auto-detects Swagger URL from env)
3. **Database → Railway PostgreSQL** (Included)
4. **Images → Cloudinary** (Already configured)

Your About page will automatically show the correct Swagger URL! 🎉
