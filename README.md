# casino-app

Full-stack casino application built as a seminar project.
The application includes a web-based frontend, a backend API,
and an external game service.

## Project Structure

```casino-app/
├── frontend/   - Web client (React + Vite)
├── backend/    - ASP.NET Core backend and external game service (ASP.NET Core + Unity)
└── docs/       - Screenshots
```

## Frontend

The frontend provides the user interface for the casino platform.

Features include:
- Landing page with the main game
- Player authentication (login and registration)
- Account overview
- Transactions, including:
    - Deposit
    - Withdraw
    - Transaction history
- Profile management, including:
  - Personal details
  - Security settings
  - Player protection
  - Identity verification
- Responsive design with mobile support
- Language selection across the application

> Note: The frontend is player-oriented. Administrative functionality exists in the backend but does not have a corresponding frontend interface.


## Backend

The backend is built with ASP.NET Core and uses SQL Server as the database.

It includes:
- Main casino backend API
- External High-Low game API
- JWT-based authentication
- Database persistence using Entity Framework Core
- Azure Blob Storage for document storage (KYC)

> Note: The external High-Low game is provided as a prebuilt Unity WebGL application and is served from the backend. Unity is only required if the game needs to be rebuilt.


## Screenshots

Selected screenshots can be found in:
docs/screenshots/


## Running the Project

### Prerequisites

- Node.js
- .NET SDK
- SQL Server


### Backend

1. Open the backend solution in a .NET development environment (e.g. Visual Studio).

2. Configure the backend by creating an `appsettings.json` file in each backend project
    (`CasinoAppBackend` and `HighLowGameApi`), using the corresponding
    `appsettings.example.json` files as references.

3. Ensure both backend projects (`CasinoAppBackend` and `HighLowGameApi`) are running
   by setting each one as the startup project when needed.


### Frontend

1. Open the frontend folder in a Node.js development environment (e.g. WebStorm).

2. Create a `.env` file in the frontend root, using the corresponding
    `.env.example` file as reference.

3. Install dependencies and start the development server:
    ```bash
    npm install
    npm run dev

> Note: Local URLs and ports for the frontend and backend are predefined in the project configuration.
