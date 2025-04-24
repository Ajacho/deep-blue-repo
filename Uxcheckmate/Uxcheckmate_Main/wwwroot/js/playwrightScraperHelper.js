// Async function to scrape the content of all external CSS stylesheets
window.scrapeDomData = () => {
    const data = {};
    try {
        // Extract raw HTML
        data.htmlContent = document.documentElement.outerHTML;
        
        // Count header and paragraph elements
        data.headings = document.querySelectorAll('h1,h2,h3,h4,h5,h6').length;
        const paragraphs = document.querySelectorAll('p');
        data.paragraphs = paragraphs.length;

        // Combine paragraph text for text content analysis
        data.text_content = [...paragraphs].map(p => p.innerText.trim()).filter(t => t.length > 0).join('\n');
        
        // Count image elements
        data.images = document.querySelectorAll('img').length;

        // Detect font-family declarations from inline and embedded styles
        const fonts = new Set();
        document.querySelectorAll('style, *[style]').forEach(el => {
            const style = (el.innerText || '') + (el.getAttribute('style') || '');
            const matches = [...style.matchAll(/font-family:\s*([^;]+)/gi)];
            matches.forEach(m => fonts.add(m[1].split(',')[0].trim().toLowerCase()));
        });
        data.fonts = [...fonts];

        // Extract favicon URL and check if it exists
        const icon = document.querySelector('link[rel*=icon]');
        data.faviconUrl = icon?.href || '';
        data.hasFavicon = !!data.faviconUrl;

        // Capture scroll and viewport dimensions
        data.scrollHeight = document.documentElement.scrollHeight;
        data.viewportHeight = window.innerHeight;
        data.scrollWidth = document.documentElement.scrollWidth;
        data.viewportWidth = window.innerWidth;
        data.viewportLabel = `${window.innerWidth}x${window.innerHeight}`;

        // Collect lists of inline and external styles and scripts
        data.inlineCssList = [...document.querySelectorAll('style')].map(e => e.textContent);
        data.inlineJsList = [...document.querySelectorAll('script:not([src])')].map(e => e.textContent);
        data.externalCssLinks = [...document.querySelectorAll('link[rel=stylesheet]')].map(e => e.href);
        data.externalJsLinks = [...document.querySelectorAll('script[src]')].map(e => e.src);

        // Extract all href links
        data.links = [...document.querySelectorAll('a[href]')].map(a => a.href);

        return data;
    } catch (err) {
        // Log any JS-side scraping error and fail safely
        console.error('Scraping error:', err);
        return null;
    }
};

window.scrapeExternalCss = async () => {
    // Collect all <link rel="stylesheet"> elements and extract their href URLs
    const links = Array.from(document.querySelectorAll('link[rel="stylesheet"]')).map(l => l.href);

    // Fetch the content of each CSS file in parallel using Promise.allSettled
    const results = await Promise.allSettled(
        links.map(async href => {
            try {
                // Attempt to fetch the CSS file
                const res = await fetch(href);
                // Return the raw text content
                return await res.text();
            } catch {
                return '';
            }
        })
    );

    // Return an array of CSS contents (empty string if fetch failed)
    return results.map(r => r.status === 'fulfilled' ? r.value : '');
};

// Define a global async function to scrape the content of all external JavaScript files
window.scrapeExternalJs = async () => {
    // Collect all <script> tags with a src attribute and extract their URLs
    const scripts = Array.from(document.querySelectorAll('script[src]')).map(s => s.src);

    // Fetch the content of each JS file in parallel using Promise.allSettled
    const results = await Promise.allSettled(
        scripts.map(async src => {
            try {
                // Attempt to fetch the JS file
                const res = await fetch(src);
                // Return the raw JavaScript content
                return await res.text();
            } catch {
                return '';                        
            }
        })
    );

    // Return an array of JS contents or empty string if fetch failed
    return results.map(r => r.status === 'fulfilled' ? r.value : '');
};
