# UX Checkmate: Scope

## Core Features and Needs

### 1. User Authentication and Accounts

#### Account Login:
- Users must log in to access the checker functionality.
- Support for guest users with limited functionality.
- Password recovery options:
  - **Primary Methods**: Email recovery (using MailKit).
  - **Optional Methods**: SMS recovery or security questions.

#### Account Management:
- Users can create and manage accounts (e.g., update personal information).
- Admin accounts stored with elevated privileges.


### 2. User Dashboard

#### Dashboard Features:
- Displays previous reports with options to view, delete, or create a new report.
- Modular design allowing users to drag and drop dashboard components.
- Buttons for:
  - **Settings**: Redirect to account settings.
  - **Feedback**: Redirect to feedback submission page.

#### Dashboard Customization:
- Users can change the theme (light/dark modes or custom themes) via a script that updates the UI in real time.


### 3. URL Submission and Analysis

#### Website URL Input:
- A form for users to input a website URL for analysis.
- Backend processes the URL with:
  - **Static Scraping**: HtmlAgilityPack.
  - **Dynamic Scraping**: Selenium (if JavaScript rendering is needed).

#### Error Handling:
- Notify users if the URL is invalid, unreachable, or returns no data.

#### Analysis Workflow:
- The extracted DOM/HTML is sent to:
  - **Pa11y**: For accessibility analysis based on WCAG standards.
  - **OpenAI**: To analyze for UX principles and generate a categorized list of improvements.


### 4. Generating UX Recommendations and Mockups

#### AI-Powered Recommendations:
- OpenAI provides a list of categorized UX and accessibility recommendations.
- Recommendations stored in a database linked to the user’s account.

#### AI-Generated Mockups:
- **DALL-E (OpenAI)** generates visual mockups based on the recommendations.
- Mockups and recommendations are displayed on the dashboard and stored for future reference.


### 5. User Feedback

#### Feedback Form:
- Users can submit feedback on the product.
- Feedback stored in a database and visible to admin accounts.

#### Admin-Only Access:
- Feedback forms are only accessible via the admin dashboard.


### 6. Admin Dashboard

#### Admin Features:
- List and manage user feedback submissions.
- Access and manage all user-generated reports.

#### Report Insights:
- View analytics on the most common accessibility/UX issues reported across all user submissions.


## What the Application Will Do

### Core Deliverables:
- Analyze a user’s website for accessibility and UX issues.
- Generate a categorized list of improvements and provide AI-generated mockups.
- Store and organize reports for easy access and tracking.

### Value Proposition:
- **Problem It Solves**: Helps users meet modern accessibility and UX standards while tracking improvement progress.
- **User Benefit**: Simplifies the process of improving website usability and compliance.


## Problems the Application Solves

### Accessibility and UX Issues:
- Many websites fail to meet accessibility and usability standards.
- The app provides actionable recommendations and visual mockups to fix issues.

### Tracking Improvements:
- Users struggle to keep track of improvement progress.
- The app provides categorized lists and stored reports to track tasks and mark them as complete.


## User Needs

### Tools for Accessibility:
- Automated tests to identify and resolve WCAG issues.
- Clear explanations of errors and recommended fixes.

### UX Improvement Suggestions:
- Insights into best practices for improving site usability.

### Data Organization:
- Easy access to previous reports and recommendations.
- Secure storage of account and report data.

### Customization and Privacy:
- Options to customize the dashboard UI.
- Secure handling of user credentials and reports.


## System Requirements

### 1. Database
#### Cloud-Based Database:
- Store user accounts, reports, feedback, and mockups.

#### Tables:
- **UserAccounts**: UserID (PK), Name, Email, Password, Role (Admin/User).
- **Reports**: ReportID (PK), UserID (FK), Date, Categories, Recommendations, MockupPath.
- **Feedback**: FeedbackID (PK), UserID (FK), Date, Feedback.


### 2. APIs and External Tools

#### APIs:
- **OpenAI API**:
  - Generate UX recommendations and textual insights.
- **DALL-E API**:
  - Create mockups based on UX recommendations.
- **Pa11y**:
  - Analyze DOM/HTML for accessibility issues.

#### Tools:
- **Selenium/Playwright**:
  - Scrape dynamic content from JavaScript-rendered websites.
- **HtmlAgilityPack**:
  - Parse static content from non-JavaScript websites.
- **MailKit**:
  - Send account recovery emails.


### 3. Frameworks/Packages

#### Core Frameworks:
- **ASP.NET Core MVC**: For the web application structure.
- **Entity Framework Core**: For database interactions.


## Activities

- **User Account Management**: Registration, login, recovery, and settings updates.
- **URL Analysis**: Process submitted URLs with scraping and accessibility/UX analysis.
- **Mockup Creation**: AI generates a visual mockup of the recommended changes.
- **Feedback Collection**: Admins review and act on user-submitted feedback.
- **Report Storage**: Store and retrieve reports for users to track progress.
