document.addEventListener("DOMContentLoaded", function () {
    const themeSelector = document.getElementById("theme-selector");
    const body = document.documentElement; // Use documentElement for immediate styling

    // Load the saved theme
    loadTheme();

    if (themeSelector) {
        themeSelector.value = localStorage.getItem("theme") || "light";

        themeSelector.addEventListener("change", function () {
            setTheme(themeSelector.value);
        });
    }
});

function saveTheme() {
    const theme = localStorage.getItem("theme") || "light";
    localStorage.setItem("theme", theme);
}

function setTheme(theme) {
    const body = document.documentElement; // Use documentElement for immediate styling

    // Remove existing theme classes
    body.classList.remove("light-theme", "dark-theme");

    // Apply new theme
    body.classList.add(theme + "-theme");

    // Save theme selection
    localStorage.setItem("theme", theme);
}

function loadTheme() {
    const savedTheme = localStorage.getItem("theme") || "light";
    setTheme(savedTheme); // Calls `setTheme()` to apply styles properly
}

module.exports = { setTheme, saveTheme, loadTheme };
