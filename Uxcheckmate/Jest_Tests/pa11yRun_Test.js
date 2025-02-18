import express from 'express';
import getPort from 'get-port';
import pa11y from 'pa11y';


// Building app
(async () => {
  const app = express();
  
  // Hardcoded test for Pa11y
  app.get('/', (req, res) => {
    res.send(`
      <!DOCTYPE html>
      <html>
        <head>
          <title>Test Page</title>
        </head>
        <body>
          <h1>Pa11y Port Test!</h1>
        </body>
      </html>
    `);
  });

  // Checking to find a port that is open for temporary pa11y access
  const ports = Array.from({ length: 101 }, (_, i) => 3000 + i);
  const port = await getPort({ port: ports });
  
  // Starting the local server for pa11y access
  const server = app.listen(port, async () => {
    console.log(`Server running at http://localhost:${port}`);

    try {
      // Running pa11y on the port
      const results = await pa11y(`http://localhost:${port}`);
      console.log('pa11y results:', results);
    } catch (error) {
      console.error('pa11y error:', error);
    } finally {
      // Shutting down temp server
      server.close(() => {
        console.log(`Server on port ${port} closed.`);
      });
    }
  });
})();
