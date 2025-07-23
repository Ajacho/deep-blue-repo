Feature: OpenAI Enhancement
As a user, I'd like AI enhancements for the design recommendations

@UX117
Scenario: Generated report provides an ai summary of the report
    Given the user navigates to the site
    When the user enters "https://momkage-lexy.github.io/" to analyze
    When the user starts the analysis
    When the report view has loaded
    Then the user clicks the let's begin button
    Then the modal will close
 