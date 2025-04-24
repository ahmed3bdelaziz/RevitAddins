# Revit Add-ins

A collection of Autodesk Revit add-ins developed in C# to automate BIM tasks and enhance productivity. These tools are designed to streamline workflows, reduce repetitive tasks, and improve efficiency for Revit users.

## Add-ins Overview

### 1. **QSIT Element Info**
Displays detailed information about a selected Revit element, including:
- **Element ID**
- **Name**
- **Type**
- **Family**
- **Category**

This tool is ideal for quickly retrieving and understanding element properties.

### 2. **QSIT HelloWorld**
A simple yet functional example add-in that displays a "Hello World" message dialog in Revit. This is a great starting point for understanding how Revit add-ins work.

### 3. **QSIT Point Selection**
Allows users to pick specific points in a Revit model, including:
- **Midpoint**
- **Endpoint**
- **Nearest Point**

The coordinates of the selected points are displayed, enabling precise point-based workflows.

### 4. **QSIT Select by Filter**
Selects and counts wall elements within a user-defined rectangular area using a custom filter. This tool is essential for quick and efficient element selection and quantification.

### 5. **QSIT Delete Selected Elements**

🧹 **Delete Selected Element**
This feature allows users to select element within the Revit model and delete it. Upon selection, the add-in presents a confirmation dialog listing the selected element, enabling users to review their choice before proceeding with the deletion.

🪟 **Delete Windows by Rectangle**
This functionality enables users to select a rectangular area within the Revit model. The add-in then identifies and deletes only the window elements within that specified area, leaving other elements intact. This selective deletion is particularly useful for efficiently managing window elements without affecting the entire model.

## Features

- **Automation**: Simplifies complex and repetitive tasks by automating BIM workflows.
- **Clean Architecture**: Designed using Revit API and .NET best practices for scalability and maintainability.
- **Productivity Enhancement**: Speeds up various Revit operations, from element selection to property analysis.

## Installation

1. Clone this repository:
   ```bash
   git clone https://github.com/ahmed3bdelaziz/RevitAddins.git
   ```
2. Open the solution in Visual Studio.
3. Build the project to generate the `.dll` files.
4. Place the generated `.dll` files in the Revit `Add-ins` folder:
   - Typically located at: `%AppData%\Autodesk\Revit\Addins\<RevitVersion>`

## Usage

1. Open Autodesk Revit.
2. Access the installed add-ins from the **Add-Ins** tab in the Revit ribbon.
3. Use the respective tools:
   - **QSIT Element Info**: Select an element to view its details.
   - **QSIT HelloWorld**: Click to display a "Hello World" message.
   - **QSIT Point Selection**: Pick points in the model and view their coordinates.
   - **QSIT Select by Filter**: Define a rectangular area to filter and count wall elements.
   - **QSIT Delete Selected Elements**: Select elements to delete with confirmation.
   - **QSIT Delete Windows by Rectangle**: Select a rectangular area to delete only windows.

## Contributing

Contributions are welcome! To contribute:

1. Fork the repository.
2. Create a new branch for your feature/bug fix:
   ```bash
   git checkout -b feature-name
   ```
3. Commit your changes and push the branch:
   ```bash
   git commit -m "Add feature description"
   git push origin feature-name
   ```
4. Open a pull request describing your changes.

## Contact

For any questions or suggestions, feel free to reach out:

- **Author**: Ahmed Abdelaziz
- **GitHub**: [ahmed3bdelaziz](https://github.com/ahmed3bdelaziz)

