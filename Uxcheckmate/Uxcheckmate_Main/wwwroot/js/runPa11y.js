import express from 'express';
import getPort from 'get-port';
import pa11y from 'pa11y';
import { readFile } from 'fs/promises'; // Created an html file to be ran in a localhost within AccessibilityController


// Making sure temp html file is created
const htmlFilePath = process.argv[2];
if (!htmlFilePath) {
  console.error('No HTML file path provided.');
  process.exit(1);
}

// Reading it into the js program
(async () => {
  let htmlContent;
  try {
    htmlContent = await readFile(htmlFilePath, 'utf8');
  } catch (err) {
    console.error(`Error reading file: ${err.message}`);
    process.exit(1);
  }

  // Create an app with the html
  const app = express();
  app.get('/', (req, res) => res.send(htmlContent));

  // Searching for avaiable port in localhost between 3000 and 3100
  const ports = Array.from({ length: 101 }, (_, i) => 3000 + i);
  const port = await getPort({ port: ports });

  // Starting the local server for pa11y access.
  const server = app.listen(port, async () => {
    console.log(`Temporary server running at http://localhost:${port}`);
    try {
      // Running pa11y on the port
      const results = await pa11y(`http://localhost:${port}`);
      console.log(JSON.stringify(results));
    } catch (err) {
      console.error(`pa11y error: ${err.message}`);
    } finally {
      // Shutting down temp server
      server.close(() => process.exit());
    }
  });
})();