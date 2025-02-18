module.exports = {
    testEnvironment: 'jsdom', // Ensure the test environment is 'jsdom' for DOM manipulation
    roots: ['<rootDir>'],
    testMatch: ['**/*.test.js'],
    transform: {
        '^.+\\.js$': 'babel-jest',
    },
};
