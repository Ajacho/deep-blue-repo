Feature: OpenAI Enhancement
As a user, I'd like AI enhancements for the design recommendations

Scenario: Generated report provides an ai summary of the report
    Given the user has generated a report for "https://momkage-lexy.github.io/"
    Then the user will see a modal containing the summary and mock up image
    And the user clicks the let's begin button
 