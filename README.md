# Creating a Professional Shift Schedule with PDF Export Using .NET MAUI Scheduler 
## Overview 
This project demonstrates how to build a weekly shift scheduler using the Syncfusion® .NET MAUI Scheduler control and Syncfusion® PDF library. It shows color-coded tasks by category, database-backed scheduling, and one-tap PDF export and sharing of the current week’s appointments.

## Features 
* Weekly scheduler view for shifts 
* Color-coded categories with dimmed completed tasks 
* Toolbar action to export current week to PDF Legend and grouped-by-day details in PDF 
* Dependency Injection for repositories and services 
* SQLite-backed data storage Share exported PDF via native share sheet
* Timestamped filenames

## Technologies Used 
* .NET MAUI 
* Syncfusion® .NET MAUI Scheduler control 
* Syncfusion® PDF library XAML for UI layout 
* C# for logic, DI, and data binding SQLite for local persistence

## How It Works 
1. A ProjectTask model represents shift items with start/end times, completion, and category.
2. A CustomSchedulerAppointment extends SchedulerAppointment for category tagging and styling. 
3. Repositories (TaskRepository, CategoryRepository) load and persist data in SQLite. 
4. The Scheduler (Week view) displays tasks; completed tasks are visually dimmed. 
5. A toolbar button triggers ExportPdfAsync, which generates a PDF using Syncfusion.Pdf. 
6. Appointments are grouped by day, formatted with time ranges, and written to the PDF. 
7. The generated PDF is saved to a local path and shared using the platform share dialog. 
8. A color mapping system assigns hex colors per category with sensible defaults.

## Notes 
* The sample export is custom-built using Syncfusion.Pdf; it does not export the Scheduler UI directly. 
* Adjust file paths and storage permissions as needed per platform. 
* You can filter visible date ranges or views before export. 
* Extend with recurrence expansion, additional filters (e.g., hide completed), or calendar integration. 
* Review Syncfusion® documentation for advanced Scheduler and PDF customization options.