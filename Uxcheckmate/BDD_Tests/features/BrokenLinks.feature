Feature: Broken Links Analysis
As a user, I want to know if my site has any broken links

  @BrokenLinks
  Scenario: User expands the Broken Links section and sees detected issues
    Given the user navigates to the site
    When the user enters "https://momkage-lexy.github.io/" to analyze
    When the user starts the analysis
    When the report view has loaded
    Then the broken links section should be visible