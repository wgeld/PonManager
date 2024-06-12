##PON Manager
Welcome to the PON Manager repository! This application is designed to manage PON (Passive Optical Network) operations, featuring PON search and PON Light Up functionalities.

#Table of Contents
Introduction
Features
Getting Started
Usage
Contributing
License

#Introduction
The PON Manager application is a tool designed to facilitate the management and operation of PONs. This repository will house the codebase for the application, and new features will be developed and tracked in separate branches. Currently, the application supports two primary functions:

PON Search: Search and retrieve details about specific PONs.
PON Light Up: Activate or light up a PON.

We anticipate expanding the feature set of the PON Manager in the future to better serve the needs of its users.

#Features
PON Search: Allows users to search for PONs and get detailed information.
PON Light Up: Facilitates the activation of PONs with a simple interface.

#Getting Started
To get a copy of the PON Manager application up and running on your local machine, follow these steps:

# Prerequisites
ASP.NET Core SDK (version 8.0)
Git

#Installation
#Clone the repository:
git clone https://github.com/your-username/pon-manager.git
#Navigate to the project directory:
cd pon-manager
#Restore dependencies and build the project:
dotnet restore
dotnet build
#Usage
To run the application locally:
dotnet run

Once the application is running, you can access it via http://localhost:5000 (or a different port if configured).

#Branching Strategy
New features and bug fixes should be developed in separate branches. Follow these steps to create a new branch:

#Checkout to the main branch and pull the latest changes:
git checkout main
git pull origin main
#Create a new branch for your feature:
git checkout -b feature/your-feature-name
