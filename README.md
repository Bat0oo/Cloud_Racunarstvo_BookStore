# Cloud_Racunarstvo_BookStore
A cloud-based bookstore simulation with integrated banking service, built using Service Fabric microservices architecture.

## 🔍 Overview
This project simulates a complete online bookstore with bank service for handling account deposits (basic). Built with .NET Service Fabric for microservices and React for the frontend, it demonstrates cloud computing principles through a distributed architecture. Users can deposit money into their accounts through the banking service, then purchase books from the bookstore if they have sufficient balance. The system handles book catalog management, user authentication, shopping functionality, and financial transactions across separate microservices.

## 🧰 Project Structure
Backend Microservices — .NET Service Fabric services for bookstore and banking
Bookstore Service — Book catalog, cart, and order management
Banking Service — Separate microservice for account deposits and payment processing
Frontend Application — React-based web interface
User Authentication — Login system with account management
Cloud Database Integration — Distributed data storage
Service Communication — Inter-service communication patterns

## 🚀 Getting Started
1. Clone the repository
```bash
git clone https://github.com/Bat0oo/Cloud_Racunarstvo_BookStore.git
cd Cloud_Racunarstvo_BookStore
```
2. Set up Service Fabric development environment
3. Configure database connection strings
4. Install backend dependencies
5. Install frontend dependencies (React)
6. Deploy Service Fabric services or run locally
7. Start the React frontend

## ✨ Key Features
Complete book catalog with search and browsing
User registration and login authentication
Separate banking service for account deposits
Deposit money to user account via banking microservice
Purchase books with account balance
Automatic balance deduction upon book purchase
Shopping cart functionality
Order management system
Service Fabric microservices architecture
Cloud-native distributed system design
Inter-service communication between bookstore and banking
Scalable and resilient architecture

## 🧩 Tech Stack
Backend: .NET Core, C#, Service Fabric
Frontend: React
Microservices: Service Fabric for orchestration
Architecture: Microservices with separate bookstore and banking services

## 📂 Use-Cases
Learning microservices architecture with Service Fabric
Understanding distributed systems and cloud computing
Building e-commerce platforms with separate payment services
Implementing service-to-service communication
Cloud-native application development
Online bookstore and retail platforms
Financial service integration patterns
Account-based transaction systems
