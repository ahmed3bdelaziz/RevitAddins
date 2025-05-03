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
This feature allows users to select an element within the Revit model and delete it. Upon selection, the add-in presents a confirmation dialog listing the selected element, enabling users to review their choice before proceeding with the deletion.

🪟 **Delete Windows by Rectangle**  
This functionality enables users to select a rectangular area within the Revit model. The add-in then identifies and deletes only the window elements within that specified area, leaving other elements intact. This selective deletion is particularly useful for efficiently managing window elements without affecting the entire model.

### 6. **QSIT Column Automator**
A powerful set of tools for automating column-related tasks in Revit. The add-in includes the following commands:

1. **QsitColumnPlacer**  
   - **Purpose**: Places random columns on a specific level in the Revit model.  
   - **Description**: This command automates the placement of columns at random positions on a user-defined level, saving time and ensuring quick placement of multiple columns.

2. **QsitColumnAutomator**  
   - **Purpose**: Assigns random or custom comments to selected columns in the Revit model.  
   - **Description**: This command allows users to select columns and assign either random comments or custom input to them, helping with better organization and project management.

3. **QsitColumnPreview**  
   - **Purpose**: Previews all structural columns in a TaskDialog with details and comments.  
   - **Description**: This command displays a TaskDialog that shows all structural columns in the project, along with their main details and any comments assigned. It provides a quick overview of column data for the user.

## Features

- **Automation**: Simplifies complex and repetitive tasks by automating BIM workflows.
- **Clean Architecture**: Designed using Revit API and .NET best practices for scalability and maintainability.
- **Productivity Enhancement**: Speeds up various Revit operations, from element selection to property analysis.

## Installation

1. Clone this repository:
   ```bash
   git clone https://github.com/ahmed3bdelaziz/RevitAddins.git


2. Open the solution in Visual Studio.
3. Build the project to generate the `.dll` files.
4. Place the generated `.dll` files in the Revit `Add-ins` folder:

   * Typically located at: `%AppData%\Autodesk\Revit\Addins\<RevitVersion>`

## Usage

1. Open Autodesk Revit.
2. Access the installed add-ins from the **Add-Ins** tab in the Revit ribbon.
3. Use the respective tools:

   * **QSIT Element Info**: Select an element to view its details.
   * **QSIT HelloWorld**: Click to display a "Hello World" message.
   * **QSIT Point Selection**: Pick points in the model and view their coordinates.
   * **QSIT Select by Filter**: Define a rectangular area to filter and count wall elements.
   * **QSIT Delete Selected Elements**: Select elements to delete with confirmation.
   * **QSIT Delete Windows by Rectangle**: Select a rectangular area to delete only windows.
   * **QSIT Column Automator**:

     * **QsitColumnPlacer**: Place random columns on a specific level.
     * **QsitColumnAutomator**: Assign comments to selected columns.
     * **QsitColumnPreview**: Preview and display details of all structural columns.

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

* **Author**: Ahmed Abdelaziz
* **LinkedIn**: [Ahmed Abd Elaziz](https://www.linkedin.com/in/ahmed3bdelaziz/)



