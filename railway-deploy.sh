#!/bin/bash

echo "🚂 Deploying E-Commerce Platform to Railway"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Check if Railway CLI is installed
if ! command -v railway &> /dev/null; then
    echo -e "${RED}❌ Railway CLI not found. Installing...${NC}"
    npm install -g @railway/cli
fi

# Login check
echo -e "${BLUE}🔐 Checking Railway authentication...${NC}"
if ! railway whoami &> /dev/null; then
    echo -e "${YELLOW}⚠️  Please login to Railway first${NC}"
    railway login
fi

# Create main project
echo -e "${BLUE}📦 Creating Railway project...${NC}"
railway init ecommerce-platform

# Deploy databases first
echo -e "${BLUE}📊 Setting up databases...${NC}"
echo -e "${YELLOW}Setting up PostgreSQL...${NC}"
railway add postgresql

echo -e "${YELLOW}Setting up Redis...${NC}"
railway add redis

# Set global environment variables
echo -e "${BLUE}🔧 Setting environment variables...${NC}"
railway variables set JWT_SECRET_KEY=$(openssl rand -base64 32)
railway variables set ASPNETCORE_ENVIRONMENT=Production

# Deploy backend services
echo -e "${BLUE}🔧 Deploying backend services...${NC}"

# API Gateway
echo -e "${YELLOW}Deploying API Gateway...${NC}"
cd backend/src/ApiGateway
railway up --detach
cd ../../..

# User Service
echo -e "${YELLOW}Deploying User Service...${NC}"
cd backend/src/Services/User
railway up --detach
cd ../../../..

# Product Service
echo -e "${YELLOW}Deploying Product Service...${NC}"
cd backend/src/Services/Product
railway up --detach
cd ../../../..

# Order Service
echo -e "${YELLOW}Deploying Order Service...${NC}"
cd backend/src/Services/Order
railway up --detach
cd ../../../..

# Payment Service
echo -e "${YELLOW}Deploying Payment Service...${NC}"
cd backend/src/Services/Payment
railway up --detach
cd ../../../..

# Notification Service
echo -e "${YELLOW}Deploying Notification Service...${NC}"
cd backend/src/Services/Notification
railway up --detach
cd ../../../..

# File Service
echo -e "${YELLOW}Deploying File Service...${NC}"
cd backend/src/Services/File
railway up --detach
cd ../../../..

# Deploy frontend last
echo -e "${BLUE}🎨 Deploying frontend...${NC}"
cd frontend
railway up

echo -e "${GREEN}✅ Deployment complete!${NC}"
echo -e "${BLUE}🌐 Check your services at: https://railway.app/dashboard${NC}"

# Show deployment URLs
echo -e "${BLUE}📋 Deployment Summary:${NC}"
echo -e "${GREEN}Frontend:${NC} Check Railway dashboard for URL"
echo -e "${GREEN}API Gateway:${NC} Check Railway dashboard for URL"
echo -e "${YELLOW}Note: Configure environment variables for service URLs after deployment${NC}"
