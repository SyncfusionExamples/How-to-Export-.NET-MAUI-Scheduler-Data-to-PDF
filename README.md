# Creating a Professional Exam Scheduler with PDF Export Using .NET MAUI Scheduler
## Overview
This project demonstrates how to build a program‑wise exam scheduling application using the Syncfusion® .NET MAUI Scheduler control and Syncfusion® PDF libraries.
It showcases automated generation of UG, PG, and MBA exam schedules, timeline‑based visualization, and one‑tap export of a complete exam report to PDF, including timetables, invigilator duties, and hall availability.

The sample is designed to reflect real academic scheduling constraints and presents a clean, production‑ready export experience across Android, iOS, Windows, and Mac Catalyst.

## Features

* Program‑specific schedules for UG, PG, and MBA
* Timeline Week view using Syncfusion® .NET MAUI Scheduler
* Exam sessions mapped to Forenoon (FN) and Afternoon (AN)
* Resource‑based scheduler grouping by exam halls
* Color‑coded appointments per hall
* Deterministic exam generation with academic constraints
* Export to PDF with:

    * Cover page
    * Date‑wise exam timetable
    * Invigilator duty roster
    * Exam hall availability summary


* Preview and Print support using Syncfusion® .NET MAUI PDF Viewer
* Cross‑platform PDF saving and viewing
* Timestamped, program‑aware PDF filenames


## Technologies Used

* .NET MAUI
* Syncfusion® .NET MAUI Scheduler control
* Syncfusion® PDF library
* Syncfusion® .NET MAUI PDF Viewer
* XAML for UI layout
* C# for application logic and data modeling


## How It Works

1. ProgramType enum defines the supported academic programs (UG, PG, MBA).
2. SampleDataPage generates deterministic data for:

    * Courses
    * Exam halls (capacity, color, identifier)
    * Invigilators


3. Program‑specific exam appointments are created using predefined academic rules:

    * No Sunday exams
    * Two FN and two AN slots per working day
    * Semester‑pairing logic per program
    * Hall rotation to distribute load
    * Minimal repetition of invigilators within the same day


4. ScheduleViewModel loads:

* Appointments
* Halls (as scheduler resources)
* Invigilators and exposes them to the UI.

5. The Syncfusion® .NET MAUI Scheduler displays appointments in a Timeline Week view, grouped by exam halls.
6. The Export PDF button invokes the PDF export pipeline:

* Builds the PDF off the UI thread
* Uses Syncfusion.Pdf to create a multi‑page document

7. The generated PDF contains:

    * A cover page with program name and timestamp
    * A date‑wise exam timetable
    * An invigilator duty roster
    * Exam hall availability (Allocated / Available)

8. The Preview / Print button opens the PDF using Syncfusion® .NET MAUI PDF Viewer, allowing printing or app‑level review.
9. SaveService abstracts platform‑specific file saving and viewing logic.


## PDF Export Architecture

* The PDF export is custom‑built using Syncfusion.Pdf
* Scheduler UI is not exported directly
* Table content is generated from the same data models used by the Scheduler
* PdfGrid is used for tabular layout with automatic pagination
* Each section is rendered on its own page with consistent styling
* Page navigation and printing are supported through the PDF Viewer


## Cross‑Platform Save and View
The project uses a partial SaveService pattern:

### Android

* Saves PDF to cache directory
* Opens using an intent with FileProvider


### iOS

* Previews PDF using UIDocumentInteractionController


### Windows

* Uses FileSavePicker
* Launches saved PDF using default application


### Mac Catalyst

* Opens native Save As dialog using UIDocumentPickerViewController
* Supports exporting outside the app sandbox


This ensures a consistent export experience across platforms without duplicating logic in UI code.

### Notes

* The exam data used in this sample is deterministically generated, not database‑backed.
* The PDF export is layout‑driven, not a screenshot or visual export of the Scheduler UI.
* You can extend the sample to:

    * Filter date ranges before export
    * Add institutional branding to the cover page
    * Include department‑wise or semester‑wise grouping


* Review Syncfusion® documentation for advanced Scheduler customization and PDF styling options.

