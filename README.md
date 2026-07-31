# StockMarket Live

StockMarket Live is a modern, real-time web application designed to consume and display AI-analyzed stock market signals. 

## 🏗️ Architecture & Ecosystem

This project is part of a distributed, event-driven ecosystem. It operates in tandem with a separate **private repository** named `StockMarket`.

*   **`StockMarket` (Private Repository) - The Publisher:** 
    A closed-source, autonomous algorithmic trading and financial analysis engine. It integrates directly with the **Alpaca Trading API** to automatically analyze and trade American **NASDAQ** stocks. 
    
    The engine employs a variety of advanced quantitative and technical trading strategies, such as:
    *   **AI-Driven Predictive Modeling**
    *   **Mean Reversion & Momentum**
    *   **Moving Average Convergence Divergence (MACD) Crossovers**
    
    Based on these strategies, it makes autonomous Buy/Sell/Hold decisions and publishes these real-time signals (`StockPriceAnalyzedEvent`) to a RabbitMQ message broker.
*   **`StockMarketLive` (This Repository) - The Consumer:**
    A real-time, user-facing web dashboard. It subscribes to the RabbitMQ exchanges via MassTransit, consumes the published AI signals, and broadcasts them securely to connected web clients via SignalR (WebSockets).

## 🚀 Technology Stack

**Backend:**
*   **.NET 10 (C# 14)**
*   **Clean Architecture** (Domain, Application, Infrastructure, Api)
*   **MassTransit & RabbitMQ** (Pub/Sub Fanout Exchange via CloudAMQP)
*   **SignalR** (Real-time WebSockets)
*   **JWT Authentication** (Secure Data Streaming)

**Frontend:**
*   **React + Vite + TypeScript**
*   **i18n** (Zero-Hardcode Multilingual Support - EN/TR)
*   **Vanilla CSS** (Premium Dark Mode with Micro-Animations)

## 🛡️ Zero-Hardcode & Security Policies

This project strictly adheres to enterprise-level security and quality standards:
*   **No Secrets in Code:** Sensitive credentials (like CloudAMQP Connection Strings and JWT Keys) are NEVER hardcoded. They are managed exclusively via Environment Variables or `.NET User Secrets`.
*   **Zero Warnings Policy:** The `.NET` projects are configured with `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`. The project will not build if a single warning exists.
*   **Result Pattern:** Uses the `Result<T>` wrapper instead of throwing exceptions for predictable and safe control flow.
*   **Clean Code:** Strict adherence to keeping cognitive complexity low and enforcing the Single Responsibility Principle (SRP).

## ⚙️ Getting Started

### Prerequisites
*   .NET 10 SDK
*   Node.js & npm
*   A CloudAMQP (RabbitMQ) instance

### Configuration
1. Initialize `.NET user-secrets` in the API project to securely store your CloudAMQP credentials and JWT tokens:
   ```bash
   cd backend/StockMarketLive.Api
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:RabbitMq" "amqps://[USER]:[PASSWORD]@[SERVER].rmq.cloudamqp.com/[VHOST]"
   dotnet user-secrets set "Jwt:Key" "[YOUR_SUPER_SECRET_JWT_KEY]"
   dotnet user-secrets set "Jwt:Issuer" "StockMarketLive"
   dotnet user-secrets set "Jwt:Audience" "StockMarketLiveUsers"
   ```

### Running the Backend
```bash
cd backend/StockMarketLive.Api
dotnet run
```

### Running the Frontend
```bash
cd frontend
npm install
npm run dev
```