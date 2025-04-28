Feature: Z Pattern
# --filter "FullyQualifiedName~ZPattern"

# Selenium Test Below
@Patterns @ZPattern
Scenario: Z Pattern issue exists
    Given the user navigates to the site
    And their local site url is "http://localhost:5002/badFPattern.html"
    When the user enters "http://localhost:5002/badFPattern.html" to analyze
    When the user starts the analysis
    When the report view has loaded
    Then the user should see the Z Pattern issue


@Patterns @ZPattern
Scenario: Z Pattern issue does not exist
    Given the user navigates to the site
    And their local site url is "http://localhost:5002/goodZPattern.html"
    When the user enters "http://localhost:5002/goodZPattern.html" to analyze
    When the user starts the analysis
    When the report view has loaded
    Then the user should not see the Z Pattern issue


# Extra Pass https://trello.com/
# Extra Fail https://example.com/
