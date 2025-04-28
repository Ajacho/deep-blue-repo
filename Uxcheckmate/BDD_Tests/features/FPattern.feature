Feature: F Pattern
# --filter "TestCategory=FPattern"

# Selenium Test Below
@Patterns @FPattern
Scenario: F Pattern issue exists
    Given the user navigates to the site
    And their local site url is "http://localhost:5002/badFPattern.html"
    When the user enters "http://localhost:5002/badFPattern.html" to analyze
    When the user starts the analysis
    When the report view has loaded
    Then the user should see the F Pattern issue


# Possibly use this instead https://www.smashingmagazine.com/
@Patterns @FPattern
Scenario: F Pattern issue does not exist
    Given the user navigates to the site
    And their local site url is "http://localhost:5002/goodFPattern.html"
    When the user enters "http://localhost:5002/goodFPattern.html" to analyze
    When the user starts the analysis
    When the report view has loaded
    Then the user should not see the F Pattern issue

# Extra Pass https://en.wikipedia.org/wiki/Screen_reading
# Extra Fail https://example.com/