const path = require('path');
module.exports = {
  parser: '@typescript-eslint/parser', // Specifies the ESLint parser
  extends: [
    "plugin:react/recommended",
    'plugin:@typescript-eslint/recommended', // Uses the recommended rules from the @typescript-eslint/eslint-plugin
  ],
  parserOptions: {
    project: path.resolve(__dirname, './tsconfig.json'),
    tsconfigRootDir: __dirname,
    ecmaVersion: 2020, // Allows for the parsing of modern ECMAScript features
    sourceType: 'module', // Allows for the use of imports
    ecmaFeatures: {
        jsx: true // Allows for the parsing of JSX  
    }
  },  
  settings: {
    react: {
      version: "detect" // Tells eslint-plugin-react to automatically detect the version of React to use
    }
  },
  rules: {
    // Place to specify ESLint rules. Can be used to overwrite rules specified from the extended configs
    "@typescript-eslint/explicit-function-return-type": "off",
    "@typescript-eslint/member-delimiter-style": "off",
    "@typescript-eslint/interface-name-prefix": "off",
    "react/display-name": "off",
    "react/prop-types": "off",
    "@typescript-eslint/no-use-before-define": "off"
  }
};  