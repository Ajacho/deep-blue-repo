Feature: UX Analysis Report Generation

  Scenario: Suggest reducing the number of fonts used
    Given Sarah submits her URL
    And her site has 16 fonts on it
    When the report loads
    Then she will see a suggestion in the report to condense the amount of fonts on her site

  Scenario: Suggest improving text layout
    Given Sarah submits her URL
    And her site has huge blocks of text
    When the report loads
    Then she will see a suggestion to separate her text content with images, padding, and color modules

  Scenario: No recommendations found
    Given Sarah submits her URL
    And her site has no recommendations
    When the report loads
    Then she will see a message that says there are no suggestions
