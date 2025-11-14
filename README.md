## 🚧 Project Status
This project is currently **in development**. Expect updates, new features, and occasional bugs as things evolve!

## 👁️👁️ Sneak peek

<img width="2540" height="1396" alt="Screenshot (309)" src="https://github.com/user-attachments/assets/990e84a5-64b0-4922-be78-1a22b6d46dcf" />
<img width="2533" height="1377" alt="Screenshot (308)" src="https://github.com/user-attachments/assets/39380736-f561-44ec-bcd3-b3fa67cb0dc3" />
<img width="2540" height="1383" alt="Screenshot (310)" src="https://github.com/user-attachments/assets/a91d7026-dae6-4092-9842-2bf68bb96787" />

## 🧭 Overview
**OpenF1 Dashboard** is an interactive web application built with **Blazor**, designed to help users explore detailed F1 historical data races.
The dashboard provides a modern interface that includes an informational popup introducing the tool and its features.

## 🙏 Acknowledgments
This project wouldn’t be possible without the work of:

- [OpenF1 API](https://openf1.org/) — for open access to F1 historical data
- [GeoJSON](https://github.com/bacinger/f1-circuits) —  F1 circuits in GeoJSON format.

This project uses publicly available data provided by the OpenF1 API. Please refer to their license/terms for details.

## ⚠️ Disclaimer
This project is **not affiliated with**, **endorsed by**, or **sponsored by**  
**Formula 1**, **FIA**, **FOM**, or any associated entities.  
All Formula 1 names, logos, and trademarks are the property of their respective owners.  
**OpenF1 Dashboard** is a **non-commercial**, **educational**, and **experimental** project made for learning and open data visualization purposes only.

## 📜 License
MIT License
Copyright (c) 2025 [Marco Conti]

## 🚀 Roadmap
- [ ] Visualize telemetry data on map and sectors
- [ ] Add new driver comparision feature
- [ ] Logging system
- [ ] Add visual rapresentation for sprint/quali and practise data
- [ ] Improve performance for large datasets  
- [ ] Enhance visual themes and layout  
- [ ] More

## 🧩 Contributing
Contributions, ideas, and feedback are welcome!  

## 🌍 Contact / Personal website
If you'd like to collaborate or share suggestions:  
**[marcoconti.1991@gmail.com]**  
**[https://www.marcoconti1991.com/]**

## Run this locally / Prerequisites
To run the OpenF1Dashboard locally, ensure you have the following installed:

    .NET 8.0 SDK
    The project is built with .NET 8.0, so you’ll need the .NET SDK installed on your machine.
    Download .NET SDK

    Node.js (for Blazor WebAssembly Dev Server)
    The Blazor WebAssembly application requires Node.js for the local development server.
    Download Node.js

    Visual Studio or Visual Studio Code
    We recommend using Visual Studio (with the "ASP.NET and web development" workload) or Visual Studio Code for development.
    Download Visual Studio

## Getting Started

First, clone the repository to your local machine:

    git clone https://github.com/yourusername/OpenF1Dashboard.git
    cd OpenF1Dashboard

Restore NuGet dependencies. Run the following command to restore all required dependencies:

    dotnet restore

Build the project after restoring the dependencies

    dotnet build

Run the application. To start the application locally, run the following command:

    dotnet run
