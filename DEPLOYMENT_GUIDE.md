# Hello Doctor API - Production Deployment Guide

**Universal Deployment Guide for Ubuntu VPS**
**Compatible with**: Contabo, DigitalOcean, Linode, Vultr, Hetzner, AWS EC2, Azure VM, etc.

**Version**: 1.0
**Last Updated**: November 2025
**Target OS**: Ubuntu 22.04 LTS / 24.04 LTS

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Server Preparation](#server-preparation)
3. [Initial Security Setup](#initial-security-setup)
4. [Install Required Software](#install-required-software)
5. [Database Setup](#database-setup)
6. [Application Deployment](#application-deployment)
7. [Web Server Configuration](#web-server-configuration)
8. [SSL Certificate Setup](#ssl-certificate-setup)
9. [Service Configuration](#service-configuration)
10. [Database Migration](#database-migration)
11. [Testing](#testing)
12. [Monitoring & Maintenance](#monitoring--maintenance)
13. [Troubleshooting](#troubleshooting)
14. [Backup Strategy](#backup-strategy)

---

## Prerequisites

### What You Need Before Starting

- [ ] VPS/Server with Ubuntu 22.04 or 24.04 LTS
- [ ] Minimum specs: 2GB RAM, 2 CPU cores, 20GB storage
- [ ] Root or sudo access to the server
- [ ] Domain name pointing to server IP
- [ ] Local development machine with:
  - .NET SDK installed
  - SSH client
  - Your Hello Doctor API source code

### Recommended Server Specs

- **Development/Testing**: 2GB RAM, 2 cores, 25GB storage
- **Production (Small)**: 4GB RAM, 2 cores, 50GB storage
- **Production (Medium)**: 8GB RAM, 4 cores, 100GB storage
- **Production (Large)**: 16GB RAM, 6+ cores, 200GB+ storage

---

## Server Preparation

### Step 1: Initial Server Access

```bash
# SSH into your server (replace with your server IP)
ssh root@YOUR_SERVER_IP

# If using a key-based authentication
ssh -i /path/to/private-key root@YOUR_SERVER_IP
```

### Step 2: Update System Packages

```bash
# Update package lists
sudo apt update

# Upgrade installed packages
sudo apt upgrade -y

# Install essential tools
sudo apt install -y curl wget git unzip nano htop net-tools
```

### Step 3: Set Server Timezone (Optional but Recommended)

```bash
# View current timezone
timedatectl

# Set to South Africa timezone (or your timezone)
sudo timedatectl set-timezone Africa/Johannesburg

# Verify
date
```

### Step 4: Set Hostname (Optional)

```bash
# Set a meaningful hostname
sudo hostnamectl set-hostname hellodoctor-api

# Verify
hostname
```

---

## Initial Security Setup

### Step 1: Create Non-Root User

```bash
# Create new user (replace 'deploy' with your username)
sudo adduser deploy

# Add user to sudo group
sudo usermod -aG sudo deploy

# Verify sudo access
sudo -l -U deploy
```

### Step 2: Set Up SSH Key Authentication (Recommended)

**On your local machine:**
```bash
# Generate SSH key if you don't have one
ssh-keygen -t ed25519 -C "your_email@example.com"

# Copy public key to server
ssh-copy-id deploy@YOUR_SERVER_IP
```

**On the server:**
```bash
# Switch to new user
su - deploy

# Verify SSH key is added
cat ~/.ssh/authorized_keys
```

### Step 3: Disable Root SSH Login (Recommended)

```bash
# Edit SSH configuration
sudo nano /etc/ssh/sshd_config

# Find and change these lines:
PermitRootLogin no
PasswordAuthentication no
PubkeyAuthentication yes

# Save and exit (Ctrl+X, Y, Enter)

# Restart SSH service
sudo systemctl restart sshd
```

**⚠️ IMPORTANT**: Test SSH with new user before closing current session!

```bash
# In a NEW terminal window, test connection
ssh deploy@YOUR_SERVER_IP
```

### Step 4: Configure Firewall

```bash
# Allow SSH
sudo ufw allow 22/tcp

# Allow HTTP
sudo ufw allow 80/tcp

# Allow HTTPS
sudo ufw allow 443/tcp

# Enable firewall
sudo ufw enable

# Check status
sudo ufw status
```

Expected output:
```
Status: active

To                         Action      From
--                         ------      ----
22/tcp                     ALLOW       Anywhere
80/tcp                     ALLOW       Anywhere
443/tcp                    ALLOW       Anywhere
```

---

## Install Required Software

### Step 1: Install .NET Runtime

**Option A: Using Microsoft's Package Repository (Recommended)**

```bash
# Get Ubuntu version
. /etc/os-release

# Download Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/$VERSION_ID/packages-microsoft-prod.deb -O packages-microsoft-prod.deb

# Install repository
sudo dpkg -i packages-microsoft-prod.deb

# Clean up
rm packages-microsoft-prod.deb

# Update package list
sudo apt update

# Install .NET SDK (includes runtime)
sudo apt install -y dotnet-sdk-8.0

# Verify installation
dotnet --version
```

Expected output: `8.0.x` or similar

**Option B: Using Install Script**

```bash
# Download install script
wget https://dot.net/v1/dotnet-install.sh

# Make executable
chmod +x dotnet-install.sh

# Install .NET 8.0
./dotnet-install.sh --channel 8.0

# Add to PATH (add to ~/.bashrc)
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc

# Verify
dotnet --version
```

### Step 2: Install PostgreSQL

```bash
# Install PostgreSQL
sudo apt install -y postgresql postgresql-contrib

# Check PostgreSQL version
psql --version

# Enable PostgreSQL to start on boot
sudo systemctl enable postgresql

# Start PostgreSQL
sudo systemctl start postgresql

# Check status
sudo systemctl status postgresql
```

Expected output: `active (running)`

### Step 3: Install Nginx

```bash
# Install Nginx
sudo apt install -y nginx

# Enable Nginx to start on boot
sudo systemctl enable nginx

# Start Nginx
sudo systemctl start nginx

# Check status
sudo systemctl status nginx
```

### Step 4: Install Certbot (for SSL)

```bash
# Install Certbot and Nginx plugin
sudo apt install -y certbot python3-certbot-nginx

# Verify installation
certbot --version
```

---

## Database Setup

### Step 1: Access PostgreSQL

```bash
# Switch to postgres user
sudo -u postgres psql
```

### Step 2: Create Database and User

```sql
-- Create database
CREATE DATABASE hello_doctor_db;

-- Create user with password (use a STRONG password!)
CREATE USER hellodoctor_user WITH PASSWORD 'YOUR_STRONG_PASSWORD_HERE';

-- Grant all privileges on database
GRANT ALL PRIVILEGES ON DATABASE hello_doctor_db TO hellodoctor_user;

-- Grant schema privileges (PostgreSQL 15+)
\c hello_doctor_db
GRANT ALL ON SCHEMA public TO hellodoctor_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO hellodoctor_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO hellodoctor_user;

-- Exit psql
\q
```

### Step 3: Configure PostgreSQL for Local Connections

```bash
# Edit PostgreSQL config
sudo nano /etc/postgresql/*/main/pg_hba.conf

# Add this line (if not already present) for local connections
# local   all             all                                     md5

# Restart PostgreSQL
sudo systemctl restart postgresql
```

### Step 4: Test Database Connection

```bash
# Test connection
psql -h localhost -U hellodoctor_user -d hello_doctor_db

# If successful, you'll see:
# hello_doctor_db=>

# Exit
\q
```

---

## Application Deployment

### Step 1: Prepare Application on Local Machine

**On your development machine:**

```bash
# Navigate to your project
cd /path/to/hello-doctor-api

# Create production configuration
nano src/Web/appsettings.Production.json
```

**appsettings.Production.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=hello_doctor_db;Username=hellodoctor_user;Password=YOUR_STRONG_PASSWORD_HERE;Include Error Detail=false"
  },
  "AppSettings": {
    "Secret": "GENERATE_A_STRONG_SECRET_KEY_MINIMUM_32_CHARACTERS_HERE",
    "ValidAudience": "https://yourdomain.com",
    "ValidIssuer": "https://yourdomain.com",
    "WebUrl": "https://yourdomain.com"
  },
  "PayFast": {
    "MerchantId": "YOUR_PRODUCTION_MERCHANT_ID",
    "MerchantKey": "YOUR_PRODUCTION_MERCHANT_KEY",
    "Passphrase": "YOUR_PRODUCTION_PASSPHRASE",
    "PaymentUrl": "https://www.payfast.co.za/eng/process",
    "SandboxPaymentUrl": "https://sandbox.payfast.co.za/eng/process",
    "UseSandbox": false
  },
  "EmailSettings": {
    "Host": "your-smtp-host.com",
    "Port": 587,
    "Username": "your-email@domain.com",
    "Password": "your-email-password"
  },
  "FileStorage": {
    "UseS3Storage": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "yourdomain.com,www.yourdomain.com"
}
```

**Generate strong JWT secret:**
```bash
# Generate random 64-character string for JWT secret
openssl rand -base64 64
```

### Step 2: Build and Publish Application

```bash
# Navigate to Web project
cd src/Web

# Publish for production (Linux x64)
dotnet publish -c Release -r linux-x64 --self-contained false -o ./publish

# Verify publish folder
ls -la ./publish
```

### Step 3: Upload Application to Server

**Option A: Using SCP**
```bash
# From your local machine (in src/Web directory)
scp -r ./publish/* deploy@YOUR_SERVER_IP:/tmp/hellodoctor/
```

**Option B: Using rsync (Recommended)**
```bash
# From your local machine
rsync -avz --progress ./publish/ deploy@YOUR_SERVER_IP:/tmp/hellodoctor/
```

**Option C: Using Git (Alternative)**
```bash
# On server
cd /tmp
git clone https://github.com/yourusername/hello-doctor-api.git
cd hello-doctor-api/src/Web
dotnet publish -c Release -o ./publish
```

### Step 4: Move Application to Final Location

**On the server:**

```bash
# Create application directory
sudo mkdir -p /var/www/hellodoctor

# Move published files
sudo mv /tmp/hellodoctor/* /var/www/hellodoctor/

# Create logs directory
sudo mkdir -p /var/www/hellodoctor/logs

# Create uploads directory (if using local file storage)
sudo mkdir -p /var/www/hellodoctor/uploads/prescriptions

# Set ownership
sudo chown -R www-data:www-data /var/www/hellodoctor

# Set permissions
sudo chmod -R 755 /var/www/hellodoctor

# Make executable
sudo chmod +x /var/www/hellodoctor/HelloDoctorApi.Web
```

### Step 5: Test Application Manually

```bash
# Navigate to app directory
cd /var/www/hellodoctor

# Test run (as www-data user)
sudo -u www-data ASPNETCORE_ENVIRONMENT=Production ./HelloDoctorApi.Web

# You should see:
# Now listening on: http://localhost:5000
# Application started. Press Ctrl+C to shut down.

# Press Ctrl+C to stop
```

---

## Web Server Configuration

### Step 1: Create Nginx Configuration

```bash
# Create Nginx configuration file
sudo nano /etc/nginx/sites-available/hellodoctor
```

**Nginx Configuration:**
```nginx
# HTTP server - redirect to HTTPS
server {
    listen 80;
    listen [::]:80;
    server_name yourdomain.com www.yourdomain.com;

    # Redirect to HTTPS
    return 301 https://$server_name$request_uri;
}

# HTTPS server
server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name yourdomain.com www.yourdomain.com;

    # SSL certificates (will be configured by Certbot)
    ssl_certificate /etc/letsencrypt/live/yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/yourdomain.com/privkey.pem;

    # SSL configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_prefer_server_ciphers on;
    ssl_ciphers ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384;
    ssl_session_cache shared:SSL:10m;
    ssl_session_timeout 10m;

    # Security headers
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    # File upload size limit (match your API setting)
    client_max_body_size 25M;

    # Proxy settings
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Real-IP $remote_addr;

        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    # Serve static files directly (optional)
    location /uploads/ {
        alias /var/www/hellodoctor/uploads/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    # Access and error logs
    access_log /var/log/nginx/hellodoctor_access.log;
    error_log /var/log/nginx/hellodoctor_error.log;
}
```

### Step 2: Enable Nginx Configuration

```bash
# Create symbolic link to enable site
sudo ln -s /etc/nginx/sites-available/hellodoctor /etc/nginx/sites-enabled/

# Remove default site (optional)
sudo rm /etc/nginx/sites-enabled/default

# Test Nginx configuration
sudo nginx -t

# Expected output: syntax is ok, test is successful
```

### Step 3: Reload Nginx (Don't Start Yet - Need SSL First)

```bash
# Don't reload yet, SSL setup comes next
# Just verify config is valid
sudo nginx -t
```

---

## SSL Certificate Setup

### Step 1: Verify DNS Points to Your Server

```bash
# Check if domain points to your server
dig yourdomain.com +short

# Expected output: YOUR_SERVER_IP

# Also check www subdomain
dig www.yourdomain.com +short
```

**If DNS not pointing correctly:**
- Add A record: `yourdomain.com` → `YOUR_SERVER_IP`
- Add A record: `www.yourdomain.com` → `YOUR_SERVER_IP`
- Wait 5-15 minutes for DNS propagation

### Step 2: Temporarily Modify Nginx for Certbot

```bash
# Edit Nginx config to temporarily remove SSL references
sudo nano /etc/nginx/sites-available/hellodoctor

# Comment out SSL lines (add # before):
# ssl_certificate /etc/letsencrypt/...
# ssl_certificate_key /etc/letsencrypt/...

# Change HTTPS server to listen on port 80 temporarily
# listen 80;

# Test config
sudo nginx -t

# Reload Nginx
sudo systemctl reload nginx
```

### Step 3: Obtain SSL Certificate

```bash
# Get SSL certificate from Let's Encrypt
sudo certbot --nginx -d yourdomain.com -d www.yourdomain.com

# Follow the prompts:
# 1. Enter email address
# 2. Agree to Terms of Service (Y)
# 3. Share email with EFF (optional, Y or N)
# 4. Choose redirect option (2 - Redirect HTTP to HTTPS)
```

**Certbot will automatically:**
- Obtain certificate
- Update Nginx configuration
- Set up auto-renewal

### Step 4: Verify SSL Certificate

```bash
# Check certificate
sudo certbot certificates

# Test auto-renewal
sudo certbot renew --dry-run

# Expected output: Congratulations, all renewals succeeded
```

### Step 5: Restore Full Nginx Configuration

```bash
# Edit Nginx config again
sudo nano /etc/nginx/sites-available/hellodoctor

# Certbot should have added SSL lines, verify they look correct

# Test configuration
sudo nginx -t

# Reload Nginx
sudo systemctl reload nginx
```

---

## Service Configuration

### Step 1: Create Systemd Service File

```bash
# Create service file
sudo nano /etc/systemd/system/hellodoctor.service
```

**Service Configuration:**
```ini
[Unit]
Description=Hello Doctor API - ASP.NET Core Application
After=network.target postgresql.service
Wants=postgresql.service

[Service]
Type=notify
# Run as www-data user
User=www-data
Group=www-data

# Working directory
WorkingDirectory=/var/www/hellodoctor

# Start command
ExecStart=/usr/bin/dotnet /var/www/hellodoctor/HelloDoctorApi.Web.dll

# Restart behavior
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=hellodoctor-api

# Environment variables
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment=ASPNETCORE_URLS=http://localhost:5000

# Resource limits (optional)
LimitNOFILE=65536
LimitNPROC=4096

# Logging
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
```

### Step 2: Enable and Start Service

```bash
# Reload systemd to read new service file
sudo systemctl daemon-reload

# Enable service to start on boot
sudo systemctl enable hellodoctor

# Start the service
sudo systemctl start hellodoctor

# Check status
sudo systemctl status hellodoctor
```

**Expected output:**
```
● hellodoctor.service - Hello Doctor API - ASP.NET Core Application
     Loaded: loaded (/etc/systemd/system/hellodoctor.service; enabled)
     Active: active (running) since ...
```

### Step 3: Check Application Logs

```bash
# View live logs
sudo journalctl -u hellodoctor -f

# View last 50 lines
sudo journalctl -u hellodoctor -n 50

# View logs from today
sudo journalctl -u hellodoctor --since today

# View errors only
sudo journalctl -u hellodoctor -p err
```

---

## Database Migration

### Step 1: Install EF Core Tools (if not already installed)

```bash
# On the server
dotnet tool install --global dotnet-ef

# Add to PATH if needed
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.bashrc
source ~/.bashrc

# Verify installation
dotnet ef --version
```

### Step 2: Upload Migration Files

**Option A: Migrations already in published folder**
- If migrations are in your publish folder, skip to Step 3

**Option B: Copy Infrastructure project with migrations**
```bash
# On your local machine
cd src
scp -r Infrastructure/ deploy@YOUR_SERVER_IP:/tmp/hellodoctor-migration/
```

### Step 3: Run Migrations

**Method 1: Using EF Tools**
```bash
# SSH into server
ssh deploy@YOUR_SERVER_IP

# Navigate to project directory
cd /var/www/hellodoctor

# Run migrations using connection string
dotnet ef database update --connection "Host=localhost;Port=5432;Database=hello_doctor_db;Username=hellodoctor_user;Password=YOUR_PASSWORD"
```

**Method 2: Using Startup Command** (Recommended)
```bash
# Stop the application
sudo systemctl stop hellodoctor

# Run application with migrate flag (if implemented)
sudo -u www-data ASPNETCORE_ENVIRONMENT=Production dotnet /var/www/hellodoctor/HelloDoctorApi.Web.dll --migrate

# Or apply migrations programmatically by adding to Program.cs:
# await app.Services.ApplyMigrationsAsync();

# Restart application
sudo systemctl start hellodoctor
```

**Method 3: Manual SQL Script**
```bash
# Generate SQL script on local machine
cd src/Infrastructure
dotnet ef migrations script -o migration.sql

# Upload to server
scp migration.sql deploy@YOUR_SERVER_IP:/tmp/

# On server, apply manually
sudo -u postgres psql -d hello_doctor_db -f /tmp/migration.sql
```

### Step 4: Verify Database Schema

```bash
# Connect to database
sudo -u postgres psql -d hello_doctor_db

# List tables
\dt

# Expected output: Prescriptions, Payments, Beneficiaries, etc.

# Check a specific table
\d "Payments"

# Exit
\q
```

---

## Testing

### Step 1: Test Health Endpoint

```bash
# Test locally on server
curl http://localhost:5000/health

# Expected output: Healthy

# Test via domain (HTTPS)
curl https://yourdomain.com/health
```

### Step 2: Test API Endpoints

```bash
# Test authentication endpoint
curl -X POST https://yourdomain.com/api/v1/authentication/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!"}'

# Expected: 401 Unauthorized or user details if credentials correct
```

### Step 3: Test Swagger (if enabled in development)

```bash
# Open in browser
https://yourdomain.com/swagger

# Note: Disable Swagger in production for security
```

### Step 4: Test File Upload Limit

```bash
# Create a 25MB test file
dd if=/dev/zero of=test25mb.bin bs=1M count=25

# Upload test (will fail without auth, but tests file size limit)
curl -X POST https://yourdomain.com/api/v1/prescription/upload-file \
  -F "file=@test25mb.bin"
```

### Step 5: Monitor Application Logs

```bash
# Watch logs in real-time
sudo journalctl -u hellodoctor -f

# Perform API requests and watch for errors
```

---

## Monitoring & Maintenance

### Daily Health Checks

```bash
# Check service status
sudo systemctl status hellodoctor

# Check disk space
df -h

# Check memory usage
free -h

# Check CPU load
top

# Or use htop (more user-friendly)
htop
```

### Weekly Tasks

```bash
# Update system packages
sudo apt update && sudo apt upgrade -y

# Check SSL certificate expiration
sudo certbot certificates

# Review logs for errors
sudo journalctl -u hellodoctor --since "1 week ago" -p err

# Check database size
sudo -u postgres psql -d hello_doctor_db -c "SELECT pg_size_pretty(pg_database_size('hello_doctor_db'));"
```

### Monthly Tasks

```bash
# Clean old journal logs (keep last 2 weeks)
sudo journalctl --vacuum-time=2weeks

# Clean apt cache
sudo apt clean

# Review and rotate logs
sudo logrotate -f /etc/logrotate.conf

# Update .NET runtime if needed
sudo apt update && sudo apt upgrade dotnet-sdk-8.0
```

### Set Up Log Rotation

```bash
# Create logrotate config
sudo nano /etc/logrotate.d/hellodoctor
```

**Log Rotation Configuration:**
```
/var/log/nginx/hellodoctor_*.log {
    daily
    rotate 14
    compress
    delaycompress
    notifempty
    create 0640 www-data adm
    sharedscripts
    postrotate
        if [ -f /var/run/nginx.pid ]; then
            kill -USR1 `cat /var/run/nginx.pid`
        fi
    endscript
}
```

### Set Up Monitoring (Optional)

**Install netdata (System Monitor):**
```bash
# Install netdata
bash <(curl -Ss https://my-netdata.io/kickstart.sh)

# Access at: http://YOUR_SERVER_IP:19999
# Configure firewall to allow only your IP if needed
```

**Set Up Uptime Monitoring (Free):**
- Sign up at https://uptimerobot.com
- Add monitor for https://yourdomain.com/health
- Get alerts via email/SMS

---

## Troubleshooting

### Application Won't Start

**Check logs:**
```bash
sudo journalctl -u hellodoctor -n 100 --no-pager
```

**Common issues:**

1. **Port already in use:**
```bash
# Check what's using port 5000
sudo lsof -i :5000

# Kill the process if needed
sudo kill -9 <PID>
```

2. **Permission denied:**
```bash
# Fix ownership
sudo chown -R www-data:www-data /var/www/hellodoctor

# Fix permissions
sudo chmod -R 755 /var/www/hellodoctor
```

3. **Database connection failed:**
```bash
# Test PostgreSQL connection
sudo -u www-data psql -h localhost -U hellodoctor_user -d hello_doctor_db

# If fails, check PostgreSQL logs
sudo tail -f /var/log/postgresql/postgresql-*-main.log
```

4. **Missing environment variable:**
```bash
# Edit service file
sudo nano /etc/systemd/system/hellodoctor.service

# Add missing environment variables
# Reload and restart
sudo systemctl daemon-reload
sudo systemctl restart hellodoctor
```

### Nginx Errors

**502 Bad Gateway:**
```bash
# Application not running
sudo systemctl status hellodoctor
sudo systemctl start hellodoctor

# Check if listening on port 5000
sudo netstat -tlnp | grep 5000
```

**413 Request Entity Too Large:**
```bash
# Edit Nginx config
sudo nano /etc/nginx/sites-available/hellodoctor

# Increase client_max_body_size
client_max_body_size 50M;

# Reload Nginx
sudo nginx -t && sudo systemctl reload nginx
```

**SSL Certificate Issues:**
```bash
# Check certificate status
sudo certbot certificates

# Renew certificate manually
sudo certbot renew

# Check Nginx SSL config
sudo nginx -t
```

### Database Issues

**Connection pool exhausted:**
```bash
# Edit PostgreSQL config
sudo nano /etc/postgresql/*/main/postgresql.conf

# Increase max_connections
max_connections = 200

# Restart PostgreSQL
sudo systemctl restart postgresql
```

**Slow queries:**
```bash
# Connect to database
sudo -u postgres psql -d hello_doctor_db

# Check slow queries
SELECT * FROM pg_stat_activity WHERE state = 'active';

# Analyze table
ANALYZE "Payments";
```

### Performance Issues

**High memory usage:**
```bash
# Check memory
free -h

# Identify memory-hungry processes
ps aux --sort=-%mem | head -10

# Restart application
sudo systemctl restart hellodoctor
```

**High CPU usage:**
```bash
# Check CPU
top

# Identify CPU-hungry processes
ps aux --sort=-%cpu | head -10
```

---

## Backup Strategy

### Step 1: Create Backup Scripts

**Database Backup Script:**
```bash
# Create backup directory
sudo mkdir -p /root/backups/database

# Create backup script
sudo nano /root/backup-database.sh
```

**backup-database.sh:**
```bash
#!/bin/bash

# Configuration
DB_NAME="hello_doctor_db"
DB_USER="hellodoctor_user"
BACKUP_DIR="/root/backups/database"
DATE=$(date +%Y%m%d_%H%M%S)
FILENAME="${BACKUP_DIR}/hellodoctor_${DATE}.sql"
DAYS_TO_KEEP=7

# Create backup
export PGPASSWORD='YOUR_DB_PASSWORD'
pg_dump -U $DB_USER -h localhost $DB_NAME > $FILENAME

# Compress backup
gzip $FILENAME

# Delete old backups
find $BACKUP_DIR -name "*.sql.gz" -mtime +$DAYS_TO_KEEP -delete

# Log
echo "$(date): Database backup completed - ${FILENAME}.gz" >> /var/log/backup.log

# Unset password
unset PGPASSWORD
```

**Application Backup Script:**
```bash
# Create application backup script
sudo nano /root/backup-application.sh
```

**backup-application.sh:**
```bash
#!/bin/bash

# Configuration
APP_DIR="/var/www/hellodoctor"
BACKUP_DIR="/root/backups/application"
DATE=$(date +%Y%m%d_%H%M%S)
FILENAME="${BACKUP_DIR}/hellodoctor_app_${DATE}.tar.gz"
DAYS_TO_KEEP=7

# Create backup
tar -czf $FILENAME $APP_DIR

# Delete old backups
find $BACKUP_DIR -name "*.tar.gz" -mtime +$DAYS_TO_KEEP -delete

# Log
echo "$(date): Application backup completed - $FILENAME" >> /var/log/backup.log
```

### Step 2: Make Scripts Executable

```bash
# Make executable
sudo chmod +x /root/backup-database.sh
sudo chmod +x /root/backup-application.sh

# Test scripts
sudo /root/backup-database.sh
sudo /root/backup-application.sh

# Check backups created
ls -lh /root/backups/database/
ls -lh /root/backups/application/
```

### Step 3: Schedule Automated Backups

```bash
# Edit crontab
sudo crontab -e

# Add these lines:
# Database backup daily at 2 AM
0 2 * * * /root/backup-database.sh

# Application backup weekly on Sunday at 3 AM
0 3 * * 0 /root/backup-application.sh

# Save and exit
```

### Step 4: Test Restore Process

**Restore Database:**
```bash
# Stop application
sudo systemctl stop hellodoctor

# Restore from backup
cd /root/backups/database
gunzip -c hellodoctor_YYYYMMDD_HHMMSS.sql.gz | sudo -u postgres psql -d hello_doctor_db

# Start application
sudo systemctl start hellodoctor
```

**Restore Application:**
```bash
# Stop application
sudo systemctl stop hellodoctor

# Backup current application (just in case)
sudo mv /var/www/hellodoctor /var/www/hellodoctor.old

# Extract backup
cd /root/backups/application
sudo tar -xzf hellodoctor_app_YYYYMMDD_HHMMSS.tar.gz -C /

# Start application
sudo systemctl start hellodoctor
```

### Step 5: Off-site Backups (Recommended)

**Option A: rsync to another server**
```bash
# Install rsync
sudo apt install -y rsync

# Sync backups to remote server
rsync -avz /root/backups/ user@backup-server:/path/to/backups/
```

**Option B: Upload to cloud storage (AWS S3, Google Cloud, etc.)**
```bash
# Install AWS CLI
sudo apt install -y awscli

# Configure AWS credentials
aws configure

# Upload to S3
aws s3 sync /root/backups/ s3://your-bucket-name/hellodoctor-backups/
```

---

## Post-Deployment Checklist

After completing all steps, verify:

- [ ] Application is running (`systemctl status hellodoctor`)
- [ ] HTTPS is working (visit https://yourdomain.com)
- [ ] Database connection is working
- [ ] Health endpoint returns "Healthy"
- [ ] Authentication endpoints work
- [ ] File upload works (test with small file)
- [ ] PayFast integration configured (sandbox or production)
- [ ] SSL certificate auto-renewal scheduled
- [ ] Backups are scheduled and working
- [ ] Firewall is enabled and configured
- [ ] Logs are being written
- [ ] Monitoring is set up
- [ ] DNS is correctly configured

---

## Security Hardening (Optional but Recommended)

### Fail2Ban (Protect against brute force)

```bash
# Install Fail2Ban
sudo apt install -y fail2ban

# Create local config
sudo cp /etc/fail2ban/jail.conf /etc/fail2ban/jail.local

# Edit config
sudo nano /etc/fail2ban/jail.local

# Enable SSH protection (already enabled by default)
# Restart Fail2Ban
sudo systemctl restart fail2ban

# Check status
sudo fail2ban-client status
```

### Automatic Security Updates

```bash
# Install unattended-upgrades
sudo apt install -y unattended-upgrades

# Enable automatic updates
sudo dpkg-reconfigure -plow unattended-upgrades

# Edit configuration (optional)
sudo nano /etc/apt/apt.conf.d/50unattended-upgrades
```

---

## Appendix

### A. Environment Variables Reference

Common environment variables used in deployment:

- `ASPNETCORE_ENVIRONMENT` - Development, Staging, Production
- `ASPNETCORE_URLS` - URLs to listen on (e.g., http://localhost:5000)
- `DOTNET_PRINT_TELEMETRY_MESSAGE` - Disable telemetry messages
- `ConnectionStrings__DefaultConnection` - Override connection string
- `AppSettings__Secret` - Override JWT secret

### B. Useful Commands Quick Reference

```bash
# Service management
sudo systemctl status hellodoctor    # Check status
sudo systemctl start hellodoctor     # Start service
sudo systemctl stop hellodoctor      # Stop service
sudo systemctl restart hellodoctor   # Restart service
sudo systemctl enable hellodoctor    # Enable on boot
sudo systemctl disable hellodoctor   # Disable on boot

# Logs
sudo journalctl -u hellodoctor -f    # Follow logs
sudo journalctl -u hellodoctor -n 50 # Last 50 lines
sudo journalctl -u hellodoctor --since "1 hour ago"

# Nginx
sudo nginx -t                        # Test config
sudo systemctl reload nginx          # Reload config
sudo systemctl restart nginx         # Restart Nginx

# PostgreSQL
sudo systemctl status postgresql     # Check status
sudo -u postgres psql                # Access psql
sudo systemctl restart postgresql    # Restart database

# SSL
sudo certbot certificates            # List certificates
sudo certbot renew                   # Renew certificates
sudo certbot renew --dry-run         # Test renewal

# System
df -h                               # Disk space
free -h                             # Memory usage
top / htop                          # CPU/Process monitor
sudo reboot                         # Reboot server
```

### C. Troubleshooting Checklist

When things go wrong, check in this order:

1. ✅ Is the service running? (`systemctl status hellodoctor`)
2. ✅ Check application logs (`journalctl -u hellodoctor -n 50`)
3. ✅ Check Nginx logs (`tail -f /var/log/nginx/hellodoctor_error.log`)
4. ✅ Test local connection (`curl http://localhost:5000/health`)
5. ✅ Test external connection (`curl https://yourdomain.com/health`)
6. ✅ Check database connection (`psql -h localhost -U hellodoctor_user -d hello_doctor_db`)
7. ✅ Check disk space (`df -h`)
8. ✅ Check memory usage (`free -h`)
9. ✅ Check firewall (`sudo ufw status`)
10. ✅ Check DNS (`dig yourdomain.com`)

---

## Support & Additional Resources

### Documentation
- ASP.NET Core: https://docs.microsoft.com/aspnet/core/
- PostgreSQL: https://www.postgresql.org/docs/
- Nginx: https://nginx.org/en/docs/
- Let's Encrypt: https://letsencrypt.org/docs/
- Systemd: https://www.freedesktop.org/software/systemd/man/

### Community
- ASP.NET Core GitHub: https://github.com/dotnet/aspnetcore
- Stack Overflow: https://stackoverflow.com/questions/tagged/asp.net-core

---

**Deployment Guide Version**: 1.0
**Last Updated**: November 2025
**Tested On**: Ubuntu 22.04 LTS, Ubuntu 24.04 LTS
**Compatible VPS Providers**: Contabo, DigitalOcean, Linode, Vultr, Hetzner, AWS EC2, Azure VM, Google Cloud

---

## Conclusion

You now have a complete, production-ready deployment of the Hello Doctor API. This guide works on any Ubuntu-based VPS provider.

**Next Steps:**
1. Deploy your Flutter mobile app
2. Test end-to-end workflows
3. Monitor performance and optimize
4. Set up monitoring alerts
5. Create disaster recovery plan

**Remember:**
- Keep backups current
- Monitor logs regularly
- Update security patches
- Review access logs
- Test restore procedures

Good luck with your deployment! 🚀
