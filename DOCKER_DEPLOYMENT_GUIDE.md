# Docker Deployment Guide for Hello Doctor API

This guide provides comprehensive instructions for deploying the Hello Doctor API using Docker containers. Docker deployment offers consistency, portability, and simplified deployment across any Docker-capable environment.

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Quick Start - Development](#quick-start---development)
4. [Production Deployment](#production-deployment)
5. [Environment Configuration](#environment-configuration)
6. [Docker Commands Reference](#docker-commands-reference)
7. [Maintenance Operations](#maintenance-operations)
8. [Troubleshooting](#troubleshooting)
9. [Comparison: Docker vs Traditional VPS](#comparison-docker-vs-traditional-vps)
10. [Advanced Configuration](#advanced-configuration)

---

## Overview

### What's Included

The Docker setup provides:

- **Multi-stage Dockerfile**: Optimized build process with separate build, publish, and runtime stages
- **Docker Compose**: Orchestration for development and production environments
- **PostgreSQL Database**: Containerized database with persistent volumes
- **Nginx Reverse Proxy**: Load balancing, SSL termination, and rate limiting
- **Health Checks**: Automatic monitoring of service health
- **Automated Scripts**: Build and deployment automation
- **Security**: Non-root user execution, network isolation

### Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Nginx (Port 80/443)                  │
│              Reverse Proxy + Rate Limiting              │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│              Hello Doctor API (Port 8080)               │
│                .NET 8.0 Application                     │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│              PostgreSQL 16 (Port 5432)                  │
│              Database with Persistent Volume            │
└─────────────────────────────────────────────────────────┘
```

---

## Prerequisites

### Required Software

1. **Docker Engine** (20.10 or higher)
2. **Docker Compose** (2.0 or higher)
3. **Git** (for cloning the repository)

### Installation on Ubuntu/Debian

```bash
# Update package index
sudo apt update

# Install prerequisites
sudo apt install -y apt-transport-https ca-certificates curl software-properties-common

# Add Docker's official GPG key
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg

# Add Docker repository
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# Install Docker
sudo apt update
sudo apt install -y docker-ce docker-ce-cli containerd.io docker-compose-plugin

# Add your user to docker group (logout/login required after this)
sudo usermod -aG docker $USER

# Verify installation
docker --version
docker compose version
```

### Installation on CentOS/RHEL

```bash
# Install required packages
sudo yum install -y yum-utils

# Add Docker repository
sudo yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo

# Install Docker
sudo yum install -y docker-ce docker-ce-cli containerd.io docker-compose-plugin

# Start and enable Docker
sudo systemctl start docker
sudo systemctl enable docker

# Add user to docker group
sudo usermod -aG docker $USER

# Verify installation
docker --version
docker compose version
```

### System Requirements

**Minimum**:
- 2 CPU cores
- 4 GB RAM
- 20 GB disk space
- Ubuntu 20.04+ or equivalent

**Recommended**:
- 4 CPU cores
- 8 GB RAM
- 50 GB disk space
- Ubuntu 22.04 LTS or 24.04 LTS

---

## Quick Start - Development

### Step 1: Clone the Repository

```bash
# Clone the repository
git clone https://github.com/Comfie/hello-doctor-api.git
cd hello-doctor-api

# Checkout the Docker deployment branch (or main branch if merged)
git checkout claude/docker-deployment-011CUv4s1KFAsdyiPth3k3aV
```

### Step 2: Configure Environment

```bash
# Copy the example environment file
cp .env.example .env

# Edit the environment file with your values
nano .env
```

**Minimum required configuration for development**:

```bash
# Database password (change this!)
DB_PASSWORD=YourStrongDatabasePassword123!

# JWT secret (generate with: openssl rand -base64 64)
JWT_SECRET=YourSuperSecretJWTKeyMinimum32CharactersLongForSecurity

# API URL (for development)
API_URL=http://localhost:8080

# PayFast sandbox credentials (for testing)
PAYFAST_MERCHANT_ID=10000100
PAYFAST_MERCHANT_KEY=46f0cd694581a
PAYFAST_PASSPHRASE=jt7NOE43FZPn
PAYFAST_USE_SANDBOX=true
```

### Step 3: Deploy Using Automated Script

```bash
# Make scripts executable
chmod +x build-docker.sh deploy-docker.sh

# Deploy to development environment
./deploy-docker.sh development
```

**Or manually with Docker Compose**:

```bash
# Build and start all services
docker compose up -d --build

# View logs
docker compose logs -f api

# Check service status
docker compose ps
```

### Step 4: Verify Deployment

```bash
# Check service health
curl http://localhost:8080/health

# Expected response:
# {"status":"Healthy"}

# View API logs
docker compose logs -f api

# View database logs
docker compose logs -f db
```

### Step 5: Access the API

- **API Base URL**: `http://localhost:8080`
- **Health Check**: `http://localhost:8080/health`
- **Swagger Documentation**: `http://localhost:8080/swagger` (if enabled in development)

### Step 6: Stop the Services

```bash
# Stop all services (keeps volumes)
docker compose down

# Stop and remove volumes (WARNING: deletes database data)
docker compose down -v
```

---

## Production Deployment

### Overview

Production deployment uses `docker-compose.prod.yml` which includes:
- Pre-built Docker images (not building from source)
- Nginx reverse proxy with SSL support
- No exposed database ports (security)
- Always restart policy
- Production environment settings

### Step 1: Server Preparation

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install Docker (see Prerequisites section)

# Create deployment directory
sudo mkdir -p /opt/hellodoctor
sudo chown $USER:$USER /opt/hellodoctor
cd /opt/hellodoctor

# Clone repository
git clone https://github.com/Comfie/hello-doctor-api.git .
git checkout claude/docker-deployment-011CUv4s1KFAsdyiPth3k3aV
```

### Step 2: Configure Environment for Production

```bash
# Copy environment template
cp .env.example .env

# Edit with production values
nano .env
```

**Production environment configuration**:

```bash
# Database Configuration
DB_PASSWORD=<STRONG_RANDOM_PASSWORD>  # Generate: openssl rand -base64 32

# JWT Configuration
JWT_SECRET=<STRONG_RANDOM_SECRET>  # Generate: openssl rand -base64 64

# API URL (your production domain)
API_URL=https://api.yourdomain.com

# PayFast Production Credentials (replace with your actual credentials)
PAYFAST_MERCHANT_ID=your_production_merchant_id
PAYFAST_MERCHANT_KEY=your_production_merchant_key
PAYFAST_PASSPHRASE=your_production_passphrase
PAYFAST_USE_SANDBOX=false

# Email Configuration (required for production)
EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
EMAIL_USERNAME=your-email@domain.com
EMAIL_PASSWORD=your-app-specific-password

# Optional: AWS S3 Storage
AWS_BUCKET_NAME=your-bucket-name
AWS_REGION=us-east-1
AWS_ACCESS_KEY_ID=your-access-key
AWS_SECRET_ACCESS_KEY=your-secret-key
USE_S3_STORAGE=true
```

### Step 3: SSL Certificate Setup

For production, you need SSL certificates. You have two options:

#### Option A: Let's Encrypt with Certbot

```bash
# Install Certbot
sudo apt install -y certbot

# Obtain certificates
sudo certbot certonly --standalone -d api.yourdomain.com

# Certificates will be in: /etc/letsencrypt/live/api.yourdomain.com/

# Create SSL directory
mkdir -p ssl

# Copy certificates (requires root)
sudo cp /etc/letsencrypt/live/api.yourdomain.com/fullchain.pem ssl/
sudo cp /etc/letsencrypt/live/api.yourdomain.com/privkey.pem ssl/
sudo chown $USER:$USER ssl/*.pem

# Set up automatic renewal
sudo crontab -e
# Add this line:
# 0 3 * * * certbot renew --quiet --deploy-hook "cp /etc/letsencrypt/live/api.yourdomain.com/*.pem /opt/hellodoctor/ssl/ && cd /opt/hellodoctor && docker compose -f docker-compose.prod.yml restart nginx"
```

#### Option B: Custom SSL Certificates

```bash
# Create SSL directory
mkdir -p ssl

# Copy your certificates
cp /path/to/your/fullchain.pem ssl/
cp /path/to/your/privkey.pem ssl/
```

### Step 4: Configure Nginx for SSL

Edit `nginx.conf` to enable HTTPS:

```bash
nano nginx.conf
```

Uncomment the HTTPS server block and comment out the HTTP redirect:

```nginx
http {
    # ... existing configuration ...

    server {
        listen 80;
        server_name _;

        # Redirect HTTP to HTTPS (uncomment in production)
        return 301 https://$server_name$request_uri;
    }

    # HTTPS server (uncomment for production)
    server {
        listen 443 ssl http2;
        server_name api.yourdomain.com;

        ssl_certificate /etc/nginx/ssl/fullchain.pem;
        ssl_certificate_key /etc/nginx/ssl/privkey.pem;

        ssl_protocols TLSv1.2 TLSv1.3;
        ssl_prefer_server_ciphers on;
        ssl_ciphers ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256;

        # Security headers
        add_header Strict-Transport-Security "max-age=31536000" always;
        add_header X-Frame-Options "SAMEORIGIN" always;
        add_header X-Content-Type-Options "nosniff" always;
        add_header X-XSS-Protection "1; mode=block" always;

        location / {
            limit_req zone=api_limit burst=20 nodelay;

            proxy_pass http://api_backend;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection keep-alive;
            proxy_set_header Host $host;
            proxy_cache_bypass $http_upgrade;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
            proxy_set_header X-Real-IP $remote_addr;

            client_max_body_size 25M;

            proxy_connect_timeout 60s;
            proxy_send_timeout 60s;
            proxy_read_timeout 60s;
        }

        location /health {
            proxy_pass http://api_backend;
            access_log off;
        }

        access_log /var/log/nginx/access.log;
        error_log /var/log/nginx/error.log;
    }
}
```

### Step 5: Build Production Image

```bash
# Build the production Docker image
./build-docker.sh v1.0.0

# Or manually:
docker build -t hellodoctor-api:v1.0.0 .
docker tag hellodoctor-api:v1.0.0 hellodoctor-api:latest
```

### Step 6: Deploy to Production

```bash
# Deploy using automated script
./deploy-docker.sh production

# Or manually:
docker compose -f docker-compose.prod.yml up -d
```

### Step 7: Run Database Migrations

```bash
# Access the API container
docker compose -f docker-compose.prod.yml exec api bash

# Run migrations (from inside container)
dotnet ef database update

# Exit container
exit
```

**Alternative: Run migrations from host**:

```bash
docker compose -f docker-compose.prod.yml exec api dotnet ef database update
```

### Step 8: Verify Production Deployment

```bash
# Check service status
docker compose -f docker-compose.prod.yml ps

# All services should show "Up" and "healthy"

# Test health endpoint
curl https://api.yourdomain.com/health

# View logs
docker compose -f docker-compose.prod.yml logs -f api

# Test API endpoint (should return 401 Unauthorized - this is correct)
curl https://api.yourdomain.com/api/v1/beneficiary
```

### Step 9: Configure Firewall

```bash
# Install UFW
sudo apt install -y ufw

# Allow SSH (important - do this first!)
sudo ufw allow 22/tcp

# Allow HTTP and HTTPS
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Enable firewall
sudo ufw enable

# Check status
sudo ufw status
```

### Step 10: Set Up Automated Backups

Create a backup script:

```bash
# Create backups directory
mkdir -p /opt/hellodoctor/backups

# Create backup script
cat > /opt/hellodoctor/backup.sh <<'EOF'
#!/bin/bash

# Configuration
BACKUP_DIR="/opt/hellodoctor/backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="hellodoctor_backup_${TIMESTAMP}.sql"
DB_CONTAINER="hellodoctor-db-prod"

# Create backup
docker exec $DB_CONTAINER pg_dump -U hellodoctor_user hello_doctor_db > "${BACKUP_DIR}/${BACKUP_FILE}"

# Compress backup
gzip "${BACKUP_DIR}/${BACKUP_FILE}"

# Delete backups older than 30 days
find "${BACKUP_DIR}" -name "*.sql.gz" -mtime +30 -delete

echo "Backup completed: ${BACKUP_FILE}.gz"
EOF

# Make executable
chmod +x /opt/hellodoctor/backup.sh

# Test backup
/opt/hellodoctor/backup.sh
```

**Schedule daily backups with cron**:

```bash
crontab -e

# Add this line (daily backup at 2 AM)
0 2 * * * /opt/hellodoctor/backup.sh >> /opt/hellodoctor/backup.log 2>&1
```

---

## Environment Configuration

### Required Environment Variables

| Variable | Description | Example | Required |
|----------|-------------|---------|----------|
| `DB_PASSWORD` | PostgreSQL password | `SecurePass123!` | Yes |
| `JWT_SECRET` | JWT signing secret | `openssl rand -base64 64` | Yes |
| `API_URL` | API base URL | `https://api.domain.com` | Yes |
| `PAYFAST_MERCHANT_ID` | PayFast merchant ID | `10000100` | Yes |
| `PAYFAST_MERCHANT_KEY` | PayFast merchant key | `46f0cd694581a` | Yes |
| `PAYFAST_PASSPHRASE` | PayFast passphrase | `jt7NOE43FZPn` | Yes |
| `PAYFAST_USE_SANDBOX` | Use PayFast sandbox | `true` or `false` | Yes |

### Optional Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `EMAIL_HOST` | SMTP host | None |
| `EMAIL_PORT` | SMTP port | `587` |
| `EMAIL_USERNAME` | SMTP username | None |
| `EMAIL_PASSWORD` | SMTP password | None |
| `AWS_BUCKET_NAME` | S3 bucket name | None |
| `AWS_REGION` | AWS region | None |
| `AWS_ACCESS_KEY_ID` | AWS access key | None |
| `AWS_SECRET_ACCESS_KEY` | AWS secret key | None |
| `USE_S3_STORAGE` | Use S3 for file storage | `false` |

### Generating Secure Secrets

```bash
# Generate JWT secret (64 bytes)
openssl rand -base64 64

# Generate database password (32 bytes)
openssl rand -base64 32

# Generate random password (alphanumeric)
openssl rand -hex 16
```

---

## Docker Commands Reference

### Build Commands

```bash
# Build image with default tag (latest)
./build-docker.sh

# Build image with specific version
./build-docker.sh v1.0.0

# Manual build
docker build -t hellodoctor-api:latest .

# Build without cache (force rebuild)
docker build --no-cache -t hellodoctor-api:latest .
```

### Deployment Commands

```bash
# Deploy to development
./deploy-docker.sh development

# Deploy to production
./deploy-docker.sh production

# Manual deployment (development)
docker compose up -d --build

# Manual deployment (production)
docker compose -f docker-compose.prod.yml up -d
```

### Service Management

```bash
# Start services
docker compose up -d

# Stop services
docker compose down

# Restart services
docker compose restart

# Restart specific service
docker compose restart api

# Stop and remove volumes (WARNING: deletes data)
docker compose down -v

# View service status
docker compose ps

# View service logs (all)
docker compose logs

# View service logs (specific service, follow)
docker compose logs -f api

# View last 100 lines of logs
docker compose logs --tail=100 api
```

### Container Access

```bash
# Access API container shell
docker compose exec api bash

# Access database container shell
docker compose exec db bash

# Access PostgreSQL CLI
docker compose exec db psql -U hellodoctor_user -d hello_doctor_db

# Run command in container without entering shell
docker compose exec api dotnet ef database update
```

### Database Operations

```bash
# Create database backup
docker compose exec db pg_dump -U hellodoctor_user hello_doctor_db > backup.sql

# Restore database from backup
cat backup.sql | docker compose exec -T db psql -U hellodoctor_user hello_doctor_db

# Access PostgreSQL shell
docker compose exec db psql -U hellodoctor_user -d hello_doctor_db

# Run SQL query
docker compose exec db psql -U hellodoctor_user -d hello_doctor_db -c "SELECT * FROM \"AspNetUsers\" LIMIT 5;"
```

### Image Management

```bash
# List all images
docker images

# Remove unused images
docker image prune

# Remove specific image
docker rmi hellodoctor-api:latest

# Tag image
docker tag hellodoctor-api:latest hellodoctor-api:v1.0.0

# Push to Docker registry (if configured)
docker push your-registry/hellodoctor-api:latest
```

### Volume Management

```bash
# List volumes
docker volume ls

# Inspect volume
docker volume inspect hellodoctor-api_postgres_data

# Remove unused volumes
docker volume prune

# Backup volume
docker run --rm -v hellodoctor-api_postgres_data:/data -v $(pwd):/backup alpine tar czf /backup/postgres_data_backup.tar.gz /data
```

### Network Inspection

```bash
# List networks
docker network ls

# Inspect network
docker network inspect hellodoctor-api_hellodoctor-network

# View container IP addresses
docker compose exec api hostname -i
docker compose exec db hostname -i
```

---

## Maintenance Operations

### Viewing Logs

```bash
# View all logs
docker compose logs

# View specific service logs
docker compose logs api
docker compose logs db
docker compose logs nginx

# Follow logs in real-time
docker compose logs -f api

# View last N lines
docker compose logs --tail=50 api

# View logs with timestamps
docker compose logs -t api

# View logs for specific time range
docker compose logs --since 2024-01-01 api
docker compose logs --since 30m api
```

### Updating the Application

```bash
# Pull latest code
git pull origin main

# Rebuild and restart services
docker compose down
docker compose up -d --build

# Or use the deployment script
./deploy-docker.sh development
```

### Database Migrations

```bash
# Create new migration (from host if .NET SDK installed)
dotnet ef migrations add MigrationName --project src/Infrastructure --startup-project src/Web

# Apply migrations
docker compose exec api dotnet ef database update

# Rollback migration
docker compose exec api dotnet ef database update PreviousMigrationName

# List migrations
docker compose exec api dotnet ef migrations list
```

### Monitoring Resource Usage

```bash
# View resource usage for all containers
docker stats

# View resource usage for specific container
docker stats hellodoctor-api-dev

# View disk usage
docker system df

# Detailed disk usage
docker system df -v
```

### Cleaning Up

```bash
# Remove stopped containers
docker container prune

# Remove unused images
docker image prune

# Remove unused volumes
docker volume prune

# Remove unused networks
docker network prune

# Remove everything unused (WARNING: aggressive cleanup)
docker system prune -a

# Remove everything including volumes (WARNING: deletes all data)
docker system prune -a --volumes
```

### Health Checks

```bash
# Check container health status
docker compose ps

# Inspect health check details
docker inspect --format='{{json .State.Health}}' hellodoctor-api-dev | jq

# View health check logs
docker inspect hellodoctor-api-dev | jq '.[0].State.Health'

# Manual health check
curl http://localhost:8080/health
```

---

## Troubleshooting

### Common Issues and Solutions

#### 1. Services Won't Start

**Symptom**: `docker compose up` fails or services exit immediately

**Diagnosis**:
```bash
# Check service logs
docker compose logs

# Check specific service
docker compose logs api

# Check if ports are in use
sudo netstat -tulpn | grep 8080
sudo netstat -tulpn | grep 5432
```

**Solutions**:
```bash
# Solution 1: Port conflict - change port in docker-compose.yml
ports:
  - "8081:8080"  # Use 8081 instead of 8080

# Solution 2: Remove old containers
docker compose down
docker compose up -d

# Solution 3: Check .env file exists and is configured
cp .env.example .env
nano .env
```

#### 2. Database Connection Failed

**Symptom**: API logs show "Failed to connect to database"

**Diagnosis**:
```bash
# Check database container status
docker compose ps db

# Check database logs
docker compose logs db

# Test database connection
docker compose exec db psql -U hellodoctor_user -d hello_doctor_db -c "SELECT 1;"
```

**Solutions**:
```bash
# Solution 1: Wait for database to be ready
# Database health check may take 10-30 seconds

# Solution 2: Check connection string in .env
# Ensure DB_PASSWORD matches in .env and docker-compose.yml

# Solution 3: Recreate database container
docker compose down db
docker compose up -d db
```

#### 3. Migrations Not Applied

**Symptom**: API logs show "Table does not exist" errors

**Solution**:
```bash
# Apply migrations manually
docker compose exec api dotnet ef database update

# If migrations don't exist, create them
# (requires .NET SDK on host)
dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/Web
```

#### 4. Permission Denied Errors

**Symptom**: "Permission denied" when accessing volumes

**Solution**:
```bash
# Fix volume permissions
docker compose down
sudo chown -R 1001:1001 ./uploads ./logs

# Or recreate volumes
docker compose down -v
docker compose up -d
```

#### 5. Out of Memory

**Symptom**: Containers crashing with OOM errors

**Diagnosis**:
```bash
# Check memory usage
docker stats

# Check system memory
free -h
```

**Solution**:
```bash
# Add memory limits to docker-compose.yml
services:
  api:
    mem_limit: 2g
    memswap_limit: 2g
  db:
    mem_limit: 1g
    memswap_limit: 1g
```

#### 6. Nginx 502 Bad Gateway

**Symptom**: Nginx returns 502 error

**Diagnosis**:
```bash
# Check if API is running
docker compose ps api

# Check API logs
docker compose logs api

# Check Nginx logs
docker compose logs nginx

# Test API directly
curl http://localhost:8080/health
```

**Solution**:
```bash
# Solution 1: Wait for API to be ready
# API health check may take 10-30 seconds

# Solution 2: Check Nginx upstream configuration
docker compose exec nginx cat /etc/nginx/nginx.conf

# Solution 3: Restart services
docker compose restart api nginx
```

#### 7. SSL Certificate Errors

**Symptom**: "SSL certificate problem" or "certificate verify failed"

**Solution**:
```bash
# Check certificate files exist
ls -la ssl/

# Check certificate expiration
openssl x509 -in ssl/fullchain.pem -noout -dates

# Renew Let's Encrypt certificate
sudo certbot renew

# Update certificates in container
sudo cp /etc/letsencrypt/live/api.yourdomain.com/*.pem ssl/
docker compose -f docker-compose.prod.yml restart nginx
```

#### 8. High Disk Usage

**Symptom**: Server running out of disk space

**Diagnosis**:
```bash
# Check Docker disk usage
docker system df

# Check volume sizes
docker volume ls
du -sh /var/lib/docker/volumes/*
```

**Solution**:
```bash
# Clean up unused resources
docker system prune -a

# Remove old images
docker image prune -a

# Clean logs
truncate -s 0 /var/lib/docker/containers/*/*-json.log

# Configure log rotation in docker-compose.yml
services:
  api:
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"
```

#### 9. PayFast Webhook Not Received

**Symptom**: Payments complete but API doesn't update

**Diagnosis**:
```bash
# Check if webhook endpoint is accessible
curl -X POST http://localhost:8080/api/v1/payment/callback/payfast

# Check API logs for webhook requests
docker compose logs api | grep callback
```

**Solution**:
```bash
# Ensure your server is publicly accessible
# PayFast sandbox requires public IP or ngrok tunnel

# For testing with ngrok:
ngrok http 80
# Update PayFast notify_url to ngrok URL
```

#### 10. Cannot Access Swagger UI

**Symptom**: Swagger UI not loading at /swagger

**Diagnosis**:
```bash
# Check if running in production mode
docker compose exec api printenv ASPNETCORE_ENVIRONMENT
```

**Solution**:
```bash
# Swagger is typically disabled in production
# For development, ensure ASPNETCORE_ENVIRONMENT=Development

# In docker-compose.yml:
environment:
  - ASPNETCORE_ENVIRONMENT=Development
```

### Debug Mode

To run containers with more verbose logging:

```bash
# Enable debug logging in appsettings.json
docker compose exec api bash
cat > appsettings.Development.json <<EOF
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Debug",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
EOF

# Restart API
docker compose restart api

# View debug logs
docker compose logs -f api
```

---

## Comparison: Docker vs Traditional VPS

### Advantages of Docker Deployment

| Feature | Docker | Traditional VPS |
|---------|--------|-----------------|
| **Consistency** | Identical environment everywhere | Environment drift possible |
| **Deployment Speed** | 2-5 minutes | 15-30 minutes |
| **Isolation** | Each service isolated | Services share OS |
| **Portability** | Run anywhere Docker runs | OS-specific setup |
| **Rollback** | Instant with image tags | Manual process |
| **Resource Usage** | Lightweight containers | Full OS overhead |
| **Scalability** | Easy horizontal scaling | Requires manual setup |
| **Updates** | Pull new image, restart | Full rebuild process |

### Disadvantages of Docker Deployment

| Aspect | Docker | Traditional VPS |
|--------|--------|-----------------|
| **Initial Complexity** | Higher learning curve | Familiar process |
| **Debugging** | Requires container knowledge | Direct access |
| **Monitoring** | Additional tools needed | Standard tools work |
| **Networking** | Container networking complexity | Direct host networking |

### When to Use Docker

**Use Docker when**:
- You need consistent environments across dev/staging/prod
- You want to deploy on multiple servers easily
- You need quick deployment and rollback
- You want service isolation
- You plan to scale horizontally
- Your team is familiar with Docker

**Use Traditional VPS when**:
- You have simple deployment needs
- Your team isn't familiar with Docker
- You need maximum performance (though Docker overhead is minimal)
- You have special networking requirements
- Compliance requires no containerization

### Performance Comparison

Docker adds minimal overhead:
- **CPU**: < 2% overhead
- **Memory**: ~50-100 MB per container
- **Network**: < 1% overhead
- **Disk I/O**: Negligible with volumes

**Real-world example**:

```
Traditional VPS:
- Startup time: 30 seconds
- Memory usage: 350 MB
- Deploy time: 15 minutes

Docker Container:
- Startup time: 5 seconds
- Memory usage: 380 MB (+30 MB)
- Deploy time: 2 minutes
```

### Cost Comparison

**Traditional VPS** (Contabo VPS S):
- 4 vCPU, 8 GB RAM, 200 GB SSD
- €5.99/month
- Can run 1-2 .NET apps comfortably

**Docker on Same VPS**:
- Same hardware
- Same cost
- Can run 3-5 containerized apps with better isolation
- **Better resource utilization**

---

## Advanced Configuration

### Multi-Stage Production Deployment

For zero-downtime deployments:

```bash
# Build new version
./build-docker.sh v1.1.0

# Update docker-compose.prod.yml to use new version
sed -i 's/hellodoctor-api:latest/hellodoctor-api:v1.1.0/' docker-compose.prod.yml

# Rolling update
docker compose -f docker-compose.prod.yml up -d --no-deps api

# Verify
curl https://api.yourdomain.com/health

# If successful, tag as latest
docker tag hellodoctor-api:v1.1.0 hellodoctor-api:latest
```

### Docker Swarm (Multiple Servers)

For high availability across multiple servers:

```bash
# Initialize swarm on manager node
docker swarm init

# Join worker nodes
docker swarm join --token <token> <manager-ip>:2377

# Convert docker-compose.yml to stack
docker stack deploy -c docker-compose.prod.yml hellodoctor

# Scale services
docker service scale hellodoctor_api=3

# View services
docker service ls
```

### Monitoring with Prometheus and Grafana

Add monitoring to `docker-compose.prod.yml`:

```yaml
services:
  prometheus:
    image: prom/prometheus
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml
      - prometheus_data:/prometheus
    ports:
      - "9090:9090"

  grafana:
    image: grafana/grafana
    volumes:
      - grafana_data:/var/lib/grafana
    ports:
      - "3000:3000"
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=admin

volumes:
  prometheus_data:
  grafana_data:
```

### Centralized Logging with ELK Stack

Add logging to `docker-compose.prod.yml`:

```yaml
services:
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.11.0
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=false
    volumes:
      - elasticsearch_data:/usr/share/elasticsearch/data

  logstash:
    image: docker.elastic.co/logstash/logstash:8.11.0
    volumes:
      - ./logstash.conf:/usr/share/logstash/pipeline/logstash.conf

  kibana:
    image: docker.elastic.co/kibana/kibana:8.11.0
    ports:
      - "5601:5601"

volumes:
  elasticsearch_data:
```

### Container Resource Limits

Prevent any single container from consuming all resources:

```yaml
services:
  api:
    deploy:
      resources:
        limits:
          cpus: '2.0'
          memory: 2G
        reservations:
          cpus: '1.0'
          memory: 1G

  db:
    deploy:
      resources:
        limits:
          cpus: '2.0'
          memory: 2G
        reservations:
          cpus: '1.0'
          memory: 1G
```

### Automated Health-Based Restarts

Docker automatically restarts unhealthy containers:

```yaml
services:
  api:
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 3s
      retries: 3
      start_period: 40s
```

### Private Docker Registry

For secure image storage:

```bash
# Start private registry
docker run -d -p 5000:5000 --restart=always --name registry registry:2

# Tag image for private registry
docker tag hellodoctor-api:latest localhost:5000/hellodoctor-api:latest

# Push to private registry
docker push localhost:5000/hellodoctor-api:latest

# Pull from private registry
docker pull localhost:5000/hellodoctor-api:latest
```

---

## Additional Resources

### Official Documentation

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET Docker Documentation](https://learn.microsoft.com/en-us/dotnet/core/docker/)
- [PostgreSQL Docker Documentation](https://hub.docker.com/_/postgres)
- [Nginx Docker Documentation](https://hub.docker.com/_/nginx)

### Helpful Commands Cheat Sheet

```bash
# Quick reference card
docker compose up -d              # Start services
docker compose down               # Stop services
docker compose logs -f api        # View logs
docker compose ps                 # Service status
docker compose restart api        # Restart service
docker compose exec api bash      # Access container
docker system prune -a            # Clean everything
docker stats                      # Monitor resources
```

### Next Steps

After successful deployment:

1. **Set up monitoring**: Implement Prometheus/Grafana or CloudWatch
2. **Configure backups**: Automate database and volume backups
3. **Set up CI/CD**: Automate builds and deployments with GitHub Actions
4. **Load testing**: Test with tools like Apache JMeter or k6
5. **Security audit**: Run security scans with Trivy or Clair
6. **Documentation**: Document your specific environment and processes

---

## Support

For issues specific to:

- **Docker setup**: Review this guide and Docker documentation
- **Hello Doctor API**: Check application logs and API documentation
- **Deployment issues**: Review `DEPLOYMENT_GUIDE.md` for traditional VPS deployment

**Getting Help**:

1. Check logs: `docker compose logs -f`
2. Review troubleshooting section
3. Consult Docker documentation
4. Check project GitHub issues

---

**Document Version**: 1.0
**Last Updated**: 2024-11-10
**Maintained By**: Hello Doctor Development Team
