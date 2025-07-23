<h1 style="text-align: center; font-size: 48;">
<span><img src="doc\assets\uxCheckmateLogo.png" alt="uxcheckmate logo" width="20%" style="display: block, item-position:left; margin: auto;"></span>
Uxcheckmate</h1>

<p style="text-align: center; font-size: 1.5em;"><i>Your design companion for a better, more accessible web.</i></p>

<img src="doc\assets\demo.gif" alt="demo">

## Vision Statement 

Our vision is to create an inclusive digital culture where everyone—whether a developer, business owner, or designer—can contribute to making websites accessible, inclusive, and equitable. With 'UX Checkmate,' we make it easy for anyone to improve their websites by simply inserting a web link. Our tool helps identify areas for accessibility enhancement, offering actionable suggestions and visual mockups to guide improvements. As technology continues to advance, we aim to bridge the gap for those without technical experience, ensuring that every user, regardless of their abilities, can navigate and engage with the web seamlessly. By fostering collaboration and inclusion, we strive to make the digital world accessible to all, building a future where no one is left behind.

<a href="doc\assets\documentation\architecture.png">System Architecture</a>

## Features

Our focal feature is report generation. We've created over a dozen scans to detect and flag common design issues that can negatively impact your user's experience. The report also includes an accessibility audit per WCAG 2.2 guidelines. 

The report can be downloaded as a PDF or registered users can save within the app. 

Registered users have access to a dashboard containing all their previous reports grouped by domain, with the added functionality to export all reported issues to Jira. 

Admin functionality has been included to manage all feedback, users, and their reports. 

## How to Use

1. Fork the repository
2. Install Playwright   
```npm install playwright --with-deps```
3. Add API keys and connection strings to appsettings.json
```{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DBConnection": "xxxxx",
    "AuthDBConnection": "xxxxx"
  },
  "Jira": {
    "ClientId": "xxxxx",
    "ClientSecret": "xxxxx",
    "RedirectUri": "https://xxxxx/JiraAuth/Callback"
  },
  "Captcha": {
    "Enabled": "true",
    "SiteKey": "xxxxxx",
    "SecretKey": "xxxxx"
  },
  "Authentication": {
    "GitHub": {
      "ClientId": "xxxxx",
      "ClientSecret": "xxxxx"
    },
    "Google": {
      "ClientId": "xxxxx",
      "ClientSecret": "xxxxx"
    }
  },
    "ReportLimit": {
    "AnonymousUserLimitEnabled": true
  }
}
```
4. To add more scans to the report, add the method call to the switch statement in ```RunCustomAnalysisAsync()``` located in <a href="Uxcheckmate\Uxcheckmate_Main\Services\Concrete\ReportService.cs"> ReportService.cs</a>

## <img src="doc\assets\branding\logo.png" alt="Deep Blue Logo" width="50">About Us

### Deep Blue

Inspired by the legendary supercomputer, Deep Blue, our team embodies the spirit of relentless learning and continuous improvement. Just as Deep Blue revolutionized its field by adapting, analyzing, and pushing boundaries, we strive to tackle challenges with the same innovative mindset. We aim to learn from every experience, evolve through collaboration, and persevere until we achieve our goals. *Every move counts.*

This project will serve as our senior capstone and demonstrate our capabilities in programming, web app development, and design. 


