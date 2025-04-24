Feature: Webpage screenshot

@websiteScreenshot
Scenario: Given the user navigates to the site
    When the user enters a URL to analyze with "https://example.com"
    When the user starts the analysis
    Then the system displays a loading overlay with the website screenshot
    Then the report view is displayed
    And the user will see a screenshot of their website 
