#!/bin/bash

# Deployment script for Hello Doctor API using Docker Compose
# Usage: ./deploy-docker.sh [development|production]

set -e  # Exit on error

# Get environment from command line or default to development
ENVIRONMENT=${1:-development}

echo "========================================="
echo "Deploying Hello Doctor API"
echo "Environment: ${ENVIRONMENT}"
echo "========================================="

# Check if .env file exists
if [ ! -f .env ]; then
    echo ""
    echo "Error: .env file not found!"
    echo "Please create .env file from .env.example:"
    echo "  cp .env.example .env"
    echo "  nano .env  # Edit with your values"
    echo ""
    exit 1
fi

# Deploy based on environment
if [ "$ENVIRONMENT" = "production" ]; then
    echo ""
    echo "Deploying to PRODUCTION..."
    echo ""
    
    # Build the production image
    echo "Building production image..."
    docker build -t hellodoctor-api:latest .
    
    # Stop existing containers
    echo "Stopping existing containers..."
    docker-compose -f docker-compose.prod.yml down
    
    # Start production containers
    echo "Starting production containers..."
    docker-compose -f docker-compose.prod.yml up -d
    
elif [ "$ENVIRONMENT" = "development" ]; then
    echo ""
    echo "Deploying to DEVELOPMENT..."
    echo ""
    
    # Build and start development containers
    docker-compose up -d --build
    
else
    echo ""
    echo "Error: Invalid environment '${ENVIRONMENT}'"
    echo "Usage: ./deploy-docker.sh [development|production]"
    echo ""
    exit 1
fi

# Wait for services to be healthy
echo ""
echo "Waiting for services to be healthy..."
sleep 10

# Check service status
echo ""
echo "Service Status:"
echo "----------------------------------------"
docker-compose ps

echo ""
echo "========================================="
echo "Deployment completed!"
echo "========================================="
echo ""

if [ "$ENVIRONMENT" = "production" ]; then
    echo "API URL: https://yourdomain.com"
    echo "Health Check: https://yourdomain.com/health"
else
    echo "API URL: http://localhost:8080"
    echo "Health Check: http://localhost:8080/health"
    echo "Database: localhost:5432"
fi

echo ""
echo "To view logs:"
echo "  docker-compose logs -f api"
echo ""
echo "To stop:"
echo "  docker-compose down"
echo ""
