#!/bin/bash

# Build script for Hello Doctor API Docker image
# This script builds the Docker image and tags it appropriately

set -e  # Exit on error

echo "========================================="
echo "Building Hello Doctor API Docker Image"
echo "========================================="

# Get version from command line or use 'latest'
VERSION=${1:-latest}

# Build the image
echo ""
echo "Building image: hellodoctor-api:${VERSION}"
echo ""

docker build -t hellodoctor-api:${VERSION} .

# Also tag as latest if a specific version was provided
if [ "$VERSION" != "latest" ]; then
    echo ""
    echo "Tagging as latest..."
    docker tag hellodoctor-api:${VERSION} hellodoctor-api:latest
fi

echo ""
echo "========================================="
echo "Build completed successfully!"
echo "========================================="
echo ""
echo "Image: hellodoctor-api:${VERSION}"
echo ""
echo "To run the image:"
echo "  docker-compose up -d"
echo ""
echo "To push to registry:"
echo "  docker tag hellodoctor-api:${VERSION} your-registry/hellodoctor-api:${VERSION}"
echo "  docker push your-registry/hellodoctor-api:${VERSION}"
echo ""
