/**
 * @jest-environment jsdom
 */

const { setTheme, saveTheme, loadTheme } = require("../Uxcheckmate_Main/wwwroot/js/themeSwitcher.js");

describe("Theme Toggle Functionality", () => {
    beforeEach(() => {
        document.body.innerHTML = `
            <select id="theme-selector">
                <option value="light">Light Mode</option>
                <option value="dark">Dark Mode</option>
            </select>
        `;
    });

    test("Sarah selects dark mode, and the interface changes to dark", () => {
        setTheme("dark");
        expect(document.documentElement.classList.contains("dark-theme")).toBe(true);
        expect(localStorage.getItem("theme")).toBe("dark");
    });

    test("David selects light mode, and the interface changes to light", () => {
        setTheme("light");
        expect(document.documentElement.classList.contains("dark-theme")).toBe(false);
        expect(localStorage.getItem("theme")).toBe("light");
    });

    test("Priya navigates across pages, and the theme persists", () => {
        // Simulate setting dark mode before navigation
        setTheme("dark");
        saveTheme();
    
        // Simulate page reload
        document.documentElement.classList.remove("dark-theme");
    
        // Ensure localStorage still holds the theme
        expect(localStorage.getItem("theme")).toBe("dark");
    
        // Reload theme
        loadTheme();
    
        // Check if the theme is correctly restored
        expect(document.documentElement.classList.contains("dark-theme")).toBe(true);
    });
});
